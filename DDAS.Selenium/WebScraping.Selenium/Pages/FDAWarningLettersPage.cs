using DDAS.Models.Enums;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;

namespace WebScraping.Selenium.Pages
{
    public partial class FDAWarningLettersPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;

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

        private bool SearchTerms(string Name)
        {
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            IWebElement Input = FDASearchTextBox;
            Input.Clear();
            Input.SendKeys(Name);

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            IWebElement Search = FDASearchButton;
            Search.Click();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            IWebElement Table = FDASortTable;

            string[] Text = Table.Text.Split(':');

            if (Text[1].Trim() == "0")
                return false;
            else
            {
                IList<IWebElement> TR = Table.FindElements(By.XPath("//tbody/tr"));

                IWebElement TD = TR[TR.Count - 1].FindElement(By.XPath("td"));

                IList<IWebElement> AnchorTags = TD.FindElements(By.XPath("a"));

                if (AnchorTags.Count > 0)
                    AnchorTags[AnchorTags.Count - 1].Click();

                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

                return true;
            }
        }

        private FDAWarningLettersSiteData _FDAWarningSiteData;

        private int RowNumber = 1;

        public void LoadFDAWarningLetters()
        {
            _FDAWarningSiteData.CreatedBy = "Patrick";
            _FDAWarningSiteData.SiteLastUpdatedOn = DateTime.Now;
            _FDAWarningSiteData.CreatedOn = DateTime.Now;

            IList<IWebElement> TR = FDASortTable.FindElements(By.XPath("//tbody/tr"));

            for (int tableRow = 12; tableRow < TR.Count - 1; tableRow++)
            {
                var FDAWarningList = new FDAWarningLetter();

                IList<IWebElement> TDs = TR[tableRow].FindElements(By.XPath("td"));

                FDAWarningList.RowNumber = RowNumber;
                FDAWarningList.Company = TDs[0].Text;
                FDAWarningList.LetterIssued = TDs[1].Text;
                FDAWarningList.IssuingOffice = TDs[2].Text;
                FDAWarningList.Subject = TDs[3].Text;
                FDAWarningList.ResponseLetterPosted = TDs[4].Text;
                FDAWarningList.CloseOutDate = TDs[5].Text;

                _FDAWarningSiteData.FDAWarningLetterList.Add(FDAWarningList);
                RowNumber += 1;
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
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
                        if (FullName[Counter].Length > 1 && SearchTerms(FullName[Counter]))
                            LoadFDAWarningLetters();
                    }
                }
                _FDAWarningSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                _FDAWarningSiteData.DataExtractionSucceeded = false;
                _FDAWarningSiteData.DataExtractionErrorMessage = e.Message;
                _FDAWarningSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {

            }
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.FDAWarningLettersRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _FDAWarningSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            if(_FDAWarningSiteData != null)
                _UOW.FDAWarningLettersRepository.Add(_FDAWarningSiteData);
        }
    }
}
