using System;
using System.Text;
using System.Linq;
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
using System.Net.Http;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using WebScraping.Selenium.JsonClasses;


namespace WebScraping.Selenium.Pages
{
    public partial class SystemForAwardManagementPage : BaseSearchPage
    {
        [DllImport("urlmon.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Int32 URLDownloadToFile(Int32 pCaller, string szURL, string szFileName, Int32 dwReserved, Int32 lpfnCB);

        private IUnitOfWork _UOW;
        private IConfig _config;
        private DateTime? _SiteLastUpdatedFromPage;
        private ILog _log;

        public SystemForAwardManagementPage(IWebDriver driver, IUnitOfWork uow,
            IConfig Config, ILog Log) 
            : base(driver)
        {
            _UOW = uow;
            _config = Config;
            _log = Log;
            //Open();
            _SAMSiteData = new SystemForAwardManagementPageSiteData();
            _SAMSiteData.RecId = Guid.NewGuid();
            _SAMSiteData.ReferenceId = _SAMSiteData.RecId;
            _SAMSiteData.Source = Url;
        }

        public override SiteEnum SiteName {
            get {
                return SiteEnum.SystemForAwardManagementPage;
            }
        }

        public override string Url {
            get {
                return @"https://www.sam.gov";
                //return @"http://www.fda.gov/ora/compliance_ref/debar/default.htm";
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

        private string DownloadExclusionFileThroughApi()
        {
            string zipFileName =
                _config.SAMFolder + SiteName.ToString() + "_";
            zipFileName += DateTime.Now.ToString("yyyyMMddHHmmss") + ".gz";
            string csvFileName = zipFileName.Replace(".gz", ".csv");

            if (File.Exists(csvFileName))
                File.Delete(csvFileName); //extracted csv file

            if (File.Exists(zipFileName))
                File.Delete(zipFileName); //zip file
            //var retFileName = "";
            try
            {
                var fileDownloadUrl = GetFileDownloadUrlFromSAM();
                _log.WriteLog(
                string.Format("Downloading File \"{0}\" from \"{1}\" .......\n\n",
                Path.GetFileName(zipFileName), fileDownloadUrl));

                var maxAttempts = 10;
                var delaySecs = 120; //; success 60
                var retValue = DownloadFileWithDelayedAction(delaySecs, maxAttempts, fileDownloadUrl, zipFileName);
                if (retValue == true)
                {
                    FileInfo fi = new FileInfo(zipFileName);
                    Decompress(fi, csvFileName);
                }else
                {
                    throw new Exception(String.Format("Could not download file even after {0} attempts, delay between attempts: {1} seconds ", maxAttempts, delaySecs) );
                }
                
                //ZipFile.ExtractToDirectory(fileName, _config.SAMFolder);
            }
            catch (WebException ex) //when using WebClient
            {
                throw new Exception("Could not download file. " + ex.Message);
            }
            //When using URLDownloadToFile win32 API
            //ZipFile.ExtractToDirectory throws up this exception
            catch (InvalidDataException)
            {
                throw new Exception("Could not extract file. " +
                    "Possible Http 404 File not found error on SAM site");
            }

            _log.WriteLog("download complete");

            return csvFileName;
        }

        private bool DownloadFileWithDelayedAction(int delay, int maxAttempts, string fileDownloadUrl, string zipFileName)
        {
            //delayInSecs
           
            var inMilliSecs = delay * 1000;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                System.Threading.Thread.Sleep(inMilliSecs);
                var retValue = DownloadFile(fileDownloadUrl, zipFileName);
                if (retValue == true)
                {
                    return true;
                }
            }
            return false;
        }

        private bool DownloadFile(string fileDownloadUrl, string zipFileName)
        {
            try
            {
                WebClient myWebClient = new WebClient();
                myWebClient.DownloadFile(fileDownloadUrl, zipFileName);
            }
            catch (Exception ex)
            {
                var exMsg = ex.Message;
                return false;
                throw;
            }
            

            
            return true;
        }

        private void Decompress(FileInfo fileToDecompress, string unzipFileName)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                //string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(unzipFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        //Console.WriteLine($"Decompressed: {fileToDecompress.Name}");
                    }
                }

                
            }
        }



