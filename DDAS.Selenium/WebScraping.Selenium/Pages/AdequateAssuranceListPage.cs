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
        private DateTime? _SiteLastUpdatedFromPage;
        private DateTime? _SiteLastUpdatedFromDatabse;

        public AdequateAssuranceListPage(IWebDriver driver, IUnitOfWork uow) : base(driver)
        {
            _UOW = uow;
            Open();
            _adequateAssuranceListSiteData = new AdequateAssuranceListSiteData();
            _adequateAssuranceListSiteData.RecId = Guid.NewGuid();
            _adequateAssuranceListSiteData.ReferenceId =
                _adequateAssuranceListSiteData.RecId;
            SavePageImage();
            _adequateAssuranceListSiteData.Source = driver.Url;
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

        public override DateTime? SiteLastUpdatedDateFromPage
        {
            get
            {
                if (_SiteLastUpdatedFromPage == null)
                    ReadSiteLastUpdatedDateFromPage();
                return _SiteLastUpdatedFromPage;
            }
        }

        public override DateTime? SiteLastUpdatedDateFromDatabase
        {
            get
            {
                return _SiteLastUpdatedFromDatabse;
            }
        }

        public override BaseSiteData baseSiteData
        {
            get
            {
                return _adequateAssuranceListSiteData;
            }
        }

        private AdequateAssuranceListSiteData _adequateAssuranceListSiteData;

        private void LoadAdequateAssuranceInvestigators()
        {
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
            try
            {
                _adequateAssuranceListSiteData.DataExtractionRequired = true;                
                LoadAdequateAssuranceInvestigators();
                _adequateAssuranceListSiteData.DataExtractionSucceeded = true;
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
                _adequateAssuranceListSiteData.CreatedOn = DateTime.Now;
                _adequateAssuranceListSiteData.CreatedBy = "Patrick";
            }
        }

        private void ReadSiteLastUpdatedDateFromPage()
        {
            string[] PageLastUpdated = PageLastUdpatedElement.Text.Split(':');

            var SiteLastUpdated = PageLastUpdated[1].Replace("\r\nNote", "").Trim();

            DateTime RecentLastUpdatedDate;

            DateTime.TryParseExact(SiteLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
        }

        public void GetSiteLastUpdatedDateFromDatabase()
        {
            var ExistingSiteData = _UOW.AdequateAssuranceListRepository.GetAll();

            if (ExistingSiteData.Count == 0)
            {
                _SiteLastUpdatedFromDatabse = null;
            }
            var AdequateAssuranceSiteData = ExistingSiteData.OrderByDescending(
                x => x.CreatedOn).First();

            _SiteLastUpdatedFromDatabse = 
                AdequateAssuranceSiteData.SiteLastUpdatedOn;
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
