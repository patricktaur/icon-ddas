using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using System.Diagnostics;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Net;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualBasic.FileIO;
using System.Threading;

namespace WebScraping.Selenium.Pages
{
    public partial class SystemForAwardManagementPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;

        public SystemForAwardManagementPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config) 
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
            Open();
            _SAMSiteData = new SystemForAwardManagementPageSiteData();
            _SAMSiteData.RecId = Guid.NewGuid();
            _SAMSiteData.ReferenceId = _SAMSiteData.RecId;
            _SAMSiteData.Source = driver.Url;
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.SystemForAwardManagementPage;
            }
        }

        public override string Url {
            get {
                return @"https://www.sam.gov/portal/public/SAM";
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _SAMSiteData.SAMSiteData;
            }
        }

        public override DateTime? SiteLastUpdatedDateFromPage
        {
            get
            {
                if (_SiteLastUpdatedFromPage == null)
                    GetSiteLastUpdatedDate();
                    //ReadSiteLastUpdatedDateFromPage(); uncomment this for live search
                return _SiteLastUpdatedFromPage;
            }
        }

        public override BaseSiteData baseSiteData
        {
            get
            {
                return _SAMSiteData;
            }
        }

        private SystemForAwardManagementPageSiteData _SAMSiteData;

        //private int RowNumber = 1;

        //need to refactor
        private void LoadSAMList()
        {
            //IList<IWebElement> TableThatContainsRecords =
            //    SAMSearchResult.FindElements
            //    (By.XPath("//tbody/tr/td/ul/table/tbody/tr/td/li/table"));

            //foreach (IWebElement RecordsTable in TableThatContainsRecords)
            //{
            //    //Debug.Print(RecordsTable.Text);

            //    var SAMDataList = new SystemForAwardManagement();

            //    string TempContent = RecordsTable.Text.Replace("\n", "");
            //    string[] ContentOfEachRecord = TempContent.Split('\r');

            //    SAMDataList.RowNumber = RowNumber;
            //    SAMDataList.Entity = ContentOfEachRecord[1];

            //    for (int counter = 2; counter < ContentOfEachRecord.Length; counter++)
            //    {
            //        string Content = ContentOfEachRecord[counter];

            //        string[] tempFieldValue = new string[0];

            //        if (Content.Contains(":"))
            //            tempFieldValue = Content.Split(':');
            //        else if (Content.Contains("Entity") || Content.Contains("Exclusion"))
            //        {
            //            SAMDataList.Entity = ContentOfEachRecord[counter + 2];
            //            continue;
            //        }

            //        if (tempFieldValue.Length >= 1)
            //        {
            //            switch (tempFieldValue[0])
            //            {
            //                case "Status": SAMDataList.Status = tempFieldValue[1]; break;

            //                case "DUNS":
            //                    SAMDataList.Duns = tempFieldValue[1].Replace("CAGE Code", "").Trim();
            //                    if (tempFieldValue.Length > 2)
            //                        SAMDataList.CAGECode = tempFieldValue[2];
            //                    break;

            //                case "Has Active Exclusion?":
            //                    SAMDataList.HasActiveExclusion =
            //                    tempFieldValue[1].Replace("DoDAAC", "").Trim();
            //                    if (tempFieldValue.Length > 2)
            //                        SAMDataList.DoDAAC = tempFieldValue[2];
            //                    break;

            //                case "Expiration Date":
            //                    string[] temp =
            //                        tempFieldValue[1].Replace("Delinquent Federal Debt?", "?").Split('?');

            //                    SAMDataList.ExpirationDate =
            //                        temp[0].Trim();
            //                    if (temp.Length > 1)
            //                        SAMDataList.DelinquentFederalDebt =
            //                        tempFieldValue[1].Split('?')[1].Trim();
            //                    break;

            //                case "Purpose of Registration":
            //                    if (tempFieldValue.Length > 1)
            //                        SAMDataList.PurposeOfRegistration = tempFieldValue[1].Trim();
            //                    break;

            //                case "Classification":
            //                    if (tempFieldValue.Length > 1)
            //                        SAMDataList.Classification = tempFieldValue[1].Trim();
            //                    break;

            //                case "Activation Date":
            //                    SAMDataList.ActivationDate =
            //                        tempFieldValue[1].Replace("Termination Date", "").Trim();
            //                    if (tempFieldValue.Length > 2)
            //                        SAMDataList.TerminationDate = tempFieldValue[2].Trim();
            //                    break;
            //            }
            //        }
            //    }
            //    _SAMSiteData.SAMSiteData.Add(SAMDataList);
            //    RowNumber += 1;
            //}
        }

        private bool SearchTerms(string NameToSearch)
        {
            var AnchorTag = SAMAnchorTag;
            if (AnchorTag == null)
                throw new Exception("Could not find element: SAMAnchorTag");
            SAMAnchorTag.SendKeys(Keys.Enter);

            IWebElement TextBox = SAMInputTag;
            if (TextBox == null)
                throw new Exception("Could not find element: SAMInputTag");
            SAMInputTag.Clear();
            SAMInputTag.SendKeys(NameToSearch);

            //SAMSubmitButton.Click();
            //SAMSubmitButton.Submit();
            var SubmitButton = SAMSubmitButton;
            if (SubmitButton == null)
                throw new Exception("Could not find element: SAMSubmitButton");
            SAMSubmitButton.SendKeys(Keys.Enter);

            if (SAMSearchResult == null) //No records found
            {
                //SAMClearSearch.SendKeys(Keys.Enter);
                return false;
            }
            else
                return true;
        }

        private string DownloadExclusionFile()
        {
            //string fileName = 
            //    _config.AppDataDownloadsFolder + "SAM_Exclusions_Public_Extract_";

            string fileName =
                _config.SAMFolder + "SAM_Exclusions_Public_Extract_";

            string CSVFilePath = fileName;

            //string UnZipPath = _config.AppDataDownloadsFolder;
            string UnZipPath = _config.SAMFolder;

            WebClient myWebClient = new WebClient();

            string myStringWebResource = "https://www.sam.gov/public-extracts/SAM-Public/SAM_Exclusions_Public_Extract_";
            string Year = DateTime.Now.ToString("yy");

            string JulianDate = LatestExclusionExtractAnchorTag.Text.Split(' ')[1];
            myStringWebResource += Year + JulianDate + ".ZIP";

            fileName += Year + JulianDate + ".ZIP";
            CSVFilePath += Year + JulianDate + ".CSV";

            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);

            if (File.Exists(CSVFilePath))
                File.Delete(CSVFilePath);

            myWebClient.DownloadFile(myStringWebResource, fileName);

            ZipFile.ExtractToDirectory(fileName, _config.SAMFolder);

            return CSVFilePath;
        }

        private void LoadSAMDatafromCSV(string CSVFilePath)
        {
            TextFieldParser parser = new TextFieldParser(CSVFilePath);

            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            string[] fields;

            int RowNumber = 1;
            while (!parser.EndOfData)
            {
                fields = parser.ReadFields();

                if(fields[3].Trim().Length == 0 &&
                    fields[4].Trim().Length == 0 &&
                    fields[5].Trim().Length == 0)
                {
                    continue;
                }
                else if(fields[3].ToLower() == "first")
                {
                    continue;
                }

                var SAMSiteRecord = new SystemForAwardManagement();
                //var SAMSiteRecord = new SAMSiteData();

                SAMSiteRecord.RecId = Guid.NewGuid();
                SAMSiteRecord.ParentId = _SAMSiteData.RecId;

                SAMSiteRecord.RowNumber = RowNumber;
                SAMSiteRecord.First = fields[3].Trim();
                SAMSiteRecord.Middle = fields[4].Trim();
                SAMSiteRecord.Last = fields[5].Trim();

                SAMSiteRecord.ExcludingAgency = fields[17].Trim();
                SAMSiteRecord.ExclusionType = fields[19].Trim();
                SAMSiteRecord.AdditionalComments = fields[20].Trim();
                SAMSiteRecord.ActiveDate = fields[21].Trim();
                SAMSiteRecord.RecordStatus = fields[23].Trim();

                //_SAMSiteData.SAMSiteData.Add(SAMSiteRecord);
                _UOW.SAMSiteDataRepository.Add(SAMSiteRecord);
                RowNumber += 1;
            }
        }

        private void DelteAllSAMSiteDataRecords()
        {
            var records = _UOW.SAMSiteDataRepository.GetAll();
            //foreach (var rec in records)
            //{
            //    //_UOW.SAMSiteDataRepository.Remove(rec);
            //    //_UOW.SAMSiteDataRepository.RemoveById(rec.RecId);
            //    Guid RecId = rec.RecId.Value;
            //    _UOW.SAMSiteDataRepository.DropRecord(RecId);
            //}
            _UOW.SAMSiteDataRepository.DropAll();
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
                    throw new Exception("page is not loaded");

                _SAMSiteData.DataExtractionRequired = true;
                var FilePath = DownloadExclusionFile();
                DelteAllSAMSiteDataRecords();
                LoadSAMDatafromCSV(FilePath);

                _SAMSiteData.DataExtractionSucceeded = true;
            }
            catch(Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "SAM_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _SAMSiteData.DataExtractionSucceeded = false;
                _SAMSiteData.DataExtractionErrorMessage = e.ToString();
                _SAMSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _SAMSiteData.CreatedBy = "Patrick";
                _SAMSiteData.CreatedOn = DateTime.Now;
            }
        }

        public override void LoadContent(string NameToSearch, int MatchCountLowerLimit)
        {
            string[] Name = NameToSearch.Split(' ');
            try
            {
                //for (int counter = 0; counter < Name.Length; counter++)
                //{
                    if (SearchTerms(NameToSearch))
                    {
                        while (CheckForAnchorTagNext())
                        {
                            LoadSAMList();
                            LoadNextSetOfRecords();
                        }
                        LoadSAMList();
                    }
                    //else
                        //continue;
                //}
                _SAMSiteData.DataExtractionSucceeded = true;
                _SAMSiteData.DataExtractionRequired = true;
            }
            catch(Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder + 
                    "SAM_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _SAMSiteData.DataExtractionSucceeded = false;
                _SAMSiteData.DataExtractionErrorMessage = e.Message +
                    " - " + ErrorCaptureFilePath;
                throw new Exception(e.ToString());
            }
            finally
            {
                _SAMSiteData.CreatedBy = "Patrick";
                _SAMSiteData.CreatedOn = DateTime.Now;
            }
        }

        private bool CheckForAnchorTagNext()
        {
            if (IsElementPresent(SAMPaginationElement, By.XPath("table/tbody/tr/td/a")))
            {
                IList<IWebElement> AnchorsInPagination =
                    SAMPaginationElement.FindElements(By.XPath("table/tbody/tr/td/a"));

                IWebElement LastAnchorTagInPagination = AnchorsInPagination[AnchorsInPagination.Count - 1];

                return
                    (LastAnchorTagInPagination.Text.ToLower() == "next") ?
                    true : false;
            }
            else
                return false;
        }

        private void LoadNextSetOfRecords()
        {
            IList<IWebElement> AnchorsInPagination = 
                SAMPaginationElement.FindElements(By.XPath("table/tbody/tr/td/a"));

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);

            AnchorsInPagination[AnchorsInPagination.Count - 1].Click();
            //AnchorsInPagination[AnchorsInPagination.Count - 1].SendKeys(Keys.Enter);

            //driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));
        }

        //for DB search
        private void GetSiteLastUpdatedDate()
        {
            DataAccessAnchorTag.SendKeys(Keys.Enter);
            var AnchorTitle = LatestExclusionExtractAnchorTag.GetAttribute("title");
            var Date = AnchorTitle.Replace("Active Exclusions Data File", "").Trim();

            DateTime LastUpdatedDate;

            DateTime.TryParseExact(Date,
                "M'/'d'/'yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out LastUpdatedDate);

            _SiteLastUpdatedFromPage = LastUpdatedDate;
        }

        //for live search
        private void ReadSiteLastUpdatedDateFromPage()
        {
            try
            {
                string[] DataInPageLastUpdatedElement =
                    PageLastUpdatedTextElement.Text.Split('.');

                if (DataInPageLastUpdatedElement.Length == 0)
                    throw new Exception(
                        "PageLastUpdatedTextElement is null, unable to read SiteLastUpdatedDate");

                string PageLastUpdated =
                    DataInPageLastUpdatedElement[3].Replace("-", " ").Trim().Split(' ')[0];

                DateTime RecentLastUpdatedDate;

                DateTime.TryParseExact(PageLastUpdated, 
                    "yyyyMMdd", 
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, 
                    out RecentLastUpdatedDate);

                _SiteLastUpdatedFromPage = RecentLastUpdatedDate;

            }
            catch (Exception)
            {
                throw new Exception("Unable to read or parse the SiteLastUpdatedDate");
            }
        }

        public override void SaveData()
        {
            _UOW.SystemForAwardManagementRepository.Add(_SAMSiteData);
        }
    }
}
