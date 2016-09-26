using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class FDAWarningLettersPage : BaseSearchPage
    {
        public FDAWarningLettersPage(IWebDriver driver) : base(driver)
        {
            _warningLetterList = new List<FDAWarningLetter>();
            Open();
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

        private List<FDAWarningLetter> _warningLetterList;

        public List<FDAWarningLetter> FDAWarningLetters {
            get {
                return _warningLetterList;
            }
        }


        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach(FDAWarningLetter WarningLetter in _warningLetterList)
            {
                string WordFound = FindSubString(WarningLetter.Company, NameToSearch);

                if (WordFound != null)
                {
                    searchResult.SiteName = SiteName.ToString();

                    searchResult.Results.Add(new MatchResult
                    {
                        MatchName = WarningLetter.Company,
                        MatchLocation = "Word(s) matched - " + WordFound
                    });
                }
            }

            if (searchResult.Results.Count == 0)
            {
                searchResult.Results.Add(new MatchResult
                {
                    MatchName = "None",
                    MatchLocation = "None"
                });
                return searchResult;
            }
            else
                return searchResult;
        }

        public bool SearchTerms(string Name)
        {
            IWebElement Input = FDASearchTextBox;
            Input.Clear();
            Input.SendKeys(Name);

            IWebElement Search = FDASearchButton;
            Search.SendKeys(Keys.Enter);

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(120));

            IWebElement Table = FDASortTable;

            string[] Text = Table.Text.Split(':');

            if (Text[1].Trim() == "0")
                return false;
            else
            {
                IList<IWebElement> TR = Table.FindElements(By.XPath("//tbody/tr"));

                IWebElement TD = TR[TR.Count - 1].FindElement(By.XPath("td"));

                IList<IWebElement> AnchorTags = TD.FindElements(By.XPath("a"));

                AnchorTags[AnchorTags.Count - 1].SendKeys(Keys.Enter);

                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

                return true;
            }
        }


        public void LoadFDAWarningLetters()
        {
            IList<IWebElement> TR = FDASortTable.FindElements(By.XPath("//tbody/tr"));

            for (int tableRow = 12; tableRow < TR.Count - 1; tableRow++)
            {
                var FDAWarningList = new FDAWarningLetter();

                IList<IWebElement> TDs = TR[tableRow].FindElements(By.XPath("td"));

                FDAWarningList.Company = TDs[0].Text;
                FDAWarningList.LetterIssued = TDs[1].Text;
                FDAWarningList.IssuingOffice = TDs[2].Text;
                FDAWarningList.Subject = TDs[3].Text;
                FDAWarningList.ResponseLetterPosted = TDs[4].Text;
                FDAWarningList.CloseoutDate = TDs[5].Text;

                _warningLetterList.Add(FDAWarningList);
            }
        }

        public override void LoadContent()
        { }
        public override void LoadContent(string NameToSearch)
        {
            string[] Name = NameToSearch.Split(' ');

            for (int counter = 0; counter < Name.Length; counter++)
            {
                Name[counter].Replace(",", "");
                if(SearchTerms(Name[counter]))
                {
                    LoadFDAWarningLetters();
                }
            }
        }

        public override void SaveData()
        {

        }


        public class FDAWarningLetter
        {
            public string Company { get; set; }
            public string LetterIssued { get; set; }
            public string IssuingOffice { get; set; }
            public string Subject { get; set; }
            public string ResponseLetterPosted { get; set; }
            public string CloseoutDate { get; set; }
        }
    }
}
