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

namespace WebScraping.Selenium.Pages
{
    public partial class ExclusionDatabaseSearchPage : BaseSearchPage 
    {

        private IUnitOfWork _UOW;
        private DateTime? _SiteLastUpdatedFromPage;
        private DateTime? _SiteLastUpdatedFromDatabse;

        public ExclusionDatabaseSearchPage(IWebDriver driver, IUnitOfWork uow) 
            : base(driver)
        {
            _UOW = uow;
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

        public override DateTime? SiteLastUpdatedDateFromDatabase
        {
            get
            {
                return _SiteLastUpdatedFromDatabse;
            }
        }

        public override BaseSiteData baseSiteData
        {
            get
            {
                return _exclusionSearchSiteData;
            }
        }

        private string DownloadExclusionList(string DownloadFolder)
        {
            string fileName = DownloadFolder + "ExclusionDatabaseList.csv";
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            string myStringWebResource = ExclusionDatabaseAnchorToDownloadCSV.
                GetAttribute("href");

            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n",
                fileName, myStringWebResource);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(myStringWebResource, fileName);

            return fileName;
        }

        private ExclusionDatabaseSearchPageSiteData _exclusionSearchSiteData;

        private void LoadExclusionDatabaseListFromCSV(string CSVFilePath)
        {
            string[] AllRecords = File.ReadAllLines(CSVFilePath);

            int RowNumber = 1;
            foreach (string Record in AllRecords)
            {
                string CurrentRecord = Record.Replace("\\", "").Replace("\"", "");

                string[] RecordDetails = CurrentRecord.Split(',');

                if (RecordDetails[0] == null && RecordDetails[1] == null ||
                    RecordDetails[0].ToLower().Contains("lastname"))
                    continue;

                if (RecordDetails[0].Length > 1)
                {
                    var ExclusionList = new ExclusionDatabaseSearchList();

                    ExclusionList.RowNumber = RowNumber;
                    ExclusionList.LastName = RecordDetails[0];
                    ExclusionList.FirstName = RecordDetails[1];
                    ExclusionList.MiddleName = RecordDetails[2];
                    //ExclusionList.BusinessName = RecordDetails[3];
                    ExclusionList.General = RecordDetails[4];
                    ExclusionList.Specialty = RecordDetails[5];
                    //ExclusionList.UPIN = RecordDetails[6];
                    //ExclusionList.NPI = RecordDetails[7];
                    //ExclusionList.DOB = RecordDetails[8];
                    //ExclusionList.Address = RecordDetails[9];
                    //ExclusionList.City = RecordDetails[10];
                    //ExclusionList.State = RecordDetails[11];
                    //ExclusionList.Zip = RecordDetails[12];
                    ExclusionList.ExclusionType = RecordDetails[13];
                    //ExclusionList.ExclusionDate = RecordDetails[14];
                    //ExclusionList.WaiverDate = RecordDetails[16];
                    //ExclusionList.WaiverState = RecordDetails[17];

                    _exclusionSearchSiteData.ExclusionSearchList.Add(
                        ExclusionList);
                    RowNumber += 1;
                }
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            try
            {
                _exclusionSearchSiteData.DataExtractionRequired = true;
                string FilePath = DownloadExclusionList(DownloadFolder);
                LoadExclusionDatabaseListFromCSV(FilePath);
                _exclusionSearchSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                _exclusionSearchSiteData.DataExtractionSucceeded = false;
                _exclusionSearchSiteData.DataExtractionErrorMessage = e.Message;
                _exclusionSearchSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _exclusionSearchSiteData.CreatedBy = "Patrick";
                _exclusionSearchSiteData.CreatedOn = DateTime.Now;
            }
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
            string PageLastUpdated  = 
                PageLastUpdatedTextElement.Text.Replace("UPDATED ", "").
                Replace("-","/").Trim();

            DateTime RecentLastUpdatedDate;

            DateTime.TryParseExact(PageLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
        }
    }
}