        private string DownloadExclusionFile()
        {
            string fileName =
                _config.SAMFolder + SiteName.ToString() + "_";
            //_config.SAMFolder + "SAM_Exclusions_Public_Extract_";

            //string CSVFilePath = fileName;
            string CSVFilePath = _config.SAMFolder + "SAM_Exclusions_Public_Extract_";
            string UnZipPath = _config.SAMFolder;

            WebClient myWebClient = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //SAM/extractfiledownload?role=WW&version=EPLSPUB&filename=SAM_Exclusions_Public_Extract_18316.ZIP
            //https://www.sam.gov/public-extracts/SAM-Public/SAM_Exclusions_Public_Extract_
            string myStringWebResource = "https://www.sam.gov/SAM/extractfiledownload?role=WW&version=EPLSPUB&filename=SAM_Exclusions_Public_Extract_";
            //06June2020
            //The file SAM_Exclusions_Public_Extract_XXXXX.ZIP
            //is available for download after ~12:00noon IST even when the link is displayed on the web site
            //The extraction fails because the extraction program is executed around 12:00 midnight.
            //Hence the file of the previous day is extracted.

            //current day:
            //string Year = DateTime.Now.ToString("yy");
            //string JulianDate = DateTime.Now.DayOfYear.ToString("000");
            //prev day:
            var fileDate = DateTime.Now.AddDays(-1);
            string Year = fileDate.ToString("yy");
            string JulianDate = fileDate.DayOfYear.ToString("000");

            //LatestExclusionExtractAnchorTag.Text.Split(' ')[1];

            myStringWebResource += Year + JulianDate + ".ZIP";

            fileName += Year + JulianDate + ".ZIP";
            CSVFilePath += Year + JulianDate + ".CSV";

            _log.WriteLog(
                string.Format("Downloading File \"{0}\" from \"{1}\" .......\n\n", 
                Path.GetFileName(fileName), myStringWebResource));

            if (File.Exists(CSVFilePath))
                File.Delete(CSVFilePath); //extracted csv file

            if (File.Exists(fileName))
                File.Delete(fileName); //zip file

            try
            {
                myWebClient.DownloadFile(myStringWebResource, fileName);
                //URLDownloadToFile(0, myStringWebResource, fileName, 0, 0);
                ZipFile.ExtractToDirectory(fileName, _config.SAMFolder);
            }
            catch(WebException ex) //when using WebClient
            {
                throw new Exception("Could not download file. " + ex.Message);
            }
            //When using URLDownloadToFile win32 API
            //ZipFile.ExtractToDirectory throws up this exception
            catch (InvalidDataException)
            {
                throw new Exception("Could not extract file. " +
                    "Possible Http 404 File not found error on SAM site");
            }

            _log.WriteLog("download complete");

            return CSVFilePath;
        }
        

        private void LoadSAMDatafromCSV(string CSVFilePath)
        {
            //Extraction of file downloaded prior to 30Sep2021:
            _log.WriteLog("Reading records from the file - " +
                Path.GetFileName(CSVFilePath));

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

                SAMSiteRecord.City = fields[11].Trim();
                SAMSiteRecord.State = fields[12].Trim();
                SAMSiteRecord.Country = fields[13].Trim();

                SAMSiteRecord.ExcludingAgency = fields[17].Trim();
                SAMSiteRecord.ExclusionType = fields[19].Trim();
                SAMSiteRecord.AdditionalComments = fields[20].Trim();
                SAMSiteRecord.ActiveDate = fields[21].Trim();
                SAMSiteRecord.RecordStatus = fields[23].Trim();

                //_SAMSiteData.SAMSiteData.Add(SAMSiteRecord);
                _UOW.SAMSiteDataRepository.Add(SAMSiteRecord);
                RowNumber += 1;
            }
            _log.WriteLog("Total records inserted - " +
                _UOW.SAMSiteDataRepository.GetAll().Count);

            parser.Dispose();

