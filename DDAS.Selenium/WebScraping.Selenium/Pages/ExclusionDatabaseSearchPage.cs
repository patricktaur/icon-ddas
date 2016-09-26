using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;

namespace WebScraping.Selenium.Pages
{
    public partial class ExclusionDatabaseSearchPage : BaseSearchPage //BaseClasses.BasePage
    {
        public ExclusionDatabaseSearchPage(IWebDriver driver) : base(driver)
        {
            _exclusionsList = new List<ExclusionDatabaseSearchList>();
            Open();
            //SaveScreenShot("ExclusionDatabaseSearch.png");
        }

        public override string Url
        {
            get
            {
                return @"http://exclusions.oig.hhs.gov";
            }
        }

        private List<ExclusionDatabaseSearchList> _exclusionsList;

        public List<ExclusionDatabaseSearchList> ExclusionDatabaseList {
            get {
                return _exclusionsList; }
        }

        public override SiteEnum SiteName
        {
            get
            {
                return SiteEnum.ExclusionDatabaseSearchPage;
            }
        }

        public bool SearchTerms(string FirstName, string LastName)
        {
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            IWebElement FNameElement = ExclusionDatabaseSearchFirstName;
            FNameElement.SendKeys(FirstName);

            IWebElement LNameElement = ExclusionDatabaseSearchLastName;
            LNameElement.SendKeys(LastName);

            IWebElement SearchElement = ExclusionDatabaseSearchElement;
            SearchElement.SendKeys(Keys.Enter);
            
            //wait for the page to load
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            if (ExclusionDatabaseSearchTable != null)
            {
                return true;
            }
            else
                return false;
        }


        public void LoadExclusionsDatabaseList()
        {
            foreach (IWebElement TR in 
                ExclusionDatabaseSearchTable.FindElements(By.XPath("tbody/tr")))
            {
                ExclusionDatabaseSearchList NewExclusionsList = new ExclusionDatabaseSearchList();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                if(TDs.Count != 0)
                {
                    NewExclusionsList.LastName = TDs[0].Text;
                    NewExclusionsList.FirstName = TDs[1].Text;
                    NewExclusionsList.MiddleName = TDs[2].Text;
                    NewExclusionsList.General = TDs[3].Text;
                    NewExclusionsList.Specialty = TDs[4].Text;
                    NewExclusionsList.Exclusion = TDs[5].Text;
                    NewExclusionsList.Waiver = TDs[6].Text;
                    NewExclusionsList.SSNorEIN = TDs[7].Text;

                    _exclusionsList.Add(NewExclusionsList);
                }
            }
        }

      

        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach (ExclusionDatabaseSearchList person in _exclusionsList)
            {
                string FullName = person.FirstName + " " + person.MiddleName + " " + person.LastName;

                string WordFound = FindSubString(FullName, NameToSearch);

                if (WordFound != null)
                {
                    searchResult.SiteName = SiteName.ToString();

                    searchResult.Results.Add(new MatchResult
                    {
                        MatchName = FullName,
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

        public override void LoadContent()
        { }

        public override void LoadContent(string NameToSearch)
        {
             
            string[] FullName = NameToSearch.Split(' ');

            if (SearchTerms(FullName[0], FullName[1]))
                LoadExclusionsDatabaseList();

            if (SearchTerms(FullName[1], FullName[0]))
                LoadExclusionsDatabaseList();
                
        }

        public override void SaveData()
        {

        }
        public class ExclusionDatabaseSearchList
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string General { get; set; }
            public string Specialty { get; set; }
            public string Exclusion { get; set; }
            public string Waiver { get; set; }
            public string SSNorEIN { get; set; }
        }
    }
}
