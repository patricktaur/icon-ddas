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
    public partial class AdequateAssuranceListPage : BaseSearchPage //BaseClasses.BasePage
    {
        public AdequateAssuranceListPage(IWebDriver driver) : base(driver)
        {
            Open();
            SaveScreenShot("AdequateAssuranceListPage.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.fda.gov/ora/compliance_ref/bimo/asurlist.htm";
            }
        }

        private List<AdequateAssuranceList> _adequateAssuranceList;

        public List<AdequateAssuranceList> AdequateAssuranceIvestigatorList {
            get {
                return _adequateAssuranceList;
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.AdequateAssuranceListPage;
            }
        }

        public void LoadAdequateAssuranceInvestigators()
        {
            _adequateAssuranceList = new List<AdequateAssuranceList>();

            foreach(IWebElement TR in 
                AdequateAssuranceListTable.FindElements(By.XPath("//tbody/tr")))
            {
                var AdequateAssuranceInvestigator = new AdequateAssuranceList();

                //if (TR.FindElements(By.XPath("th")).Count > 0)
                //{
                //    continue;
                //}

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));
                if (TDs.Count > 0)
                {
                    AdequateAssuranceInvestigator.NameAndAddress = TDs[0].Text;
                    AdequateAssuranceInvestigator.Center = TDs[1].Text;
                    AdequateAssuranceInvestigator.Type = TDs[2].Text;
                    AdequateAssuranceInvestigator.ActionDate = TDs[3].Text;
                    AdequateAssuranceInvestigator.Comments = TDs[4].Text;

                    _adequateAssuranceList.Add(AdequateAssuranceInvestigator);
                }
            }
        }

        public override ResultAtSite Search(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach(AdequateAssuranceList AssuranceList in _adequateAssuranceList)
            {
                string WordFound = FindSubString(AssuranceList.NameAndAddress, NameToSearch);

                if (WordFound != null)
                {
                    searchResult.Results.Add(new MatchResult
                    {
                        MatchName = AssuranceList.NameAndAddress,
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

        public override void LoadContent(string NameToSearch)
        {
            LoadAdequateAssuranceInvestigators();
        }

        public class AdequateAssuranceList
        {
            public string NameAndAddress { get; set; }
            public string Center { get; set; }
            public string Type { get; set; }
            public string ActionDate { get; set; }
            public string Comments { get; set; }
        }
    }
}
