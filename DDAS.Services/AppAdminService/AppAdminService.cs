using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DDAS.Services.LiveScan;
using System.Diagnostics;
using System.ComponentModel;
using DDAS.Models.ViewModels;
using DDAS.Models.Entities;
using System.IO;
using DDAS.Models.Repository;

namespace DDAS.Services.AppAdminService
{
    public class AppAdminService : IAppAdminService
    {
        private IUnitOfWork _UOW;
        private IConfig _config;

        private string _LiveSiteScannerExeName = "DDAS.LiveSiteExtractor";
        public AppAdminService(IUnitOfWork UOW, IConfig config)
        {
            _UOW = UOW;
            _config = config;
        }

        //public List<CBERClinicalInvestigator> GetCBERData()
        //{
        //    var CBERData = _UOW.CBERClinicalInvestigatorRepository.
        //        GetAll()
        //        .OrderByDescending(x => x.CreatedOn).Last();

        //    var Data = CBERData.ClinicalInvestigator;
        //    return Data;
        //}

        //public List<DataExtractionHistory> GetDataExtractionHistory()
        //{
        //    var FDADebarSiteData = _UOW.FDADebarPageRepository.GetAll();

        //    var ListOfExtractionHistory = new List<DataExtractionHistory>();

        //    var ExtractionHistory1 = new DataExtractionHistory();

        //    if (FDADebarSiteData.Count > 0)
        //    {
        //        var SiteData = FDADebarSiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(
        //            ListOfExtractionHistory, ExtractionHistory1,
        //            SiteData, 1, SiteEnum.FDADebarPage);
        //    }

        //    var ExtractionHistory2 = new DataExtractionHistory();

        //    var ClinicalInvestigatorSiteData =
        //        _UOW.ClinicalInvestigatorInspectionListRepository.GetAll();

        //    if (ClinicalInvestigatorSiteData.Count > 0)
        //    {
        //        var SiteData = ClinicalInvestigatorSiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(
        //            ListOfExtractionHistory, ExtractionHistory2,
        //            SiteData, 2, SiteEnum.ClinicalInvestigatorInspectionPage);
        //    }

        //    var ExtractionHistory3 = new DataExtractionHistory();
        //    var FDAWarningLetters = _UOW.FDAWarningLettersRepository.GetAll();

        //    if (FDAWarningLetters.Count > 0)
        //    {
        //        var SiteData = FDAWarningLetters.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(
        //            ListOfExtractionHistory, ExtractionHistory3,
        //            SiteData, 3, SiteEnum.FDAWarningLettersPage);
        //    }

        //    var ExtractionHistory4 = new DataExtractionHistory();

        //    var ERRSiteData = _UOW.ERRProposalToDebarRepository.GetAll();

        //    if (ERRSiteData.Count > 0)
        //    {
        //        var SiteData = ERRSiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(
        //            ListOfExtractionHistory, ExtractionHistory4,
        //            SiteData, 4, SiteEnum.ERRProposalToDebarPage);
        //    }

        //    var ExtractionHistory5 = new DataExtractionHistory();

        //    var AdequateAssuraceSiteData = _UOW.AdequateAssuranceListRepository.GetAll();

        //    if (AdequateAssuraceSiteData.Count > 0)
        //    {
        //        var SiteData = AdequateAssuraceSiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(
        //            ListOfExtractionHistory, ExtractionHistory5,
        //            SiteData, 5, SiteEnum.AdequateAssuranceListPage);
        //    }

        //    var ExtractionHistory6 = new DataExtractionHistory();

        //    var ClinicalInvestigatorDisqualificationData =
        //        _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll();

        //    if (ClinicalInvestigatorDisqualificationData.Count > 0)
        //    {
        //        var SiteData = ClinicalInvestigatorDisqualificationData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory6,
        //            SiteData, 6, SiteEnum.ClinicalInvestigatorDisqualificationPage);
        //    }

        //    var ExtractionHistory7 = new DataExtractionHistory();

        //    var PHSSiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll();

        //    if (PHSSiteData.Count > 0)
        //    {
        //        var SiteData = PHSSiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory7,
        //            SiteData, 7, SiteEnum.PHSAdministrativeActionListingPage);
        //    }

        //    var ExtractionHistory8 = new DataExtractionHistory();

        //    var CBERSiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll();

        //    if (CBERSiteData.Count > 0)
        //    {
        //        var SiteData = CBERSiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory8,
        //            SiteData, 8, SiteEnum.CBERClinicalInvestigatorInspectionPage);
        //    }

        //    var ExtractionHistory9 = new DataExtractionHistory();

        //    var ExclusionSiteData = _UOW.ExclusionDatabaseSearchRepository.GetAll();

        //    if (ExclusionSiteData.Count > 0)
        //    {
        //        var SiteData = ExclusionSiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory9,
        //            SiteData, 9, SiteEnum.ExclusionDatabaseSearchPage);
        //    }

        //    var ExtractionHistory10 = new DataExtractionHistory();

        //    var CIASiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll();

        //    if (CIASiteData.Count > 0)
        //    {
        //        var SiteData = CIASiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory10,
        //            SiteData, 10, SiteEnum.CorporateIntegrityAgreementsListPage);
        //    }

        //    var ExtractionHistory11 = new DataExtractionHistory();

        //    var SamSiteData = _UOW.SystemForAwardManagementRepository.GetAll();

        //    if (SamSiteData.Count > 0)
        //    {
        //        var SiteData = SamSiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory11,
        //            SiteData, 11, SiteEnum.SystemForAwardManagementPage);
        //    }

        //    var ExtractionHistory12 = new DataExtractionHistory();

        //    var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetAll();

        //    if (SDNSiteData.Count > 0)
        //    {
        //        var SiteData = SDNSiteData.OrderByDescending(
        //            x => x.CreatedOn)
        //            .First();

        //        AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory12,
        //            SiteData, 12, SiteEnum.SpeciallyDesignedNationalsListPage);
        //    }

        //    return ListOfExtractionHistory;
        //}

        //public IEnumerable<string> GetSitesWhereDataExtractionEarlierThan(int Hour = 32)
        //{
        //    return GetDataExtractionHistory()
        //        .Where(x => x.ExtractionDate <= DateTime.Now.AddHours(-Hour))
        //        .Select(x => x.SiteName + " (" +  x.SiteNumber + ")");
        //}