            File.Delete(CSVFilePath); //delete CSV file, retain zipped file
        }

        private void LoadSAMDatafromApiCsv(string CSVFilePath)
        {
            //File downloaded through API route:
            /* CSV File Headers:
             0	classificationType
                1	exclusionType
                2	exclusionProgram
                3	excludingAgencyCode
                4	excludingAgencyName
                5	ueiSAM
                6	ueiDUNS
                7	cageCode
                8	npi
                9	prefix
                10	firstName
                11	middleName
                12	lastName
                13	suffix
                14	entityName
                15	dnbOpenData
                16	createDate
                17	updateDate
                18	activateDate
                19	terminationDate
                20	terminationType
                21	recordStatus
                22	exclusionPrimaryAddress:addressLine1
                23	exclusionPrimaryAddress:addressLine2
                24	exclusionPrimaryAddress:city
                25	exclusionPrimaryAddress:stateOrProvinceCode
                26	exclusionPrimaryAddress:zipCode
                27	exclusionPrimaryAddress:zipCodePlus4
                28	exclusionPrimaryAddress:countryCode
                29	exclusionSecondaryAddress
                30	additionalComments
                31	ctCode
                32	evsInvestigationStatus
                33	references:exclusionName
                34	references:type
                35	moreLocations:exclusionName
                36	moreLocations:duns
                37	moreLocations:cageCode
                38	moreLocations:npi
                39	moreLocations:primaryAddress:addressLine1
                40	moreLocations:primaryAddress:addressLine2
                41	moreLocations:primaryAddress:city
                42	moreLocations:primaryAddress:stateOrProvinceCode
                43	moreLocations:primaryAddress:zipCode
                44	moreLocations:primaryAddress:zipCodePlus4
                45	moreLocations:primaryAddress:countryCode
                46	callSign
                47	type
                48	tonnage
                49	grt
                50	flag
                51	owner

             */
            _log.WriteLog("Reading records from the file - " +
                Path.GetFileName(CSVFilePath));

            TextFieldParser parser = new TextFieldParser(CSVFilePath);
            var skippedRecords = 0;
            var insertedRecords = 0;
            var parseErrorRecords = 0;
            var parseError = "";
            var comma = "";
            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");
            //skip header:
            parser.ReadFields();
            string[] fields;

            int RowNumber = 1;
            while (!parser.EndOfData)
            {
                try
                {
                    fields = parser.ReadFields();
                }
                catch (MalformedLineException ex)
                {
                     
                    
                    
                    if (ex.Message.Contains("cannot be parsed using the current Delimiters"))
                    {
                        

                        if (parser.ErrorLine.StartsWith("\""))
                        {
                            var line = parser.ErrorLine.Substring(1, parser.ErrorLine.Length - 2);
                            //fields = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                            fields = line.Split(new string[] { "\",\"", "," }, StringSplitOptions.None);
                        }
                        else
                        {
                            var items = ex.Message.Split(' ');
                            parseError += comma + items[1];
                            parseErrorRecords += 1;
                            comma = ",";
                            //Ignore - and contintue
                            continue;
                        }
                    }
                    else {
                       throw    ;
                    }
                    
                }

                var SAMSiteRecord = new SystemForAwardManagement();
                //var SAMSiteRecord = new SAMSiteData();

                SAMSiteRecord.RecId = Guid.NewGuid();
                SAMSiteRecord.ParentId = _SAMSiteData.RecId;

                SAMSiteRecord.RowNumber = RowNumber;
                SAMSiteRecord.First = fields[10].Trim().Length > 0 && fields[10].Trim() != "null" ? fields[10].Trim() : "";
                SAMSiteRecord.Middle = fields[11].Trim().Length > 0 && fields[11].Trim() != "null" ? fields[11].Trim() : "";
                SAMSiteRecord.Last = fields[12].Trim().Length > 0 && fields[12].Trim() != "null" ? fields[12].Trim() : "";

                if (SAMSiteRecord.First.Length > 0 || SAMSiteRecord.Middle.Length > 0 || SAMSiteRecord.Last.Length > 0)
                {
                    SAMSiteRecord.City =  fields[24].Trim().Length > 0 && fields[24].Trim() != "null" ? fields[24].Trim() : "";
                    SAMSiteRecord.State = fields[25].Trim().Length > 0 && fields[25].Trim() != "null" ? fields[25].Trim() : "";
                    SAMSiteRecord.Country = fields[28].Trim().Length > 0 && fields[28].Trim() != "null" ? fields[28].Trim() : "";

                    SAMSiteRecord.ExcludingAgency = fields[4].Trim().Length > 0 && fields[4].Trim() != "null" ? fields[4].Trim() : "";
                    SAMSiteRecord.ExclusionType = fields[1].Trim().Length > 0 && fields[1].Trim() != "null" ? fields[1].Trim() : "";
                    SAMSiteRecord.AdditionalComments = fields[30].Trim().Length > 0 && fields[30].Trim() != "null" ? fields[30].Trim() : "";
                    SAMSiteRecord.ActiveDate = fields[18].Trim().Length > 0 && fields[18].Trim() != "null" ? fields[18].Trim() : "";
                    SAMSiteRecord.RecordStatus = fields[21].Trim().Length > 0 && fields[21].Trim() != "null" ? fields[21].Trim() : "";

                    //_SAMSiteData.SAMSiteData.Add(SAMSiteRecord);
                    _UOW.SAMSiteDataRepository.Add(SAMSiteRecord);
                    insertedRecords += 1;
                }else
                {
                    skippedRecords += 1;
                }
                RowNumber += 1;
            }

            _log.WriteLog("Total records read - " + (RowNumber - 1));
            _log.WriteLog("Total skipped records- " + skippedRecords);
            _log.WriteLog("Parse Error (malformed) - " + parseError);
            _log.WriteLog("Total Parse Error records - " + parseErrorRecords);

            _log.WriteLog("Total records inserted - " + insertedRecords);

            parser.Dispose();

            File.Delete(CSVFilePath); //delete CSV file, retain zipped file
        }

        private string GetFileDownloadUrlFromSAM()
        {
            WebClient webClient = new WebClient();
            var apiKeyPat = "nNJ3ofKIuXYElBV0oAVtMV3PwjxSF12pKBpyzQUy";
            var apiKey = "keJ5DP9i1UsLIXPsi3pJ47EXJWjyg3DQKkkKVKeT";

            //apiKey = apiKeyPat;
            
            

            try
            {
                var baseUrl = "https://api.sam.gov/entity-information/v2/exclusions";
                var extraParams = "&format=CSV";
                var url = String.Format("{0}?api_key={1}{2}", baseUrl, apiKey, extraParams);
                var response = webClient.DownloadData(url);
                string responseString = Encoding.ASCII.GetString(response);
                string downloadUrl = ExtractDownloadUrl(responseString, apiKey);
                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not obtain download url, possible reasons: Password expired or too many attempts" + ex.Message);
               
            }
 
        }
        private string ExtractDownloadUrl(string responseFromSAM, string apiKey)
        {
            /* Sample response from SAM Site:
             Extract File will be available for download with url: https://api.sam.gov/entity-information/v2/download-exclusions?api_key=REPLACE_WITH_API_KEY&token=cfQOzKSSYv in some time. If you have requested for an email notification, you will receive it once the file is ready for download. Requests for Larger Set of Data may take longer time to process.
            */
            var startText = "https://api.sam.gov";
            var apiKeyPlaceHolder = "REPLACE_WITH_API_KEY";
            var startPos = responseFromSAM.IndexOf(startText);
            var url = responseFromSAM.Substring(startPos);
            var endText = " ";
            var endPos = url.IndexOf(endText);
            url = url.Substring(0, endPos);
            url = url.Replace(apiKeyPlaceHolder, apiKey);
            return url;
        }

        private void LoadSAMDataFromApi()
        {
            _log.WriteLog("Reading from api - " );

            var apiKey = "keJ5DP9i1UsLIXPsi3pJ47EXJWjyg3DQKkkKVKeT";

            WebClient webClient = new WebClient();
            //https://api.sam.gov/opportunities/v2/search
            //https://api-alpha.sam.gov/prodlike/opportunities/v1/search
            //https://api.sam.gov/entity-information/v2/exclusions?api_key=< value >
            //https://api.sam.gov/entity-information/v2/exclusions?api_key=< a valid Public API Key >&classification=[Individual~Special Entity Designation]&excludingAgencyCode=!DOJ&country=KOR&q=CHONG
            //https://api.sam.gov/entity-information/v2/exclusions?api_key=< a valid Public API Key >&classification=[Individual~Special Entity Designation]&excludingAgencyCode=!DOJ&country=KOR&q=CHONG
            var baseUrl = "https://api.sam.gov/entity-information/v2/exclusions";
            var page = 1;
            var size = 10;

            var url = String.Format("{0}?api_key={1}", baseUrl, apiKey, page, size);
            //var url = String.Format("{0}?api_key={1}&page={1}&size={2}", baseUrl, apiKey, page, size);

            try
            {
                var dataBuffer = webClient.DownloadData(url);

                string download = Encoding.ASCII.GetString(dataBuffer);
                var samData = JsonConvert.DeserializeObject<ExclusionDataFromSAM>(download);
                
                Console.Write(download);
            }
            catch (Exception ex)
            {
                //Error code 429
                Console.Write(ex.Message);
                throw;
            }
            
            //TextFieldParser parser = new TextFieldParser(CSVFilePath);

            //parser.HasFieldsEnclosedInQuotes = true;
            //parser.SetDelimiters(",");

            //string[] fields;

            //int RowNumber = 1;
            //while (!parser.EndOfData)
            //{
            //    fields = parser.ReadFields();

            //    if (fields[3].Trim().Length == 0 &&
            //        fields[4].Trim().Length == 0 &&
            //        fields[5].Trim().Length == 0)
            //    {
            //        continue;
            //    }
            //    else if (fields[3].ToLower() == "first")
            //    {
            //        continue;
            //    }

            //    var SAMSiteRecord = new SystemForAwardManagement();
            //    //var SAMSiteRecord = new SAMSiteData();

            //    SAMSiteRecord.RecId = Guid.NewGuid();
            //    SAMSiteRecord.ParentId = _SAMSiteData.RecId;

            //    SAMSiteRecord.RowNumber = RowNumber;
            //    SAMSiteRecord.First = fields[3].Trim();
            //    SAMSiteRecord.Middle = fields[4].Trim();
            //    SAMSiteRecord.Last = fields[5].Trim();

            //    SAMSiteRecord.City = fields[11].Trim();
            //    SAMSiteRecord.State = fields[12].Trim();
            //    SAMSiteRecord.Country = fields[13].Trim();

            //    SAMSiteRecord.ExcludingAgency = fields[17].Trim();
            //    SAMSiteRecord.ExclusionType = fields[19].Trim();
            //    SAMSiteRecord.AdditionalComments = fields[20].Trim();
            //    SAMSiteRecord.ActiveDate = fields[21].Trim();
            //    SAMSiteRecord.RecordStatus = fields[23].Trim();

            //    //_SAMSiteData.SAMSiteData.Add(SAMSiteRecord);
            //    _UOW.SAMSiteDataRepository.Add(SAMSiteRecord);
            //    RowNumber += 1;
            //}
            //_log.WriteLog("Total records inserted - " +
            //    _UOW.SAMSiteDataRepository.GetAll().Count);

            //parser.Dispose();

            //File.Delete(CSVFilePath); //delete CSV file, retain zipped file
        }

        private void DelteAllSAMSiteDataRecords()
        {
            //var record = _UOW.SAMSiteDataRepository.GetAll().FirstOrDefault();
            //_UOW.SAMSiteDataRepository.DropAll(record);

            var Record = _UOW.SAMSiteDataRepository.GetAll()
                .FirstOrDefault();

            if (Record != null)
            {
                _log.WriteLog("Old records found.. Deleting old records...");
                _UOW.SAMSiteDataRepository.DropAll(Record);
                _log.WriteLog("Old records deleted...");
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

        public override void LoadContent()
        {
            try
            {

                //LoadSAMDataFromApi();
                //GetCSVFileUrl();

                if (!IsPageLoaded())
                    throw new Exception("page is not loaded");

                _SAMSiteData.DataExtractionRequired = true;

                //
                //var FilePath = DownloadExclusionFile();
                var FilePath = DownloadExclusionFileThroughApi();
                
                DelteAllSAMSiteDataRecords();

                //LoadSAMDatafromCSV(FilePath); 
                //var FilePath = @"C:\Development\p926-ddas\DDAS.API\DataFiles\Downloads\SAM\SystemForAwardManagementPage_20211001100948.csv";
                LoadSAMDatafromApiCsv(FilePath);
                _SAMSiteData.DataExtractionSucceeded = true;
            }
            catch(Exception e)
            {
                //var ErrorCaptureFilePath = _config.ErrorScreenCaptureFolder +
                //    "SAM_" +
                //    DateTime.Now.ToString("dd MMM yyyy hh_mm")
                //    + ".jpeg";
                //SaveScreenShot(ErrorCaptureFilePath);
                
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
                    if (SearchTerms(NameToSearch))
                    {
                        while (CheckForAnchorTagNext())
                        {
                            LoadSAMList();
                            LoadNextSetOfRecords();
                        }
                        LoadSAMList();
                    }
                   
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
            DataAccessAnchorTag.Click();
            //DataAccessAnchorTag.SendKeys(Keys.Enter);
            var AnchorTitle = LatestExclusionExtractAnchorTag.GetAttribute("title");
            var Date = AnchorTitle.Replace("Active Exclusions Data File", "").Trim();

            DateTime LastUpdatedDate;

            DateTime.TryParseExact(Date,
                "M/d/yyyy",
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

                var IsDateParsed = DateTime.TryParseExact(PageLastUpdated, 
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
