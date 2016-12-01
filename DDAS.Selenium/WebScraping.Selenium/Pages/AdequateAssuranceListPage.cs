using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Repository;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

namespace WebScraping.Selenium.Pages
{
    public partial class AdequateAssuranceListPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;

        public AdequateAssuranceListPage(IWebDriver driver, IUnitOfWork uow) : base(driver)
        {
            _UOW = uow;
            Open();
            _adequateAssuranceListSiteData = new AdequateAssuranceListSiteData();
            SavePageImage();
            //SaveScreenShot("AdequateAssuranceListPage.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.fda.gov/ora/compliance_ref/bimo/asurlist.htm";
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.AdequateAssuranceListPage;
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _adequateAssuranceListSiteData.AdequateAssurances;
            }
        }

        private AdequateAssuranceListSiteData _adequateAssuranceListSiteData;

        private void LoadAdequateAssuranceInvestigators()
        {
            _adequateAssuranceListSiteData.CreatedOn = DateTime.Now;
            _adequateAssuranceListSiteData.CreatedBy = "Patrick";
            _adequateAssuranceListSiteData.Source = driver.Url;

            int RowCount = 1;
            foreach(IWebElement TR in 
                AdequateAssuranceListTable.FindElements(By.XPath("//tbody/tr")))
            {
                var AdequateAssuranceInvestigator = new AdequateAssuranceList();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));
                if (TDs.Count > 0)
                {
                    AdequateAssuranceInvestigator.RowNumber = RowCount;
                    AdequateAssuranceInvestigator.NameAndAddress = TDs[0].Text;
                    AdequateAssuranceInvestigator.Center = TDs[1].Text;
                    AdequateAssuranceInvestigator.Type = TDs[2].Text;
                    AdequateAssuranceInvestigator.ActionDate = TDs[3].Text;
                    AdequateAssuranceInvestigator.Comments = TDs[4].Text;

                    _adequateAssuranceListSiteData.AdequateAssurances.Add
                        (AdequateAssuranceInvestigator);
                    RowCount = RowCount + 1;
                }
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            //refactor - add code to validate ExtractionDate
            try
            {
                _adequateAssuranceListSiteData.DataExtractionRequired = true;
                if (_adequateAssuranceListSiteData.DataExtractionRequired)
                {
                    LoadAdequateAssuranceInvestigators();
                    _adequateAssuranceListSiteData.DataExtractionSucceeded = true;
                }
            }
            catch (Exception e)
            {
                _adequateAssuranceListSiteData.DataExtractionSucceeded = false;
                _adequateAssuranceListSiteData.DataExtractionErrorMessage = e.Message;
                _adequateAssuranceListSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                if (!_adequateAssuranceListSiteData.DataExtractionRequired)
                    AssignReferenceIdOfPreviousDocument();
                else
                    _adequateAssuranceListSiteData.ReferenceId = 
                        _adequateAssuranceListSiteData.RecId;
            }
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.AdequateAssuranceListRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _adequateAssuranceListSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.AdequateAssuranceListRepository.Add(
                _adequateAssuranceListSiteData);
        }
    }
}
