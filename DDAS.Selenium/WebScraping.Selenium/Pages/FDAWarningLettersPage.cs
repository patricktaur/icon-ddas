﻿using DDAS.Models.Enums;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Linq;
using DDAS.Models.Entities.Domain;
using System.Threading;
using DDAS.Models.Interfaces;
using SpreadsheetLight;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Xml;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebScraping.Selenium.Pages
{
    public partial class FDAWarningLettersPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        [DllImport("urlmon.dll")]
        static extern Int32 URLDownloadToFile(Int32 pCaller, string szURL, string szFileName, Int32 dwReserved, Int32 lpfnCB);

        public FDAWarningLettersPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config, ILog Log) : base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
            Open();
            _FDAWarningSiteData = new FDAWarningLettersSiteData();
            _FDAWarningSiteData.RecId = Guid.NewGuid();
            _FDAWarningSiteData.ReferenceId = _FDAWarningSiteData.RecId;
            _FDAWarningSiteData.Source = driver.Url;
        }

        public override string Url {
            get {
                return @"http://www.fda.gov/ICECI/EnforcementActions/WarningLetters/default.htm";
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.FDAWarningLettersPage;
            }
        }

        public FDAWarningLettersSiteData FDADebarSiteData {
            get {
                return _FDAWarningSiteData;
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _FDAWarningSiteData.FDAWarningLetterList;
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
                return _FDAWarningSiteData;
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

        private bool SearchTerms(string Name)
        {
            if (!IsPageLoaded())
                throw new Exception("Could not load the page. Site is down/unavailable");

            if (IsFeedbackPopUpDisplayed)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "PopUp_FDAWarningLetters_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);
                driver.Navigate().GoToUrl(Url);
                IsPageLoaded();
            }

            if (IsSiteDown)
            {
                throw new Exception("Unable to search records, the site is down");
            }

            IWebElement Input = FDASearchTextBox;
            if (Input == null)
                throw new Exception("Could not find element: FDASearchTextBox");
            FDASearchTextBox.Clear();
            FDASearchTextBox.SendKeys(Name);

            IWebElement Search = FDASearchButton;
            if (Search == null)
                throw new Exception("Could not find element: FDASearchButton");

            FDASearchButton.SendKeys(Keys.Enter);

            if (!IsPageLoaded())
                throw new Exception("Page did not load after FDASearchButton sendkeys event");

            IWebElement Table = FDAWarningSortTable;

            if (Table == null)
                throw new Exception("FDASortTable is null");

            IList<IWebElement> TR = FDAWarningSortTable.FindElements(By.XPath("tbody/tr"));

            if(TR.Count <= 1)
                return false;
            else
            {
                IWebElement TD = TR[TR.Count - 1].FindElement(By.XPath("td"));

                IList<IWebElement> AnchorTags = TD.FindElements(By.XPath("a"));

                //click on 'All' anchor to display all records
                if (AnchorTags.Count > 0)
                    AnchorTags[AnchorTags.Count - 1].SendKeys(Keys.Enter);
                return true;
            }
        }

        private FDAWarningLettersSiteData _FDAWarningSiteData;

        private int RowNumber = 1;

        public void LoadFDAWarningLetters()
        {
            IList<IWebElement> TR = FDAWarningSortTable.FindElements(By.XPath("//tbody/tr"));

            for (int tableRow = 12; tableRow < TR.Count - 1; tableRow++)
            {
                var FDAWarningList = new FDAWarningLetter();

                IList<IWebElement> TDs = TR[tableRow].FindElements(By.XPath("td"));

                if(TDs.Count > 1)
                {
                    FDAWarningList.RowNumber = RowNumber;
                    FDAWarningList.Company = TDs[0].Text;
                    FDAWarningList.LetterIssued = TDs[1].Text;
                    FDAWarningList.IssuingOffice = TDs[2].Text;
                    FDAWarningList.Subject = TDs[3].Text;
                    FDAWarningList.ResponseLetterPosted = TDs[4].Text;
                    FDAWarningList.CloseoutDate = TDs[5].Text;

                    if (IsElementPresent(TDs[0], By.XPath("a")))
                    {
                        IWebElement anchor = TDs[0].FindElement(By.XPath("a"));
                        Link link = new Link();
                        link.Title = "Company";
                        link.url = anchor.GetAttribute("href");
                        FDAWarningList.Links.Add(link);
                    }
                }
                _FDAWarningSiteData.FDAWarningLetterList.Add(FDAWarningList);
                RowNumber += 1;
            }
            _log.WriteLog("Total records inserted - " +
                _FDAWarningSiteData.FDAWarningLetterList.Count());
        }

        public override void LoadContent(string NameToSearch, int MatchCountLowerLimit)
        {
            string[] FullName = NameToSearch.Split(' ');
            try
            {
                if (SearchTerms(NameToSearch))
                    LoadFDAWarningLetters();
                else
                {
                    for (int Counter = 0; Counter < FullName.Length; Counter++)
                    {
                        if (FullName[Counter].Length > 1 &&
                            _FDAWarningSiteData.FDAWarningLetterList.Count == 0 &&
                            SearchTerms(FullName[Counter]))
                            LoadFDAWarningLetters();
                    }
                }
                _FDAWarningSiteData.DataExtractionRequired = true;
                _FDAWarningSiteData.DataExtractionSucceeded = true;
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder + 
                    "Error_FDAWarningLetters_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _FDAWarningSiteData.DataExtractionSucceeded = false;
                _FDAWarningSiteData.DataExtractionErrorMessage = e.Message +
                    " - " + ErrorCaptureFilePath;

                _FDAWarningSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _FDAWarningSiteData.CreatedBy = "Patrick";
                _FDAWarningSiteData.CreatedOn = DateTime.Now;
            }
        }

        private void ReadSiteLastUpdatedDateFromPage()
        {
            string[] DataInPageLastUpdatedElement = PageLastUpdatedTextElement.Text.Split(':');

            string PageLastUpdated =
                DataInPageLastUpdatedElement[1].Replace("\r\nNote", "").Trim();

            DateTime RecentLastUpdatedDate;

            var IsDateParsed = DateTime.TryParseExact(
                PageLastUpdated, 
                "M/d/yyyy", 
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

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.FDAWarningLettersRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _FDAWarningSiteData.ReferenceId = SiteData.RecId;
        }

        public override void SaveData()
        {
            _UOW.FDAWarningLettersRepository.Add(_FDAWarningSiteData);
        }

        private void DeleteAllFDAWarningRecords()
        {
            var Record = _UOW.FDAWarningRepository.GetAll()
                .FirstOrDefault();

            if (Record != null)
            {
                _log.WriteLog("Old records found.. Deleting old records...");
                _UOW.FDAWarningRepository.DropAll(Record);
                _log.WriteLog("Old records deleted...");
            }
        }

        public override void LoadContent()
        {
            try
            {
                if (!IsPageLoaded())
                    throw new Exception("Could not load the page. Site is down/unavailable at the moment");

                if (IsFeedbackPopUpDisplayed)
                {
                    var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                        "PopUp_FDAWarningLetters_" +
                        DateTime.Now.ToString("dd MMM yyyy hh_mm")
                        + ".jpeg";
                    SaveScreenShot(ErrorCaptureFilePath);
                    driver.Navigate().GoToUrl(Url);
                    IsPageLoaded();
                }
                _FDAWarningSiteData.DataExtractionRequired = true;
                var FilePath = DownloadFDAWarningLettersList();
                DeleteAllFDAWarningRecords();
                ReadFDAWarningLetters(FilePath);
                _FDAWarningSiteData.DataExtractionSucceeded = true;
            }
            catch(Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder + 
                    "Error_FDAWarningLetters_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _FDAWarningSiteData.DataExtractionSucceeded = false;
                _FDAWarningSiteData.DataExtractionErrorMessage = e.Message +
                    " - " + ErrorCaptureFilePath;

                _FDAWarningSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _FDAWarningSiteData.CreatedBy = "Patrick";
                _FDAWarningSiteData.CreatedOn = DateTime.Now;
            }
        }

        private string DownloadFDAWarningLettersList()
        {
            //string fileName = _config.AppDataDownloadsFolder + "FDAWarningLetters.xls";

            string fileName = _config.FDAWarningLettersFolder + 
                SiteName.ToString() + "_" +
                DateTime.Now.ToString("dd_MMM_yyyy_hh_mm") +
                ".json";

            //if (File.Exists(fileName))
            //    File.Delete(fileName);

            WebClient myWebClient = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // Concatenate the domain with the Web resource filename.
            string myStringWebResource = "https://www.fda.gov/files/api/datatables/static/warning-letters.json";
            //"https://www.accessdata.fda.gov/scripts/warningletters/wlSearchResultExcel.cfm?qryStr=";

            _log.WriteLog(string.Format(
                "Downloading File \"{0}\" from \"{1}\" .......\n\n", 
                Path.GetFileName(fileName), myStringWebResource));

            try
            {
                // Download the Web resource and save it into the current filesystem folder.
                myWebClient.DownloadFile(myStringWebResource, fileName);
            }
            catch (WebException Ex)
            {
                throw new Exception("file download failed - " + Ex.ToString());
            }

            _log.WriteLog("download complete");

            return fileName;
        }

        private void DownloadFDAWarningLettersCSVFileXXXX(string LetterIssuedDateFrom)
        {
            driver.Navigate().GoToUrl("https://www.accessdata.fda.gov/scripts/warningletters/wlSearchExcel.cfm");

            if(!IsPageLoaded())
                throw new Exception("Could not navigate to: /scripts/warningletters/wlSearchExcel.cfm");

            LetterIssuedDateFromElement.Clear();
            //LetterIssuedDateFromElement.SendKeys(LetterIssuedDateFrom);
            Thread.Sleep(500);
            SearchElement.SendKeys(Keys.Enter);
            //driver.FindElement(By.Id("searchAdvanced")).Submit();
        }

        public void ReadFDAWarningLetters(string DownloadFolder)
        {
            if (DownloadFolder == null || DownloadFolder.Trim().Length == 0)
                throw new Exception("DownloadFolder file path is empty. Cannot read data");

            _log.WriteLog("Reading records from the file - " +
                Path.GetFileName(DownloadFolder));

            var records = JsonConvert.DeserializeObject<List<FDAWarningLetterFile>>(File.ReadAllText(DownloadFolder));
            
            foreach (var record in records)
            {
                var FDAWarningLetterRecord = new FDAWarningLetter();
                FDAWarningLetterRecord.RecId = Guid.NewGuid();
                FDAWarningLetterRecord.ParentId = _FDAWarningSiteData.RecId;

                FDAWarningLetterRecord.Company = record.field_company_name_warning_lette; //Company Name Anchor tag in web page
                FDAWarningLetterRecord.PostedDate = record.field_change_date_2; //Posted Date in web page
                FDAWarningLetterRecord.LetterIssued = record.field_letter_issue_datetime; //Letter Issue Date in web page
                FDAWarningLetterRecord.IssuingOffice = record.field_building;  //Issuing Office in Web Page
                FDAWarningLetterRecord.Subject = record.field_detailed_description_2; //Subject in Web Page
                FDAWarningLetterRecord.ResponseLetterPosted = record.field_associated_for_response_le;  //Response Letter Anchor tag in web page
                FDAWarningLetterRecord.CloseoutDate = record.field_associated_for_closeout_le; //Close Out Letter Anchor tag in web page

                _UOW.FDAWarningRepository.Add(FDAWarningLetterRecord);
            }

            _log.WriteLog("Total records inserted - " +
                (_UOW.FDAWarningRepository.GetAll().Count() + 1));
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
