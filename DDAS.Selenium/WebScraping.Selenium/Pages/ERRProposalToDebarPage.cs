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

namespace WebScraping.Selenium.Pages
{
    public partial class ERRProposalToDebarPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;

        public ERRProposalToDebarPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config) : base(driver)
        {
            _UOW = uow;
            _config = Config;
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
            foreach (IWebElement TR in ProposalToDebarTable.FindElements(By.XPath("//tbody/tr")))
            {
                var proposalToDebarList = new ProposalToDebar();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                proposalToDebarList.Name = TDs[0].Text;
                proposalToDebarList.center = TDs[1].Text;
                proposalToDebarList.date = TDs[2].Text;
                proposalToDebarList.IssuingOffice = TDs[3].Text;

                if (IsElementPresent(TDs[0], By.XPath("p/a")))
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

                _proposalToDebarSiteData.ProposalToDebar.Add(proposalToDebarList);
            }
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

            DateTime.TryParseExact(SiteLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;

            //var ExistingERRSiteData = _UOW.ERRProposalToDebarRepository.GetAll();

            //if (ExistingERRSiteData.Count == 0)
            //{
            //    _proposalToDebarSiteData.SiteLastUpdatedOn = RecentLastUpdatedDate;
            //    _proposalToDebarSiteData.DataExtractionRequired = true;
            //}
            //else
            //{
            //    ERRSiteData = ExistingERRSiteData.OrderByDescending(
            //        x => x.CreatedOn).First();

            //    if (RecentLastUpdatedDate > ERRSiteData.SiteLastUpdatedOn)
            //    {
            //        _proposalToDebarSiteData.SiteLastUpdatedOn = RecentLastUpdatedDate;
            //        _proposalToDebarSiteData.DataExtractionRequired = true;
            //    }
            //    else
            //    {
            //        _proposalToDebarSiteData.SiteLastUpdatedOn =
            //            ERRSiteData.SiteLastUpdatedOn;
            //        _proposalToDebarSiteData.DataExtractionRequired = false;
            //    }
            //}

            //if (!_proposalToDebarSiteData.DataExtractionRequired)
            //    _proposalToDebarSiteData.ReferenceId = ERRSiteData.RecId;
            //else
            //    _proposalToDebarSiteData.ReferenceId =
            //        _proposalToDebarSiteData.RecId;
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

        public override void LoadContent()
        {
            try
            {
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
