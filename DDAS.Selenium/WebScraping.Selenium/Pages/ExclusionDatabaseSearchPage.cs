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

namespace WebScraping.Selenium.Pages
{
    public partial class ExclusionDatabaseSearchPage : BaseSearchPage 
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;

        public ExclusionDatabaseSearchPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config) 
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
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
            string DownloadFilePath = _config.AppDataDownloadsFolder + 
                "ExclusionDatabaseList.csv";
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            string myStringWebResource = ExclusionDatabaseAnchorToDownloadCSV.
                GetAttribute("href");

            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n",
                DownloadFilePath, myStringWebResource);

            if (File.Exists(DownloadFilePath))
                File.Delete(DownloadFilePath);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(myStringWebResource, DownloadFilePath);

            return DownloadFilePath;
        }

        private ExclusionDatabaseSearchPageSiteData _exclusionSearchSiteData;

        private void LoadExclusionDatabaseListFromCSV(string CSVFilePath)
        {
            TextFieldParser parser = new TextFieldParser(CSVFilePath);

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            string[] fields;

            int RowNumber = 1;
            while (!parser.EndOfData)
            {
                fields = parser.ReadFields();

                if (fields.Count() != 18)
                    throw new Exception("record with First Name:" + fields[1] +
                        " and Last Name:" + fields[0] +
                        "does not have 18 fields");

                if (fields[0] == "" && fields[1] == "" ||
                    fields[0].ToLower().Contains("lastname"))
                    continue;

                if (fields[0].Length > 1)
                {
                    var ExclusionList = new ExclusionDatabaseSearchList();

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

                    _exclusionSearchSiteData.ExclusionSearchList.Add(
                        ExclusionList);
                    RowNumber += 1;
                }
            }
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

            DateTime.TryParseExact(PageLastUpdated, "M'/'d'/'yyyy", null,
                System.Globalization.DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
        }

        public override void LoadContent()
        {
            try
            {
                _exclusionSearchSiteData.DataExtractionRequired = true;

                string FilePath = DownloadExclusionList();
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
    }
}
