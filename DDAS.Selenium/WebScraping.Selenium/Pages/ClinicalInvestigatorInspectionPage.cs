﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Enums;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Runtime.InteropServices;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Globalization;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Threading;

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorInspectionPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        [DllImport("urlmon.dll")]
        public static extern long URLDownloadToFile(long pCaller, string szURL,
            string szFileName, long dwReserved, long lpfnCB);

        public ClinicalInvestigatorInspectionPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config, ILog Log) 
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
            Open();
            _clinicalSiteData = new ClinicalInvestigatorInspectionSiteData();
            _clinicalSiteData.RecId = Guid.NewGuid();
            _clinicalSiteData.ReferenceId = _clinicalSiteData.RecId;
            _clinicalSiteData.Source = driver.Url;
        }

        public override string Url
        {
            get
            {
                //return @"http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm";
                return @"http://www.fda.gov/Drugs/InformationOnDrugs/ucm135198.htm";
            }
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.ClinicalInvestigatorInspectionPage;
            }
        }

        public override IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _clinicalSiteData.ClinicalInvestigatorInspectionList;
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
                return _clinicalSiteData;
            }
        }

        private string DownloadCIIList()
        {
            string fileName = _config.CIILFolder + 
                SiteName.ToString() + "_" +
                DateTime.Now.ToString("dd_MM_yyyy_hh_mm") +
                ".zip";

            string UnZipPath = _config.CIILFolder;

            WebClient myWebClient = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string myStringWebResource = ClinicalInvestigatorZipAnchor.GetAttribute("href");

            _log.WriteLog(
                string.Format("Downloading File \"{0}\" from \"{1}\" .......\n\n",
                Path.GetFileName(fileName), myStringWebResource));


            Uri uriWebClient = new Uri(myStringWebResource);
            myWebClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)");
            myWebClient.Headers.Add("Content-Type", "application / zip, application / octet - stream");
            myWebClient.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
            myWebClient.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

            try
            {
                //myWebClient.DownloadFile(myStringWebResource, fileName);
                myWebClient.DownloadFile(uriWebClient, fileName);

            }
            catch (WebException Ex)
            {
                throw new Exception("file download failed - " + Ex.ToString());
            }

            /*
            var cliilFileName = "cliil" + DateTime.Now.ToString("MMMM").ToLower() + DateTime.Now.Year + ".txt";

            if (File.Exists(UnZipPath + cliilFileName)) //filename is cliil.txt by default
                File.Delete(UnZipPath + cliilFileName);

            ZipFile.ExtractToDirectory(fileName, UnZipPath);
            */
            //---
            var ExtractFolder = UnZipPath + "Extracts";
            if (!Directory.Exists(ExtractFolder))
            {
                Directory.CreateDirectory(ExtractFolder);
            }

            //Delete all files in folder:
            Array.ForEach(Directory.GetFiles(ExtractFolder),
              delegate (string path) { File.Delete(path); });

            ZipFile.ExtractToDirectory(fileName, ExtractFolder);

            var FileNameInExtractFolder = Directory.GetFiles(ExtractFolder).FirstOrDefault();

            //---


            _log.WriteLog("download complete");

            return (FileNameInExtractFolder);
        }

        private ClinicalInvestigatorInspectionSiteData _clinicalSiteData;

        private int LoadClinicalInvestigatorList(string FilePath)
        {
            int RowNumber = 1;

            _log.WriteLog("Reading records from the file - " + Path.GetFileName(FilePath));

            string[] LinesFromTextFile = File.ReadAllLines(FilePath);

            DateTime CurrentRowInspectionDate = new DateTime();

            for (int Counter = 0; Counter <= LinesFromTextFile.Length - 1; Counter++)
            {
                
                string[] FieldData = LinesFromTextFile[Counter].Split('~');
                //string[] FieldData = LinesFromTextFile[Counter].Split('~');
                //string[] FieldData = LinesFromTextFile[Counter].Split('\t');

                var InvestigatorList = new ClinicalInvestigator();
                InvestigatorList.RecId = Guid.NewGuid();
                InvestigatorList.ParentId = _clinicalSiteData.RecId;

                if (FieldData.Length > 1)
                {
                    if (FieldData[1].ToLower().Contains("last name"))
                        continue;

                    string[] NextRowData;

                    if (Counter < LinesFromTextFile.Length - 1)
                    {
                        //NextRowData = LinesFromTextFile[Counter + 1].Split('~');
                        NextRowData = LinesFromTextFile[Counter + 1].Split('\t');
                    }
                    else
                    {
                        //NextRowData = LinesFromTextFile[Counter].Split('~');
                        NextRowData = LinesFromTextFile[Counter].Split('\t');
                    }

                    DateTime.TryParseExact(
                        FieldData[9], "MM/dd/yyyy", null, 
                        DateTimeStyles.None, out CurrentRowInspectionDate);

                    string DeficiencyCode = null;
                    int TempCounter = Counter + 1;

                    //merge def code of records with similar Insp date
                    while(FieldData[0] == NextRowData[0])
                    {
                        DateTime NextRowInspectionDate = new DateTime(); 

                        DateTime.TryParseExact(
                            NextRowData[9], "MM/dd/yyyy", null, DateTimeStyles.None,
                            out NextRowInspectionDate);

                        if (CurrentRowInspectionDate == NextRowInspectionDate)
                        {
                            if (DeficiencyCode == null)
                                DeficiencyCode = FieldData[12] + "," + NextRowData[12];
                            else
                                DeficiencyCode += "," + NextRowData[12];

                            TempCounter += 1;

                            if (TempCounter > LinesFromTextFile.Length - 1)
                                break;

                            NextRowData = LinesFromTextFile[TempCounter].Split('~');
                        }
                        else
                            break;
                    }
                    Counter = TempCounter - 1; //For the 'for' loop

                    InvestigatorList.RowNumber = RowNumber;
                    InvestigatorList.IdNumber = FieldData[0];
                    InvestigatorList.Name = FieldData[1] + " " + FieldData[2];
                    InvestigatorList.Location = FieldData[3];
                    InvestigatorList.Address = FieldData[4];
                    InvestigatorList.City = FieldData[5];
                    InvestigatorList.State = FieldData[6];
                    InvestigatorList.Country = FieldData[7];
                    InvestigatorList.Zip = FieldData[8];
                    InvestigatorList.InspectionDate = FieldData[9];
                    InvestigatorList.InspectionType = FieldData[10];
                    InvestigatorList.ClassificationCode = FieldData[11];

                    if (DeficiencyCode != null)
                        InvestigatorList.DeficiencyCode = DeficiencyCode;
                    else
                        InvestigatorList.DeficiencyCode = FieldData[12];

                    //_clinicalSiteData.ClinicalInvestigatorInspectionList.Add(
                    //    InvestigatorList);
                    _UOW.ClinicalInvestigatorInspectionRepository.Add(InvestigatorList);

                    RowNumber += 1;
                }
            }
            //_log.WriteLog("Total records inserted - " +
            //    _clinicalSiteData.ClinicalInvestigatorInspectionList.Count());

            var recsInserted = _UOW.ClinicalInvestigatorInspectionRepository.GetAll().Count();
            _log.WriteLog("Total records inserted - " +
                recsInserted);

            if (LinesFromTextFile.Length > 0 && recsInserted == 0)
            {
                _log.WriteLog(string.Format("Warning. Likely error in reading data, No of Rows found: {0}, Records Inserted: {1} ", LinesFromTextFile.Length, recsInserted) 
                    );

            }

            File.Delete(FilePath); //delete txt file, retain zipped file
            return recsInserted;
        }

        public override void LoadContent(string NameToSearch,
            int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
        }

        public void ReadSiteLastUpdatedDateFromPage()
        {
            //string[] DatabaseLastUpdated = DatabaseLastUpdatedElement.Text.Split(' ');

            //string LastUpdatedDate = 
            //    DatabaseLastUpdated[DatabaseLastUpdated.Length - 1];

            string LastUpdatedDate = DatabaseLastUpdatedElement.Text.Trim();

            DateTime RecentLastUpdatedDate;

            var IsDateParsed = DateTime.TryParseExact(
                LastUpdatedDate, 
                "M/d/yyyy", 
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, 
                out RecentLastUpdatedDate);

            if(IsDateParsed)
                _SiteLastUpdatedFromPage = RecentLastUpdatedDate;
            else
                throw new Exception(
                    "Could not parse Page last updated string - '" +
                    LastUpdatedDate +
                    "' to DateTime.");
        }

        public string GetSiteLastUpdatedDate()
        {
            string[] DatabaseLastUpdated = DatabaseLastUpdatedElement.Text.Split(' ');

            string LastUpdatedDate = DatabaseLastUpdated[DatabaseLastUpdated.Length - 1];

            return LastUpdatedDate;
        }

        private void AssignReferenceIdOfPreviousDocument()
        {
            var SiteData = _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).First();

            _clinicalSiteData.ReferenceId = SiteData.RecId;
        }
        
        public override void SaveData()
        {
            _UOW.ClinicalInvestigatorInspectionListRepository.Add(
                _clinicalSiteData);
        }

        private void DeleteAllClinicalInvestigatorInspectionRecords()
        {
            var Record = _UOW.ClinicalInvestigatorInspectionRepository.GetAll()
                .FirstOrDefault();

            if (Record != null)
            {
                _log.WriteLog("Old records found.. Deleting old records...");
                _UOW.ClinicalInvestigatorInspectionRepository.DropAll(Record);
                _log.WriteLog("Old records deleted...");
            }
        }

        public override void LoadContent()
        {
            try
            {
                if (!IsPageLoaded())
                    throw new Exception("page is not loaded");

                _clinicalSiteData.DataExtractionRequired = true;
                var FilePath = DownloadCIIList();
                DeleteAllClinicalInvestigatorInspectionRecords();
                var recsInserted = LoadClinicalInvestigatorList(FilePath);
                if (recsInserted > 0)
                {
                    _clinicalSiteData.DataExtractionSucceeded = true;
                }else
                {
                    _clinicalSiteData.DataExtractionSucceeded = false;
                }
            }
            catch (Exception e)
            {
                var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                    "ClinicalInvestigatorInspectionPage_" +
                    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                    + ".jpeg";
                SaveScreenShot(ErrorCaptureFilePath);

                _clinicalSiteData.DataExtractionSucceeded = false;
                _clinicalSiteData.DataExtractionErrorMessage = e.Message;
                _clinicalSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                _clinicalSiteData.CreatedBy = "Patrick";
                _clinicalSiteData.CreatedOn = DateTime.Now;
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
    }
}
