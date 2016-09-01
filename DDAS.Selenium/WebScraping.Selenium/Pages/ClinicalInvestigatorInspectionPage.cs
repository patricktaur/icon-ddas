using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorInspectionPage : BaseSearchPage //BaseClasses.BasePage
    {
        public ClinicalInvestigatorInspectionPage(IWebDriver driver) : base(driver)
        {
            _clinicalInvestigatorList = new List<ClinicalInvestigator>();
            Open();
            SaveScreenShot("ClinicalInvestigatorInspectionPage.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm";
            }
        }

        private List<ClinicalInvestigator> _clinicalInvestigatorList;

        public List<ClinicalInvestigator> clinicalInvestigatorList {
            get { return _clinicalInvestigatorList; }
        }

        public bool SearchTerms(string Name)
        {
            IWebElement InputTag = ClinicalInvestigatorInputTag;
            InputTag.SendKeys(Name);

            IWebElement SubmitButton = ClinicalInvestigatorSubmit;
            SubmitButton.Submit();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            if (ClinicalInvestigatorNext != null)
            {
                return true;
            }
            else
                return false;
        }

        public void LoadClinicalInvestigatorList()
        {
            foreach (IWebElement TR in
                ClinicalInvestigatorTable.FindElements(By.XPath("//tbody/tr")))
            {
                var InvestigatorList = new ClinicalInvestigator();
                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                InvestigatorList.IdNumber = TDs[0].Text;
                InvestigatorList.Name = TDs[1].Text;
                InvestigatorList.Location = TDs[2].Text;
                InvestigatorList.Address = TDs[3].Text;
                InvestigatorList.City = TDs[4].Text;
                InvestigatorList.State = TDs[5].Text;
                InvestigatorList.Country = TDs[6].Text;
                InvestigatorList.Zipcode = TDs[7].Text;
                InvestigatorList.InspectionDate = TDs[8].Text;
                InvestigatorList.Type = TDs[9].Text;
                InvestigatorList.Class = TDs[10].Text;
                InvestigatorList.DefTypes = TDs[11].Text;

                _clinicalInvestigatorList.Add(InvestigatorList);
            }
        }

        public int GetCountOfRecords()
        {
            IWebElement element = ClinicalInvestigatorNextList;

            IList<IWebElement> ANCHORs = element.FindElements(By.XPath("//span/a"));

            int AnchorCount = ANCHORs.Count;

            return Convert.ToInt32(ANCHORs[AnchorCount - 1].Text);
        }


        public override SiteEnum SiteName {
            get {
                return SiteEnum.ClinicalInvestigatorInspectionPage;
            }
        }

        public override ResultAtSite Search(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach (ClinicalInvestigator person in _clinicalInvestigatorList)
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

        public override void LoadContent(string NameToSearch)
        {
            string[] Name = NameToSearch.Split(' ');
            
            for (int counter = 0; counter < Name.Length; counter++)
            {
                if (SearchTerms(Name[counter]))
                {
                    int totalRecords = GetCountOfRecords();

                    for (int records = 0; records < totalRecords; records++)
                    {
                        LoadClinicalInvestigatorList();

                        if (totalRecords > 1)
                        {
                            LoadNextRecord();
                        }
                    }
                }
                else
                    continue;

                driver.Url = "http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm";
            }
        }

        public void LoadNextRecord()
        {
            IWebElement element = ClinicalInvestigatorNext;

            element.SendKeys(Keys.Enter);
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
        }

        public class ClinicalInvestigator
        {
            public string IdNumber { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string Zipcode { get; set; }
            public string InspectionDate { get; set; }
            public string Type { get; set; }
            public string Class { get; set; }
            public string DefTypes { get; set; }
        }

    }
}
