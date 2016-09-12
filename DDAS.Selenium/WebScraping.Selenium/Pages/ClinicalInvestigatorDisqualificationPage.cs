using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models.Entities.Domain;

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorDisqualificationPage : BaseSearchPage //BaseClasses.BasePage
    {
        public ClinicalInvestigatorDisqualificationPage(IWebDriver driver) : base(driver)
        {
            Open();
            //SaveScreenShot("ClinicalInvestigatorDisqualificationPage.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.accessdata.fda.gov/scripts/SDA/sdNavigation.cfm?sd=clinicalinvestigatorsdisqualificationproceedings&previewMode=true&displayAll=true";
            }
        }

        private List<DisqualifiedInvestigator> _disqulifiedInvestigotor;

        public List<DisqualifiedInvestigator> DisqualifiedInvestigatorList {
            get {
                return _disqulifiedInvestigotor;
            }
        }

        public void LoadDisqualificationProceedingsList()
        {
            _disqulifiedInvestigotor = new List<DisqualifiedInvestigator>();

            foreach (IWebElement TR in
                DisqualifiedInvestigatorTable.FindElements(By.XPath("tbody/tr")))
            {
                var DisqualifiedClinicalInvestigator = new DisqualifiedInvestigator();

                //if (TR.FindElements(By.XPath("th")).Count > 0)
                //{
                //    continue;
                //}

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                if (TDs.Count > 0)
                {
                    DisqualifiedClinicalInvestigator.Name = TDs[0].Text;
                    DisqualifiedClinicalInvestigator.Center = TDs[1].Text;
                    DisqualifiedClinicalInvestigator.Status = TDs[2].Text;
                    DisqualifiedClinicalInvestigator.DateOfStatus = TDs[3].Text;
                    DisqualifiedClinicalInvestigator.DateNIDPOEIssued = TDs[4].Text;
                    DisqualifiedClinicalInvestigator.DateNOOHIssued = TDs[5].Text;
                    DisqualifiedClinicalInvestigator.LinkToNIDPOELetter = TDs[6].Text;
                    DisqualifiedClinicalInvestigator.LinkToNOOHLetter = TDs[7].Text;

                    _disqulifiedInvestigotor.Add(DisqualifiedClinicalInvestigator);
                }
            }
        }

        public override SiteEnum SiteName
        {
            get
            {
                return SiteEnum.ClinicalInvestigatorDisqualificationPage;
            }
        }

        public override void LoadContent(string NameToSearch)
        {
            LoadDisqualificationProceedingsList();
        }

     
        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach (DisqualifiedInvestigator person in _disqulifiedInvestigotor)
            {
                string WordFound = FindSubString(person.Name, NameToSearch);

                if (WordFound != null)
                {
                    searchResult.SiteName = SiteName.ToString();

                    searchResult.Results.Add(new MatchResult
                    {
                        MatchName = person.Name,
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

        public class DisqualifiedInvestigator
        {
            public string Name { get; set; }
            public string Center { get; set; }
            public string Status { get; set; }
            public string DateOfStatus { get; set; }
            public string DateNIDPOEIssued { get; set; }
            public string DateNOOHIssued { get; set; }
            public string LinkToNIDPOELetter { get; set; }
            public string LinkToNOOHLetter { get; set; }
        }
    }
}
