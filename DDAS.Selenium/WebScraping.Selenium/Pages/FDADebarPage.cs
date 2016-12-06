using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models;
using DDAS.Models.Entities.Domain.SiteData;
using System.IO;
using System.Linq;

namespace WebScraping.Selenium.Pages
{
    public partial class FDADebarPage : BaseSearchPage
    {

        IUnitOfWork _UOW;
        public FDADebarPage(IWebDriver driver, IUnitOfWork uow) : base(driver)
        {
            Open();
            _UOW = uow;
            _FDADebarPageSiteData = new FDADebarPageSiteData();
            _FDADebarPageSiteData.RecId = Guid.NewGuid();
            _FDADebarPageSiteData.Source = driver.Url;
            //SaveScreenShot("abc.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.fda.gov/ora/compliance_ref/debar/default.htm";
            }
        }

        public override SiteEnum SiteName
        {
            get
            {
                return SiteEnum.FDADebarPage;
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _FDADebarPageSiteData.DebarredPersons;
            }
        }


        private FDADebarPageSiteData _FDADebarPageSiteData;
        
        private void LoadDebarredPersonList()
        {
            _FDADebarPageSiteData.RecId = Guid.NewGuid();

            _FDADebarPageSiteData.CreatedBy = "patrick";
            _FDADebarPageSiteData.SiteLastUpdatedOn = DateTime.Now;
            _FDADebarPageSiteData.CreatedOn = DateTime.Now;

            int RowCount = 1;
            foreach (IWebElement TR in PersonsTable.FindElements(By.XPath("tbody/tr")))
            {
                var debarredPerson = new DebarredPerson();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));
                debarredPerson.RowNumber = RowCount;
                debarredPerson.NameOfPerson = TDs[0].Text;
                debarredPerson.EffectiveDate = TDs[1].Text;
                debarredPerson.EndOfTermOfDebarment = TDs[2].Text;
                debarredPerson.FrDateText = TDs[3].Text;
                debarredPerson.VolumePage = TDs[4].Text;

                if (IsElementPresent(TDs[4], By.XPath("a")))
                {
                    IWebElement anchor = TDs[4].FindElement(By.XPath("a"));
                    debarredPerson.DocumentLink = anchor.GetAttribute("href");
                    debarredPerson.DocumentName = 
                        Path.GetFileName(debarredPerson.DocumentLink);
                }

                _FDADebarPageSiteData.DebarredPersons.Add(debarredPerson);
                RowCount = RowCount + 1;
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            //refactor - add code to validate ExtractionDate
            try
            {
                _FDADebarPageSiteData.DataExtractionRequired = true;
                if (_FDADebarPageSiteData.DataExtractionRequired)
                {
                    LoadDebarredPersonList();
                    _FDADebarPageSiteData.DataExtractionSucceeded = true;
                }
            }
            catch (Exception e)
            {
                _FDADebarPageSiteData.DataExtractionSucceeded = false;
                _FDADebarPageSiteData.DataExtractionErrorMessage = e.Message;
                _FDADebarPageSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                if (!_FDADebarPageSiteData.DataExtractionRequired)
                    AssignReferenceIdOfPreviousDocument();
                else
                    _FDADebarPageSiteData.ReferenceId =
                        _FDADebarPageSiteData.RecId;
            }
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.FDADebarPageRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _FDADebarPageSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.FDADebarPageRepository.Add(_FDADebarPageSiteData);

        }
    }
}
