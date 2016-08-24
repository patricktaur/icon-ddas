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
            Open();
            SaveScreenShot("ExclusionDatabaseSearch.png");
        }

        public override string Url
        {
            get
            {
                return @"http://exclusions.oig.hhs.gov/";
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

        public void SearchTerms(string FirstName, string LastName)
        {
            IWebElement FNameElement = ExclusionDatabaseSearchFirstName;
            FNameElement.SendKeys(FirstName);

            IWebElement LNameElement = ExclusionDatabaseSearchLastName;
            LNameElement.SendKeys(LastName);

            IWebElement SearchElement = ExclusionDatabaseSearchElement;
            SearchElement.SendKeys(Keys.Enter);
            
            //wait for the page to load
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
        }


        public void LoadExclusionsDatabaseList()
        {
            _exclusionsList = new List<ExclusionDatabaseSearchList>();

            var date = DateTime.Now;

            foreach (IWebElement TR in 
                ExclusionDatabaseSearchTable.FindElements(By.XPath("tbody/tr")))
            {
                ExclusionDatabaseSearchList NewExclusionsList = new ExclusionDatabaseSearchList();

                if (TR.FindElements(By.XPath("th")).Count > 0)
                {
                    continue;
                }

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
                    //Console.WriteLine("Completed Record No:{0}", NewExclusionsList.FirstName);
                }
            }
            //Console.WriteLine("Start Time: {0}", date);
            //Console.WriteLine("End Time: {0}", DateTime.Now);
        }

        public override ResultAtSite Search(string NameToSearch)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            throw new NotImplementedException();
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
