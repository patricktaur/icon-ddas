using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Services.LiveScan;
using System.Diagnostics;
using System.ComponentModel;
using DDAS.Models.ViewModels;

namespace DDAS.Services.AppAdminService
{
   
    public class AppAdminService : IAppAdminService
    {
        private IUnitOfWork _UOW;
        private string _LiveSiteScannerExeName = "DDAS.LiveSiteExtractor";
        public AppAdminService(IUnitOfWork UOW)
        {
            _UOW = UOW;
        }

        public List<DataExtractionHistory> GetDataExtractionHistory()
        {
            var FDADebarSiteData = _UOW.FDADebarPageRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            var ListOfExtractionHistory = new List<DataExtractionHistory>();
            var ExtractionHistory1 = new DataExtractionHistory();
            AddToExtractionHistoryList(
                ListOfExtractionHistory, ExtractionHistory1, 
                FDADebarSiteData, 1, SiteEnum.FDADebarPage);
            


            var ExtractionHistory2 = new DataExtractionHistory();

            var ClinicalInvestigatorSiteData =
                _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory2, ClinicalInvestigatorSiteData, 2, SiteEnum.ClinicalInvestigatorInspectionPage);
            

            var ExtractionHistory3 = new DataExtractionHistory();
            var FDAWarningLetters = _UOW.FDAWarningLettersRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();
            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory3, FDAWarningLetters, 3, SiteEnum.FDAWarningLettersPage);

            var ExtractionHistory4 = new DataExtractionHistory();

