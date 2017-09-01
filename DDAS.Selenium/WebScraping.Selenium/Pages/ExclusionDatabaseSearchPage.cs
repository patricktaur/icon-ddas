using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Net;
using System.IO;
using System.Linq;
using DDAS.Models.Entities.Domain;
using Microsoft.VisualBasic.FileIO;
using DDAS.Models.Interfaces;
using System.Threading;

namespace WebScraping.Selenium.Pages
{
    public partial class ExclusionDatabaseSearchPage : BaseSearchPage 
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        public ExclusionDatabaseSearchPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config, ILog Log) 
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
            Open();
            _exclusionSearchSiteData = new ExclusionDatabaseSearchPageSiteData();
            _exclusionSearchSiteData.RecId = Guid.NewGuid();
            _exclusionSearchSiteData.ReferenceId = _exclusionSearchSiteData.RecId;
            _exclusionSearchSiteData.Source = driver.Url;
            //SaveScreenShot("ExclusionDatabaseSearch.png");
        }

        public override string Url
        {
            get
            {
                return @"https://oig.hhs.gov/exclusions/exclusions_list.asp";
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.ExclusionDatabaseSearchPage;
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _exclusionSearchSiteData.ExclusionSearchList;
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
                return _exclusionSearchSiteData;
            }
        }

        private string DownloadExclusionList()
        {
            string DownloadFilePath = 
                _config.ExclusionDatabaseFolder + 
                "ExclusionDatabaseList_" +
                DateTime.Now.ToString("dd_MMM_yyyy_hh_mm") +
                ".csv";

            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            string myStringWebResource = ExclusionDatabaseAnchorToDownloadCSV.
                GetAttribute("href");

            _log.WriteLog(
            string.Format("Downloading File \"{0}\" from \"{1}\" .......\n\n",
                Path.GetFileName(DownloadFilePath), myStringWebResource));
            
            myWebClient.DownloadFile(myStringWebResource, DownloadFilePath);

            _log.WriteLog("download complete");

            return DownloadFilePath;
        }

        private ExclusionDatabaseSearchPageSiteData _exclusionSearchSiteData;

        private void LoadExclusionDatabaseListFromCSV(string CSVFilePath)
        {
            _log.WriteLog("Reading records from the file - " + 
                Path.GetFileName(CSVFilePath));

            TextFieldParser parser = new TextFieldParser(CSVFilePath);

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            string[] fields;

            int RowNumber = 1;
            while (!parser.EndOfData)
            {
                fields = parser.ReadFields();

                if (fields.Count() != 18)
                    throw new Exception("Error - record with First Name:" + fields[1] +
                        " and Last Name:" + fields[0] +
                        "does not have 18 fields");

                if (fields[0] == "" && fields[1] == "" ||
                    fields[0].ToLower().Contains("lastname"))
                    continue;

                if (fields[0].Length > 1)
                {
                    var ExclusionList = new ExclusionDatabaseSearchList();
                    ExclusionList.RecId = Guid.NewGuid();
                    ExclusionList.ParentId = _exclusionSearchSiteData.RecId;

                    ExclusionList.RowNumber = RowNumber;
                    ExclusionList.LastName = fields[0];
                    ExclusionList.FirstName = fields[1];
                    ExclusionList.MiddleName = fields[2];
                    //ExclusionList.BusinessName = RecordDetails[3];
                    ExclusionList.General = fields[4];
                    ExclusionList.Specialty = fields[5];
                    //ExclusionList.UPIN = RecordDetails[6];
                    //ExclusionList.NPI = RecordDetails[7];
                    //ExclusionList.DOB = RecordDetails[8];
                    //ExclusionList.Address = RecordDetails[9];
                    //ExclusionList.City = RecordDetails[10];
                    //ExclusionList.State = RecordDetails[11];
                    //ExclusionList.Zip = RecordDetails[12];
                    ExclusionList.ExclusionType = fields[13];
                    ExclusionList.ExclusionDate = fields[14];
                    //ExclusionList.WaiverDate = RecordDetails[16];
                    //ExclusionList.WaiverState = RecordDetails[17];

                    //_exclusionSearchSiteData.ExclusionSearchList.Add(
                    //    ExclusionList);
                    _UOW.ExclusionDatabaseRepository.Add(ExclusionList);
                    RowNumber += 1;
                }
            }
            //_log.WriteLog("Total records inserted - " +
            //    _exclusionSearchSiteData.ExclusionSearchList.Count());
            _log.WriteLog("Total records inserted - " +
                _UOW.ExclusionDatabaseRepository.GetAll().Count());
        }

        public override void LoadContent(
            string NameToSearch, int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.ExclusionDatabaseSearchRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _exclusionSearchSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.ExclusionDatabaseSearchRepository.Add(
                _exclusionSearchSiteData);
        }

        public void ReadSiteLastUpdatedDateFromPage()
        {
            string PageLastUpdated =
                PageLastUpdatedTextElement.Text;

            if (PageLastUpdated == null || PageLastUpdated == "")
                throw new Exception("PageLastUpdated is null");
            else
                PageLastUpdated = PageLastUpdated.Replace("UPDATED ", "").
                Replace("-", "/").Trim();

            DateTime RecentLastUpdatedDate;

            var IsDateParsed = DateTime.TryParseExact(
                PageLastUpdated, 
                "M'/'d'/'yyyy", 
                null,
                System.Globalization.DateTimeStyles.None, 
                out RecentLastUpdatedDate);

            if(IsDateParsed)
                _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
            else
                throw new Exception(
                    "Could not parse Page last updated string - '" +
                    PageLastUpdated +
                    "' to DateTime.");
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

        private void DeleteAllExclusionDatabaseRecords()
        {
            var Record = _UOW.ExclusionDatabaseRepository.GetAll().FirstOrDefault();

            if (Record != null)
            {
                _log.WriteLog("Old records found.. Deleting old records...");
                _UOW.ExclusionDatabaseRepository.DropAll(Record);
                _log.WriteLog("Old records deleted...");
            }
        }

        public override void LoadContent()
        {
            try
            {
                if (!IsPageLoaded())
                    throw new Exception("page is not loaded");

                _exclusionSearchSiteData.DataExtractionRequired = true;
                var FilePath = DownloadExclusionList();
                DeleteAllExclusionDatabaseRecords();
                LoadExclusionDatabaseListFromCSV(FilePath);

                _exclusionSearchSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "ExclusionDatabaseSearchPage_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _exclusionSearchSiteData.DataExtractionSucceeded = false;
                _exclusionSearchSiteData.DataExtractionErrorMessage = e.ToString();
                _exclusionSearchSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _exclusionSearchSiteData.CreatedBy = "Patrick";
                _exclusionSearchSiteData.CreatedOn = DateTime.Now;
            }
        }
    }
}
