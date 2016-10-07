using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorInspectionPage : BaseSearchPage //BaseClasses.BasePage
    {
        private IUnitOfWork _UOW;

        public ClinicalInvestigatorInspectionPage(IWebDriver driver, IUnitOfWork uow) 
            : base(driver)
        {
            _UOW = uow;
            Open();
            _clinicalSiteData = new ClinicalInvestigatorInspectionSiteData();
            //SaveScreenShot("ClinicalInvestigatorInspectionPage.png");
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
            InputTag.Clear();
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

        private ClinicalInvestigatorInspectionSiteData _clinicalSiteData;

        public void LoadClinicalInvestigatorList()
        {
            //_clinicalSiteData = new ClinicalInvestigatorInspectionSiteData();

            _clinicalSiteData.SiteLastUpdatedOn = DateTime.Now;
            _clinicalSiteData.CreatedBy = "Patrick";
            _clinicalSiteData.CreatedOn = DateTime.Now;
            _clinicalSiteData.Source = driver.Url;

            int RowNumber = 1;
            foreach (IWebElement TR in
                ClinicalInvestigatorTable.FindElements(By.XPath("//tbody/tr")))
            {
                var InvestigatorList = new ClinicalInvestigator();
                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                InvestigatorList.RowNumber = RowNumber;
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

                _clinicalSiteData.ClinicalInvestigatorInspectionList.Add
                    (InvestigatorList);
                RowNumber += 1;
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

        public override void LoadContent()
        {
            if (SearchTerms())
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
                driver.Url = 
                    "http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm";
        }

        public override void LoadContent(string NameToSearch)
        {
            string[] Name = NameToSearch.Split(' ');
            
            for (int counter = 0; counter < Name.Length; counter++)
            {
                Name[counter] = Name[counter].Replace(",", "");

                if (SearchTerms())
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
            element.Click();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
        }

        public bool SearchTerms()
        {
            IWebElement AdvancedSearchElement = ClinicalInvestigatorAdvancedSearch;
            AdvancedSearchElement.Click();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            IWebElement FirstNameDropDown = ClinicalInvestigatorFirstNameDropDown;
            FirstNameDropDown.SendKeys("not equal to");

            IWebElement FirstNameTextField = ClinicalInvestigatorFirstNameTextField;
            FirstNameTextField.SendKeys("zzzzzz");

            ClinicalInvestigatorSubmit.Submit();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            return true;
        }
        
        public override void SaveData()
        {
            _UOW.ClinicalInvestigatorInspectionListRepository.Add(
                _clinicalSiteData);
        }
    }
}
