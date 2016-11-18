using System;
using System.Collections.Generic;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.BaseClasses;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models;
using System.Net;
using System.IO;

namespace WebScraping.Selenium.Pages
{
    public partial class ExclusionDatabaseSearchPage : BaseSearchPage 
    {

        private IUnitOfWork _UOW;

        public ExclusionDatabaseSearchPage(IWebDriver driver, IUnitOfWork uow) 
            : base(driver)
        {
            _UOW = uow;
            Open();
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

        public bool SearchTerms(string FirstName, string LastName)
        {
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

            IWebElement FNameElement = ExclusionDatabaseSearchFirstName;
            FNameElement.SendKeys(FirstName);

            IWebElement LNameElement = ExclusionDatabaseSearchLastName;
            LNameElement.SendKeys(LastName);

            IWebElement SearchElement = ExclusionDatabaseSearchElement;
            SearchElement.SendKeys(Keys.Enter);
            
            //wait for the page to load
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            if (ExclusionDatabaseSearchTable != null)
            {
                return true;
            }
            else
                ExclusionDatabaseSearchAgain.Click(); //back to search page
                return false;
        }

        private ExclusionDatabaseSearchPageSiteData _exclusionSearchSiteData;

        public void LoadExclusionsDatabaseList()
        {
            _exclusionSearchSiteData = new ExclusionDatabaseSearchPageSiteData();

            _exclusionSearchSiteData.CreatedBy = "Patrick";
            _exclusionSearchSiteData.SiteLastUpdatedOn = DateTime.Now;
            _exclusionSearchSiteData.CreatedOn = DateTime.Now;
            _exclusionSearchSiteData.Source = driver.Url;

            foreach (IWebElement TR in 
                ExclusionDatabaseSearchTable.FindElements(By.XPath("tbody/tr")))
            {
                ExclusionDatabaseSearchList NewExclusionsList = new ExclusionDatabaseSearchList();

                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                if(TDs.Count != 0)
                {
                    NewExclusionsList.LastName = TDs[0].Text;
                    NewExclusionsList.FirstName = TDs[1].Text;
                    NewExclusionsList.MiddleName = TDs[2].Text;
                    NewExclusionsList.General = TDs[3].Text;
                    NewExclusionsList.Specialty = TDs[4].Text;
                    NewExclusionsList.ExclusionType = TDs[5].Text;
                    //NewExclusionsList.WaiverDate = TDs[6].Text;
                    //NewExclusionsList.SSNorEIN = TDs[7].Text;

                    _exclusionSearchSiteData.ExclusionSearchList.Add
                        (NewExclusionsList);
                }
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            //refactor, add code to enter search names
            //LoadExclusionsDatabaseList();

            string FilePath = DownloadExclusionList(DownloadFolder);
            LoadExclusionDatabaseListFromCSV(FilePath);
        }

        public void LoadExclusionDatabaseListFromCSV(string CSVFilePath)
        {
            string[] AllRecords = File.ReadAllLines(CSVFilePath);

            _exclusionSearchSiteData = new ExclusionDatabaseSearchPageSiteData();

            _exclusionSearchSiteData.CreatedBy = "Patrick";
            _exclusionSearchSiteData.SiteLastUpdatedOn = DateTime.Now;
            _exclusionSearchSiteData.CreatedOn = DateTime.Now;
            _exclusionSearchSiteData.Source = driver.Url;

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

        public string DownloadExclusionList(string DownloadFolder)
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

        public override void SaveData()
        {
            _UOW.ExclusionDatabaseSearchRepository.Add(
                _exclusionSearchSiteData);
        }
    }
}
