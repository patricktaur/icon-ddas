using OpenQA.Selenium;
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

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorInspectionPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        [DllImport("urlmon.dll")]
        public static extern long URLDownloadToFile(long pCaller, string szURL,
            string szFileName, long dwReserved, long lpfnCB);

        public ClinicalInvestigatorInspectionPage(IWebDriver driver, IUnitOfWork uow) 
            : base(driver)
        {
            _UOW = uow;
            Open();
            _clinicalSiteData = new ClinicalInvestigatorInspectionSiteData();
            _clinicalSiteData.RecId = Guid.NewGuid();
            //SaveScreenShot("ClinicalInvestigatorInspectionPage.png");
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

        private void DownloadSDNList(string DownloadFolder)
        {
            //string fileName = _folderPath + @"\test.pdf";

            string fileName = DownloadFolder  + "cliil.zip";
            string UnZipPath = DownloadFolder;
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            string myStringWebResource = ClinicalInvestigatorZipAnchor.GetAttribute("href");

            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", 
                fileName, myStringWebResource);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(myStringWebResource, fileName);

            if (!File.Exists(UnZipPath + "\\cliil.txt"))
                ZipFile.ExtractToDirectory(fileName, UnZipPath);
        }

        private ClinicalInvestigatorInspectionSiteData _clinicalSiteData;

        //this function currently not in use. Using the downloaded text file to save data
        private void LoadClinicalInvestigatorListAlt(string DownloadFolder)
        {
            _clinicalSiteData.SiteLastUpdatedOn = DateTime.Now;
            _clinicalSiteData.CreatedBy = "Patrick";
            _clinicalSiteData.CreatedOn = DateTime.Now;
            _clinicalSiteData.Source = driver.Url;

            int RowNumber = 1;

            string[] LinesFromTextFile = File.ReadAllLines(DownloadFolder + "cliil.txt");

            foreach (string line in LinesFromTextFile)
            {
                string[] FieldData = line.Split('~');

                var InvestigatorList = new ClinicalInvestigator();

                if (FieldData.Length > 1)
                {
                    if (FieldData[1].ToLower().Contains("last name"))
                        continue;

                    InvestigatorList.RowNumber = RowNumber;
                    InvestigatorList.IdNumber = FieldData[0];
                    InvestigatorList.Name = FieldData[1] + " " + FieldData[2];
                    InvestigatorList.Location = FieldData[3];
                    InvestigatorList.Address = FieldData[4];
                    InvestigatorList.City = FieldData[5];
                    InvestigatorList.State = FieldData[6];
                    InvestigatorList.Country = FieldData[7];
                    InvestigatorList.Zipcode = FieldData[8];
                    InvestigatorList.InspectionDate = FieldData[9];
                    InvestigatorList.ClassificationType = FieldData[10];
                    InvestigatorList.ClassificationCode = FieldData[11];
                    InvestigatorList.DeficiencyCode = FieldData[12];

                    _clinicalSiteData.ClinicalInvestigatorInspectionList.Add(
                        InvestigatorList);
                    RowNumber += 1;   
                }
            }
        }

        public override void LoadContent(string NameToSearch, string DownloadFolder)
        {
            //refactor - add code to validate ExtractionDate
            try
            {
                _clinicalSiteData.DataExtractionRequired = true;
                if (_clinicalSiteData.DataExtractionRequired)
                {
                    LoadClinicalInvestigatorListAlt(DownloadFolder);
                    _clinicalSiteData.DataExtractionSucceeded = true;
                }
            }
            catch (Exception e)
            {
                _clinicalSiteData.DataExtractionSucceeded = false;
                _clinicalSiteData.DataExtractionErrorMessage = e.Message;
                _clinicalSiteData.ReferenceId = null;
                throw new Exception(e.ToString());
            }
            finally
            {
                if (!_clinicalSiteData.DataExtractionRequired)
                    AssignReferenceIdOfPreviousDocument();
                else
                    _clinicalSiteData.ReferenceId =
                        _clinicalSiteData.RecId;
            }
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
    }
}
