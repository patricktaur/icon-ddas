﻿using System;
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
            _DisqualificationSiteData = 
                new ClinicalInvestigatorDisqualificationSiteData();
            //SaveScreenShot("ClinicalInvestigatorDisqualificationPage.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.accessdata.fda.gov/scripts/SDA/sdNavigation.cfm?sd=clinicalinvestigatorsdisqualificationproceedings&previewMode=true&displayAll=true";
            }
        }

        private ClinicalInvestigatorDisqualificationSiteData _DisqualificationSiteData;

        public void LoadDisqualificationProceedingsList()
        {
            _DisqualificationSiteData.CreatedBy = "Patrick";
            _DisqualificationSiteData.SiteLastUpdatedOn = DateTime.Now;
            _DisqualificationSiteData.CreatedOn = DateTime.Now;
            _DisqualificationSiteData.Source = driver.Url;

            int RowNumber = 1;
            foreach (IWebElement TR in
                DisqualifiedInvestigatorTable.FindElements(By.XPath("tbody/tr")))
            {
                var DisqualifiedClinicalInvestigator = new DisqualifiedInvestigator();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                if (TDs.Count > 0)
                {
                    DisqualifiedClinicalInvestigator.RowNumber = RowNumber;
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
                    RowNumber += 1;
                }
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.ClinicalInvestigatorDisqualificationPage;
            }
        }

        public bool SearchTerms(string NameToSearch)
        {
            IWebElement SearchTextBox = DisqualifiedInvestigatorSearchTextBox;
            SearchTextBox.Clear();
            SearchTextBox.SendKeys(NameToSearch);

            IWebElement SubmitButton = DisqualifiedInvestigatorSubmitButton;
            SubmitButton.Click();

            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            IWebElement Table = DisqualifiedInvestigatorCountTable;

            IList<IWebElement> TRs = Table.FindElements(By.XPath("tbody/tr"));

            string[] TextInLastRow = TRs[TRs.Count - 1].Text.Split(':');

            if (TextInLastRow[TextInLastRow.Length - 1].Trim() == "0")
                return false;

            return true;
        }

        public override void LoadContent(string NameToSearch)
        {
            string [] WordsInNameToSearch = NameToSearch.Split(' ');

            string Name = WordsInNameToSearch[1] + ", " + WordsInNameToSearch[0];

            //for (int counter = 0; counter <= WordsInNameToSearch.Length; counter++)
            //{
                if (SearchTerms(Name))
                    LoadDisqualificationProceedingsList();
            //}
        }

        public override void SaveData()
        {
            _UOW.ClinicalInvestigatorDisqualificationRepository.Add
                (_DisqualificationSiteData);   
        }
    }
}
