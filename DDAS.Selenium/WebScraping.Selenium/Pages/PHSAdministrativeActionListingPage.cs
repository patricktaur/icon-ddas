using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

namespace WebScraping.Selenium.Pages
{
    public partial class PHSAdministrativeActionListingPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private DateTime? _SiteLastUpdatedFromPage;
        private DateTime? _SiteLastUpdatedFromDatabse;

        public PHSAdministrativeActionListingPage(IWebDriver driver, IUnitOfWork uow)
            : base(driver)
        {
            Open();
            _UOW = uow;
            _PHSAdministrativeSiteData = new PHSAdministrativeActionListingSiteData();
            _PHSAdministrativeSiteData.RecId = Guid.NewGuid();
            _PHSAdministrativeSiteData.ReferenceId = 
                _PHSAdministrativeSiteData.RecId;
            _PHSAdministrativeSiteData.Source = driver.Url;
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.PHSAdministrativeActionListingPage;
            }
        }

        public override string Url {
            get {
                return @"https://ori.hhs.gov/ORI_PHS_alert.html?d=update";
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _PHSAdministrativeSiteData.PHSAdministrativeSiteData;
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
                return _PHSAdministrativeSiteData;
            }
        }

        private PHSAdministrativeActionListingSiteData _PHSAdministrativeSiteData;

        private void LoadAdministrativeActionList()
        {
            IList<IWebElement> TRs = PHSTable.FindElements(By.XPath("//tbody/tr"));

            int RowCount = 1;
            foreach (IWebElement TR in TRs)
            {
                var AdministrativeActionListing = new PHSAdministrativeAction();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                if (TDs.Count > 0)
                {
                    AdministrativeActionListing.RowNumber = RowCount;
                    AdministrativeActionListing.LastName = TDs[0].Text;
                    AdministrativeActionListing.FirstName = TDs[1].Text;
                    AdministrativeActionListing.MiddleName = TDs[2].Text;
                    AdministrativeActionListing.DebarmentUntil = TDs[3].Text;
                    AdministrativeActionListing.NoPHSAdvisoryUntil = TDs[4].Text;
                    AdministrativeActionListing.CertificationOfWorkUntil = TDs[5].Text;
                    AdministrativeActionListing.SupervisionUntil = TDs[6].Text;
                    AdministrativeActionListing.RetractionOfArticle = TDs[7].Text;
                    AdministrativeActionListing.CorrectionOfArticle = TDs[8].Text;
                    AdministrativeActionListing.Memo = TDs[9].Text;

                    if(IsElementPresent(TDs[0], By.XPath("a")))
                    {
                        IWebElement anchor = TDs[0].FindElement(By.XPath("a"));
                        Link link = new Link();
                        link.Title = "Last Name";
                        link.url = anchor.GetAttribute("href");
                        AdministrativeActionListing.Links.Add(link);
                    }

                    if (IsElementPresent(TDs[1], By.XPath("a")))
                    {
                        IWebElement anchor = TDs[1].FindElement(By.XPath("a"));
                        Link link = new Link();
                        link.Title = "First Name";
                        link.url = anchor.GetAttribute("href");
                        AdministrativeActionListing.Links.Add(link);
                    }

                    if (IsElementPresent(TDs[2], By.XPath("a")))
                    {
                        IWebElement anchor = TDs[2].FindElement(By.XPath("a"));
                        Link link = new Link();
                        link.Title = "Middle Name";
                        link.url = anchor.GetAttribute("href");
                        AdministrativeActionListing.Links.Add(link);
                    }

                    _PHSAdministrativeSiteData.PHSAdministrativeSiteData.Add
                        (AdministrativeActionListing);
                    RowCount = RowCount + 1;
                }
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            try
            {
                _PHSAdministrativeSiteData.DataExtractionRequired = true;
                LoadAdministrativeActionList();
                _PHSAdministrativeSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                _PHSAdministrativeSiteData.DataExtractionSucceeded = false;
                _PHSAdministrativeSiteData.DataExtractionErrorMessage = e.Message;
                _PHSAdministrativeSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _PHSAdministrativeSiteData.CreatedBy = "Patrick";
                _PHSAdministrativeSiteData.CreatedOn = DateTime.Now;
            }
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _PHSAdministrativeSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.PHSAdministrativeActionListingRepository.
                Add(_PHSAdministrativeSiteData);
        }

        public void ReadSiteLastUpdatedDateFromPage()
        {
            string[] DataInPageLastUpdatedElement = 
                PageLastUpdatedTextElement.Text.Split('-');

            string PageLastUpdated =
                DataInPageLastUpdatedElement[1].Trim();

            DateTime RecentLastUpdatedDate;

            DateTime.TryParseExact(PageLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;

            //var ExistingPHSSiteData = 
            //    _UOW.PHSAdministrativeActionListingRepository.GetAll();

            //PHSAdministrativeActionListingSiteData PHSSiteData = null;

            //if (ExistingPHSSiteData.Count == 0)
            //{
            //    _PHSAdministrativeSiteData.SiteLastUpdatedOn = RecentLastUpdatedDate;
            //    _PHSAdministrativeSiteData.DataExtractionRequired = true;
            //}
            //else
            //{
            //    PHSSiteData = ExistingPHSSiteData.OrderByDescending(
            //        x => x.CreatedOn).First();

            //    if (RecentLastUpdatedDate > PHSSiteData.SiteLastUpdatedOn)
            //    {
            //        _PHSAdministrativeSiteData.SiteLastUpdatedOn = RecentLastUpdatedDate;
            //        _PHSAdministrativeSiteData.DataExtractionRequired = true;
            //    }
            //    else
            //    {
            //        _PHSAdministrativeSiteData.SiteLastUpdatedOn =
            //            PHSSiteData.SiteLastUpdatedOn;
            //        _PHSAdministrativeSiteData.DataExtractionRequired = false;
            //    }
            //}
            //if (!_PHSAdministrativeSiteData.DataExtractionRequired)
            //    _PHSAdministrativeSiteData.ReferenceId = PHSSiteData.RecId;
            //else
            //    _PHSAdministrativeSiteData.ReferenceId =
            //        _PHSAdministrativeSiteData.RecId;
        }
    }
}
