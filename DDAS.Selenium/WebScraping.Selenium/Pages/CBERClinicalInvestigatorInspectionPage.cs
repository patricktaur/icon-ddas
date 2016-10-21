using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;

namespace WebScraping.Selenium.Pages
{
    public partial class CBERClinicalInvestigatorInspectionPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;

        public CBERClinicalInvestigatorInspectionPage(IWebDriver driver, IUnitOfWork uow) :
            base(driver)
        {
            _UOW = uow;
            Open();
            _CBERSiteData = new CBERClinicalInvestigatorInspectionSiteData();
            //SaveScreenShot("CBERClinicalInvestigatorInspectionPage.png");
        }

        public override string Url {
            get {
                return @"http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195364.htm";
            }
        }

        public override SiteEnum SiteName
        {
            get
            {
                return SiteEnum.CBERClinicalInvestigatorInspectionPage;
            }
        }

        private int RowCount = 1;

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

        private CBERClinicalInvestigatorInspectionSiteData _CBERSiteData;

        public void LoadCBERClinicalInvestigators()
        {
            _CBERSiteData.CreatedBy = "patrick";
            _CBERSiteData.SiteLastUpdatedOn = DateTime.Now;
            _CBERSiteData.CreatedOn = DateTime.Now;
            _CBERSiteData.Source = driver.Url;

            foreach (IWebElement TR in 
                CBERClinicalInvestigatorTable.FindElements(By.XPath("tbody/tr")))
            {
                var ClinicalInvestigatorCBER = new CBERClinicalInvestigator();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));
                if (TDs.Count > 0)
                {
                    ClinicalInvestigatorCBER.RowNumber = RowCount;
                    ClinicalInvestigatorCBER.Name = TDs[0].Text;
                    ClinicalInvestigatorCBER.Title = TDs[1].Text;
                    ClinicalInvestigatorCBER.InstituteAndAddress = TDs[2].Text;
                    ClinicalInvestigatorCBER.InspectionStartAndEndDate = TDs[3].Text;
                    ClinicalInvestigatorCBER.Classification = TDs[4].Text;

                    _CBERSiteData.ClinicalInvestigator.Add(ClinicalInvestigatorCBER);
                    RowCount = RowCount + 1;
                }
            }
        }

        public override void LoadContent()
        {
            LoadNextInspectionList();
        }

        public override void SaveData()
        {
            _UOW.CBERClinicalInvestigatorRepository.Add(_CBERSiteData);
        }
    }
}
