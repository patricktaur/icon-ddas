using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;

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
            _DisqualificationSiteData.Source = driver.Url;
            //SaveScreenShot("ClinicalInvestigatorDisqualificationPage.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.accessdata.fda.gov/scripts/SDA/sdNavigation.cfm?sd=clinicalinvestigatorsdisqualificationproceedings&previewMode=true&displayAll=true";
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.ClinicalInvestigatorDisqualificationPage;
            }
        }

        public ClinicalInvestigatorDisqualificationSiteData 
            DisqualificationSiteData {
            get {
                return _DisqualificationSiteData;
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _DisqualificationSiteData.DisqualifiedInvestigatorList;
            }
        }

        private ClinicalInvestigatorDisqualificationSiteData _DisqualificationSiteData;

        private int RowNumber = 1;

        private void LoadDisqualificationProceedingsList()
        {
            _DisqualificationSiteData.CreatedBy = "Patrick";
            _DisqualificationSiteData.SiteLastUpdatedOn = DateTime.Now;
            _DisqualificationSiteData.CreatedOn = DateTime.Now;

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

        private bool SearchTerms(string NameToSearch)
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

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            string [] WordsInNameToSearch = NameToSearch.Split(' ');

            //refactor - add code to validate ExtractionDate
            try
            {
                for (int counter = 0; counter < WordsInNameToSearch.Length; counter++)
                {
                    bool ComponentGreaterThanTwoCharacters =
                        (WordsInNameToSearch[counter].Length > 2);

                    if (ComponentGreaterThanTwoCharacters &&
                        SearchTerms(WordsInNameToSearch[counter]))
                        LoadDisqualificationProceedingsList();
                }
                _DisqualificationSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                _DisqualificationSiteData.DataExtractionSucceeded = false;
                _DisqualificationSiteData.DataExtractionErrorMessage = e.Message;
                _DisqualificationSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {

            }
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _DisqualificationSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.ClinicalInvestigatorDisqualificationRepository.Add
                (_DisqualificationSiteData);   
        }
    }
}