        //private void AddToExtractionHistoryList(
        //    List<DataExtractionHistory> list,
        //    DataExtractionHistory ExtractionHistory,
        //    BaseSiteData SiteData,
        //    int SiteNumber,
        //    SiteEnum siteEnum)
        //{
        //    ExtractionHistory.SiteNumber = SiteNumber;
        //    ExtractionHistory.SiteName = siteEnum.ToString();
        //    ExtractionHistory.Enum = siteEnum;
        //    ExtractionHistory.ExtractionDate = SiteData.CreatedOn;
        //    if (SiteData.DataExtractionErrorMessage != null)
        //    {
        //        ExtractionHistory.ErrorDescription =
        //            SiteData.DataExtractionErrorMessage;
        //    }
        //    else if (!SiteData.DataExtractionRequired)
        //        ExtractionHistory.ExtractionMessage = "Source Date is not updated";
        //    else
        //        ExtractionHistory.ExtractionMessage = "Data extracted successfully";

        //    ExtractionHistory.SiteLastUpdatedOn =
        //        SiteData.SiteLastUpdatedOn;

        //    list.Add(ExtractionHistory);
        //}

        #region GetDataExtractionPerSite
        public List<ExtractionStatus> GetDataExtractionPerSite(SiteEnum Enum)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarRepository();

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetCiilRepository();

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLetters();

                case SiteEnum.ERRProposalToDebarPage:
                    return GetErrProposalToDebarRepository();

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssuranceRepository();

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetClinicalInvestigatorDisqualificationRepository();

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERRepository();

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSRepository();

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabaseRepository();

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIARepository();

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSAMRepository();

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSDNRepository();

