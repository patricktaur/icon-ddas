using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;

namespace WebScraping.Selenium.Pages
{
   public partial class FDADebarPage : BaseSearchPage //BaseClasses.BasePage
    {
       
       public FDADebarPage(IWebDriver driver) : base(driver)
        {
            Open();
            SaveScreenShot("abc.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.fda.gov/ora/compliance_ref/debar/default.htm";
            }
        }

        private List<DebarredPerson> _DebarredPersonList;
        public List<DebarredPerson> DebarredPersons
        {
            get
            {
                return _DebarredPersonList;
            }
        }

        public void LoadDebarredPersonList()
        {
            _DebarredPersonList = new List<DebarredPerson>();

            foreach (IWebElement TR in PersonsTable.FindElements(By.XPath("tbody/tr")))
            {
                var debarredPerson = new DebarredPerson();
                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                debarredPerson.NameOfPerson = TDs[0].Text;
                debarredPerson.EffectiveDate =TDs[1].Text;
                debarredPerson.EndOfTermOfDebarment = TDs[2].Text;
                debarredPerson.FrDateText = TDs[3].Text;
                debarredPerson.VolumePage = TDs[4].Text;

                if (IsElementPresent(TDs[4], By.XPath("a")))
                {
                    IWebElement anchor = TDs[4].FindElement(By.XPath("a"));
                    debarredPerson.DocumentLink = anchor.GetAttribute("href");
                }

                _DebarredPersonList.Add(debarredPerson);
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.FDADebarPage;
            }
        }

        public override ResultAtSite Search(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();
            //searchResult.Results = new List<MatchResult>();

            searchResult.SiteName = SiteName.ToString();

            foreach (DebarredPerson person in _DebarredPersonList)
            {
                string WordFound = FindSubString(person.NameOfPerson, NameToSearch);

                if (WordFound != null)
                {
                    searchResult.SiteName = SiteName.ToString();

                    searchResult.Results.Add(new MatchResult
                    {
                        MatchName = person.NameOfPerson,
                        MatchLocation = "Word(s) matched - " + WordFound
                    });
                }
            }

            if (searchResult.Results.Count == 0)
            {
                searchResult.Results.Add(new MatchResult {
                MatchName = "None",
                MatchLocation = "None"});
                return searchResult;
            }
            else
                return searchResult;
        }

        public override void LoadContent()
        {
            LoadDebarredPersonList();
        }

        public string FindSubString(string SearchString, string NameToSearch)
        {
            SearchString = SearchString.ToLower();

            string[] FullName = NameToSearch.Split(' ');

            int count = 0;
            string WordMatched = null;

            for (int i=0; i<FullName.Length; i++)
            {
                if (SearchString.Contains(FullName[i].ToLower()))
                {
                    count = count + 1;
                    WordMatched = WordMatched + " " + FullName[i];
                }
            }
            return WordMatched;
        }

        public class DebarredPerson
        {
            public string NameOfPerson { get; set; }
            public string EffectiveDate { get; set; }
            public string EndOfTermOfDebarment { get; set; }
            public string FrDateText { get; set; }
            public string VolumePage { get; set; }
            public string DocumentLink { get; set; }
        }

    }
}
