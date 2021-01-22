using DDAS.Data.Mongo;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using DDAS.Models.Repository;
using DDAS.Models.Entities.Domain.SiteData;
using System.IO;
using DDAS.Models.ViewModels;
using System.Diagnostics;

namespace DDAS.Services.Search
{
    public class DataExtractorService :IDataExtractorService
    {
        private ISearchEngine _searchEngine;
        private IUnitOfWork _UOW;
        private IConfig _config;
        //public ExtractData(ISearchEngine SearchEngine)
        //{
        //    _searchEngine = SearchEngine;
        //}

        public DataExtractorService(ISearchEngine SearchEngine, IUnitOfWork UOW)
        {
            _searchEngine = SearchEngine;
            _UOW = UOW;
            _config = _searchEngine.Config; //config info required for GetDataFile Functin.

        }

        //public ExtractDataService(ISearchEngine SearchEngine, IUnitOfWork UOW, IConfig Config)
        //{
        //    _searchEngine = SearchEngine;
        //    _UOW = UOW;
        //    _config = Config;
        //}

        //Currently ExtractData is only for DB sites

        public void ExtractDataSingleSite(SiteEnum siteEnum, ILog log)
        {
            try
            {
                _searchEngine.ExtractData(siteEnum, log);
            }
            catch(Exception e)
            {
                var NewLog = new Log();
                NewLog.Step = "";
                NewLog.SiteEnumString = siteEnum.ToString();
                NewLog.Status = "Error";
                NewLog.Message = "Unable to extract data. " +
                    "Error Details: " + e.ToString();
                NewLog.CreatedOn = DateTime.Now;
                NewLog.CreatedBy = "DataExtractorService";

                log.WriteLog(NewLog);

                throw new Exception(e.ToString());
            }
        }

        public void ExtractDataAllDBSites(List<SitesToSearch> Sites, ILog log)
        {
            _searchEngine.ExtractData(Sites, log);
        }

        public void ExtractDataSingleSite(SiteEnum siteEnum, string userName)
        {

            //


            //--------------------''
            DBLog _WriteLog;
            _WriteLog = new DBLog(_UOW, "DDAS.Extractor", true);
            _WriteLog.LogStart();

            var NewLog = new Log();
            NewLog.CreatedBy = "Test";
            NewLog.Message = "Execution Started by:" + userName;
            NewLog.SiteEnumString = siteEnum.ToString();
            NewLog.Step = "Start";
            NewLog.Status = NewLog.Step;
            NewLog.CreatedOn = DateTime.Now;

            _WriteLog.WriteLog(NewLog);

            try
            {
                _searchEngine.ExtractData(siteEnum, _WriteLog);
                _WriteLog.WriteLog(DateTime.Now.ToString(), "Extract Data ends");
            }
            catch (Exception e)
            {
                NewLog = new Log();
                NewLog.CreatedBy = "DDAS.Extractor";
                NewLog.Step = "";
                NewLog.Status = "Error";
                NewLog.Message = "Unable to complete the data extract. Error Details: " +
                    e.ToString();
                NewLog.CreatedOn = DateTime.Now;

                _WriteLog.WriteLog(NewLog);
                throw new Exception("Error logged");
            }
            finally
            {
                _WriteLog.LogEnd();
                _WriteLog.WriteLog("===============================");

                //Process currentProcess = Process.GetCurrentProcess();
                //currentProcess.CloseMainWindow();
            }
        }

        public void ExtractThruShell(int siteNumber, string ExePath)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo(ExePath);
            processInfo.Arguments = siteNumber.ToString();
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.Arguments = siteNumber.ToString();
            //processInfo.RedirectStandardOutput = true;
            //processInfo.RedirectStandardError = true;
            try
            {
                Process process = Process.Start(processInfo);
                process.WaitForExit();
                //var Output = process.StandardOutput.ReadToEnd();
                //var Error = process.StandardError.ReadToEnd();
                if (process.ExitCode != 0)
                { // success}
                    throw new Exception("Extractor did not complete the process");
                }
                process.Close();
            }
            catch (Exception ex)
            {
                throw new Exception (ex.Message);
            }
        }

        public List<ExtractionStatus> GetLatestExtractionStatus(DateTime fromDate, DateTime toDate)
        {

            //var FDADebarSiteData = _UOW.FDADebarPageRepository.GetAll();

            var ListOfExtractionHistory = new List<ExtractionStatus>();

            var ExtractionHistory1 = new ExtractionStatus();
            var FDADebarSiteData = _UOW.FDADebarPageRepository.FilterRecordsByDate(new FDADebarPageSiteData(), fromDate, toDate);
            if (FDADebarSiteData.Count > 0)
            {
                var SiteData = FDADebarSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory1,
                    SiteData, 1, SiteEnum.FDADebarPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory1,
                    null, 1, SiteEnum.FDADebarPage);
            }

