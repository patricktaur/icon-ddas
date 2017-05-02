using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models;
using DDAS.Models.Entities.Domain.SiteData;
using System.IO;
using System.Linq;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;

namespace WebScraping.Selenium.Pages
{
    public partial class FDADebarPage : BaseSearchPage
    {
        IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;

        public FDADebarPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config) : base(driver)
        {
            _UOW = uow;
            _config = Config;
            Open();
            _FDADebarPageSiteData = new FDADebarPageSiteData();
            _FDADebarPageSiteData.RecId = Guid.NewGuid();
            _FDADebarPageSiteData.ReferenceId = _FDADebarPageSiteData.RecId;
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

        public override DateTime? SiteLastUpdatedDateFromPage
        {
            get
            {
                if (_SiteLastUpdatedFromPage == null)
                    ReadSiteLastUpdatedDateFromPage();
                return _SiteLastUpdatedFromPage;
            }
        }

        public override BaseSiteData baseSiteData
        {
            get
            {
                return _FDADebarPageSiteData;
            }
        }

        public FDADebarPageSiteData _FDADebarPageSiteData;
        
        private void LoadDebarredPersonList()
        {
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

                var AnchorTag = TDs[4].FindElement(By.XPath("a"));

                //if (IsElementPresent(TDs[4], By.XPath("a")))
                if(AnchorTag != null)
                {
                    IWebElement anchor = TDs[4].FindElement(By.XPath("a"));
                    Link link = new Link();
                    link.Title = "Company";
                    link.url = anchor.GetAttribute("href");
                    debarredPerson.Links.Add(link);
                }
                _FDADebarPageSiteData.DebarredPersons.Add(debarredPerson);
                RowCount = RowCount + 1;
            }
        }

        public override void LoadContent(
            string NameToSearch,
            int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
        }

        private void ReadSiteLastUpdatedDateFromPage()
        {
            string[] DataInPageLastUpdatedElement = PageLastUpdatedTextElement.Text.Split(':');

            string PageLastUpdated = 
                DataInPageLastUpdatedElement[1].Replace("\r\nNote", "").Trim();

            DateTime RecentLastUpdatedDate;

            DateTime.TryParseExact(PageLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.FDADebarPageRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _FDADebarPageSiteData.ReferenceId = SiteData.RecId;
        }

        public override void LoadContent()
        {
            try
            {
                _FDADebarPageSiteData.DataExtractionRequired = true;
                LoadDebarredPersonList();
                _FDADebarPageSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "FDADebarPage_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _FDADebarPageSiteData.DataExtractionSucceeded = false;
                _FDADebarPageSiteData.DataExtractionErrorMessage = e.Message;
                _FDADebarPageSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _FDADebarPageSiteData.CreatedBy = "patrick";
                _FDADebarPageSiteData.CreatedOn = DateTime.Now;
            }
        }

        public override void SaveData()
        {
            _UOW.FDADebarPageRepository.Add(_FDADebarPageSiteData);

        }
    }
}
