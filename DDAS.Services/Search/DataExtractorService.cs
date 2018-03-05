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
            _searchEngine.ExtractData(siteEnum, log);
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

        public void ExtractThruShell(Int32 siteNumber)
        {
            string exePath = @"C:\Development\p926-ddas\DDAS.API\bin\DDAS.DataExtractor.exe";

            ProcessStartInfo processInfo = new ProcessStartInfo(exePath);
            processInfo.Arguments = siteNumber.ToString();
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            try
            {
                Process process = Process.Start(processInfo);
                process.WaitForExit();
                if (process.ExitCode != 0)
                { // success}
                    throw new Exception("Extractor did not complete the process");
                }
            }
            catch (Exception ex)
            {

                throw new Exception (ex.Message);
            }
        }

        public List<ExtractionStatus> GetLatestExtractionStatus()
        {
            var FDADebarSiteData = _UOW.FDADebarPageRepository.GetAll();

            var ListOfExtractionHistory = new List<ExtractionStatus>();

            var ExtractionHistory1 = new ExtractionStatus();

            if (FDADebarSiteData.Count > 0)
            {
                var SiteData = FDADebarSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory1,
                    SiteData, 1, SiteEnum.FDADebarPage);
            }

            var ExtractionHistory2 = new ExtractionStatus();

            var ClinicalInvestigatorSiteData =
                _UOW.ClinicalInvestigatorInspectionListRepository.GetAll();

            if (ClinicalInvestigatorSiteData.Count > 0)
            {
                var SiteData = ClinicalInvestigatorSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory2,
                    SiteData, 2, SiteEnum.ClinicalInvestigatorInspectionPage);
            }

            var ExtractionHistory3 = new ExtractionStatus();
            var FDAWarningLetters = _UOW.FDAWarningLettersRepository.GetAll();

            if (FDAWarningLetters.Count > 0)
            {
                var SiteData = FDAWarningLetters.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory3,
                    SiteData, 3, SiteEnum.FDAWarningLettersPage);
            }

            var ExtractionHistory4 = new ExtractionStatus();

            var ERRSiteData = _UOW.ERRProposalToDebarRepository.GetAll();

            if (ERRSiteData.Count > 0)
            {
                var SiteData = ERRSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory4,
                    SiteData, 4, SiteEnum.ERRProposalToDebarPage);
            }

            var ExtractionHistory5 = new ExtractionStatus();

            var AdequateAssuraceSiteData = _UOW.AdequateAssuranceListRepository.GetAll();

            if (AdequateAssuraceSiteData.Count > 0)
            {
                var SiteData = AdequateAssuraceSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(
                    ListOfExtractionHistory, ExtractionHistory5,
                    SiteData, 5, SiteEnum.AdequateAssuranceListPage);
            }

            var ExtractionHistory6 = new ExtractionStatus();

            var ClinicalInvestigatorDisqualificationData =
                _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll();

            if (ClinicalInvestigatorDisqualificationData.Count > 0)
            {
                var SiteData = ClinicalInvestigatorDisqualificationData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory6,
                    SiteData, 6, SiteEnum.ClinicalInvestigatorDisqualificationPage);
            }

            var ExtractionHistory7 = new ExtractionStatus();

            var PHSSiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll();

            if (PHSSiteData.Count > 0)
            {
                var SiteData = PHSSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory7,
                    SiteData, 7, SiteEnum.PHSAdministrativeActionListingPage);
            }

            var ExtractionHistory8 = new ExtractionStatus();

            var CBERSiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll();

            if (CBERSiteData.Count > 0)
            {
                var SiteData = CBERSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory8,
                    SiteData, 8, SiteEnum.CBERClinicalInvestigatorInspectionPage);
            }

            var ExtractionHistory9 = new ExtractionStatus();

            var ExclusionSiteData = _UOW.ExclusionDatabaseSearchRepository.GetAll();

            if (ExclusionSiteData.Count > 0)
            {
                var SiteData = ExclusionSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory9,
                    SiteData, 9, SiteEnum.ExclusionDatabaseSearchPage);
            }

            var ExtractionHistory10 = new ExtractionStatus();

            var CIASiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll();

            if (CIASiteData.Count > 0)
            {
                var SiteData = CIASiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory10,
                    SiteData, 10, SiteEnum.CorporateIntegrityAgreementsListPage);
            }

            var ExtractionHistory11 = new ExtractionStatus();

            var SamSiteData = _UOW.SystemForAwardManagementRepository.GetAll();

            if (SamSiteData.Count > 0)
            {
                var SiteData = SamSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory11,
                    SiteData, 11, SiteEnum.SystemForAwardManagementPage);
            }

            var ExtractionHistory12 = new ExtractionStatus();

            var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetAll();

            if (SDNSiteData.Count > 0)
            {
                var SiteData = SDNSiteData.OrderByDescending(
                    x => x.CreatedOn)
                    .First();

                AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory12,
                    SiteData, 12, SiteEnum.SpeciallyDesignedNationalsListPage);
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
            ExtractionHistory.ExtractionDate = SiteData.CreatedOn;
            if (SiteData.DataExtractionErrorMessage != null)
            {
                ExtractionHistory.ErrorDescription =
                    SiteData.DataExtractionErrorMessage;
            }
            else if (!SiteData.DataExtractionRequired)
                ExtractionHistory.ExtractionMessage = "Source Date is not updated";
            else
                ExtractionHistory.ExtractionMessage = "Data extracted successfully";

            ExtractionHistory.SiteLastUpdatedOn =
                SiteData.SiteLastUpdatedOn;

            list.Add(ExtractionHistory);
        }

        public IEnumerable<string> GetSitesWhereDataExtractionEarlierThan(int Hour = 32)
        {
            return GetLatestExtractionStatus()
                .Where(x => x.ExtractionDate < DateTime.Now.AddHours(-Hour))
                .Select(x => x.SiteName + " (" + x.SiteNumber + ")");
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
                "*.zip", "*.xls", "*.csv", "*.zip", "*.txt"
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
            return DownloadDataFilesVMList;
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

        #endregion
    }
}
