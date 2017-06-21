using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Threading;

namespace WebScraping.Selenium.Pages
{
    public partial class CBERClinicalInvestigatorInspectionPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        public CBERClinicalInvestigatorInspectionPage(
            IWebDriver driver, IUnitOfWork uow,
            IConfig Config, ILog Log) : base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
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

        private CBERClinicalInvestigatorInspectionSiteData _CBERSiteData;

        private void GetDataFromAllLists()
        {
            //links to extract records starting with E-K, L-P, Q-S and T-Z
            //link for A-D records is the in the 'Url' property
            string[] InspectionList = new string[] {
                "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195367.htm",
                "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195632.htm",
                "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195633.htm",
                "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm197065.htm"
            };

            var List = new string[] { "A-D", "E-K", "L-P", "Q-S", "T-Z" };

            int Index = 0;

            LoadCBERClinicalInvestigators(List[Index]);

            foreach(string arrItem in InspectionList)
            {
                Index += 1;

                driver.Url = arrItem;

                if (!IsPageLoaded())
                    throw new Exception("Page is not loaded for Url - " + driver.Url);

                if (IsFeedbackPopUpDisplayed)
                {
                    var ErrorCaptureFilePath =
                        _config.ErrorScreenCaptureFolder +
                         "PopUp_CBERClinicalInvesigator_" +
                        DateTime.Now.ToString("dd MMM yyyy hh_mm");
                    SaveScreenShot(ErrorCaptureFilePath);
                    //driver.Navigate().GoToUrl(arrItem);
                    //IsPageLoaded();
                }
                LoadCBERClinicalInvestigators(List[Index]);
            }
        }

        private int TotalRecordsFound = 0;
        private int NullRecords = 0;

        private void LoadCBERClinicalInvestigators(string AlphabetList)
        {
            var RecordsFound = 
                CBERClinicalInvestigatorTable.FindElements(By.XPath("tbody/tr")).Count();

            _log.WriteLog("Records found for list - " +
                AlphabetList + " " +
                RecordsFound);

            TotalRecordsFound += RecordsFound;

            var RecordsAdded = _CBERSiteData.ClinicalInvestigator.Count();

            if (RecordsAdded != 0)
                RecordsAdded += 1;

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

                    if (ClinicalInvestigatorCBER.Name != null ||
                        ClinicalInvestigatorCBER.Name != "")
                        _CBERSiteData.ClinicalInvestigator.Add(ClinicalInvestigatorCBER);
                    else
                        NullRecords += 1;

                    RowCount = RowCount + 1;
                }
            }
            _log.WriteLog("Records inserted for list - " +
                AlphabetList + " " +
                ((_CBERSiteData.ClinicalInvestigator.Count() + 1) - RecordsAdded));
            //RowCount is equivivalent to records added to the collection
        }

        private bool IsPageLoaded()
        {
            IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
            bool PageLoaded = false;

            for (int Index = 1; Index <= 25; Index++)
            {
                Thread.Sleep(500);
                if (executor.ExecuteScript("return document.readyState").ToString().
                    Equals("complete"))
                {
                    PageLoaded = true;
                    break;
                }
            }
            return PageLoaded;
        }

        public override void LoadContent()
        {
            try
            {
                if (!IsPageLoaded())
                    throw new Exception("Page is not loaded");

                if (IsFeedbackPopUpDisplayed)
                {
                    var ErrorCaptureFilePath =
                        _config.ErrorScreenCaptureFolder +
                         "PopUp_CBERClinicalInvesigator_" +
                        DateTime.Now.ToString("dd MMM yyyy hh_mm");
                    SaveScreenShot(ErrorCaptureFilePath);
                    driver.Navigate().GoToUrl(Url);
                    IsPageLoaded();
                }

                _CBERSiteData.DataExtractionRequired = true;
                GetDataFromAllLists();
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
            _log.WriteLog("Total records Found - " + TotalRecordsFound);

            _UOW.CBERClinicalInvestigatorRepository.Add(_CBERSiteData);

            _log.WriteLog("Total records inserted - " +
                _CBERSiteData.ClinicalInvestigator.Count());

            _log.WriteLog("Total null records(name field) found - " + NullRecords);
        }
    }
}
