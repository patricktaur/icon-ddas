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

        public override string Url {
            get {
                return 
                @"http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx";
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
            //string fileName = _config.AppDataDownloadsFolder + "SDNList.txt";
            string fileName = _config.SDNFolder + "SDNList.txt";

            WebClient myWebClient = new WebClient();

            // PDF file path --> https://www.treasury.gov/ofac/downloads/sdnlist.pdf

            string myStringWebResource = "https://www.treasury.gov/ofac/downloads/sdnlist.txt";
            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
            
            myWebClient.DownloadFile(myStringWebResource, fileName);

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

        private void LoadSDNList()
        {
            //Patrick: 24April2017
            //string AllRecords = File.ReadAllText(_config.AppDataDownloadsFolder);
            var dataFile = _config.SDNFolder + "SDNList.txt";
            string AllRecords = File.ReadAllText(dataFile);

            string[] Records = 
                AllRecords.Split(new string[] { "\n\n" }, StringSplitOptions.None);

            int RecordNumber = 1;
            foreach(string Record in Records)
            {
                SDNList SDNRecord = new SDNList();

                SDNRecord.Name = Record;
                SDNRecord.RecordNumber = RecordNumber;

                _SDNSiteData.SDNListSiteData.Add(SDNRecord);
                RecordNumber += 1;
            }
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

        public void ReadSiteLastUpdatedDateFromPage()
        {
            string temp = PageLastUpdatedElement.Text.Replace("Last Updated: ", "").Trim();

            DateTime CurrentSiteUpdatedDate;

            //DateTime.TryParseExact(temp, "M'/'d'/'yyyy",
            //CultureInfo.InvariantCulture,
            //DateTimeStyles.None, out CurrentSiteUpdatedDate);

            var result = DateTime.TryParse(temp, out CurrentSiteUpdatedDate);

            
            
            if (result == true)
            {
                _SiteLastUpdatedFromPage = CurrentSiteUpdatedDate;
            }
            
            
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

        public void LoadContent()
        {
            try
            {
                if (!IsPageLoaded())
                    throw new Exception("Page is not loaded");

                _SDNSiteData.DataExtractionRequired = true;
                DownloadSDNList();
                //GetTextFromPDF("", DownloadFolder);
                LoadSDNList();
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
