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

namespace WebScraping.Selenium.Pages
{
    public partial class SystemForAwardManagementPage : BaseSearchPage
    {
        public SystemForAwardManagementPage(IWebDriver driver) : base(driver)
        {
            _samList = new List<SystemForAwardManagement>();
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

        //need to refactor
        public void LoadSAMList()
        {
            IList<IWebElement> Tables =
                SAMCheckResult.FindElements
                (By.XPath("//tbody/tr/td/ul/table/tbody/tr/td/li/table/tbody/tr/td/table"));

            foreach (IWebElement Table in Tables)
            {
                IList<IWebElement> TRs = Table.FindElements(By.XPath("tbody/tr"));

                var SAMDataList = new SystemForAwardManagement();

                IList<IWebElement> OuterTDs = TRs[0].FindElements(By.XPath("td"));

                //condtion is for Duns, Expiration date, HasActiveExlcusion, purposeOfRegistration etc..
                if(IsElementPresent(OuterTDs[0], By.XPath("table/tbody/tr")))
                {
                    IList<IWebElement> TRows = OuterTDs[0].FindElements(By.XPath("table/tbody/tr"));

                    foreach(IWebElement Tr in TRows)
                    {
                        IList<IWebElement> InnerTDs = Tr.FindElements(By.XPath("td"));

                        if (IsElementPresent(InnerTDs[0], By.XPath("span")))
                        {
                            IList<IWebElement> Spans = InnerTDs[0].FindElements(By.XPath("span"));

                            if (Spans[0].Text.ToLower().Contains("duns"))
                                SAMDataList.Duns = Spans[1].Text;

                            else if (Spans[0].Text.ToLower().Contains("has active exclsion"))
                                SAMDataList.HasActiveExclusion = Spans[1].Text;

                            else if (Spans[0].Text.ToLower().Contains("expiration date"))
                                SAMDataList.ExpirationDate = Spans[1].Text;

                            else if (Spans[0].Text.ToLower().Contains("purpose of registration"))
                                SAMDataList.PurposeOfRegistration = Spans[1].Text;
                        }

                        else if(IsElementPresent(InnerTDs[1], By.XPath("span")))
                        {
                            IList<IWebElement> Spans = OuterTDs[1].FindElements(By.XPath("span"));
                            if(Spans[0].Text.ToLower().Contains("doddac"))
                                SAMDataList.DoDAAC = Spans[1].Text;

                            else if (Spans[0].Text.ToLower().Contains("delinquent federal debt"))
                                SAMDataList.DelinquentFederalDebt = Spans[1].Text;
                        }

                        else if (IsElementPresent(InnerTDs[2], By.XPath("span")))
                        {
                            IList<IWebElement> Spans = OuterTDs[2].FindElements(By.XPath("span"));
                            SAMDataList.CAGECode = Spans[1].Text;
                        }
                    }
                }
                //for Entity and Status
                else if (IsElementPresent(OuterTDs[1], By.XPath("span")))
                {
                    IList<IWebElement> Spans = OuterTDs[1].FindElements(By.XPath("span"));
                    SAMDataList.Entity = Spans[0].Text;

                    if(IsElementPresent(OuterTDs[2], By.XPath("div/span")))
                    {
                        IList<IWebElement> Span = OuterTDs[2].FindElements(By.XPath("div/span"));
                        SAMDataList.Status = Span[1].Text;
                    }   
                }
                _samList.Add(SAMDataList);
            }
        }

        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            throw new NotImplementedException();
        }

        public bool SearchTerms(string NameToSearch)
        {
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            IWebElement Anchor = SAMAnchorTag;
            Anchor.Click();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            IWebElement TextBox = SAMInputTag;
            TextBox.SendKeys(NameToSearch);

            IWebElement Submit = SAMSubmitButton;
            Submit.SendKeys(Keys.Enter);

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            if (SAMCheckResult != null)
            {
                return true;
            }
            else
            {
                IWebElement ClearSearch = SAMClearSearch;
                ClearSearch.SendKeys(Keys.Enter);

                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

                return false;
            }
        }

        public override void LoadContent(string NameToSearch)
        {
            string[] Name = NameToSearch.Split(' ');

            for (int counter = 0; counter < Name.Length; counter++)
            {
                Name[counter] = Name[counter].Replace(",", " ");

                if (SearchTerms(Name[counter]))
                {
                    LoadSAMList();
                    //int totalRecords = GetCountOfRecords();

                    //for (int records = 0; records < totalRecords; records++)
                    //{
                        //Load();

                        //if (totalRecords > 1)
                        //{
                        //    LoadNextRecord();
                        //}
                    }
                else
                    continue;
            }
        }

        public class SystemForAwardManagement
        {
            public string Entity { get; set; }
            public string Status { get; set; }
            public string Duns { get; set; }
            public string HasActiveExclusion { get; set; }
            public string ExpirationDate { get; set; }
            public string PurposeOfRegistration { get; set; }
            public string CAGECode { get; set; }
            public string DoDAAC { get; set; }
            public string DelinquentFederalDebt { get; set; }
        }
    }
}
