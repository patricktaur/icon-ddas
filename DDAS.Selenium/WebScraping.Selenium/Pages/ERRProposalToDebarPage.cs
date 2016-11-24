using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;

namespace WebScraping.Selenium.Pages
{
    public partial class ERRProposalToDebarPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;

        public ERRProposalToDebarPage(IWebDriver driver, IUnitOfWork uow) : base(driver)
        {
            _UOW = uow;
            Open();
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

        private ERRProposalToDebarPageSiteData _proposalToDebarSiteData;

        public void LoadProposalToDebarList()
        {
            _proposalToDebarSiteData = new ERRProposalToDebarPageSiteData();

            _proposalToDebarSiteData.CreatedBy = "Patrick";
            _proposalToDebarSiteData.SiteLastUpdatedOn = DateTime.Now;
            _proposalToDebarSiteData.CreatedOn = DateTime.Now;
            _proposalToDebarSiteData.Source = driver.Url;

            foreach (IWebElement TR in ProposalToDebarTable.FindElements(By.XPath("//tbody/tr")))
            {
                var proposalToDebarList = new ProposalToDebar();
                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                proposalToDebarList.Name = TDs[0].Text;
                proposalToDebarList.center = TDs[1].Text;
                proposalToDebarList.date = TDs[2].Text;
                proposalToDebarList.IssuingOffice = TDs[3].Text;

                _proposalToDebarSiteData.ProposalToDebar.Add(proposalToDebarList);
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            //refactor - add code to validate ExtractionDate
            try
            {
                if (_proposalToDebarSiteData.DataExtractionRequired)
                {
                    LoadProposalToDebarList();
                    _proposalToDebarSiteData.DataExtractionSucceeded = true;
                }
            }
            catch (Exception e)
            {
                _proposalToDebarSiteData.DataExtractionSucceeded = false;
                _proposalToDebarSiteData.DataExtractionErrorMessage = e.Message;
                _proposalToDebarSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                if (!_proposalToDebarSiteData.DataExtractionRequired)
                    AssignReferenceIdOfPreviousDocument();
            }
        }

        public void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.ERRProposalToDebarRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _proposalToDebarSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.ERRProposalToDebarRepository.Add(_proposalToDebarSiteData);
        }
    }
}
