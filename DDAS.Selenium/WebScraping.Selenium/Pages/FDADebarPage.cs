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
using System.Threading;

namespace WebScraping.Selenium.Pages
{
    public partial class FDADebarPage : BaseSearchPage
    {
        IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        public FDADebarPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config, ILog Log) : base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
            Open();
            _FDADebarPageSiteData = new FDADebarPageSiteData();
            _FDADebarPageSiteData.RecId = Guid.NewGuid();
            _FDADebarPageSiteData.ReferenceId = _FDADebarPageSiteData.RecId;
            _FDADebarPageSiteData.Source = driver.Url;
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
            int NullRecords = 0;

            _log.WriteLog("Total records found - " +
                PersonsTable.FindElements(By.XPath("tbody/tr")).Count());

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

                var AnchorTags = TDs[4].FindElements(By.XPath("a"));

                //if (IsElementPresent(TDs[4], By.XPath("a")))
                if(AnchorTags.Count > 0)
                {
                    IWebElement anchor = TDs[4].FindElement(By.XPath("a"));
                    Link link = new Link();
                    link.Title = "Company";
                    link.url = anchor.GetAttribute("href");
                    debarredPerson.Links.Add(link);
                }
                if (debarredPerson.NameOfPerson != "" ||
                    debarredPerson.NameOfPerson != null)
                    _FDADebarPageSiteData.DebarredPersons.Add(debarredPerson);
                else
                    NullRecords += 1;

                RowCount = RowCount + 1;
            }
            _log.WriteLog("Total records inserted - " +
                _FDADebarPageSiteData.DebarredPersons.Count());
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
            var SiteData = _UOW.FDADebarPageRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _FDADebarPageSiteData.ReferenceId = SiteData.RecId;
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
