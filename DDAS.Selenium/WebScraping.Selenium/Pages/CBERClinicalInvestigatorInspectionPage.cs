﻿using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;

namespace WebScraping.Selenium.Pages
{
    public partial class CBERClinicalInvestigatorInspectionPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;

        public CBERClinicalInvestigatorInspectionPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config) :
            base(driver)
        {
            _UOW = uow;
            _config = Config;
            Open();
            _CBERSiteData = new CBERClinicalInvestigatorInspectionSiteData();
            _CBERSiteData.RecId = Guid.NewGuid();
            _CBERSiteData.ReferenceId = _CBERSiteData.RecId;
            _CBERSiteData.Source = driver.Url;
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

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _CBERSiteData.ClinicalInvestigator;
            }
        }

        public override DateTime? SiteLastUpdatedDateFromPage
        {
            get
            {
                if (_SiteLastUpdatedFromPage == null)
                    ReadSiteLastUpdatedDateFromPage();
                return _SiteLastUpdatedFromPage;
            }
        }


        public override BaseSiteData baseSiteData
        {
            get
            {
                return _CBERSiteData;
            }
        }

        private int RowCount = 1;

        private void LoadNextInspectionList()
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

        private void LoadCBERClinicalInvestigators()
        {
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
            try
            {
                _CBERSiteData.DataExtractionRequired = true;
                LoadNextInspectionList();
                _CBERSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "CBERClinicalInvestigator_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _CBERSiteData.DataExtractionSucceeded = false;
                _CBERSiteData.DataExtractionErrorMessage = e.Message;
                _CBERSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _CBERSiteData.CreatedBy = "Patrick";
                _CBERSiteData.CreatedOn = DateTime.Now;
            }
        }

        public override void LoadContent(string NameToSearch,
            int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
        }

        public  void ReadSiteLastUpdatedDateFromPage()
        {
            string[] DataInPageLastUpdatedElement = 
                PageLastUpdatedTextElement.Text.Split(':');

            string PageLastUpdated =
                DataInPageLastUpdatedElement[1].Replace("\r\nNote", "").Trim();

            DateTime RecentLastUpdatedDate;

            DateTime.TryParseExact(PageLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;

            //var ExistingCBERSiteData = 
            //    _UOW.CBERClinicalInvestigatorRepository.GetAll();

            //CBERClinicalInvestigatorInspectionSiteData CBERSiteData = null;

            //if (ExistingCBERSiteData.Count == 0)
            //{
            //    _CBERSiteData.SiteLastUpdatedOn = RecentLastUpdatedDate;
            //    _CBERSiteData.DataExtractionRequired = true;
            //}
            //else
            //{
            //    CBERSiteData = ExistingCBERSiteData.OrderByDescending(
            //        x => x.CreatedOn).First();

            //    if (RecentLastUpdatedDate > CBERSiteData.SiteLastUpdatedOn)
            //    {
            //        _CBERSiteData.SiteLastUpdatedOn = RecentLastUpdatedDate;
            //        _CBERSiteData.DataExtractionRequired = true;
            //    }
            //    else
            //    {
            //        _CBERSiteData.SiteLastUpdatedOn =
            //            CBERSiteData.SiteLastUpdatedOn;
            //        _CBERSiteData.DataExtractionRequired = false;
            //    }
            //}
            //if (!_CBERSiteData.DataExtractionRequired)
            //    _CBERSiteData.ReferenceId = CBERSiteData.RecId;
            //else
            //    _CBERSiteData.ReferenceId =
            //        _CBERSiteData.RecId;
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _CBERSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.CBERClinicalInvestigatorRepository.Add(_CBERSiteData);
        }
    }
}
