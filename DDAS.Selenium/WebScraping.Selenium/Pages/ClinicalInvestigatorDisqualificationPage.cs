using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;
using DDAS.Models.Entities.Domain;
using System.Threading;
using DDAS.Models.Interfaces;

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorDisqualificationPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;

        public ClinicalInvestigatorDisqualificationPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config)
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
            Open();
            _DisqualificationSiteData = 
                new ClinicalInvestigatorDisqualificationSiteData();
            _DisqualificationSiteData.Source = driver.Url;
            _DisqualificationSiteData.RecId = Guid.NewGuid();
            _DisqualificationSiteData.ReferenceId = 
                _DisqualificationSiteData.RecId;
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
                return _DisqualificationSiteData;
            }
        }

        private ClinicalInvestigatorDisqualificationSiteData _DisqualificationSiteData;

        private int RowNumber = 1;

        private void LoadDisqualificationProceedingsList(string NameToSearch,
            int MatchCountLowerLimit = 0)
        {
            if (DisqualifiedInvestigatorTable == null)
                throw new Exception(
                    "Could not find DisqualifiedInvestigatorTable in " +
                    "LoadDisqualificationProceedingsList()");
            foreach (IWebElement TR in
                DisqualifiedInvestigatorTable.FindElements(By.XPath("tbody/tr")))
            {
                var DisqualifiedClinicalInvestigator = new DisqualifiedInvestigator();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                if (TDs.Count > 0 && 
                    GetMatchCount(NameToSearch, TDs[0].Text) >= MatchCountLowerLimit)
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

                    if (IsElementPresent(TDs[0], By.XPath("a")))
                    {
                        Link link = new Link();
                        IWebElement anchor = TDs[0].FindElement(By.XPath("a"));
                        link.Title = "Name";
                        link.url = anchor.GetAttribute("href");
                        DisqualifiedClinicalInvestigator.Links.Add(link);
                    }

                    if (IsElementPresent(TDs[6], By.XPath("a")))
                    {
                        IList<IWebElement> anchors = TDs[6].FindElements(By.XPath("a"));

                        foreach (IWebElement anchor in anchors)
                        {
                            Link link = new Link();
                            link.Title = "Link To NIDPOE Letter - " + anchor.Text;
                            link.url = anchor.GetAttribute("href");
                            DisqualifiedClinicalInvestigator.Links.Add(link);
                        }
                    }

                    if (IsElementPresent(TDs[7], By.XPath("a")))
                    {
                        IList<IWebElement> anchors = TDs[7].FindElements(By.XPath("a"));

                        foreach (IWebElement anchor in anchors)
                        {
                            Link link = new Link();
                            link.Title = "Link To NOOH Letter - " + anchor.Text;
                            link.url = anchor.GetAttribute("href");
                            DisqualifiedClinicalInvestigator.Links.Add(link);
                        }
                    }

                    _DisqualificationSiteData.DisqualifiedInvestigatorList.Add(
                        DisqualifiedClinicalInvestigator);
                    RowNumber += 1;
                }
            }
        }

        private int GetMatchCount(string NameToSearch, string FullName)
        {
            string[] NameToSearchComponents = NameToSearch.Split(' ');
            string[] FullNameComponents = FullName.Split(' ');

            int MatchCount = 0;

            for(int NameToSearchIndex = 0; 
                NameToSearchIndex < NameToSearchComponents.Count();
                NameToSearchIndex++)
            {
                for(int FullNameIndex = 0; 
                    FullNameIndex < FullNameComponents.Count();
                    FullNameIndex++)
                {
                    if (FullNameComponents[FullNameIndex].Replace(",", "").ToLower()
                        == NameToSearchComponents[NameToSearchIndex].ToLower())
                        MatchCount += 1;
                }
            }
            return MatchCount;
        }

        private bool SearchTerms(string NameToSearch)
        {
            if (!IsPageLoaded())
                throw new Exception("Could not load the page");

            if (IsFeedbackPopUpDisplayed)
            {
                var ErrorCaptureFilePath =
                    _config.ErrorScreenCaptureFolder +
                    "PopUp_DisqualificationProceedings_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);
                driver.Navigate().GoToUrl(Url);
                IsPageLoaded();
            }

            IWebElement SearchTextBox = DisqualifiedInvestigatorSearchTextBox;
            if (SearchTextBox == null)
                throw new Exception("Could not find element: " +
                    "DisqualifiedInvestigatorSearchTextBox");
            SearchTextBox.Clear();
            SearchTextBox.SendKeys(NameToSearch);

            IWebElement SubmitButton = DisqualifiedInvestigatorSubmitButton;
            if (SubmitButton == null)
                throw new Exception("Could not find element: " +
                    "DisqualifiedInvestigatorSubmitButton");
            //SubmitButton.Click();
            SubmitButton.SendKeys(Keys.Enter);

            IWebElement Table = DisqualifiedInvestigatorCountTable;
            if (Table == null)
                throw new Exception("Could not find element: " +
                    "DisqualifiedInvestigatorCountTable");

            IList<IWebElement> TRs = Table.FindElements(By.XPath("tbody/tr"));

            string[] TextInLastRow = TRs[TRs.Count - 1].Text.Split(':');

            if (TextInLastRow[TextInLastRow.Length - 1].Trim() == "0")
                return false;

            return true;
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
                    throw new Exception("Could not load the page");

                if (IsFeedbackPopUpDisplayed)
                {
                    var ErrorCaptureFilePath =
                        _config.ErrorScreenCaptureFolder +
                         "PopUp_DisqualificationProceedings_" +
                        DateTime.Now.ToString("dd MMM yyyy hh_mm");
                    SaveScreenShot(ErrorCaptureFilePath);
                    driver.Navigate().GoToUrl(Url);
                    IsPageLoaded();
                }

                _DisqualificationSiteData.DataExtractionRequired = true;
                LoadDisqualificationProceedingsList("", 0);
                _DisqualificationSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath =
                    _config.ErrorScreenCaptureFolder +
                    "Error_DisqualificationProceedings_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm");
                SaveScreenShot(ErrorCaptureFilePath);

                _DisqualificationSiteData.DataExtractionSucceeded = false;
                _DisqualificationSiteData.DataExtractionErrorMessage = e.ToString();
                _DisqualificationSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _DisqualificationSiteData.CreatedOn = DateTime.Now;
                _DisqualificationSiteData.CreatedBy = "Patrick";
            }
        }

        public override void LoadContent(string NameToSearch, int MatchCountLowerLimit)
        {
            string [] WordsInNameToSearch = NameToSearch.Split(' ');

            try
            {
                for (int counter = 0; counter < WordsInNameToSearch.Length; counter++)
                {
                    bool ComponentGreaterThanTwoCharacters =
                        (WordsInNameToSearch[counter].Length > 1);

                    if (ComponentGreaterThanTwoCharacters &&
                        SearchTerms(WordsInNameToSearch[counter]) &&
                        _DisqualificationSiteData.DisqualifiedInvestigatorList.Count >= 0)
                        LoadDisqualificationProceedingsList(NameToSearch,
                            MatchCountLowerLimit);
                }
                _DisqualificationSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder + 
                    "Error_DisqualificationProceedings_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _DisqualificationSiteData.DataExtractionSucceeded = false;
                _DisqualificationSiteData.DataExtractionErrorMessage = e.Message +
                    " - " + ErrorCaptureFilePath;
                _DisqualificationSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _DisqualificationSiteData.CreatedBy = "Patrick";
                _DisqualificationSiteData.CreatedOn = DateTime.Now;
            }
        }

        public void ReadSiteLastUpdatedDateFromPage()
        {
            string[] DataInPageLastUpdatedElement = PageLastUpdatedTextElement.Text.Split(':');

            string PageLastUpdated =
                DataInPageLastUpdatedElement[1].Replace("\r\nNote","").Trim();

            DateTime RecentLastUpdatedDate;

            DateTime.TryParseExact(PageLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
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