            var ExtractionHistory2 = new ExtractionStatus();
            var ClinicalInvestigatorSiteData =
                _UOW.ClinicalInvestigatorInspectionListRepository.FilterRecordsByDate(new ClinicalInvestigatorInspectionSiteData(), fromDate, toDate);

            if (ClinicalInvestigatorSiteData.Count > 0)
            {
                var SiteData = ClinicalInvestigatorSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory2,
                    SiteData, 2, SiteEnum.ClinicalInvestigatorInspectionPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory2,
                    null, 2, SiteEnum.ClinicalInvestigatorInspectionPage);
            }

            var ExtractionHistory3 = new ExtractionStatus();

            var FDAWarningLetters = _UOW.FDAWarningLettersRepository.FilterRecordsByDate(
                new FDAWarningLettersSiteData(), fromDate, toDate);

            if (FDAWarningLetters.Count > 0)
            {
                var SiteData = FDAWarningLetters.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory3,
                    SiteData, 3, SiteEnum.FDAWarningLettersPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory3,
                    null, 3, SiteEnum.FDAWarningLettersPage);
            }

            var ExtractionHistory4 = new ExtractionStatus();

            var ERRSiteData = _UOW.ERRProposalToDebarRepository.FilterRecordsByDate(
                new ERRProposalToDebarPageSiteData(), fromDate, toDate);


            if (ERRSiteData.Count > 0)
            {
                var SiteData = ERRSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory4,
                    SiteData, 4, SiteEnum.ERRProposalToDebarPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory4,
                    null, 4, SiteEnum.ERRProposalToDebarPage);
            }

            var ExtractionHistory5 = new ExtractionStatus();

            //AdequateAssuranceList site no longer used
            //Version 1.1.5
            //var AdequateAssuraceSiteData = _UOW.AdequateAssuranceListRepository.FilterRecordsByDate(
            //    new AdequateAssuranceListSiteData(), fromDate, toDate);

            //if (AdequateAssuraceSiteData.Count > 0)
            //{
            //    var SiteData = AdequateAssuraceSiteData.OrderByDescending(
            //        x => x.CreatedOn)
            //        .First();

            //    AddToExtractionHistoryList(
            //        ListOfExtractionHistory, ExtractionHistory5,
            //        SiteData, 5, SiteEnum.AdequateAssuranceListPage);
            //}

            var ExtractionHistory6 = new ExtractionStatus();

            var ClinicalInvestigatorDisqualificationData =
                _UOW.ClinicalInvestigatorDisqualificationRepository.FilterRecordsByDate(
                    new ClinicalInvestigatorDisqualificationSiteData(), fromDate, toDate);

            if (ClinicalInvestigatorDisqualificationData.Count > 0)
            {
                var SiteData = ClinicalInvestigatorDisqualificationData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory6,
                    SiteData, 6, SiteEnum.ClinicalInvestigatorDisqualificationPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory6,
                    null, 6, SiteEnum.ClinicalInvestigatorDisqualificationPage);
            }

            var ExtractionHistory7 = new ExtractionStatus();

            var PHSSiteData = _UOW.PHSAdministrativeActionListingRepository.FilterRecordsByDate(
                new PHSAdministrativeActionListingSiteData(), fromDate, toDate);

            if (PHSSiteData.Count > 0)
            {
                var SiteData = PHSSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory7,
                    SiteData, 7, SiteEnum.PHSAdministrativeActionListingPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory7,
                    null, 7, SiteEnum.PHSAdministrativeActionListingPage);
            }

            var ExtractionHistory8 = new ExtractionStatus();

            var CBERSiteData = _UOW.CBERClinicalInvestigatorRepository.FilterRecordsByDate(
                new CBERClinicalInvestigatorInspectionSiteData(), fromDate, toDate);

            if (CBERSiteData.Count > 0)
            {
                var SiteData = CBERSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory8,
                    SiteData, 8, SiteEnum.CBERClinicalInvestigatorInspectionPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory8,
                    null, 8, SiteEnum.CBERClinicalInvestigatorInspectionPage);
            }

            var ExtractionHistory9 = new ExtractionStatus();

            var ExclusionSiteData = _UOW.ExclusionDatabaseSearchRepository.FilterRecordsByDate(new ExclusionDatabaseSearchPageSiteData(), fromDate, toDate);

            if (ExclusionSiteData.Count > 0)
            {
                var SiteData = ExclusionSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory9,
                    SiteData, 9, SiteEnum.ExclusionDatabaseSearchPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory9,
                    null, 9, SiteEnum.ExclusionDatabaseSearchPage);
            }

            var ExtractionHistory10 = new ExtractionStatus();

            var CIASiteData = _UOW.CorporateIntegrityAgreementRepository.FilterRecordsByDate(
                new CorporateIntegrityAgreementListSiteData(), fromDate, toDate);

            if (CIASiteData.Count > 0)
            {
                var SiteData = CIASiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory10,
                    SiteData, 10, SiteEnum.CorporateIntegrityAgreementsListPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory10,
                    null, 10, SiteEnum.CorporateIntegrityAgreementsListPage);
            }
            var ExtractionHistory11 = new ExtractionStatus();

            var SamSiteData = _UOW.SystemForAwardManagementRepository.FilterRecordsByDate(
                new SystemForAwardManagementPageSiteData(), fromDate, toDate);

            if (SamSiteData.Count > 0)
            {
                var SiteData = SamSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory11,
                    SiteData, 11, SiteEnum.SystemForAwardManagementPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory11,
                    null, 11, SiteEnum.SystemForAwardManagementPage);
            }
            var ExtractionHistory12 = new ExtractionStatus();

            var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.FilterRecordsByDate(new SpeciallyDesignatedNationalsListSiteData(), fromDate, toDate);

            if (SDNSiteData.Count > 0)
            {
                var SiteData = SDNSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory12,
                    SiteData, 12, SiteEnum.SpeciallyDesignedNationalsListPage);
            }
            else
            {
                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory12,
                    null, 12, SiteEnum.SpeciallyDesignedNationalsListPage);
            }
            return ListOfExtractionHistory;
        }

        private void AddToExtractionHistoryList(
            List<ExtractionStatus> list,
            ExtractionStatus ExtractionHistory,
            BaseSiteData SiteData,
            int SiteNumber,
            SiteEnum siteEnum)
        {
            ExtractionHistory.SiteNumber = SiteNumber;
            ExtractionHistory.SiteName = siteEnum.ToString();
            ExtractionHistory.Enum = siteEnum;
            if (SiteData != null)
            {
                ExtractionHistory.ExtractionDate = SiteData.CreatedOn;
                

                if (SiteData.DataExtractionErrorMessage != null)
                {
                    ExtractionHistory.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                    ExtractionHistory.ExtractionMessage = "Source Date is not updated";
                else
                {
                    ExtractionHistory.ExtractionMessage = "Data extracted successfully";
                }
                    

                ExtractionHistory.SiteLastUpdatedOn =
                    SiteData.SiteLastUpdatedOn;
            }
            else
            {
                ExtractionHistory.ErrorDescription = "Data not extracted";
            }
            

            list.Add(ExtractionHistory);
        }

        public IEnumerable<string> GetSitesWhereDataExtractionEarlierThan(int Hour = 32)
        {
            var fromDate = DateTime.Now.AddMonths(-6);
            var toDate = DateTime.Now.AddDays(1).Date;

            var sites = GetLatestExtractionStatus(fromDate, toDate);

            var dataExtractedInLast32Hours = 
                sites.Where(x => x.ExtractionDate > DateTime.Now.AddHours(-Hour)).ToList();

            var sitesWithNoDataExtractedIn32Hours = 
                sites.Except(dataExtractedInLast32Hours)
                .Select(x => x.SiteName + " (" + x.SiteNumber + ")");

            return sitesWithNoDataExtractedIn32Hours;
            
        }


        #region Download Data Files

        public List<DownloadDataFilesViewModel> GetDataFiles(int SiteEnum)
        {
            bool IsFilteredBySiteEnum = false;

            var DownloadDataFilesVMList = new List<DownloadDataFilesViewModel>();

            var DataFolders = new string[] {
                _config.CIILFolder,
                _config.FDAWarningLettersFolder,
                _config.ExclusionDatabaseFolder,
                _config.SAMFolder,
                _config.SDNFolder
            };

            var FileTypes = new string[] {
                "*.zip", "*.json",  "*.csv", "*.zip", "*.txt"
                //"*.zip", "*.json", "*.xls", "*.csv", "*.zip", "*.txt"
            };

            int Index = 0;
            foreach (string Folder in DataFolders)
            {
                if (IsFilteredBySiteEnum)
                    break;

                var Files = GetDataFiles(Folder, FileTypes[Index]);

                Files.ForEach(fileInfo =>
                {
                    if (_UOW.SiteSourceRepository.GetAll().Find(
                        x => (int)x.SiteEnum == SiteEnum) != null)
                    {
                        var VM = new DownloadDataFilesViewModel();
                        VM.FileName = fileInfo.Name;
                        var siteEnum = VM.FileName.Split('_')[0];

                        var Site = _UOW.SiteSourceRepository.GetAll().Find(
                        x => (int)x.SiteEnum == SiteEnum &&
                        x.SiteEnum.ToString() == siteEnum);

                        if (Site != null)
                        {
                            VM.SiteName = Site.SiteName;
                            VM.FullPath = Folder + VM.FileName;
                            VM.FileSize = (fileInfo.Length / 1024) //bytes to KB
                            .ToString();
                            VM.DownloadedOn = fileInfo.CreationTime;
                            VM.FileType = fileInfo.Extension;
                            DownloadDataFilesVMList.Add(VM);
                            IsFilteredBySiteEnum = true;
                        }
                    }
                });
                Index += 1;
            }
            return DownloadDataFilesVMList.OrderByDescending(x => x.DownloadedOn).ToList();
        }

        private List<FileInfo> GetDataFiles(string Folder, string FileType)
        {
            var Files = new DirectoryInfo(Folder).GetFiles(FileType);

            var AllFiles = new List<FileInfo>();
            foreach (FileInfo fileInfo in Files)
            {
                AllFiles.Add(fileInfo);
            }
            return AllFiles;
        }

        #endregion

        #region getExtractedData

        //Site No: 1
        public FDADebarPageSiteData GetFDADebarPageSiteData()
        {
            var FDADebarredPage = _UOW.FDADebarPageRepository.
                GetAll()
                .OrderByDescending(x => x.CreatedOn).First();
            var Data = FDADebarredPage;
            return Data;
        }

        //Site No: 1
        public List<FDAWarningLetter> GetFDAWarningLetterSiteData()
        {
            var FDAWarningLetterPage = _UOW.FDAWarningRepository.
                GetAll();
            var Data = FDAWarningLetterPage;
            return Data;
        }



        //Site No: 4
        public ERRProposalToDebarPageSiteData GetERRProposalToDebarPageSiteData()
        {
            return  _UOW.ERRProposalToDebarRepository.
                GetAll()
                .OrderByDescending(x => x.CreatedOn).First();
        }

        //Site No: 5
        public AdequateAssuranceListSiteData GetAdequateAssuranceListSiteData()
        {
            return _UOW.AdequateAssuranceListRepository.
                GetAll()
                .OrderByDescending(x => x.CreatedOn).First();
        }

        //Site No: 6
        public ClinicalInvestigatorDisqualificationSiteData GetClinicalInvestigatorDisqualificationSiteData()
        {
            return _UOW.ClinicalInvestigatorDisqualificationRepository.
                GetAll()
                .OrderByDescending(x => x.CreatedOn).First();
        }

        
        

        //Site No: 7
        public PHSAdministrativeActionListingSiteData GetPHSAdministrativeActionListingSiteData()
        {
            return _UOW.PHSAdministrativeActionListingRepository.
                GetAll()
                .OrderByDescending(x => x.CreatedOn).First();
        }

        //Site No: 8
        public CBERClinicalInvestigatorInspectionSiteData GetCBERClinicalInvestigatorInspectionSiteData()
        {
            return _UOW.CBERClinicalInvestigatorRepository.
                GetAll()
                .OrderByDescending(x => x.CreatedOn).First();
        }



        //Site No: 10
        public CorporateIntegrityAgreementListSiteData GetCorporateIntegrityAgreementListSiteData()
        {
            return _UOW.CorporateIntegrityAgreementRepository.
                GetAll()
                .OrderByDescending(x => x.CreatedOn).First();
        }


        //Site No: ? added on 20Apr2020 - Patrick
        public ClinicalInvestigatorInspectionSiteData GetClinicalInvestigatorInspectionSiteData()
        {
            var doc = _UOW.ClinicalInvestigatorInspectionListRepository.GetLatestDocument();
            doc.ClinicalInvestigatorInspectionList = _UOW.ClinicalInvestigatorInspectionRepository.GetAll();
            return doc;
        }

        //Site No: ? added on 20Apr2020 - Patrick
        public ExclusionDatabaseSearchPageSiteData GetExclusionDatabaseSearchPageSiteData()
        {
            var doc = _UOW.ExclusionDatabaseSearchRepository.GetLatestDocument();
            doc.ExclusionSearchList= _UOW.ExclusionDatabaseRepository.GetAll();
            //doc.ExclusionSearchList = _UOW.ExclusionDatabaseRepository.GetAll().Take(10).ToList();
            return doc;
        }

        
        //Site No: ? added on 20Apr2020 - Patrick
        public SpeciallyDesignatedNationalsListSiteData GetSpeciallyDesignatedNationalsSiteData()
        {
            var doc = _UOW.SpeciallyDesignatedNationalsRepository.GetLatestDocument();
            doc.SDNListSiteData = _UOW.SDNSiteDataRepository.GetAll();
            return doc;
        }

        //Site No: ? added on 20Apr2020 - Patrick
        public SystemForAwardManagementPageSiteData GetSystemForAwardManagementPageSiteData()
        {
            var doc = _UOW.SystemForAwardManagementRepository.GetLatestDocument();
            doc.SAMSiteData = _UOW.SAMSiteDataRepository.GetAll();
            return doc;
        }
        

        #endregion
    }
}
