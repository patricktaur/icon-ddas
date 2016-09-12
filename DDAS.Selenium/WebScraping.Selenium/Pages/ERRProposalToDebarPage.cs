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
    public partial class ERRProposalToDebarPage : BaseSearchPage //BaseClasses.BasePage
    {

        public ERRProposalToDebarPage(IWebDriver driver) : base(driver)
        {
            Open();
            //SaveScreenShot("ProposalToDebarPage.png");
        }

        public override string Url
        {
            get
            {
                return 
                @"http://www.fda.gov/RegulatoryInformation/FOI/ElectronicReadingRoom/ucm143240.htm";
            }
        }

        private List<ProposalToDebar> _ProposalToDebarList;

        public List<ProposalToDebar> propToDebar
        {
            get { return _ProposalToDebarList; }
        }

        public override SiteEnum SiteName
        {
            get
            {
                return SiteEnum.ERRProposalToDebarPage;
            }
        }

        public void LoadProposalToDebarList()
        {
            _ProposalToDebarList = new List<ProposalToDebar>();

            foreach (IWebElement TR in ProposalToDebarTable.FindElements(By.XPath("//tbody/tr")))
            {
                var proposalToDebarList = new ProposalToDebar();
                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                proposalToDebarList.name = TDs[0].Text;
                proposalToDebarList.center = TDs[1].Text;
                proposalToDebarList.date = TDs[2].Text;
                proposalToDebarList.IssuingOffice = TDs[3].Text;

                _ProposalToDebarList.Add(proposalToDebarList);
            }
        }

        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach (ProposalToDebar proposalToDebar in propToDebar)
            {
                string WordFound = FindSubString(proposalToDebar.name, NameToSearch);

                if (WordFound != null)
                {
                    searchResult.Results.Add(new MatchResult
                    {
                        MatchName = proposalToDebar.name,
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
            LoadProposalToDebarList();
        }

        public class ProposalToDebar
        {
            public string name { get; set; }
            public string center { get; set; }
            public string date { get; set; }
            public string IssuingOffice { get; set; }
        }
    }
}
