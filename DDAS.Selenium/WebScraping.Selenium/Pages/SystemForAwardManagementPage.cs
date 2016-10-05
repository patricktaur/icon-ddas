using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
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

        private List<SystemForAwardManagement> _samList;

        public List<SystemForAwardManagement> SAMList {
            get {
                return _samList;
            }
        }

        private SystemForAwardManagementPageSiteData _SAMSiteData;

        //need to refactor
        public void LoadSAMList()
        {
            _SAMSiteData = new SystemForAwardManagementPageSiteData();

            _SAMSiteData.CreatedBy = "Patrick";
            _SAMSiteData.SiteLastUpdatedOn = DateTime.Now;

            IList<IWebElement> TableThatContainsRecords =
                SAMCheckResult.FindElements
                (By.XPath("//tbody/tr/td/ul/table/tbody/tr/td/li/table"));

            foreach (IWebElement RecordsTable in TableThatContainsRecords)
            {
                Debug.Print(RecordsTable.Text);

                var SAMDataList = new SystemForAwardManagement();

                string TempContent = RecordsTable.Text.Replace("\n", "");
                string[] ContentOfEachRecord = TempContent.Split('\r');

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
            }
        }

        public bool SearchTerms(string NameToSearch)
        {
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            IWebElement Anchor = SAMAnchorTag;
            Anchor.Click();

            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            IWebElement TextBox = SAMInputTag;
            TextBox.SendKeys(NameToSearch);

            IWebElement Submit = SAMSubmitButton;
            Submit.Click();

            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            if (SAMCheckResult != null)
            {
                return true;
            }
            else
            {
                IWebElement ClearSearch = SAMClearSearch;
                ClearSearch.Click();

                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

                return false;
            }
        }

        public override void LoadContent()
        {
            
        }

        public override void LoadContent(string NameToSearch)
        {
            string[] Name = NameToSearch.Split(' ');

            for (int counter = 0; counter < Name.Length; counter++)
            {
                if (SearchTerms(Name[counter]))
                {
                    while (CheckForAnchorTagNext())
                    {
                        LoadSAMList();

                        LoadNextRecord();
                    }
                    LoadSAMList();
                }
                else
                    continue;
            }
        }

        public bool CheckForAnchorTagNext()
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

        public void LoadNextRecord()
        {
            IList<IWebElement> AnchorsInPagination = 
                SAMPaginationElement.FindElements(By.XPath("table/tbody/tr/td/a"));

            AnchorsInPagination[AnchorsInPagination.Count - 1].Click();

            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
        }

        public override void SaveData()
        {

        }
    }
}
