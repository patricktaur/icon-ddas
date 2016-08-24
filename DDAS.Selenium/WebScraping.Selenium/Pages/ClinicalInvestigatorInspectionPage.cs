using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public List<ClinicalInvestigator> clinicalInvestigatorList
        {
            get { return _clinicalInvestigatorList; }
        }

        public override SiteEnum SiteName
        {
            get
            {
                return SiteEnum.ClinicalInvestigatorInspectionPage;
            }
        }

        public void SearchTerms(string Name)
        {
            IWebElement InputTag = ClinicalInvestigatorInputTag;
            InputTag.SendKeys(Name);

            IWebElement SubmitButton = ClinicalInvestigatorSubmit;

            SubmitButton.Submit();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
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

        public bool GetNextList()
        {
            IWebElement element = ClinicalInvestigatorNext;

            if (!element.GetAttribute("ClassName").Contains("disabled"))
            {
                element.SendKeys(Keys.Enter);
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                return true;
            }
            return false;
        }

        public override ResultAtSite Search(string NameToSearch)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            throw new NotImplementedException();
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
