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
using System.Globalization;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;

namespace WebScraping.Selenium.Pages
{
    public partial class ClinicalInvestigatorInspectionPage : BaseSearchPage
    {
        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;

        [DllImport("urlmon.dll")]
        public static extern long URLDownloadToFile(long pCaller, string szURL,
            string szFileName, long dwReserved, long lpfnCB);

        public ClinicalInvestigatorInspectionPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config) 
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
            Open();
            _clinicalSiteData = new ClinicalInvestigatorInspectionSiteData();
            _clinicalSiteData.RecId = Guid.NewGuid();
            _clinicalSiteData.ReferenceId = _clinicalSiteData.RecId;
            _clinicalSiteData.Source = driver.Url;
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

        private void DownloadCIIList()
        {
            //string fileName = _folderPath + @"\test.pdf";

            string fileName = _config.AppDataDownloadsFolder + "cliil.zip";
            string UnZipPath = _config.AppDataDownloadsFolder;
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            string myStringWebResource = ClinicalInvestigatorZipAnchor.GetAttribute("href");

            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", 
                fileName, myStringWebResource);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(myStringWebResource, fileName);

            if (!File.Exists(UnZipPath + "\\cliil.txt")) //filename is cliil.txt by default
                ZipFile.ExtractToDirectory(fileName, UnZipPath);
        }

        private ClinicalInvestigatorInspectionSiteData _clinicalSiteData;

        private void LoadClinicalInvestigatorListAlt()
        {
            int RowNumber = 1;

            string[] LinesFromTextFile = 
                File.ReadAllLines(
                    _config.AppDataDownloadsFolder + "cliil.txt");

            DateTime CurrentRowInspectionDate = new DateTime();

            //foreach (string line in LinesFromTextFile)
            for (int Counter = 0; Counter < LinesFromTextFile.Length; Counter++)
            {
                string[] FieldData = LinesFromTextFile[Counter].Split('~');

                var InvestigatorList = new ClinicalInvestigator();

                if (FieldData.Length > 1)
                {
                    if (FieldData[1].ToLower().Contains("last name"))
                        continue;

                    string[] NextRowData = LinesFromTextFile[Counter + 1].Split('~');

                    //CurrentRowInspectionDate = DateTime.ParseExact(
                    //    FieldData[9],
                    //    "MM/dd/yyyy", null);

                    DateTime.TryParseExact(
                        FieldData[9], "MM/dd/yyyy", null, 
                        DateTimeStyles.None, out CurrentRowInspectionDate);

                    string DeficiencyCode = null;
                    int TempCounter = Counter + 1;

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
                            NextRowData = LinesFromTextFile[TempCounter].Split('~');
                        }
                        else
                            break;
                    }
                    Counter = TempCounter - 1; //For the 'for' loop!

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
                    InvestigatorList.ClassificationType = FieldData[10];
                    InvestigatorList.ClassificationCode = FieldData[11];

                    if (DeficiencyCode != null)
                        InvestigatorList.DeficiencyCode = DeficiencyCode;
                    else
                        InvestigatorList.DeficiencyCode = FieldData[12];

                    _clinicalSiteData.ClinicalInvestigatorInspectionList.Add(
                        InvestigatorList);
                    RowNumber += 1;
                }
            }
        }

        public override void LoadContent(string NameToSearch,
            int MatchCountLowerLimit)
        {
            throw new NotImplementedException();
        }

        public void ReadSiteLastUpdatedDateFromPage()
        {
            //ClinicalInvestigatorInspectionSiteData ClinicalSiteData = null;

            string[] DatabaseLastUpdated = DatabaseLastUpdatedElement.Text.Split(' ');

            string LastUpdatedDate = 
                DatabaseLastUpdated[DatabaseLastUpdated.Length - 1];

            DateTime RecentLastUpdatedDate;

            DateTime.TryParseExact(LastUpdatedDate, "M'/'d'/'yyyy", 
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out RecentLastUpdatedDate);

            _SiteLastUpdatedFromPage = RecentLastUpdatedDate;

            //var ExistingClinicalSiteData =
            //    _UOW.ClinicalInvestigatorInspectionListRepository.GetAll();

            //if (ExistingClinicalSiteData.Count == 0)
            //{
            //    _clinicalSiteData.SiteLastUpdatedOn = RecentLastUpdatedDate;
            //    _clinicalSiteData.DataExtractionRequired = true;
            //}
            //else
            //{
            //    ClinicalSiteData = ExistingClinicalSiteData.OrderByDescending(
            //        x => x.CreatedOn).First();

            //    if (RecentLastUpdatedDate > ClinicalSiteData.SiteLastUpdatedOn)
            //    {
            //        _clinicalSiteData.SiteLastUpdatedOn = RecentLastUpdatedDate;
            //        _clinicalSiteData.DataExtractionRequired = true;
            //    }
            //    else
            //    {
            //        _clinicalSiteData.SiteLastUpdatedOn =
            //            ClinicalSiteData.SiteLastUpdatedOn;
            //        _clinicalSiteData.DataExtractionRequired = false;
            //    }
            //}

            //if (!_clinicalSiteData.DataExtractionRequired)
            //    _clinicalSiteData.ReferenceId = ClinicalSiteData.RecId;
            //else
            //    _clinicalSiteData.ReferenceId =
            //        _clinicalSiteData.RecId;
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

        public override void LoadContent()
        {
            try
            {
                _clinicalSiteData.DataExtractionRequired = true;
                DownloadCIIList();
                LoadClinicalInvestigatorListAlt();
                _clinicalSiteData.DataExtractionSucceeded = true;
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
                _clinicalSiteData.CreatedBy = "Patrick";
                _clinicalSiteData.CreatedOn = DateTime.Now;
            }
        }
    }
}
