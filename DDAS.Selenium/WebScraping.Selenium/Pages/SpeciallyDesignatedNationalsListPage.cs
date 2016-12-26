﻿using System;
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

namespace WebScraping.Selenium.Pages
{
    public partial class SpeciallyDesignatedNationalsListPage: ISearchPage //BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private DateTime? _SiteLastUpdatedFromPage;
        private DateTime? _SiteLastUpdatedFromDatabse;

        [DllImport("urlmon.dll")]
        public static extern long URLDownloadToFile(long pCaller, string szURL, 
            string szFileName, long dwReserved, long lpfnCB);

        public SpeciallyDesignatedNationalsListPage(
            IUnitOfWork uow, IWebDriver driver)
            :  base(driver)
        {
            _UOW = uow;
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

        public DateTime? SiteLastUpdatedDateFromDatabase
        {
            get
            {
                return _SiteLastUpdatedFromDatabse;
            }
        }

        public BaseSiteData baseSiteData
        {
            get
            {
                return _SDNSiteData;
            }
        }

        private void DownloadSDNList(string DownloadFolder)
        {
            //string fileName = _folderPath + @"\test.pdf"; // "c:\\development\\temp\\test.pdf";

            string fileName = DownloadFolder + "\\SDNList.pdf";

            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            string myStringWebResource = "https://www.treasury.gov/ofac/downloads/sdnlist.pdf";
            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(myStringWebResource, fileName);
        }

        private SpeciallyDesignatedNationalsListSiteData _SDNSiteData;

        private List<SDNList> GetTextFromPDF(string NameToSearch, string DownloadFolder)
        {
            string tempSiteDate = SDNSiteUpdatedDate.Text.Replace("Last Updated: ", "");

            DateTime SiteDateTime;

            DateTime.TryParse(tempSiteDate, out SiteDateTime);

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

                    //Console.WriteLine(PageContent.Length);

                    string[] SplitNames = PageContent.Split(']');

                    int RecordNumber = 1;
                    for (int records = 0; records < SplitNames.Length; records++)
                    {
                        string TempRecord = SplitNames[records].Replace(" ", "");

                        SDNList NamesFromPDF = new SDNList();

                        NamesFromPDF.RowNumber = RowNumber;
                        NamesFromPDF.Name = TempRecord;
                        NamesFromPDF.RecordNumber = RecordNumber;
                        NamesFromPDF.PageNumber = i;
                        NamesFromPDF.WordsMatched = WordsMatched;
                        _SDNSiteData.SDNListSiteData.Add(NamesFromPDF);

                        RecordNumber += 1;
                        RowNumber += 1;
                    }
                }
            }
            return Names;
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

        public void LoadContent(string NameToSearch, string DownloadFolder)
        {
            try
            {
                //if (!CheckSiteUpdatedDate())
                //{
                    _SDNSiteData.DataExtractionRequired = true;
                    DownloadSDNList(DownloadFolder);
                    GetTextFromPDF("", DownloadFolder);
                    _SDNSiteData.DataExtractionSucceeded = true;
                //}
            }
            catch (Exception e)
            {
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
            string temp = SDNSiteUpdatedDate.Text.Replace("Last Updated: ", "").Trim();

            DateTime CurrentSiteUpdatedDate;

            DateTime.TryParseExact(temp, "M'/'d'/'yyyy h:mm tt",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None, out CurrentSiteUpdatedDate);

            _SiteLastUpdatedFromPage = CurrentSiteUpdatedDate;
        }

        public void LoadContent(string DownloadFolder)
        {
            try
            {
                //if (!CheckSiteUpdatedDate())
                //{
                _SDNSiteData.DataExtractionRequired = true;
                DownloadSDNList(DownloadFolder);
                GetTextFromPDF("", DownloadFolder);
                _SDNSiteData.DataExtractionSucceeded = true;
                //}
            }
            catch (Exception e)
            {
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
