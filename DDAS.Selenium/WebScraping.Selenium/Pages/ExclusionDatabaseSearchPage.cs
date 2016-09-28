using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

namespace WebScraping.Selenium.Pages
{
    public partial class ExclusionDatabaseSearchPage : BaseSearchPage 
    {

        private IUnitOfWork _UOW;

        public ExclusionDatabaseSearchPage(IWebDriver driver, IUnitOfWork uow) 
            : base(driver)
        {
            _UOW = uow;
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
                ExclusionDatabaseSearchAgain.Click(); //back to search page
                return false;
        }

        private ExclusionDatabaseSearchPageSiteData _exclusionSearchSiteData;

        public void LoadExclusionsDatabaseList()
        {
            _exclusionSearchSiteData = new ExclusionDatabaseSearchPageSiteData();

            _exclusionSearchSiteData.CreatedBy = "Patrick";
            _exclusionSearchSiteData.SiteLastUpdatedOn = DateTime.Now;

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

                    _exclusionSearchSiteData.ExclusionSearchList.Add
                        (NewExclusionsList);
                }
            }
        }

        public override void LoadContent()
        {

        }

        public override void LoadContent(string NameToSearch)
        {
             
            string[] FullName = NameToSearch.Split(' ');

            //for(int counter = 0; counter < FullName.Length; counter++)
            //{
            //    if (FullName.Length == 2)
            //    {
                    
            //    }
            //}

            if (SearchTerms(FullName[0], FullName[1]))
                LoadExclusionsDatabaseList();

            if (SearchTerms(FullName[1], FullName[0]))
                LoadExclusionsDatabaseList();
                
        }

        public override void SaveData()
        {

        }
    }
}
