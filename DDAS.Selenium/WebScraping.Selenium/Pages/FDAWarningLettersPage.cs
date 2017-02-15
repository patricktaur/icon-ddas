using DDAS.Models.Enums;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;
using DDAS.Models.Entities.Domain;

namespace WebScraping.Selenium.Pages
{
    public partial class FDAWarningLettersPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private DateTime? _SiteLastUpdatedFromPage;

        public FDAWarningLettersPage(IWebDriver driver, IUnitOfWork uow) : base(driver)
        {
            _UOW = uow;
            Open();
            _FDAWarningSiteData = new FDAWarningLettersSiteData();
            _FDAWarningSiteData.Source = driver.Url;
        }

        public override string Url {
            get {
                return @"http://www.fda.gov/ICECI/EnforcementActions/WarningLetters/default.htm";
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.FDAWarningLettersPage;
            }
        }

        public FDAWarningLettersSiteData FDADebarSiteData {
            get {
                return _FDAWarningSiteData;
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _FDAWarningSiteData.FDAWarningLetterList;
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
                return _FDAWarningSiteData;
            }
        }

        private bool SearchTerms(string Name)
        {
            //if (CheckForFeedbackWindow != null)
            //    throw new Exception("Could not close Feedback window");

            IWebElement Input = FDASearchTextBox;
            if (Input == null)
                throw new Exception("Could not find element: FDASearchTextBox");
            //Input.Clear();
            //Input.SendKeys(Name);
            FDASearchTextBox.Clear();
            FDASearchTextBox.SendKeys(Name);

            IWebElement Search = FDASearchButton;
            if (Search == null)
                throw new Exception("Could not find element: FDASearchButton");
            FDASearchButton.SendKeys(Keys.Enter);

            IWebElement Table = SortTableTest1;

            if (Table == null)
                throw new Exception("FDASortTable is null");

            IList<IWebElement> TR = SortTableTest1.FindElements(By.XPath("tbody/tr"));

            if(TR.Count <= 1)
                return false;
            else
            {
                IWebElement TD = TR[TR.Count - 1].FindElement(By.XPath("td"));

                IList<IWebElement> AnchorTags = TD.FindElements(By.XPath("a"));

                if (AnchorTags.Count > 0)
                    AnchorTags[AnchorTags.Count - 1].SendKeys(Keys.Enter);
                return true;
            }
        }

        private FDAWarningLettersSiteData _FDAWarningSiteData;

        private int RowNumber = 1;

        public void LoadFDAWarningLetters()
        {
            IList<IWebElement> TR = SortTableTest1.FindElements(By.XPath("//tbody/tr"));

            for (int tableRow = 12; tableRow < TR.Count - 1; tableRow++)
            {
                var FDAWarningList = new FDAWarningLetter();

                IList<IWebElement> TDs = TR[tableRow].FindElements(By.XPath("td"));

                if(TDs.Count > 1)
                {
                    FDAWarningList.RowNumber = RowNumber;
                    FDAWarningList.Company = TDs[0].Text;
                    FDAWarningList.LetterIssued = TDs[1].Text;
                    FDAWarningList.IssuingOffice = TDs[2].Text;
                    FDAWarningList.Subject = TDs[3].Text;
                    FDAWarningList.ResponseLetterPosted = TDs[4].Text;
                    FDAWarningList.CloseoutDate = TDs[5].Text;

                    if (IsElementPresent(TDs[0], By.XPath("a")))
                    {
                        IWebElement anchor = TDs[0].FindElement(By.XPath("a"));
                        Link link = new Link();
                        link.Title = "Company";
                        link.url = anchor.GetAttribute("href");
                        FDAWarningList.Links.Add(link);
                    }
                }

                _FDAWarningSiteData.FDAWarningLetterList.Add(FDAWarningList);
                RowNumber += 1;
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder,
            int MatchCountLowerLimit)
        {
            string[] FullName = NameToSearch.Split(' ');
            try
            {
                if (SearchTerms(NameToSearch))
                    LoadFDAWarningLetters();
                else
                {
                    for (int Counter = 0; Counter < FullName.Length; Counter++)
                    {
                        if (FullName[Counter].Length > 1 &&
                            _FDAWarningSiteData.FDAWarningLetterList.Count == 0 &&
                            SearchTerms(FullName[Counter]))
                            LoadFDAWarningLetters();
                    }
                }
                _FDAWarningSiteData.DataExtractionRequired = true;
                _FDAWarningSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                SaveScreenShot(@"c:\Development\p926-ddas\documents\technical\images\" +
                    "FDAWarningsLetter_" + DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".png");

                _FDAWarningSiteData.DataExtractionSucceeded = false;
                _FDAWarningSiteData.DataExtractionErrorMessage = e.Message;
                _FDAWarningSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _FDAWarningSiteData.CreatedBy = "Patrick";
                _FDAWarningSiteData.CreatedOn = DateTime.Now;
            }
        }

        private void ReadSiteLastUpdatedDateFromPage()
        {
            string[] DataInPageLastUpdatedElement = PageLastUpdatedTextElement.Text.Split(':');

            string PageLastUpdated =
                DataInPageLastUpdatedElement[1].Replace("\r\nNote", "").Trim();

            DateTime RecentLastUpdatedDate;

            DateTime.TryParseExact(PageLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.FDAWarningLettersRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _FDAWarningSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.FDAWarningLettersRepository.Add(_FDAWarningSiteData);
        }

        public override void LoadContent(string DownloadsFolder)
        {
            throw new NotImplementedException();
        }
    }
}
