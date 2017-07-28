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
using DDAS.Models.Interfaces;
using System.Threading;

namespace WebScraping.Selenium.Pages
{
    public partial class PHSAdministrativeActionListingPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        public PHSAdministrativeActionListingPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config, ILog Log)
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
            Open();
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
            int RowCount = 1;
            int NullRecords = 0;

            _log.WriteLog("Total records found - " +
                PHSTable.FindElements(By.XPath("//tbody/tr")).Count());

            IList<IWebElement> TRs = PHSTable.FindElements(By.XPath("//tbody/tr"));

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

                    var Anchors = TDs[0].FindElements(By.XPath("a"));

                    if(Anchors.Count > 0)
                    //if(IsElementPresent(TDs[0], By.XPath("a")))
                    {
                        IWebElement anchor = TDs[0].FindElement(By.XPath("a"));
                        Link link = new Link();
                        link.Title = "Last Name";
                        link.url = anchor.GetAttribute("href");
                        AdministrativeActionListing.Links.Add(link);
                    }

                    Anchors = null;

                    Anchors = TDs[1].FindElements(By.XPath("a"));

                    if(Anchors.Count > 0)
                    //if (IsElementPresent(TDs[1], By.XPath("a")))
                    {
                        IWebElement anchor = TDs[1].FindElement(By.XPath("a"));
                        Link link = new Link();
                        link.Title = "First Name";
                        link.url = anchor.GetAttribute("href");
                        AdministrativeActionListing.Links.Add(link);
                    }

                    Anchors = null;

                    Anchors = TDs[2].FindElements(By.XPath("a"));

                    if (Anchors.Count > 0)
                    //if (IsElementPresent(TDs[2], By.XPath("a")))
                    {
                        IWebElement anchor = TDs[2].FindElement(By.XPath("a"));
                        Link link = new Link();
                        link.Title = "Middle Name";
                        link.url = anchor.GetAttribute("href");
                        AdministrativeActionListing.Links.Add(link);
                    }

                    if (AdministrativeActionListing.FullName != "" ||
                        AdministrativeActionListing.FullName != null)
                        _PHSAdministrativeSiteData.PHSAdministrativeSiteData.Add
                            (AdministrativeActionListing);
                    else
                        NullRecords += 1;

                    RowCount = RowCount + 1;
                }
            }
            _log.WriteLog("Total records inserted - " +
                _PHSAdministrativeSiteData.PHSAdministrativeSiteData.Count());

            _log.WriteLog("Total null records found - " + NullRecords);
        }

        public override void LoadContent(string NameToSearch, int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
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

        private bool IsPageLoaded()
        {
            IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
            bool PageLoaded = false;

            for (int Index = 1; Index <= 25; Index++)
            {
                Thread.Sleep(500);
                if (executor.ExecuteScript("return document.readyState").ToString().
                    Equals("complete"))
                {
                    PageLoaded = true;
                    break;
                }
            }
            return PageLoaded;
        }

        public override void LoadContent()
        {
            try
            {
                if (!IsPageLoaded())
                    throw new Exception("page is not loaded");

                _PHSAdministrativeSiteData.DataExtractionRequired = true;
                LoadAdministrativeActionList();
                _PHSAdministrativeSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "PHSAdministrativeActionListingPage_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

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
    }
}
