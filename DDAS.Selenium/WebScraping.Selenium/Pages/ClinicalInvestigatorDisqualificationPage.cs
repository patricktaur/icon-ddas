using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorDisqualificationPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        public ClinicalInvestigatorDisqualificationPage(IWebDriver driver, IUnitOfWork uow)
            : base(driver)
        {
            _UOW = uow;
            Open();
            //SaveScreenShot("ClinicalInvestigatorDisqualificationPage.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.accessdata.fda.gov/scripts/SDA/sdNavigation.cfm?sd=clinicalinvestigatorsdisqualificationproceedings&previewMode=true&displayAll=true";
            }
        }

        private List<DisqualifiedInvestigator> _disqualifiedInvestigotor;

        public List<DisqualifiedInvestigator> DisqualifiedInvestigatorList {
            get {
                return _disqualifiedInvestigotor;
            }
        }

        private ClinicalInvestigatorDisqualificationSiteData _DisqualificationSiteData;

        public void LoadDisqualificationProceedingsList()
        {
            _DisqualificationSiteData = new ClinicalInvestigatorDisqualificationSiteData();

            _DisqualificationSiteData.CreatedBy = "Patrick";
            _DisqualificationSiteData.SiteLastUpdatedOn = DateTime.Now;

            foreach (IWebElement TR in
                DisqualifiedInvestigatorTable.FindElements(By.XPath("tbody/tr")))
            {
                var DisqualifiedClinicalInvestigator = new DisqualifiedInvestigator();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                if (TDs.Count > 0)
                {
                    DisqualifiedClinicalInvestigator.Name = TDs[0].Text;
                    DisqualifiedClinicalInvestigator.Center = TDs[1].Text;
                    DisqualifiedClinicalInvestigator.Status = TDs[2].Text;
                    DisqualifiedClinicalInvestigator.DateOfStatus = TDs[3].Text;
                    DisqualifiedClinicalInvestigator.DateNIDPOEIssued = TDs[4].Text;
                    DisqualifiedClinicalInvestigator.DateNOOHIssued = TDs[5].Text;
                    DisqualifiedClinicalInvestigator.LinkToNIDPOELetter = TDs[6].Text;
                    DisqualifiedClinicalInvestigator.LinkToNOOHLetter = TDs[7].Text;

                    _DisqualificationSiteData.DisqualifiedInvestigatorList.Add(
                        DisqualifiedClinicalInvestigator);
                }
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.ClinicalInvestigatorDisqualificationPage;
            }
        }

        public override void LoadContent()
        {
            LoadDisqualificationProceedingsList();
        }

        public override void LoadContent(string NameToSearch)
        {

        }

        public override void SaveData()
        {
            
        }
    }
}
