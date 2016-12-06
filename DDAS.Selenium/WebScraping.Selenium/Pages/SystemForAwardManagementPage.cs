using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using System.Diagnostics;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

namespace WebScraping.Selenium.Pages
{
    public partial class SystemForAwardManagementPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;

        public SystemForAwardManagementPage(IWebDriver driver, IUnitOfWork uow) 
            : base(driver)
        {
            _UOW = uow;
            Open();
            _SAMSiteData.Source = driver.Url;
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.SystemForAwardManagementPage;
            }
        }

        public override string Url {
            get {
                return @"https://www.sam.gov/portal/public/SAM";
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _SAMSiteData.SAMSiteData;
            }
        }

        private SystemForAwardManagementPageSiteData _SAMSiteData;

        //need to refactor
        private void LoadSAMList()
        {
            _SAMSiteData = new SystemForAwardManagementPageSiteData();

            _SAMSiteData.CreatedBy = "Patrick";
            _SAMSiteData.CreatedOn = DateTime.Now;
            _SAMSiteData.SiteLastUpdatedOn = DateTime.Now;

            int RowNumber = 1;

            IList<IWebElement> TableThatContainsRecords =
                SAMCheckResult.FindElements
                (By.XPath("//tbody/tr/td/ul/table/tbody/tr/td/li/table"));

            foreach (IWebElement RecordsTable in TableThatContainsRecords)
            {
                Debug.Print(RecordsTable.Text);

                var SAMDataList = new SystemForAwardManagement();

                string TempContent = RecordsTable.Text.Replace("\n", "");
                string[] ContentOfEachRecord = TempContent.Split('\r');

                SAMDataList.RowNumber = RowNumber;
                SAMDataList.Entity = ContentOfEachRecord[1];

                for (int counter = 2; counter < ContentOfEachRecord.Length; counter++)
                {
                    string Content = ContentOfEachRecord[counter];

                    string[] tempFieldValue = new string[0];

                    if (Content.Contains(":"))
                        tempFieldValue = Content.Split(':');
                    else if (Content.Contains("Entity") || Content.Contains("Exclusion"))
                    {
                        SAMDataList.Entity = ContentOfEachRecord[counter + 2];
                        continue;
                    }

                    if (tempFieldValue.Length >= 1)
                    {
                        switch (tempFieldValue[0])
                        {
                            case "Status": SAMDataList.Status = tempFieldValue[1]; break;

                            case "DUNS":
                                SAMDataList.Duns = tempFieldValue[1].Replace("CAGE Code", "").Trim();
                                if (tempFieldValue.Length > 2)
                                    SAMDataList.CAGECode = tempFieldValue[2];
                                break;

                            case "Has Active Exclusion?":
                                SAMDataList.HasActiveExclusion =
                                tempFieldValue[1].Replace("DoDAAC", "").Trim();
                                if (tempFieldValue.Length > 2)
                                    SAMDataList.DoDAAC = tempFieldValue[2];
                                break;

                            case "Expiration Date":
                                string[] temp =
                                    tempFieldValue[1].Replace("Delinquent Federal Debt?", "?").Split('?');

                                SAMDataList.ExpirationDate =
                                    temp[0].Trim();
                                if (temp.Length > 1)
                                    SAMDataList.DelinquentFederalDebt =
                                    tempFieldValue[1].Split('?')[1].Trim();
                                break;

                            case "Purpose of Registration":
                                if (tempFieldValue.Length > 1)
                                    SAMDataList.PurposeOfRegistration = tempFieldValue[1].Trim();
                                break;

                            case "Classification":
                                if (tempFieldValue.Length > 1)
                                    SAMDataList.Classification = tempFieldValue[1].Trim();
                                break;

                            case "Activation Date":
                                SAMDataList.ActivationDate =
                                    tempFieldValue[1].Replace("Termination Date", "").Trim();
                                if (tempFieldValue.Length > 2)
                                    SAMDataList.TerminationDate = tempFieldValue[2].Trim();
                                break;
                        }
                    }
                }
                _SAMSiteData.SAMSiteData.Add(SAMDataList);
                RowNumber += 1;
            }
        }

        private bool SearchTerms(string NameToSearch)
        {
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            //IWebElement Anchor = SAMAnchorTag;
            SAMAnchorTag.Click();
            //Anchor.SendKeys(Keys.Enter);

            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            IWebElement TextBox = SAMInputTag;
            TextBox.SendKeys(NameToSearch);

            //IWebElement Submit = SAMSubmitButton;
            SAMSubmitButton.Click();
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
            //Submit.SendKeys(Keys.Enter);

            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            if (SAMCheckResult != null)
            {
                return true;
            }
            else
            {
                //IWebElement ClearSearch = SAMClearSearch;
                SAMClearSearch.Click();
                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
                //ClearSearch.Click();
                //ClearSearch.SendKeys(Keys.Enter);

                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

                return false;
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            //string[] Name = NameToSearch.Split(' ');

            //for (int counter = 0; counter < Name.Length; counter++)
            //{
                if (SearchTerms(NameToSearch))
                {
                    while (CheckForAnchorTagNext())
                    {
                        LoadSAMList();

                        LoadNextRecord();
                    }
                    LoadSAMList();
                }
                //else
                //    continue;
            //}
        }

        private bool CheckForAnchorTagNext()
        {
            if (IsElementPresent(SAMPaginationElement, By.XPath("table/tbody/tr/td/a")))
            {
                IList<IWebElement> AnchorsInPagination =
                    SAMPaginationElement.FindElements(By.XPath("table/tbody/tr/td/a"));

                IWebElement LastAnchorTagInPagination = AnchorsInPagination[AnchorsInPagination.Count - 1];

                return
                    (LastAnchorTagInPagination.Text.ToLower() == "next") ?
                    true : false;
            }
            else
                return false;
        }

        private void LoadNextRecord()
        {
            IList<IWebElement> AnchorsInPagination = 
                SAMPaginationElement.FindElements(By.XPath("table/tbody/tr/td/a"));

            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            AnchorsInPagination[AnchorsInPagination.Count - 1].Click();
            //AnchorsInPagination[AnchorsInPagination.Count - 1].SendKeys(Keys.Enter);

            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
        }

        public override void SaveData()
        {
            _UOW.SystemForAwardManagementRepository.Add(_SAMSiteData);
        }
    }
}
