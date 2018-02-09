using DDAS.Data.Mongo;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using DDAS.Models.Repository;

namespace DDAS.Services.Search
{
    public class ExtractData :IExtractData
    {
        private ISearchEngine _searchEngine;
        private IUnitOfWork _UOW;
        //public ExtractData(ISearchEngine SearchEngine)
        //{
        //    _searchEngine = SearchEngine;
        //}

        public ExtractData(ISearchEngine SearchEngine, IUnitOfWork UOW)
        {
            _searchEngine = SearchEngine;
            _UOW = UOW;
        }


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
    }
}
