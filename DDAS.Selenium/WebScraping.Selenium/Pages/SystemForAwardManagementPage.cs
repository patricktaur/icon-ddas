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

        public void LoadSAMList()
        {
            IList<IWebElement> Tables =
                SAMSearchResult.FindElements(By.XPath("//table"));

            foreach (IWebElement Table in Tables)
            {
                var SAMDataList = new SystemForAwardManagement();
                Debug.WriteLine(Table.Text);
            }
        }

        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            throw new NotImplementedException();
        }

        public bool SearchTerms(string NameToSearch)
        {
            IWebElement Anchor = SAMAnchorTag;
            Anchor.SendKeys(Keys.Enter);

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