            var ERRSiteData = _UOW.ERRProposalToDebarRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();
            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory4, ERRSiteData, 4, SiteEnum.ERRProposalToDebarPage);


            var ExtractionHistory5 = new DataExtractionHistory();

            var AdequateAssuraceSiteData = _UOW.AdequateAssuranceListRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory5, AdequateAssuraceSiteData, 5, SiteEnum.AdequateAssuranceListPage);   


            var ExtractionHistory6 = new DataExtractionHistory();

            var ClinicalInvestigatorDisqualificationData = _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory6, ClinicalInvestigatorDisqualificationData, 6, SiteEnum.ClinicalInvestigatorDisqualificationPage);

            var ExtractionHistory7 = new DataExtractionHistory();

            var PHSSiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory7, PHSSiteData, 7, SiteEnum.PHSAdministrativeActionListingPage);

            var ExtractionHistory8 = new DataExtractionHistory();

            var CBERSiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory8, CBERSiteData, 8, SiteEnum.CBERClinicalInvestigatorInspectionPage);

            var ExtractionHistory9 = new DataExtractionHistory();

            var ExclusionSiteData = _UOW.ExclusionDatabaseSearchRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory9, ExclusionSiteData, 9, SiteEnum.ExclusionDatabaseSearchPage);

            var ExtractionHistory10 = new DataExtractionHistory();

            var CIASiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory10, CIASiteData, 10, SiteEnum.CorporateIntegrityAgreementsListPage);


            //var ExtractionHistory11 - LiveSite

            //11 SAM / SYSTEM FOR AWARD MANAGEMENT Live No
            //Live Site - Data not extracted
            var ExtractionHistory11 = new DataExtractionHistory();
            var site11 = new BaseSiteData();
            
            site11.DataExtractionRequired = false;
            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory11, site11, 11, SiteEnum.SystemForAwardManagementPage);

            var ExtractionHistory12 = new DataExtractionHistory();

            var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).
                First();

            AddToExtractionHistoryList(ListOfExtractionHistory, ExtractionHistory12, SDNSiteData, 12, SiteEnum.SpeciallyDesignedNationalsListPage);

            return ListOfExtractionHistory;
        }

        private void AddToExtractionHistoryList(
            List<DataExtractionHistory> list, 
            DataExtractionHistory ExtractionHistory, 
            BaseSiteData SiteData, 
            int SiteNumber, 
            SiteEnum siteEnum)
        {
            //var ExtractionHistory9 = new DataExtractionHistory();

            //var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetAll().
            //    OrderByDescending(x => x.CreatedOn).
            //    First();

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

        #region GetDataExtractionPerSite
        public List<DataExtractionHistory> GetDataExtractionPerSite(SiteEnum Enum)
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

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSDNRepository();

                default: throw new Exception("Invalid Enum");
            }
        }

        private List<DataExtractionHistory> GetFDADebarRepository()
        {
            var FDASiteData = _UOW.FDADebarPageRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach(FDADebarPageSiteData SiteData in FDASiteData)
            {
                var DataExtraction = new DataExtractionHistory();

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

        private List<DataExtractionHistory> GetCiilRepository()
        {
            var CIILSiteData = _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (ClinicalInvestigatorInspectionSiteData SiteData in CIILSiteData)
            {
                var DataExtraction = new DataExtractionHistory();

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

        
        private List<DataExtractionHistory> GetFDAWarningLetters()
        {
            var FDAWarningLetterData = _UOW.FDAWarningLettersRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (FDAWarningLettersSiteData SiteData in FDAWarningLetterData)
            {
                var DataExtraction = new DataExtractionHistory();

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

        private List<DataExtractionHistory> GetErrProposalToDebarRepository()
        {
            var ERRSiteData = _UOW.ERRProposalToDebarRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (ERRProposalToDebarPageSiteData SiteData in ERRSiteData)
            {
                var DataExtraction = new DataExtractionHistory();

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

        private List<DataExtractionHistory> GetAdequateAssuranceRepository()
        {
            var AdequateAssuranceSiteData = _UOW.AdequateAssuranceListRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (AdequateAssuranceListSiteData SiteData in AdequateAssuranceSiteData)
            {
                var DataExtraction = new DataExtractionHistory();

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

        
        private List<DataExtractionHistory> GetClinicalInvestigatorDisqualificationRepository()
        {
            //SiteEnum.ClinicalInvestigatorDisqualificationPage

            var ClinicalInvestigatorDisqualificationData = _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (ClinicalInvestigatorDisqualificationSiteData SiteData in ClinicalInvestigatorDisqualificationData)
            {
                var DataExtraction = new DataExtractionHistory();

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
        private List<DataExtractionHistory> GetCBERRepository()
        {
            var CBERSiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (CBERClinicalInvestigatorInspectionSiteData SiteData in CBERSiteData)
            {
                var DataExtraction = new DataExtractionHistory();

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

        private List<DataExtractionHistory> GetPHSRepository()
        {
            var PHSSiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (PHSAdministrativeActionListingSiteData SiteData in PHSSiteData)
            {
                var DataExtraction = new DataExtractionHistory();

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

        private List<DataExtractionHistory> GetExclusionDatabaseRepository()
        {
            var ExclusionSiteData = _UOW.ExclusionDatabaseSearchRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (ExclusionDatabaseSearchPageSiteData SiteData in ExclusionSiteData)
            {
                var DataExtraction = new DataExtractionHistory();

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

        private List<DataExtractionHistory> GetCIARepository()
        {
            var CIASiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (CorporateIntegrityAgreementListSiteData SiteData in CIASiteData)
            {
                var DataExtraction = new DataExtractionHistory();

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

        private List<DataExtractionHistory> GetSDNRepository()
        {
            var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetAll().
                OrderByDescending(x => x.CreatedOn).ToList();

            var DataExtractionList = new List<DataExtractionHistory>();

            foreach (SpeciallyDesignatedNationalsListSiteData SiteData in SDNSiteData)
            {
                var DataExtraction = new DataExtractionHistory();

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

                case SiteEnum.ERRProposalToDebarPage:
                    DeleteERRProposalToDebarExtractionEntry(RecId);
                    return;

                case SiteEnum.AdequateAssuranceListPage:
                    DeleteAdequateAssuranceExtractionEntry(RecId);
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
            var CurrentDocument = _UOW.ClinicalInvestigatorInspectionListRepository.FindById(RecId);

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
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "FDA Debarment List", SiteShortName = "FDA Debarment List", SiteEnum = SiteEnum.FDADebarPage, SiteUrl = "http://www.fda.gov/ora/compliance_ref/debar/default.htm", ExcludePI = false, ExcludeSI = false};
            _UOW.SiteSourceRepository.Add(s1);
            var s2 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "Clinical Investigator Inspection List (CLIL)(CDER", SiteShortName = "Clinical Investigator Insp...", SiteEnum = SiteEnum.ClinicalInvestigatorInspectionPage, SiteUrl = "http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s2);
            var s3 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "FDA Warning Letters and Responses", SiteShortName = "FDA Warning Letters ...", SiteEnum = SiteEnum.FDAWarningLettersPage, SiteUrl = "http://www.fda.gov/ICECI/EnforcementActions/WarningLetters/default.htm", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s3);
            var s4 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "Notice of Opportunity for Hearing (NOOH) – Proposal to Debar", SiteShortName = "NOOH – Proposal to Debar", SiteEnum = SiteEnum.ERRProposalToDebarPage, SiteUrl = "http://www.fda.gov/RegulatoryInformation/FOI/ElectronicReadingRoom/ucm143240.htm", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s4);
            var s5 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "Adequate Assurances List for Clinical Investigators", SiteShortName = "Adequate Assurances List ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl = "http://www.fda.gov/ora/compliance_ref/bimo/asurlist.htm", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s5);
            var s6 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName = "Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl = "http://www.accessdata.fda.gov/scripts/SDA/sdNavigation.cfm?sd=clinicalinvestigatorsdisqualificationproceedings&previewMode=true&displayAll=true", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s6);
            var s7 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "PHS Administrative Actions Listing ", SiteShortName = "PHS Administrative Actions", SiteEnum = SiteEnum.PHSAdministrativeActionListingPage, SiteUrl = "https://ori.hhs.gov/ORI_PHS_alert.html?d=update", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s7);
            var s8 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "Clinical Investigator Inspection List (CBER)", SiteShortName = "CBER Clinical Investigator ...", SiteEnum = SiteEnum.CBERClinicalInvestigatorInspectionPage, SiteUrl = "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195364.htm", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s8);
            var s9 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "HHS/OIG/ EXCLUSIONS DATABASE SEARCH/ FRAUD", SiteShortName = "HHS/OIG/ EXCLUSIONS ...", SiteEnum = SiteEnum.ExclusionDatabaseSearchPage, SiteUrl = "https://oig.hhs.gov/exclusions/exclusions_list.asp", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s9);
            var s10 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "HHS/OIG Corporate Integrity Agreements/Watch List", SiteShortName = "HHS/OIG Corporate Integrity", SiteEnum = SiteEnum.CorporateIntegrityAgreementsListPage, SiteUrl = "http://oig.hhs.gov/compliance/corporate-integrity-agreements/cia-documents.asp", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s10);
            var s11 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "Live", SiteName = "SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName = "SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.SystemForAwardManagementPage, SiteUrl = "https://www.sam.gov/portal/public/SAM", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s11);
            var s12 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "DB", SiteName = "LIST OF SPECIALLY DESIGNATED NATIONALS", SiteShortName = "SPECIALLY DESIGNATED ...", SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage, SiteUrl = "http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(s12);
            var s13 =
                    new SitesToSearch { Mandatory = true, ExtractionMode = "Manual", SiteName = "World Check (Only for PI)", SiteShortName = "World Check...", SiteEnum = SiteEnum.WorldCheckPage, SiteUrl = "http://www.truthtechnologies.com/", ExcludePI = false, ExcludeSI = true };
            _UOW.SiteSourceRepository.Add(s13);


            var site1 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Pfizer DMC Checks", SiteShortName = "Pfizer DMC Checks", SiteEnum = SiteEnum.PfizerDMCChecksPage, SiteUrl = " http://ecf12.pfizer.com/sites/clinicaloversightcommittees/default.aspx", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site1);
            var site2 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Pfizer Unavailable Checks", SiteShortName = "Pfizer DMC Checks", SiteEnum = SiteEnum.PfizerUnavailableChecksPage, SiteUrl = "http://ecf12.pfizer.com/", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site2);
            var site3 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "GSK Do Not Use Check", SiteShortName = "GSK DNU Check", SiteEnum = SiteEnum.GSKDoNotUseCheckPage, SiteUrl = "", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site3);
            var site4 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Regeneron Usability Check", SiteShortName = "Regeneron Usability Check", SiteEnum = SiteEnum.RegeneronUsabilityCheckPage, SiteUrl = "", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site4);
            var site5 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "AUSTRALIAN HEALTH PRACTITIONER REGULATION AGENCY", SiteShortName = "HEALTH PRACTITIONER ...", SiteEnum = SiteEnum.AustralianHealthPratitionerRegulationPage, SiteUrl = "http://www.ahpra.gov.au/Registration/Registers-of-Practitioners.aspx", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site5);
            var site6 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Belgium1 - ZOEK EEN ARTS", SiteShortName = "ZOEK EEN ARTS", SiteEnum = SiteEnum.ZoekEenArtsPage, SiteUrl = "https://ordomedic.be/nl/zoek-een-arts/", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site6);
            var site7 =
                new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Belgium2 - RIZIV - Zoeken", SiteShortName = "RIZIV - Zoeken", SiteEnum = SiteEnum.RizivZoekenPage, SiteUrl = "https://www.riziv.fgov.be/webprd/appl/psilverpages/nl", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site7);
            var site8 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Brazil - CONSELHOS DE MEDICINA", SiteShortName = "CONSELHOS DE MEDICINA", SiteEnum = SiteEnum.ConselhosDeMedicinaPage, SiteUrl = "http://portal.cfm.org.br/index.php?option=com_medicos&Itemid=59", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site8);
            var site9 =
                new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Colombia - EL TRIBUNAL NACIONAL DE ÉTICA MÉDICA", SiteShortName = "EL TRIBUNAL NACIONAL...", SiteEnum = SiteEnum.TribunalNationalDeEticaMedicaPage, SiteUrl = "http://www.tribunalnacionaldeeticamedica.org/site/biblioteca_documental", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site9);
            var site10 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Finland - VALVIRA", SiteShortName = "VALVIRA", SiteEnum = SiteEnum.ValviraPage, SiteUrl = "https://julkiterhikki.valvira.fi/", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site10);
            var site11 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "France - CONSEIL NATIONAL DE L'ORDRE DES MEDECINS", SiteShortName = "CONSEIL NATIONAL DE L'ORDRE...", SiteEnum = SiteEnum.ConseilNationalDeMedecinsPage, SiteUrl = "http://www.conseil-national.medecin.fr/annuaire", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site11);
            var site12 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "MEDICAL COUNCIL OF INDIA", SiteShortName = "MEDICAL COUNSIL OF INDIA", SiteEnum = SiteEnum.MedicalCouncilOfIndiaPage, SiteUrl = "http://online.mciindia.org/online//Index.aspx?qstr_level=01", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site12);
            var site13 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Israel - MINISTRY OF HEALTH ISRAEL", SiteShortName = "MINISTRY OF HEALTH ISRAEL", SiteEnum = SiteEnum.MinistryOfHealthIsraelPage, SiteUrl = "http://www.health.gov.il/UnitsOffice/HR/professions/postponements/Pages/default.aspx", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site13);
            var site14 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "New Zeland - LIST OF REGISTERED DOCTORS", SiteShortName = "LIST OF REGISTERED DOCTORS", SiteEnum = SiteEnum.ListOfRegisteredDoctorsPage, SiteUrl = "https://www.mcnz.org.nz/support-for-doctors/list-of-registered-doctors/", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site14);
            var site15 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Poland - NACZELNA IZBA LEKARSKA", SiteShortName = "NACZELNA IZBA LEKARSKA", SiteEnum = SiteEnum.NaczelnaIzbaLekarskaPage, SiteUrl = "http://rejestr.nil.org.pl/xml/nil/rejlek/hurtd", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site15);
            var site16 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Portugal - PORTAL OFICIAL DA ORDEM DOS MEDICOS", SiteShortName = "PORTAL OFICIAL DA ORDEM...", SiteEnum = SiteEnum.PortalOficialDaOrdemDosMedicosPage, SiteUrl = "https://www.ordemdosmedicos.pt/", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site16);
            var site17 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "Spain - ORGANIZACION MEDICA COLEGIAL DE ESPANA", SiteShortName = "ORGANIZACION MEDICA COLEGIAL...", SiteEnum = SiteEnum.OrganizacionMedicaColegialDeEspanaPage, SiteUrl = "http://www.cgcom.es/consultapublicacolegiados", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site17);
            var site18 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "SINGAPORE MEDICAL COUNCIL", SiteShortName = "SINGAPORE MEDICAL COUNCIL...", SiteEnum = SiteEnum.SingaporeMedicalCouncilPage, SiteUrl = "http://www.healthprofessionals.gov.sg/content/hprof/smc/en.html", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site18);
            var site19 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "SRI LANKA MEDICAL COUNCIL", SiteShortName = "SRI LANKA MEDICAL COUNCIL...", SiteEnum = SiteEnum.SriLankaMedicalCouncilPage, SiteUrl = "http://www.srilankamedicalcouncil.org/registry.php", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site19);
            var site20 =
                    new SitesToSearch { Mandatory = false, ExtractionMode = "Manual", SiteName = "HEALTH GUIDE USA", SiteShortName = "HEALTH GUIDE USA", SiteEnum = SiteEnum.HealthGuideUSAPage, SiteUrl = "http://www.healthguideusa.org/medical_license_lookup.htm", ExcludePI = false, ExcludeSI = false };
            _UOW.SiteSourceRepository.Add(site20);
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
            _UOW.SiteSourceRepository.RemoveById(RecId);
        }

        #endregion

        #region Add/Get/Delete Country

        public List<CountryViewModel> GetCountries()
        {
            var CountriesViewModel = new List<CountryViewModel>();

            var Countries = _UOW.CountryRepository.GetAll().OrderBy(x => x.Name).ToList();

            if (Countries.Count == 0)
                return null;

            foreach(Country country in Countries)
            {
                var CountryViewModel = new CountryViewModel();
                CountryViewModel.Name = country.Name;
                CountryViewModel.SiteId = country.SiteId;
                CountryViewModel.RecId = country.RecId;
                var site = _UOW.SiteSourceRepository.FindById(country.SiteId);
                if (site != null) {
                    CountryViewModel.SiteName =  site.SiteName;
                    CountryViewModel.SiteUrl = site.SiteUrl;
                    CountriesViewModel.Add(CountryViewModel);
                }
                //CountryViewModel.SiteName =
                //    _UOW.SiteSourceRepository.FindById(country.SiteId).SiteName;
            }
            return CountriesViewModel;
        }
        
        public bool AddCountry(Country country)
        {
            if (country.Name == "" || country.Name == null ||
                country.SiteId == null)
                return false;

            _UOW.CountryRepository.Add(country);
            return true;
        }

        public void DeleteCountry(Guid? RecId)
        {
            _UOW.CountryRepository.RemoveById(RecId);
        }

        #endregion

        #region Add/Get/Delete Sponsor
        
        public bool AddSponsor(SponsorProtocol sponsor)
        {
            if (sponsor.SponsorProtocolNumber == "" ||
                sponsor.SponsorProtocolNumber == null ||
                sponsor.SiteId == null)
                return false;

            _UOW.SponsorProtocolRepository.Add(sponsor);
            return true;
        }

        public void DeleteSponsor(Guid? RecId)
        {
            _UOW.SponsorProtocolRepository.RemoveById(RecId);
        }

        public List<SponsorProtocolViewModel> GetSponsorProtocols()
        {
            var SponsorProtocols = _UOW.SponsorProtocolRepository.GetAll();

            if (SponsorProtocols.Count == 0)
                return null;

            var Sponsors = new List<SponsorProtocolViewModel>();

            foreach(SponsorProtocol sponsor in  SponsorProtocols)
            {
                var sponsorViewModel = new SponsorProtocolViewModel();
                sponsorViewModel.SponsorProtocolNumber = sponsor.SponsorProtocolNumber;

                var site = _UOW.SiteSourceRepository.FindById(sponsor.SiteId);
                if (site != null)
                {
                    sponsorViewModel.SiteName = site.SiteName;
                    sponsorViewModel.SiteUrl = site.SiteUrl;
                    sponsorViewModel.SiteId = sponsor.SiteId;
                    sponsorViewModel.RecId = sponsor.RecId;

                    Sponsors.Add(sponsorViewModel);
                }



                //sponsorViewModel.SiteName =
                //    _UOW.SiteSourceRepository.FindById(sponsor.SiteId).SiteName;

                
            }
            return Sponsors;
        }
        #endregion

        #region DefaultSites
        public bool AddDefaultSite(DefaultSite site)
        {
            if (
                site.SiteId == null)
                return false;

            _UOW.DefaultSiteRepository.Add(site);
            return true;
        }



        public List<DefaultSitesViewModel> GetDefaultSites()
        {
           

            var defaultSitesInDB = _UOW.DefaultSiteRepository.GetAll().OrderBy(x => x.OrderNo).ToList();

            if (defaultSitesInDB.Count == 0)
                return null;

            var defaultSites = new List<DefaultSitesViewModel>();

            foreach (DefaultSite defaultSite in defaultSitesInDB)
            {
                var defaultSiteViewModel = new DefaultSitesViewModel();
                defaultSiteViewModel.OrderNo = defaultSite.OrderNo;
                defaultSiteViewModel.IsMandatory = defaultSite.IsMandatory;
                defaultSiteViewModel.ExcludeSI = defaultSite.ExcludeSI;
                var site = _UOW.SiteSourceRepository.FindById(defaultSite.SiteId);
                if (site != null)
                {
                    defaultSiteViewModel.SiteName = site.SiteName;
                    defaultSiteViewModel.SiteUrl = site.SiteUrl;
                    defaultSiteViewModel.SiteId = defaultSite.SiteId;
                    defaultSiteViewModel.RecId = defaultSite.RecId;

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

        public void DeleteDefaultSite(Guid? RecId)
        {
            _UOW.DefaultSiteRepository.RemoveById(RecId);
        }
        #endregion
    }
}
