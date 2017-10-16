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
    public partial class ERRProposalToDebarPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        public ERRProposalToDebarPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config, ILog Log) : base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
            Open();
            _proposalToDebarSiteData = new ERRProposalToDebarPageSiteData();
            _proposalToDebarSiteData.RecId = Guid.NewGuid();
            _proposalToDebarSiteData.ReferenceId = _proposalToDebarSiteData.RecId;
            _proposalToDebarSiteData.Source = driver.Url;
            //SaveScreenShot("ProposalToDebarPage.png");
        }

        public override string Url {
            get {
                return 
                @"http://www.fda.gov/RegulatoryInformation/FOI/ElectronicReadingRoom/ucm143240.htm";
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.ERRProposalToDebarPage;
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _proposalToDebarSiteData.ProposalToDebar;
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
                return _proposalToDebarSiteData;
            }
        }

        private ERRProposalToDebarPageSiteData _proposalToDebarSiteData;

        private void LoadProposalToDebarList()
        {
            _log.WriteLog("Total records found - " +
                ProposalToDebarTable.FindElements(By.XPath("//tbody/tr")).Count());

            int RowNumber = 1;
            int NullRecords = 0;

            foreach (IWebElement TR in ProposalToDebarTable.FindElements(By.XPath("//tbody/tr")))
            {
                var proposalToDebarList = new ProposalToDebar();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                proposalToDebarList.RowNumber = RowNumber;
                proposalToDebarList.Name = TDs[0].Text;
                proposalToDebarList.center = TDs[1].Text;
                proposalToDebarList.date = TDs[2].Text;
                proposalToDebarList.IssuingOffice = TDs[3].Text;

                var Anchors = TDs[0].FindElements(By.XPath("p/a"));

                if(Anchors.Count > 0)
                //if (IsElementPresent(TDs[0], By.XPath("p/a")))
                {
                    Link link = new Link();
                    IList<IWebElement> anchors = TDs[0].FindElements(By.XPath("a"));

                    foreach (IWebElement anchor in anchors)
                    {
                        link.Title = "Name - " + anchor.Text;
                        link.url = anchor.GetAttribute("href");
                        proposalToDebarList.Links.Add(link);
                    }
                }
                if (proposalToDebarList.Name != "" ||
                    proposalToDebarList.Name != null)
                    _proposalToDebarSiteData.ProposalToDebar.Add(proposalToDebarList);
                else
                    NullRecords += 1;
            }
            _log.WriteLog("Total records inserted - " +
                _proposalToDebarSiteData.ProposalToDebar.Count());

            _log.WriteLog("Total null records found - " + NullRecords);
        }

        public override void LoadContent(string NameToSearch, int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
        }

        public void ReadSiteLastUpdatedDateFromPage()
        {
            string[] PageLastUpdated = PageLastUdpatedElement.Text.Split(':');

            var SiteLastUpdated = PageLastUpdated[1].Replace("\r\nNote", "").Trim();

            DateTime RecentLastUpdatedDate;

            var IsDateParsed = DateTime.TryParseExact(
                SiteLastUpdated, 
                "M'/'d'/'yyyy", 
                null,
                System.Globalization.DateTimeStyles.None, 
                out RecentLastUpdatedDate);

            if(IsDateParsed)
                _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
            else
                throw new Exception(
                    "Could not parse Page last updated string - '" +
                    SiteLastUpdated +
                    "' to DateTime.");
        }

        public string GetSiteLastUpdatedDate()
        {
            string[] PageLastUpdated = PageLastUdpatedElement.Text.Split(':');

            return PageLastUpdated[1].Replace("\r\nNote", "");
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.ERRProposalToDebarRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _proposalToDebarSiteData.ReferenceId = SiteData.RecId;
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

                _proposalToDebarSiteData.DataExtractionRequired = true;
                LoadProposalToDebarList();
                _proposalToDebarSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "ERRProposalToDebarPage_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _proposalToDebarSiteData.DataExtractionSucceeded = false;
                _proposalToDebarSiteData.DataExtractionErrorMessage = e.Message;
                _proposalToDebarSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _proposalToDebarSiteData.CreatedBy = "Patrick";
                _proposalToDebarSiteData.CreatedOn = DateTime.Now;
            }
        }

        public override void SaveData()
        {
            _UOW.ERRProposalToDebarRepository.Add(_proposalToDebarSiteData);
        }
    }
}
