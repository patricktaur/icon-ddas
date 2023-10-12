using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using OpenQA.Selenium;
using DDAS.Models.Entities.Domain;
using System.Globalization;
using System.IO;
using System.Threading;

namespace WebScraping.Selenium.Pages
{
    public partial class SpeciallyDesignatedNationalsListPage: ISearchPage //BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        [DllImport("urlmon.dll")]
        public static extern long URLDownloadToFile(long pCaller, string szURL, 
            string szFileName, long dwReserved, long lpfnCB);

        public SpeciallyDesignatedNationalsListPage(
            IUnitOfWork uow, IWebDriver driver, IConfig Config, ILog Log)
            :  base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
            Open();
            _SDNSiteData = new SpeciallyDesignatedNationalsListSiteData();
            _SDNSiteData.RecId = Guid.NewGuid();
            _SDNSiteData.ReferenceId = _SDNSiteData.RecId;
            _SDNSiteData.Source = driver.Url;
            //SaveScreenShot("SpeciallyDesignatedNationalsList.png");
        }
        //"http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx";
        //
        public override string Url {
            get {
                return
                @"https://home.treasury.gov/policy-issues/financial-sanctions/specially-designated-nationals-and-blocked-persons-list-sdn-human-readable-lists";
            }
        }

        public  SiteEnum SiteName {
            get {
                return SiteEnum.SpeciallyDesignedNationalsListPage;
            }
        }

        public IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _SDNSiteData.SDNListSiteData;
            }
        }

        public DateTime? SiteLastUpdatedDateFromPage
        {
            get
            {
                if (_SiteLastUpdatedFromPage == null)
                    ReadSiteLastUpdatedDateFromPage();
                return _SiteLastUpdatedFromPage;
            }
        }

        public BaseSiteData baseSiteData
        {
            get
            {
                return _SDNSiteData;
            }
        }

        private string DownloadSDNList()
        {
            string fileName = _config.SDNFolder + 
                SiteName.ToString() + "_" +
                DateTime.Now.ToString("dd_MMM_yyyy_hh_mm") +
                ".txt";

            WebClient myWebClient = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // PDF file path --> https://www.treasury.gov/ofac/downloads/sdnlist.pdf

            string myStringWebResource = "https://www.treasury.gov/ofac/downloads/sdnlist.txt";

            _log.WriteLog(
                string.Format("Downloading File \"{0}\" from \"{1}\" .......\n\n",
                System.IO.Path.GetFileName(fileName), myStringWebResource));

            try
            {
                myWebClient.DownloadFile(myStringWebResource, fileName);
            }
            catch(WebException Ex)
            {
                throw new Exception("file download failed - " + Ex.ToString());
            }
            _log.WriteLog("download complete");

            return fileName;
        }

        private SpeciallyDesignatedNationalsListSiteData _SDNSiteData;

        private List<SDNList> GetTextFromPDF(string NameToSearch, string DownloadFolder)
        {
            List<SDNList> Names = new List<SDNList>();

            StringBuilder text = new StringBuilder();

            using (PdfReader reader = new PdfReader(DownloadFolder + "\\SDNList.pdf"))
            {
                string PageContent;

                int RowNumber = 1;
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string WordsMatched = null;

                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy(); 
                    PageContent = PdfTextExtractor.GetTextFromPage(reader, i, strategy);

                    string[] SplitNames = PageContent.Split(']');

                    int RecordNumber = 1;
                    for (int records = 0; records < SplitNames.Length; records++)
                    {
                        string TempRecord = SplitNames[records].Replace(" ", "");

                        SDNList NamesFromPDF = new SDNList();

                        NamesFromPDF.RowNumber = RowNumber;
                        NamesFromPDF.Name = TempRecord;
                        NamesFromPDF.RecordNumber = RecordNumber;
                        //NamesFromPDF.PageNumber = i;
                        NamesFromPDF.WordsMatched = WordsMatched;
                        _SDNSiteData.SDNListSiteData.Add(NamesFromPDF);

                        RecordNumber += 1;
                        RowNumber += 1;
                    }
                }
            }
            return Names;
        }

        private void LoadSDNList(string FilePath)
        {
            //Patrick: 24April2017
            //string AllRecords = File.ReadAllText(_config.AppDataDownloadsFolder);

            _log.WriteLog("Reading records from the file - " +
                System.IO.Path.GetFileName(FilePath));

            string AllRecords = File.ReadAllText(FilePath);

            string[] Records = 
                AllRecords.Split(new string[] { "\n\n" }, StringSplitOptions.None);

            int RecordNumber = 1;
            foreach(string Record in Records)
            {
                SDNList SDNRecord = new SDNList();
                SDNRecord.RecId = Guid.NewGuid();
                SDNRecord.ParentId = _SDNSiteData.RecId;

                SDNRecord.Name = Record;
                SDNRecord.RecordNumber = RecordNumber;

                //_SDNSiteData.SDNListSiteData.Add(SDNRecord);
                _UOW.SDNSiteDataRepository.Add(SDNRecord);
                RecordNumber += 1;
            }
            //_log.WriteLog("Total records inserted - " +
            //    _SDNSiteData.SDNListSiteData.Count());
            _log.WriteLog("Total records inserted - " +
                _UOW.SDNSiteDataRepository.GetAll().Count());
        }

        private bool CheckSiteUpdatedDate()
        {
            return false;
            //var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //if (SDNSiteData == null)
            //    return false;

            //Console.WriteLine(SDNSiteUpdatedDate.Text.Replace("Last Updated: ", ""));

            //string temp = SDNSiteUpdatedDate.Text.Replace("Last Updated: ", "");

            //DateTime CurrentSiteUpdatedDate;

            //DateTime.TryParse(temp, out CurrentSiteUpdatedDate);

            //DateTime SiteUpdatedDate = SDNSiteData.SiteLastUpdatedOn;

            //return SiteUpdatedDate != CurrentSiteUpdatedDate ? false : true;
        }

        public void LoadContent(string NameToSearch, int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _SDNSiteData.ReferenceId = SiteData.RecId;
        }

        public void SaveData()
        {
            _UOW.SpeciallyDesignatedNationalsRepository.
                Add(_SDNSiteData);
        }

        private void ReadSiteLastUpdatedDateFromPage()
        {
            string temp = PageLastUpdatedElement.Text.Replace("Last Updated: ", "").Trim();

            DateTime CurrentSiteUpdatedDate;

            var result = DateTime.TryParseExact(
                temp, "M/d/yyyy h:m tt",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out CurrentSiteUpdatedDate);

            if (result)
                _SiteLastUpdatedFromPage = CurrentSiteUpdatedDate;
            else
                throw new Exception(
                    "Could not parse Page last updated string - '" +
                    temp +
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

        private void DeleteAllSDNSiteDataRecords()
        {
            var Record = _UOW.SDNSiteDataRepository.GetAll()
                .FirstOrDefault();

            if (Record != null)
            {
                _log.WriteLog("Old records found.. Deleting old records...");
                _UOW.SDNSiteDataRepository.DropAll(Record);
                _log.WriteLog("Old records deleted...");
            }
        }

        public void LoadContent()
        {
            try
            {
                if (!IsPageLoaded())
                    throw new Exception("Page is not loaded");

                _SDNSiteData.DataExtractionRequired = true;
                var FilePath = DownloadSDNList();
                DeleteAllSDNSiteDataRecords();
                //GetTextFromPDF("", DownloadFolder);
                LoadSDNList(FilePath);
                _SDNSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "SDNPage_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _SDNSiteData.DataExtractionSucceeded = false;
                _SDNSiteData.DataExtractionErrorMessage = e.Message;
                _SDNSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _SDNSiteData.CreatedBy = "Patrick";
                _SDNSiteData.CreatedOn = DateTime.Now;
            }
        }
    }
}