                default: throw new Exception("Invalid Enum");
            }
        }

        private List<ExtractionStatus> GetFDADebarRepository()
        {
            var Data = _UOW.FDADebarPageRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var FDASiteData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach(FDADebarPageSiteData SiteData in FDASiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetCiilRepository()
        {
            var Data = _UOW.ClinicalInvestigatorInspectionListRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var CIILSiteData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (ClinicalInvestigatorInspectionSiteData SiteData in CIILSiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }
        
        private List<ExtractionStatus> GetFDAWarningLetters()
        {
            var Data = _UOW.FDAWarningLettersRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var FDAWarningLetterData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (FDAWarningLettersSiteData SiteData in FDAWarningLetterData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetErrProposalToDebarRepository()
        {
            var Data = _UOW.ERRProposalToDebarRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var ERRSiteData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (ERRProposalToDebarPageSiteData SiteData in ERRSiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetAdequateAssuranceRepository()
        {
            var Data = _UOW.AdequateAssuranceListRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var AdequateAssuranceSiteData = 
                Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (AdequateAssuranceListSiteData SiteData in AdequateAssuranceSiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetClinicalInvestigatorDisqualificationRepository()
        {
            var Data = _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var ClinicalInvestigatorDisqualificationData = 
                Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (ClinicalInvestigatorDisqualificationSiteData SiteData in ClinicalInvestigatorDisqualificationData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetCBERRepository()
        {
            var Data = _UOW.CBERClinicalInvestigatorRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var CBERSiteData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (CBERClinicalInvestigatorInspectionSiteData SiteData in CBERSiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetPHSRepository()
        {
            var Data = _UOW.PHSAdministrativeActionListingRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var PHSSiteData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (PHSAdministrativeActionListingSiteData SiteData in PHSSiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetExclusionDatabaseRepository()
        {
            var Data = _UOW.ExclusionDatabaseSearchRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var ExclusionSiteData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (ExclusionDatabaseSearchPageSiteData SiteData in ExclusionSiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetCIARepository()
        {
            var Data = _UOW.CorporateIntegrityAgreementRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var CIASiteData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (CorporateIntegrityAgreementListSiteData SiteData in CIASiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetSAMRepository()
        {
            var Data = _UOW.SystemForAwardManagementRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var SAMSiteData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (SystemForAwardManagementPageSiteData SiteData in SAMSiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }

        private List<ExtractionStatus> GetSDNRepository()
        {
            var Data = _UOW.SpeciallyDesignatedNationalsRepository.GetAll();

            if (Data.Count == 0)
                return null;

            var SDNSiteData = Data.OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<ExtractionStatus>();

            foreach (SpeciallyDesignatedNationalsListSiteData SiteData in SDNSiteData)
            {
                var DataExtraction = new ExtractionStatus();

                DataExtraction.ExtractionDate = SiteData.CreatedOn;
                if (SiteData.DataExtractionErrorMessage != null)
                {
                    DataExtraction.ErrorDescription =
                        SiteData.DataExtractionErrorMessage;
                }
                else if (!SiteData.DataExtractionRequired)
                {
                    DataExtraction.ExtractionMessage = "Source Date is not updated";
                }
                else
                    DataExtraction.ExtractionMessage = "Data extracted successfully";

                DataExtraction.RecId = SiteData.RecId;
                DataExtraction.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;

                DataExtractionList.Add(DataExtraction);
            }
            return DataExtractionList;
        }
        #endregion

        #region DeleteExtractionEntry

        public void DeleteExtractionEntry(SiteEnum Enum, Guid? RecId)
        {
            switch (Enum)
            {
                case SiteEnum.FDADebarPage:
                    DeleteFDADebarExtractionEntry(RecId);
                    return;

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    DeleteCIILExtractionEntry(RecId);
                    return;

                case SiteEnum.FDAWarningLettersPage:
                    DeleteFDAWarningExtractionEntry(RecId);
                    return;

                case SiteEnum.ERRProposalToDebarPage:
                    DeleteERRProposalToDebarExtractionEntry(RecId);
                    return;

                case SiteEnum.AdequateAssuranceListPage:
                    DeleteAdequateAssuranceExtractionEntry(RecId);
                    return;

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    DeleteDisqualificationExtractionEntry(RecId);
                    return;

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    DeleteCBERExtractionEntry(RecId);
                    return;

                case SiteEnum.PHSAdministrativeActionListingPage:
                    DeletePHSExtractionEntry(RecId);
                    return;

                case SiteEnum.ExclusionDatabaseSearchPage:
                    DeleteExclusionDatabaseExtractionEntry(RecId);
                    return;

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    DeleteCIAExtractionEntry(RecId);
                    return;

                case SiteEnum.SystemForAwardManagementPage:
                    DeleteSAMExtractionEntry(RecId);
                    return;

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    DeleteSDNExtractionEntry(RecId);
                    return;

                default: throw new Exception("Invalid Enum");
            }
        }

        private void DeleteFDADebarExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = _UOW.FDADebarPageRepository.FindById(RecId);

            //Remove all documents referecing the CurrentDocument, if any
            if(CurrentDocument.DataExtractionSucceeded)
            {
                var FDASiteData = _UOW.FDADebarPageRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (FDADebarPageSiteData SiteData in FDASiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.FDADebarPageRepository.RemoveById(SiteData.RecId);
                }
            }
            //Remove the currentdocument
            _UOW.FDADebarPageRepository.RemoveById(RecId);
        }

        private void DeleteCIILExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = 
                _UOW.ClinicalInvestigatorInspectionListRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var CIILSiteData = _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (ClinicalInvestigatorInspectionSiteData SiteData in CIILSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.ClinicalInvestigatorInspectionListRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.ClinicalInvestigatorInspectionListRepository.RemoveById(RecId);
        }

        private void DeleteFDAWarningExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = 
                _UOW.FDAWarningLettersRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var FDAWarningSiteData = _UOW.FDAWarningLettersRepository.GetAll()
                    .OrderByDescending(x => x.CreatedOn)
                    .ToList();
                foreach (FDAWarningLettersSiteData SiteData in FDAWarningSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.FDAWarningLettersRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.FDAWarningLettersRepository.RemoveById(RecId);
        }

        private void DeleteERRProposalToDebarExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = _UOW.ERRProposalToDebarRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var ERRSiteData = _UOW.ERRProposalToDebarRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (ERRProposalToDebarPageSiteData SiteData in ERRSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.ERRProposalToDebarRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.ERRProposalToDebarRepository.RemoveById(RecId);
        }

        private void DeleteAdequateAssuranceExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = _UOW.AdequateAssuranceListRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var AdequateAssuranceSiteData = _UOW.AdequateAssuranceListRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (AdequateAssuranceListSiteData SiteData in AdequateAssuranceSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.AdequateAssuranceListRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.AdequateAssuranceListRepository.RemoveById(RecId);
        }

        private void DeleteDisqualificationExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = 
                _UOW.ClinicalInvestigatorDisqualificationRepository.FindById(RecId);

            //Remove all documents referecing the CurrentDocument, if any
            if (CurrentDocument.DataExtractionSucceeded)
            {
                var DisqualificationSiteData = 
                    _UOW.ClinicalInvestigatorDisqualificationRepository
                    .GetAll()
                    .OrderByDescending(x => x.CreatedOn)
                    .ToList();

                foreach (ClinicalInvestigatorDisqualificationSiteData SiteData 
                    in DisqualificationSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.ClinicalInvestigatorDisqualificationRepository.RemoveById(SiteData.RecId);
                }
            }
            //Remove the currentdocument
            _UOW.ClinicalInvestigatorDisqualificationRepository.RemoveById(RecId);
        }

        private void DeleteCBERExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = _UOW.CBERClinicalInvestigatorRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var CBERSiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (CBERClinicalInvestigatorInspectionSiteData SiteData in CBERSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.CBERClinicalInvestigatorRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.CBERClinicalInvestigatorRepository.RemoveById(RecId);

        }

        private void DeletePHSExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = _UOW.PHSAdministrativeActionListingRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var PHSSiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (PHSAdministrativeActionListingSiteData SiteData in PHSSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.PHSAdministrativeActionListingRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.PHSAdministrativeActionListingRepository.RemoveById(RecId);
        }

        private void DeleteExclusionDatabaseExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = _UOW.ExclusionDatabaseSearchRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var ExclusionSiteData = _UOW.ExclusionDatabaseSearchRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (ExclusionDatabaseSearchPageSiteData SiteData in ExclusionSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.ERRProposalToDebarRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.ExclusionDatabaseSearchRepository.RemoveById(RecId);
        }

        private void DeleteCIAExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = _UOW.CorporateIntegrityAgreementRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var CIASiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (CorporateIntegrityAgreementListSiteData SiteData in CIASiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.CorporateIntegrityAgreementRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.CorporateIntegrityAgreementRepository.RemoveById(RecId);
        }

        private void DeleteSAMExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = 
                _UOW.SystemForAwardManagementRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var SDNSiteData = _UOW.SystemForAwardManagementRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (SystemForAwardManagementPageSiteData SiteData in SDNSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.SystemForAwardManagementRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.SystemForAwardManagementRepository.RemoveById(RecId);
        }

        private void DeleteSDNExtractionEntry(Guid? RecId)
        {
            var CurrentDocument = _UOW.SpeciallyDesignatedNationalsRepository.FindById(RecId);

            if (CurrentDocument.DataExtractionSucceeded)
            {
                var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).ToList();

                foreach (SpeciallyDesignatedNationalsListSiteData SiteData in SDNSiteData)
                {
                    if (SiteData.ReferenceId == RecId)
                        _UOW.SpeciallyDesignatedNationalsRepository.RemoveById(SiteData.RecId);
                }
            }
            _UOW.SpeciallyDesignatedNationalsRepository.RemoveById(RecId);
        }
        #endregion

        #region LiveScanner
        public bool LaunchLiveScanner(string exeFolder)
        {
            var launcher = new LiveScanLauncher();
            var QueueNumber = getMaxProcessNumber() + 1;
            launcher.LaunchLiveScanner(exeFolder, QueueNumber);
                return true;
        }

        public LiveSiteScannerMemoryModel LiveScannerInfo()
        {
            var retValue = new LiveSiteScannerMemoryModel();
            
            System.Diagnostics.Process[] processes =  System.Diagnostics.Process.GetProcessesByName(_LiveSiteScannerExeName);

            long totalPhysicalMemory = 0;
            TimeSpan totalProcessorTime = new TimeSpan(0, 0, 0, 0, 0); ;
            long totalVirtualMemory = 0;

            foreach (System.Diagnostics.Process proc in processes)
            {
                

                totalPhysicalMemory += proc.WorkingSet64;
                totalProcessorTime += proc.TotalProcessorTime;
                totalVirtualMemory = proc.VirtualMemorySize64;
                // imp prop to add:
                //proc.Responding
                //Console.WriteLine("Current physical memory : " + proc.WorkingSet64.ToString());
                //Console.WriteLine("Total processor time : " + proc.TotalProcessorTime.ToString());
                //Console.WriteLine("Virtual memory size : " + proc.VirtualMemorySize64.ToString());
                
            }
            retValue.NumberOfProcesses = processes.Length;
            retValue.TotalCurrentPhysicalMemory = totalPhysicalMemory / 1024;
            retValue.TotalProcessorTime = totalProcessorTime;
            retValue.TotalVirtualMemory = totalVirtualMemory /1024;

            return retValue;
        }

        public List<LiveSiteScannerProcessModel> getLiveScannerProcessorsInfo()
        {
            var retValue = new List<LiveSiteScannerProcessModel>();

            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(_LiveSiteScannerExeName);

            foreach (System.Diagnostics.Process proc in processes)
            {
                var procInfo = new LiveSiteScannerProcessModel();
                procInfo.ProcessId = proc.Id;
                procInfo.Responding = proc.Responding;
                procInfo.StartTime = proc.StartTime;
               
                if (proc.StartInfo.Arguments.Length > 0)
                {
                    procInfo.QueueNumber = int.Parse(proc.StartInfo.Arguments);
                }
  
                procInfo.VirtualMemory = proc.VirtualMemorySize64;
                procInfo.CurrentPhysicalMemory = proc.WorkingSet64;
                procInfo.ProcessorTime = proc.TotalProcessorTime;

                retValue.Add(procInfo);
            }
  
            return retValue;
        }

        public bool KillLiveSiteScanner(int HowMany = 1)
        {
            int processedKilled = 0;
            Process[] processes = Process.GetProcessesByName(_LiveSiteScannerExeName);
            foreach (Process p in processes)
            {
                try
                {
                    p.Kill();
                    p.WaitForExit(); // possibly with a timeout
                    processedKilled += 1;
                    if (processedKilled >= HowMany)
                    {
                        break;
                    }

                }
                catch (Win32Exception)
                {
                    // process was terminating or can't be terminated - deal with it
                }
                catch (InvalidOperationException)
                {
                    // process has already exited - might be able to let this one go
                }
            }
            return true;
        }

        private int  getMaxProcessNumber()
        {
            int maxNumber = 0;
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(_LiveSiteScannerExeName);
            foreach (Process p in processes)
            {
                var args = p.StartInfo.Arguments;
                  
                
            }
            return maxNumber;
        }

        #endregion

        #region AddSites
        public void AddSitesInDbCollection(SitesToSearch Site)
        {
            //_UOW.SiteSourceRepository.Add(Site);
            var s1 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "FDA Debarment List", SiteShortName = "FDA Debarment List", SiteEnum = SiteEnum.FDADebarPage, SiteUrl = "http://www.fda.gov/ora/compliance_ref/debar/default.htm"};
            _UOW.SiteSourceRepository.Add(s1);
            var s2 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Clinical Investigator Inspection List (CLIL)(CDER", SiteShortName = "Clinical Investigator Insp...", SiteEnum = SiteEnum.ClinicalInvestigatorInspectionPage, SiteUrl = "http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm"};
            _UOW.SiteSourceRepository.Add(s2);
            var s3 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "FDA Warning Letters and Responses", SiteShortName = "FDA Warning Letters ...", SiteEnum = SiteEnum.FDAWarningLettersPage, SiteUrl = "http://www.fda.gov/ICECI/EnforcementActions/WarningLetters/default.htm"};
            _UOW.SiteSourceRepository.Add(s3);
            var s4 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Notice of Opportunity for Hearing (NOOH) – Proposal to Debar", SiteShortName = "NOOH – Proposal to Debar", SiteEnum = SiteEnum.ERRProposalToDebarPage, SiteUrl = "http://www.fda.gov/RegulatoryInformation/FOI/ElectronicReadingRoom/ucm143240.htm"};
            _UOW.SiteSourceRepository.Add(s4);
            var s5 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Adequate Assurances List for Clinical Investigators", SiteShortName = "Adequate Assurances List ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl = "http://www.fda.gov/ora/compliance_ref/bimo/asurlist.htm"};
            _UOW.SiteSourceRepository.Add(s5);
            var s6 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName = "Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl = "http://www.accessdata.fda.gov/scripts/SDA/sdNavigation.cfm?sd=clinicalinvestigatorsdisqualificationproceedings&previewMode=true&displayAll=true"};
            _UOW.SiteSourceRepository.Add(s6);
            var s7 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "PHS Administrative Actions Listing ", SiteShortName = "PHS Administrative Actions", SiteEnum = SiteEnum.PHSAdministrativeActionListingPage, SiteUrl = "https://ori.hhs.gov/ORI_PHS_alert.html?d=update"};
            _UOW.SiteSourceRepository.Add(s7);
            var s8 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Clinical Investigator Inspection List (CBER)", SiteShortName = "CBER Clinical Investigator ...", SiteEnum = SiteEnum.CBERClinicalInvestigatorInspectionPage, SiteUrl = "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195364.htm"};
            _UOW.SiteSourceRepository.Add(s8);
            var s9 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "HHS/OIG/ EXCLUSIONS DATABASE SEARCH/ FRAUD", SiteShortName = "HHS/OIG/ EXCLUSIONS ...", SiteEnum = SiteEnum.ExclusionDatabaseSearchPage, SiteUrl = "https://oig.hhs.gov/exclusions/exclusions_list.asp"};
            _UOW.SiteSourceRepository.Add(s9);
            var s10 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "HHS/OIG Corporate Integrity Agreements/Watch List", SiteShortName = "HHS/OIG Corporate Integrity", SiteEnum = SiteEnum.CorporateIntegrityAgreementsListPage, SiteUrl = "http://oig.hhs.gov/compliance/corporate-integrity-agreements/cia-documents.asp"};
            _UOW.SiteSourceRepository.Add(s10);
            var s11 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName = "SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.SystemForAwardManagementPage, SiteUrl = "https://www.sam.gov/portal/public/SAM"};
            _UOW.SiteSourceRepository.Add(s11);
            var s12 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "LIST OF SPECIALLY DESIGNATED NATIONALS", SiteShortName = "SPECIALLY DESIGNATED ...", SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage, SiteUrl = "http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx"};
            _UOW.SiteSourceRepository.Add(s12);
            var s13 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "World Check (PI)", SiteShortName = "World Check...", SiteEnum = SiteEnum.WorldCheckPage, SiteUrl = "http://www.truthtechnologies.com/"};
            _UOW.SiteSourceRepository.Add(s13);
            //27July2017 adding world check(institute) and Icon internal flag check sites
            var s14 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "World Check (Institute)", SiteShortName = "World Check...", SiteEnum = SiteEnum.WorldCheckPage, SiteUrl = "http://www.truthtechnologies.com/" };
            _UOW.SiteSourceRepository.Add(s14);
            var s15 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Icon Internal Flag Check", SiteShortName = "Icon Internal...", SiteEnum = SiteEnum.IconInternalFlagCheck, SiteUrl = "" };
            _UOW.SiteSourceRepository.Add(s15);

            var site1 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Pfizer DMC Checks", SiteShortName = "Pfizer DMC Checks", SiteEnum = SiteEnum.PfizerDMCChecksPage, SiteUrl = " http://ecf12.pfizer.com/sites/clinicaloversightcommittees/default.aspx"};
            _UOW.SiteSourceRepository.Add(site1);
            var site2 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Pfizer Unavailable Checks", SiteShortName = "Pfizer DMC Checks", SiteEnum = SiteEnum.PfizerUnavailableChecksPage, SiteUrl = "http://ecf12.pfizer.com/"};
            _UOW.SiteSourceRepository.Add(site2);
            var site3 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "GSK Do Not Use Check", SiteShortName = "GSK DNU Check", SiteEnum = SiteEnum.GSKDoNotUseCheckPage, SiteUrl = ""};
            _UOW.SiteSourceRepository.Add(site3);
            var site4 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Regeneron Usability Check", SiteShortName = "Regeneron Usability Check", SiteEnum = SiteEnum.RegeneronUsabilityCheckPage, SiteUrl = ""};
            _UOW.SiteSourceRepository.Add(site4);
            var site5 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "AUSTRALIAN HEALTH PRACTITIONER REGULATION AGENCY", SiteShortName = "HEALTH PRACTITIONER ...", SiteEnum = SiteEnum.AustralianHealthPratitionerRegulationPage, SiteUrl = "http://www.ahpra.gov.au/Registration/Registers-of-Practitioners.aspx"};
            _UOW.SiteSourceRepository.Add(site5);
            var site6 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Belgium1 - ZOEK EEN ARTS", SiteShortName = "ZOEK EEN ARTS", SiteEnum = SiteEnum.ZoekEenArtsPage, SiteUrl = "https://ordomedic.be/nl/zoek-een-arts/"};
            _UOW.SiteSourceRepository.Add(site6);
            var site7 =
                new SitesToSearch { ExtractionMode = "Manual", SiteName = "Belgium2 - RIZIV - Zoeken", SiteShortName = "RIZIV - Zoeken", SiteEnum = SiteEnum.RizivZoekenPage, SiteUrl = "https://www.riziv.fgov.be/webprd/appl/psilverpages/nl"};
            _UOW.SiteSourceRepository.Add(site7);
            var site8 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Brazil - CONSELHOS DE MEDICINA", SiteShortName = "CONSELHOS DE MEDICINA", SiteEnum = SiteEnum.ConselhosDeMedicinaPage, SiteUrl = "http://portal.cfm.org.br/index.php?option=com_medicos&Itemid=59"};
            _UOW.SiteSourceRepository.Add(site8);
            var site9 =
                new SitesToSearch { ExtractionMode = "Manual", SiteName = "Colombia - EL TRIBUNAL NACIONAL DE ÉTICA MÉDICA", SiteShortName = "EL TRIBUNAL NACIONAL...", SiteEnum = SiteEnum.TribunalNationalDeEticaMedicaPage, SiteUrl = "http://www.tribunalnacionaldeeticamedica.org/site/biblioteca_documental"};
            _UOW.SiteSourceRepository.Add(site9);
            var site10 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Finland - VALVIRA", SiteShortName = "VALVIRA", SiteEnum = SiteEnum.ValviraPage, SiteUrl = "https://julkiterhikki.valvira.fi/"};
            _UOW.SiteSourceRepository.Add(site10);
            var site11 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "France - CONSEIL NATIONAL DE L'ORDRE DES MEDECINS", SiteShortName = "CONSEIL NATIONAL DE L'ORDRE...", SiteEnum = SiteEnum.ConseilNationalDeMedecinsPage, SiteUrl = "http://www.conseil-national.medecin.fr/annuaire"};
            _UOW.SiteSourceRepository.Add(site11);
            var site12 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "MEDICAL COUNCIL OF INDIA", SiteShortName = "MEDICAL COUNSIL OF INDIA", SiteEnum = SiteEnum.MedicalCouncilOfIndiaPage, SiteUrl = "http://online.mciindia.org/online//Index.aspx?qstr_level=01"};
            _UOW.SiteSourceRepository.Add(site12);
            var site13 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Israel - MINISTRY OF HEALTH ISRAEL", SiteShortName = "MINISTRY OF HEALTH ISRAEL", SiteEnum = SiteEnum.MinistryOfHealthIsraelPage, SiteUrl = "http://www.health.gov.il/UnitsOffice/HR/professions/postponements/Pages/default.aspx"};
            _UOW.SiteSourceRepository.Add(site13);
            var site14 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "New Zeland - LIST OF REGISTERED DOCTORS", SiteShortName = "LIST OF REGISTERED DOCTORS", SiteEnum = SiteEnum.ListOfRegisteredDoctorsPage, SiteUrl = "https://www.mcnz.org.nz/support-for-doctors/list-of-registered-doctors/"};
            _UOW.SiteSourceRepository.Add(site14);
            var site15 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Poland - NACZELNA IZBA LEKARSKA", SiteShortName = "NACZELNA IZBA LEKARSKA", SiteEnum = SiteEnum.NaczelnaIzbaLekarskaPage, SiteUrl = "http://rejestr.nil.org.pl/xml/nil/rejlek/hurtd"};
            _UOW.SiteSourceRepository.Add(site15);
            var site16 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Portugal - PORTAL OFICIAL DA ORDEM DOS MEDICOS", SiteShortName = "PORTAL OFICIAL DA ORDEM...", SiteEnum = SiteEnum.PortalOficialDaOrdemDosMedicosPage, SiteUrl = "https://www.ordemdosmedicos.pt/"};
            _UOW.SiteSourceRepository.Add(site16);
            var site17 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Spain - ORGANIZACION MEDICA COLEGIAL DE ESPANA", SiteShortName = "ORGANIZACION MEDICA COLEGIAL...", SiteEnum = SiteEnum.OrganizacionMedicaColegialDeEspanaPage, SiteUrl = "http://www.cgcom.es/consultapublicacolegiados"};
            _UOW.SiteSourceRepository.Add(site17);
            var site18 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "SINGAPORE MEDICAL COUNCIL", SiteShortName = "SINGAPORE MEDICAL COUNCIL...", SiteEnum = SiteEnum.SingaporeMedicalCouncilPage, SiteUrl = "http://www.healthprofessionals.gov.sg/content/hprof/smc/en.html"};
            _UOW.SiteSourceRepository.Add(site18);
            var site19 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "SRI LANKA MEDICAL COUNCIL", SiteShortName = "SRI LANKA MEDICAL COUNCIL...", SiteEnum = SiteEnum.SriLankaMedicalCouncilPage, SiteUrl = "http://www.srilankamedicalcouncil.org/registry.php"};
            _UOW.SiteSourceRepository.Add(site19);
            var site20 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "HEALTH GUIDE USA", SiteShortName = "HEALTH GUIDE USA", SiteEnum = SiteEnum.HealthGuideUSAPage, SiteUrl = "http://www.healthguideusa.org/medical_license_lookup.htm"};
            _UOW.SiteSourceRepository.Add(site20);
            var site21 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "India - Medical Council of India", SiteShortName = "Indian Medical...", SiteEnum = SiteEnum.MedicalCouncilOfIndiaPage, SiteUrl = "http://online.mciindia.org/online//Index.aspx?qstr_level=01" };
            _UOW.SiteSourceRepository.Add(site21);
        }
        #endregion

        #region Get/update SiteSources

        public List<SitesToSearch> GetAllSiteSources()
        {
            var SiteSources = _UOW.SiteSourceRepository.GetAll().OrderBy(x => x.SiteName).ToList(); 

            return SiteSources;
        }

        public SitesToSearch GetSingleSiteSource(Guid? RecId)
        {
            return _UOW.SiteSourceRepository.FindById(RecId);
        }

        public bool UpdateSiteSource(SitesToSearch SiteSource)
        {
            if (SiteSource.RecId == null)
            {
                _UOW.SiteSourceRepository.Add(SiteSource);
                return true;
            }
            else
            {
                _UOW.SiteSourceRepository.UpdateSiteSource(SiteSource);
                return true;
            }
        }

        public void DeleteSiteSource(Guid? RecId)
        {
            //Referential check:
            var DefaultSiteRecords = _UOW.DefaultSiteRepository.GetAll();
            var matchingDefaultSiteRecord = DefaultSiteRecords.Where(x => x.SiteId == RecId).FirstOrDefault();
            if (matchingDefaultSiteRecord != null)
            {
                throw new Exception("This record is used in Default Site Sources");
            }


            var CountryRecords = _UOW.CountryRepository.GetAll();
            var matchingCountryRecord = CountryRecords.Where(x => x.SiteId == RecId).FirstOrDefault();
            if (matchingCountryRecord != null)
            {
                throw new Exception("This record is used in Country Specific Site Sources");
            }

            var SponosorRecords = _UOW.SponsorProtocolRepository.GetAll();
            var matchingSponsorRecord = SponosorRecords.Where(x => x.SiteId == RecId).FirstOrDefault();
            if (matchingSponsorRecord != null)
            {
                throw new Exception("This record is used in Sponsor Specific Site Sources");
            }



            _UOW.SiteSourceRepository.RemoveById(RecId);
        }

        #endregion

        #region Add/Get/Delete Country

        public List<CountryViewModel> GetCountries()
        {
            var Countries = _UOW.CountryRepository.GetAll()
                .OrderBy(x => x.CountryName)
                .ToList();

            if (Countries.Count == 0)
                return null;

            var CountriesViewModel = new List<CountryViewModel>();

            foreach (Country country in Countries)
            {
                var CountryViewModel = new CountryViewModel();
                CountryViewModel.CountryName = country.CountryName;
                CountryViewModel.SiteId = country.SiteId;
                CountryViewModel.RecId = country.RecId;
                CountryViewModel.SearchAppliesToText =
                    country.SearchAppliesTo.ToString();
                CountryViewModel.IsMandatory = country.IsMandatory;

                var site = _UOW.SiteSourceRepository.FindById(country.SiteId);

                if (site == null)
                    throw new Exception("Site Source for Country " + 
                        country.CountryName + " not found");

                if (site != null)
                {
                    CountryViewModel.SiteName =  site.SiteName;
                    CountryViewModel.SiteUrl = site.SiteUrl;
                    if (country.SearchAppliesTo == SearchAppliesToEnum.Institute)
                        CountryViewModel.ExtractionMode = "Manual";
                    else
                        CountryViewModel.ExtractionMode =
                            site.ExtractionMode;

                    CountriesViewModel.Add(CountryViewModel);
                }
            }
            return CountriesViewModel;
        }
        
        public Country GetCountry(Guid? RecId)
        {
            return _UOW.CountryRepository.FindById(RecId);
        }

        public bool AddCountry(Country country)
        {
            if(country.RecId == null)
            {
                _UOW.CountryRepository.Add(country);
                return true;
            }
            else
            {
                _UOW.CountryRepository.UpdateCountry(country);
                var siteSourceToUpdate = _UOW.SiteSourceRepository.FindById(country.SiteId);
                siteSourceToUpdate.SiteName = country.Name;
                _UOW.SiteSourceRepository.UpdateSiteSource(siteSourceToUpdate);
                return true;
            }
        }

        public void DeleteCountry(Guid? RecId)
        {
            _UOW.CountryRepository.RemoveById(RecId);
        }

        #endregion

        #region Add/Get/Delete Sponsor
        
        public bool AddSponsor(SponsorProtocol sponsor)
        {
            if (sponsor.SiteType != SiteTypeEnum.Normal)
            {
                var Site =
                    _UOW.SponsorProtocolRepository.GetAll().Find(x =>
                    x.SiteType == sponsor.SiteType &&
                    //x.SearchAppliesTo == sponsor.SearchAppliesTo &&
                    x.RecId != sponsor.RecId);

                if (Site != null)
                {
                    //site with same SiteType exists..
                    return false;
                }
            }

            if (sponsor.RecId == null)
            {
                _UOW.SponsorProtocolRepository.Add(sponsor);
                return true;
            }
            else
            {
                _UOW.SponsorProtocolRepository.UpdateSponsorProtocol(sponsor);
                return true;
            }
        }

        public List<SponsorProtocolViewModel> GetSponsorProtocols()
        {
            var SponsorProtocols = _UOW.SponsorProtocolRepository.GetAll()
                .OrderBy(x => x.SponsorProtocolNumber)
                .ToList();

            if (SponsorProtocols.Count == 0)
                return null;

            var Sponsors = new List<SponsorProtocolViewModel>();

            foreach (SponsorProtocol sponsor in SponsorProtocols)
            {
                var sponsorViewModel = new SponsorProtocolViewModel();
                sponsorViewModel.SponsorProtocolNumber = 
                    sponsor.SponsorProtocolNumber;
                sponsorViewModel.SearchAppliesTo = 
                    sponsor.SearchAppliesTo;
                sponsorViewModel.SearchAppliesToText =
                    sponsor.SearchAppliesTo.ToString();
                sponsorViewModel.IsMandatory = sponsor.IsMandatory;

                if (sponsor.SiteType == SiteTypeEnum.WorldCheck)
                    sponsorViewModel.SiteTypeText = "World Check";
                else if (sponsor.SiteType == SiteTypeEnum.DMCExclusion)
                    sponsorViewModel.SiteTypeText = "DMC Exclusion";
                else if (sponsor.SiteType == SiteTypeEnum.Normal)
                    sponsorViewModel.SiteTypeText = "Normal";
                sponsorViewModel.SiteType = sponsor.SiteType;

                var site = _UOW.SiteSourceRepository.FindById(sponsor.SiteId);

                if (site == null)
                    throw new Exception("Site Source for sponsor protocol " + 
                        sponsor.SponsorProtocolNumber + " not found");

                if (site != null)
                {
                    sponsorViewModel.SiteName = site.SiteName;
                    sponsorViewModel.SiteUrl = site.SiteUrl;
                    sponsorViewModel.SiteId = sponsor.SiteId;
                    sponsorViewModel.RecId = sponsor.RecId;
                    if (sponsor.SearchAppliesTo == SearchAppliesToEnum.Institute)
                        sponsorViewModel.ExtractionMode = "Manual";
                    else
                        sponsorViewModel.ExtractionMode = site.ExtractionMode;

                    Sponsors.Add(sponsorViewModel);
                }
            }
            return Sponsors;
        }

        public SponsorProtocol GetSponsorProtocol(Guid? RecId)
        {
            return _UOW.SponsorProtocolRepository.FindById(RecId);
        }

        public void DeleteSponsor(Guid? RecId)
        {
            _UOW.SponsorProtocolRepository.RemoveById(RecId);
        }

        #endregion

        #region Extrtraction


        #endregion

        #region DefaultSites
        //public bool AddDefaultSite(DefaultSite site)
        //{
        //    if (site.SiteId == null)
        //        return false;

        //    var siteSource = _UOW.SiteSourceRepository.FindById(site.SiteId);

        //    site.SiteEnum = siteSource.SiteEnum;
        //    site.SiteName = siteSource.SiteName;
        //    site.SiteShortName = siteSource.SiteShortName;
        //    site.SiteUrl = siteSource.SiteUrl;
        //    site.ExtractionMode = siteSource.ExtractionMode;

        //    _UOW.DefaultSiteRepository.Add(site);
        //    return true;
        //}

        public List<DefaultSitesViewModel> GetDefaultSites()
        {
            var defaultSitesInDB = _UOW.DefaultSiteRepository.GetAll()
                .OrderBy(x => x.OrderNo)
                .ToList();

            if (defaultSitesInDB.Count == 0)
                return null;

            var defaultSites = new List<DefaultSitesViewModel>();

            foreach (DefaultSite defaultSite in defaultSitesInDB)
            {
                var defaultSiteViewModel = new DefaultSitesViewModel();
                defaultSiteViewModel.OrderNo = defaultSite.OrderNo;
                defaultSiteViewModel.IsMandatory = defaultSite.IsMandatory;

                defaultSiteViewModel.SearchAppliesTo = defaultSite.SearchAppliesTo;
                defaultSiteViewModel.SearchAppliesToText = defaultSite.SearchAppliesTo.ToString();
                //defaultSiteViewModel.ExcludeSI = defaultSite.ExcludeSI;
                if (defaultSite.SiteType == SiteTypeEnum.WorldCheck)
                    defaultSiteViewModel.SiteTypeText = "World Check";
                else if(defaultSite.SiteType == SiteTypeEnum.DMCExclusion)
                    defaultSiteViewModel.SiteTypeText = "DMC Exclusion";
                else if (defaultSite.SiteType == SiteTypeEnum.Normal)
                    defaultSiteViewModel.SiteTypeText = "Normal";
                defaultSiteViewModel.SiteType = defaultSite.SiteType;

                var site = _UOW.SiteSourceRepository.FindById(defaultSite.SiteId);
                
                if (site == null)
                {
                    throw new Exception("Site Source for Order No: " + defaultSite.OrderNo + " not found");
                }
                if (site != null)
                {
                    defaultSiteViewModel.SiteName = defaultSite.Name;
                    defaultSiteViewModel.SiteUrl = site.SiteUrl;
                    defaultSiteViewModel.SiteId = defaultSite.SiteId;
                    defaultSiteViewModel.RecId = defaultSite.RecId;
                    if (defaultSite.SearchAppliesTo == SearchAppliesToEnum.Institute)
                    {
                        defaultSiteViewModel.ExtractionMode = "Manual";
                    }
                    else
                    {
                        defaultSiteViewModel.ExtractionMode = site.ExtractionMode;
                    }

                    defaultSites.Add(defaultSiteViewModel);
                }
                else
                {
                    defaultSiteViewModel.SiteName = "Site not found in Site Source repository";
                    defaultSiteViewModel.SiteUrl = "Delete the record and add a new one.";
                }

                //sponsorViewModel.SiteName =
                //    _UOW.SiteSourceRepository.FindById(sponsor.SiteId).SiteName;


            }
            return defaultSites;
        }

        public DefaultSite GetSingleDefaultSite(Guid? RecId)
        {
            return _UOW.DefaultSiteRepository.FindById(RecId);
        }

        public bool UpdateDefaultSite(DefaultSite defaultSite)
        {
            //var ExtractionMode = sourceSite.ExtractionMode;
            //defaultSite.SiteName = sourceSite.SiteName;
            //defaultSite.SiteShortName = sourceSite.SiteShortName;
            //defaultSite.SiteUrl = sourceSite.SiteUrl;
            //Code does not handle searching by Institute Name.
            //Changing ExtractionMode to Manual for all sites where Search applies to Manual is
            //On UI Compliance Form Edit page - in the list of sites Source Date can be maded editable 
            //even for sites with Extraction Mode 'DB'
            //'Source Date' of the DB Extracted site cann be used for Sites that are applied to Institute
            // as user can add findings on any later day.  In the case of DB extracted sites (for PI and SI) the search, 
            //full match and partial match is carried out immediately, therefore the 'Source Date' derived from the site is valid.
            //if (defaultSite.SearchAppliesTo == SearchAppliesToEnum.Institute)
            //{
            //    defaultSite.ExtractionMode = "Manual";
            //}
            //else
            //{
            //    defaultSite.ExtractionMode = ExtractionMode;
            //}

            if(defaultSite.SiteType != SiteTypeEnum.Normal)
            {
                var Site =
                    _UOW.DefaultSiteRepository.GetAll().Find(x =>
                    x.SiteType == defaultSite.SiteType &&
                    x.SearchAppliesTo == defaultSite.SearchAppliesTo &&
                    x.RecId != defaultSite.RecId);
                
                if(Site != null)
                {
                    //site with same SearchAppliesTo and SiteType exists..
                    return false;
                }
            }

            if (defaultSite.RecId == null)
            {
                _UOW.DefaultSiteRepository.Add(defaultSite);
                return true;
            }
            else
            {
                _UOW.DefaultSiteRepository.UpdateDefaultSite(defaultSite);
                return true;
            }
        }

        public void DeleteDefaultSite(Guid? RecId)
        {
            _UOW.DefaultSiteRepository.RemoveById(RecId);
        }
        #endregion

        #region Get/Delete UploadedFiles

        public List<UploadsViewModel> GetUploadedFiles()
        {
            //13Oct2023:
            //Appears to be incomplete
            //canbe called from AppAdmin menu
            //hence commented to prevent call to _UOW.ComplianceFormRepository.GetAll()
            throw new Exception("GetUploadedFiles - not ready");
            //var forms = _UOW.ComplianceFormRepository.GetAll();

            //if (forms.Count == 0)
            //    return null;

            //forms.RemoveAll(x => x.GeneratedFileName == null || x.GeneratedFileName == "");

            //var UploadedFiles = new List<UploadsViewModel>();

            //int Counter = 0;
            //foreach (string file in Directory.GetFiles(_config.UploadsFolder))
            //{
            //    var fileName = Path.GetFileNameWithoutExtension(file);

            //    var form = forms.Find(x =>
            //    x.GeneratedFileName == fileName);

            //    if(form != null)
            //    {
            //        var UploadedFile = new UploadsViewModel();
            //        UploadedFile.UploadedFileName = form.UploadedFileName;
            //        UploadedFile.GeneratedFileName = form.GeneratedFileName;
            //        UploadedFile.AssignedTo = form.AssignedTo;
            //        UploadedFile.UploadedOn = form.SearchStartedOn;
            //        UploadedFiles.Add(UploadedFile);
            //    }
            //    Counter += 1;

            //    //if (Counter < forms.Count &&
            //    //    fileName == forms[Counter].GeneratedFileName)
            //    //{
            //    //    var UploadedFile = new UploadsViewModel();
            //    //    UploadedFile.UploadedFileName = forms[Counter].UploadedFileName;
            //    //    UploadedFile.GeneratedFileName = forms[Counter].GeneratedFileName;
            //    //    UploadedFile.AssignedTo = forms[Counter].AssignedTo;
            //    //    UploadedFile.UploadedOn = forms[Counter].SearchStartedOn;
            //    //    UploadedFiles.Add(UploadedFile);
            //    //}
            //}
            //return UploadedFiles;
        }

        public bool DeleteUploadedFile(string GeneratedFileName)
        {
            foreach(string file in Directory.GetFiles(_config.UploadsFolder))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if(fileName == GeneratedFileName)
                {
                    File.Delete(file);
                    return true;
                }
            }
            return false;
        }

        public bool DeleteAllUploadedFiles()
        {
            foreach(string file in Directory.GetFiles(_config.UploadsFolder))
            {
                File.Delete(file);
            }
            return true;
        }

        #endregion

        #region Get/Delete OutputFiles
        
        public List<OutputFileViewModel> GetOutputFiles()
        {

            return null;
        }

        #endregion

        #region ExceptionLogs
        
        public List<ExceptionLoggerViewModel> GetExceptionLogs()
        {
            if(_UOW.ExceptionLoggerRepository.GetAll().Count <= 0)
                return null;

            var ExceptionLogs = 
                _UOW.ExceptionLoggerRepository
                .GetAll()
                .OrderByDescending(x => x.AddedOn).ToList();

            var Exceptions = new List<ExceptionLoggerViewModel>();

            foreach(ExceptionLogger log in ExceptionLogs)
            {
                var ExceptionViewModel = new ExceptionLoggerViewModel();

                ExceptionViewModel.Id = log.Id;
                ExceptionViewModel.AddedOn = log.AddedOn;
                ExceptionViewModel.Address = log.Address;
                ExceptionViewModel.Request = log.Request;
                ExceptionViewModel.UserId = log.UserId;
                ExceptionViewModel.Message = log.Message;
                ExceptionViewModel.StackTrace = log.StackTrace;

                Exceptions.Add(ExceptionViewModel);
            }
            return Exceptions;
        }

        #endregion

        #region ExtrationLog
        public List<ExtractionLogViewModel> GetExtractionLog()
        {
            var ExtractionLog = _UOW.LogRepository.GetAll()
                .OrderByDescending(x => x.CreatedOn).ToList();

            var Logs = new List<ExtractionLogViewModel>();

            foreach(Log DBLog in ExtractionLog)
            {
                var Log = new ExtractionLogViewModel();
                Log.SiteEnumString = DBLog.SiteEnumString;
                Log.Step = DBLog.Step;
                Log.Status = DBLog.Status;
                Log.CreatedBy = DBLog.CreatedBy;
                Log.CreatedOn = DBLog.CreatedOn;
                Log.Caption = DBLog.Caption;
                Log.Message = DBLog.Message;

                Logs.Add(Log);
            }
            return Logs;
        }

        #endregion

        #region iSprint to DDAS Log

        public List<LogWSDDASViewModel> GetiSprintToDDASLog()
        {
            var Logs = _UOW.LogWSDDASRepository.GetAll();

            if (Logs.Count == 0)
                return null;

            var LogViewModel = new List<LogWSDDASViewModel>();

            foreach (LogWSDDAS Log in Logs)
            {
                var ViewModel = new LogWSDDASViewModel();
                ViewModel.CreatedOn = DateTime.Now;
                ViewModel.CreatedOn = Log.CreatedOn;
                ViewModel.RecId = Log.RecId;
                ViewModel.RequestPayload = Log.RequestPayload;
                ViewModel.Response = Log.Response;
                ViewModel.Status = Log.Status;

                LogViewModel.Add(ViewModel);
            }

            if (LogViewModel.Count > 0)
                return LogViewModel.OrderByDescending(x => x.CreatedOn).ToList();
            else
                return LogViewModel;

        }
        #endregion

        #region DDAS to iSprint Log

        public List<LogWSiSprintViewModel> GetDDtoiSprintLog()
        {
            var Logs = _UOW.LogWSISPRINTRepository.GetAll();

            if (Logs.Count == 0)
                return null;

            var LogViewModel = new List<LogWSiSprintViewModel>();

            foreach (LogWSISPRINT Log in Logs)
            {
                var ViewModel = new LogWSiSprintViewModel();
                ViewModel.CreatedOn = DateTime.Now;
                ViewModel.CreatedOn = Log.CreatedOn;
                ViewModel.RecId = Log.RecId;
                ViewModel.RequestPayload = Log.RequestPayload;
                ViewModel.Response = Log.Response;
                ViewModel.Status = Log.Status;

                LogViewModel.Add(ViewModel);
            }

            if (LogViewModel.Count > 0)
                return LogViewModel.OrderByDescending(x => x.CreatedOn).ToList();
            else
                return LogViewModel;
        }
        
        #endregion
    }
}
