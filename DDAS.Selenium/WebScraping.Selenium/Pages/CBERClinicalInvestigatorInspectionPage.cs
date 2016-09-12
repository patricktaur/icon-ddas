using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;

namespace WebScraping.Selenium.Pages
{
    public partial class CBERClinicalInvestigatorInspectionPage : BaseSearchPage //BaseClasses.BasePage
    {
        public CBERClinicalInvestigatorInspectionPage(IWebDriver driver) : base(driver)
        {
            _clinicalInvestigator = new List<CBERClinicalInvestigator>();
            Open();
            //SaveScreenShot("CBERClinicalInvestigatorInspectionPage.png");
        }

        public override string Url
        {
            get
            {
                return @"http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195364.htm";
            }
        }

        private List<CBERClinicalInvestigator> _clinicalInvestigator;
        public List<CBERClinicalInvestigator> ClinicalInvestigator
        {
            get {
                return _clinicalInvestigator;
            }
        }

        public void LoadNextInspectionList()
        {
            //links to extract records starting with E-K, L-P, Q-S and T-Z 
            string[] InspectionList = new string[] {
                "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195367.htm",
                "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195632.htm",
                "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195633.htm",
                "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm197065.htm"
            };

            LoadCBERClinicalInvestigators();

            foreach(string arrItem in InspectionList)
            {
                driver.Url = arrItem;
                LoadCBERClinicalInvestigators();
            }
        }

        public void LoadCBERClinicalInvestigators()
        {
            foreach (IWebElement TR in 
                CBERClinicalInvestigatorTable.FindElements(By.XPath("tbody/tr")))
            {
                var ClinicalInvestigatorCBER = new CBERClinicalInvestigator();

                //if (TR.FindElements(By.XPath("th")).Count > 0)
                //{
                //    continue;
                //}

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));
                if (TDs.Count > 0)
                {
                    ClinicalInvestigatorCBER.Name = TDs[0].Text;
                    ClinicalInvestigatorCBER.Title = TDs[1].Text;
                    ClinicalInvestigatorCBER.InstituteAndAddress = TDs[2].Text;
                    ClinicalInvestigatorCBER.InspectionStartAndEndDate = TDs[3].Text;
                    ClinicalInvestigatorCBER.Classification = TDs[4].Text;

                    _clinicalInvestigator.Add(ClinicalInvestigatorCBER);
                }
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.CBERClinicalInvestigatorInspectionPage;
            }
        }

        public override void LoadContent(string NameToSearch)
        {
            LoadNextInspectionList();
        }

     

        public override ResultAtSite GetResultAtSite(string NameToSearch)
        {
            ResultAtSite searchResult = new ResultAtSite();

            searchResult.SiteName = SiteName.ToString();

            foreach (CBERClinicalInvestigator person in _clinicalInvestigator)
            {
                string WordFound = base.FindSubString(person.Name, NameToSearch);

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


        public class CBERClinicalInvestigator
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string InstituteAndAddress { get; set; }
            public string InspectionStartAndEndDate { get; set; }
            public string Classification { get; set; }
        }
    }
}
