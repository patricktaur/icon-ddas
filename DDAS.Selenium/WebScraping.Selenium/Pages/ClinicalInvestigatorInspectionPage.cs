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

        public void DownloadSDNList()
        {
            //string fileName = _folderPath + @"\test.pdf"; // "c:\\development\\temp\\test.pdf";

            string fileName = "c:\\development\\temp\\cliil.zip";
            string UnZipPath = "c:\\development\\temp\\";
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

        public bool SearchTerms(string Name)
        {
            IWebElement InputTag = ClinicalInvestigatorInputTag;
            InputTag.Clear();
            InputTag.SendKeys(Name);

            IWebElement SubmitButton = ClinicalInvestigatorSubmit;
            SubmitButton.Submit();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            if (ClinicalInvestigatorNext != null)
            {
                return true;
            }
            else
                return false;
        }

        private ClinicalInvestigatorInspectionSiteData _clinicalSiteData;

        public void LoadClinicalInvestigatorListAlt()
        {
            _clinicalSiteData.SiteLastUpdatedOn = DateTime.Now;
            _clinicalSiteData.CreatedBy = "Patrick";
            _clinicalSiteData.CreatedOn = DateTime.Now;
            _clinicalSiteData.Source = driver.Url;

            int RowNumber = 1;

            string[] LinesFromTextFile = File.ReadAllLines("c:\\development\\temp\\cliil.txt");

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

        public void LoadClinicalInvestigatorList()
        {
            _clinicalSiteData.SiteLastUpdatedOn = DateTime.Now;
            _clinicalSiteData.CreatedBy = "Patrick";
            _clinicalSiteData.CreatedOn = DateTime.Now;
            _clinicalSiteData.Source = driver.Url;

            int RowNumber = 1;
            foreach (IWebElement TR in
                ClinicalInvestigatorTable.FindElements(By.XPath("//tbody/tr")))
            {
                var InvestigatorList = new ClinicalInvestigator();
                IList<IWebElement> TDs = TR.FindElements(By.XPath("td"));

                InvestigatorList.RowNumber = RowNumber;
                InvestigatorList.IdNumber = TDs[0].Text;
                InvestigatorList.Name = TDs[1].Text;
                InvestigatorList.Location = TDs[2].Text;
                InvestigatorList.Address = TDs[3].Text;
                InvestigatorList.City = TDs[4].Text;
                InvestigatorList.State = TDs[5].Text;
                InvestigatorList.Country = TDs[6].Text;
                InvestigatorList.Zipcode = TDs[7].Text;
                InvestigatorList.InspectionDate = TDs[8].Text;
                InvestigatorList.ClassificationType = TDs[9].Text;
                InvestigatorList.ClassificationCode = TDs[10].Text;
                InvestigatorList.DeficiencyCode = TDs[11].Text;

                _clinicalSiteData.ClinicalInvestigatorInspectionList.Add
                    (InvestigatorList);
                RowNumber += 1;
            }
        }

        public int GetCountOfRecords()
        {
            IWebElement element = ClinicalInvestigatorNextList;

            IList<IWebElement> ANCHORs = element.FindElements(By.XPath("//span/a"));

            int AnchorCount = ANCHORs.Count;

            return Convert.ToInt32(ANCHORs[AnchorCount - 1].Text);
        }

        public override void LoadContent(string NameToSearch)
        {
            DownloadSDNList();
            LoadClinicalInvestigatorListAlt();
            //if (SearchTerms())
            //{
            //    int totalRecords = GetCountOfRecords();

            //    for (int records = 0; records < totalRecords; records++)
            //    {
            //        LoadClinicalInvestigatorList();

            //        if (totalRecords > 1)
            //        {
            //            LoadNextRecord();
            //        }
            //    }
            //}
            //else
            //    driver.Url = 
            //        "http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm";
        }

        public void LoadNextRecord()
        {
            IWebElement element = ClinicalInvestigatorNext;
            element.Click();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
        }

        public bool SearchTerms()
        {
            IWebElement AdvancedSearchElement = ClinicalInvestigatorAdvancedSearch;
            AdvancedSearchElement.Click();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            IWebElement FirstNameDropDown = ClinicalInvestigatorFirstNameDropDown;
            FirstNameDropDown.SendKeys("not equal to");

            IWebElement FirstNameTextField = ClinicalInvestigatorFirstNameTextField;
            FirstNameTextField.SendKeys("zzzzzz");

            ClinicalInvestigatorSubmit.Submit();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));

            return true;
        }
        
        public override void SaveData()
        {
            _UOW.ClinicalInvestigatorInspectionListRepository.Add(
                _clinicalSiteData);
        }
    }
}
