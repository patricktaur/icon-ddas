using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Threading;

namespace WebScraping.Selenium.Pages
{
    public partial class CorporateIntegrityAgreementsListPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        public CorporateIntegrityAgreementsListPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config, ILog Log)
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
            Open();
            _CIASiteData = new CorporateIntegrityAgreementListSiteData();
            _CIASiteData.RecId = Guid.NewGuid();
            _CIASiteData.ReferenceId = _CIASiteData.RecId;
            _CIASiteData.Source = driver.Url;
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.CorporateIntegrityAgreementsListPage;
            }
        }

        public override string Url {
            get {
                //return @"http://oig.hhs.gov/compliance/corporate-integrity-agreements/cia-documents.asp";
                return @"https://oig.hhs.gov/compliance/corporate-integrity-agreements/cia-documents.asp";
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _CIASiteData.CIAListSiteData;
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
                return _CIASiteData;
            }
        }

        private CorporateIntegrityAgreementListSiteData _CIASiteData;

        private void LoadCIAList()
        {
            //_log.WriteLog("Total records found - " +
            //    CIAListTable.FindElements(By.XPath("//tbody/tr")).Count());

            IList<IWebElement> TRs = CIAListTable.FindElements(By.XPath("//tbody/tr"));

            int RowCount = 1;
            int NullRecords = 0;

            for (int TableRow = 0; TableRow < TRs.Count; TableRow++)
            {
                var CiaList = new CIAList();

                IList<IWebElement> TDs = TRs[TableRow].FindElements(By.XPath("td"));

                //if(TDs.Count == 4)
                if (TDs.Count >= 4)
                {
                    CiaList.RowNumber = RowCount;
                    CiaList.Provider = TDs[0].Text;
                    CiaList.City = TDs[1].Text;
                    CiaList.State = TDs[2].Text;
                    CiaList.Effective = TDs[3].Text;

                    //if(IsElementPresent(TDs[0], By.XPath("a")))
                    //{
                    //    IWebElement anchor = driver.FindElement(By.XPath("a"));
                    //    var link = new Link();
                    //    link.Title = "Provider";
                    //    link.url = anchor.GetAttribute("href");
                    //    CiaList.Links.Add(link);
                    //}

                    if (CiaList.Provider != "" ||
                        CiaList.Provider != null)
                        _CIASiteData.CIAListSiteData.Add(CiaList);
                    else
                        NullRecords += 1;

                    RowCount = RowCount + 1;
                }
            }

            //The table has 54 header lines.
            //first section '#' , two rows.
            //There are 26 A-Z sections, two rows each, title + column header = 26 x 2 52
            //Total of 54 rows are not inserted to DB
            //Not sure if alphabets A to Z are always present.
            if (TRs.Count > 0 && _CIASiteData.CIAListSiteData.Count() == 0)
            {
                throw new Exception(String.Format("Error in extraction. Rows found in table: {0}, Inserted: {1} ", TRs.Count, _CIASiteData.CIAListSiteData.Count));
            }

            _log.WriteLog("Total records inserted - " +
                _CIASiteData.CIAListSiteData.Count());

            _log.WriteLog("Total null records found - " + NullRecords);
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

                _CIASiteData.DataExtractionRequired = true;
                LoadCIAList();
                _CIASiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "CorporateIntegrityAgreementPage_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _CIASiteData.DataExtractionSucceeded = false;
                _CIASiteData.DataExtractionErrorMessage = e.Message;
                _CIASiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _CIASiteData.CreatedOn = DateTime.Now;
            }
        }

        public override void LoadContent(string NameToSearch, int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
        }

        public void ReadSiteLastUpdatedDateFromPage()
        {
            string PageLastUpdated =
                PageLastUpdatedElement.Text;

            PageLastUpdated = PageLastUpdated.Replace("-", "/").Trim();

            DateTime RecentLastUpdatedDate;

            var IsDateParsed = DateTime.TryParseExact(
                PageLastUpdated, 
                "M/d/yyyy", 
                null,
                System.Globalization.DateTimeStyles.None, 
                out RecentLastUpdatedDate);

            if(IsDateParsed)
                _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
            else
                throw new Exception(
                    "Could not parse Page last updated string - '" +
                    PageLastUpdated +
                    "' to DateTime.");
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _CIASiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.CorporateIntegrityAgreementRepository.Add(_CIASiteData);
        }
    }
}
