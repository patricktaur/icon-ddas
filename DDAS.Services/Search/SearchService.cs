using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DDAS.Models.Entities.Domain.SiteData;

namespace DDAS.Services.Search
{
    public class SearchService : ISearchSummary
    {
        private IUnitOfWork _UOW;

        public SearchService(IUnitOfWork uow)
        {
            _UOW = uow;
        }

        public SearchSummary GetSearchSummary(NameToSearchQuery query)
        {
            SearchSummary searchSummary = new SearchSummary();
            var searchSummaryItems = new List<SearchSummaryItem>();

            string NameToSearch = query.NameToSearch.Replace(",", "");

            SiteScanData ScanData = new SiteScanData(_UOW);
            List<SiteScan> SiteScanList = ScanData.GetSiteScanSummary();

            searchSummary.NameToSearch = NameToSearch;

            foreach (SiteScan Site in SiteScanList)
            {
                var SummaryItem = new SearchSummaryItem();

                SummaryItem.MatchStatus = GetMatchStatus(
                    Site.SiteEnum, 
                    NameToSearch, 
                    Site.DataId);
                //???
                SummaryItem.SiteEnum = Site.SiteEnum;
                SummaryItem.SiteName = Site.SiteName;
                SummaryItem.SiteUrl = Site.SiteUrl;
                SummaryItem.RecId = Site.DataId;

                searchSummaryItems.Add(SummaryItem);
            }
            searchSummary.SearchSummaryItems = searchSummaryItems;

            return searchSummary;
        }

        public string GetMatchStatus(SiteEnum Enum, string NameToSearch, Guid? DataId)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchCount(NameToSearch, DataId);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInvestigatorMatchCount(NameToSearch, DataId);

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLettersMatchCount(NameToSearch, DataId);

                case SiteEnum.ERRProposalToDebarPage:
                    return GetProposalToDebarPageMatchCount(NameToSearch, DataId);

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssuranceListPageMatchCount(NameToSearch, DataId);

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetDisqualifionProceedingsMatchCount(NameToSearch, DataId);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorPageMatchCount(NameToSearch, DataId);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSAdministrativeMatchCount(NameToSearch, DataId);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabaseSearchPageMatchCount(NameToSearch, DataId);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIAPageMatchCount(NameToSearch, DataId);

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSAMMatchCount(NameToSearch, DataId);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSpeciallyDesignatedNationalsMatchCount(NameToSearch, 
                        DataId);

                default : throw new Exception("Invalid Enum");
            }
        }

        #region FDADebarSite
        public string GetFDADebarPageMatchCount(string NameToSearch, Guid? DataId)
        {
            var FDASearchResult = GetFDADebarPageMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = FDASearchResult.Where(
                    x => x.Matched == counter).Count();
                //if (MatchesFound == Name.Length && counter == Name.Length)
                //    MatchStatus = MatchesFound + "";
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public List<DebarredPerson> GetFDADebarPageMatch(string NameToSearch, 
            Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            FDADebarPageSiteData FDASearchResult = 
                _UOW.FDADebarPageRepository.FindById(DataId);
    
            UpdateMatchStatus(FDASearchResult.DebarredPersons, NameToSearch);

            var DebarList = FDASearchResult.DebarredPersons.Where(
               debarredList => debarredList.Matched > 0).ToList();

            FDADebarPageMatchRecords FDAMatchRecords = new FDADebarPageMatchRecords();
            FDAMatchRecords.FDADebarMatchedRecords = DebarList;

            var ComplianceFormData = new ComplianceFormService(_UOW);
            ComplianceFormData.CreateComplianceForm(NameToSearch, "India", "abc11",
                "111/22A", "Bangalore");

            FDAMatchRecords.ComplianceFormId = 
                ComplianceFormData.GetComplianceFormId(NameToSearch);

            return FDAMatchRecords.FDADebarMatchedRecords;
        }
        #endregion

        #region ClinicalInvestigatorInspectionSite

        public string GetClinicalInvestigatorMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var ClinicalSiteData = GetClinicalInvestigatorSiteMatch(NameToSearch,
                DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound =
                    ClinicalSiteData.ClinicalInvestigatorInspectionList
                    .Where(x => x.Matched == counter).Count();

                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public ClinicalInvestigatorInspectionSiteData
            GetClinicalInvestigatorSiteMatch(string NameToSearch, Guid? DataId)
        {
            var ClinicalSiteData =
                _UOW.ClinicalInvestigatorInspectionListRepository.FindById(DataId);

            string[] Name = NameToSearch.Split(' ');

            UpdateMatchStatus(ClinicalSiteData.ClinicalInvestigatorInspectionList,
                NameToSearch);

            var CIISiteData = ClinicalSiteData.ClinicalInvestigatorInspectionList.
                Where(CIIData => CIIData.Matched > 0).ToList();

            ClinicalSiteData.ClinicalInvestigatorInspectionList = CIISiteData;

            return ClinicalSiteData;
        }

        #endregion

        #region FDAWarningLetters
        
        public string GetFDAWarningLettersMatchCount(string NameToSearch, Guid? DataId)
        {
            var FDASearchResult = GetFDAWarningLettersMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = FDASearchResult.FDAWarningLetterList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public FDAWarningLettersSiteData GetFDAWarningLettersMatch(string NameToSearch,
            Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            FDAWarningLettersSiteData FDASearchResult =
                _UOW.FDAWarningLettersRepository.FindById(DataId);

            UpdateMatchStatus(FDASearchResult.FDAWarningLetterList, NameToSearch);

            var WarningLetterList = FDASearchResult.FDAWarningLetterList.Where(
               FDAWarningList => FDAWarningList.Matched > 0).ToList();

            FDASearchResult.FDAWarningLetterList = WarningLetterList;

            return FDASearchResult;
        }
        #endregion

        #region ProposalToDebar
        public string GetProposalToDebarPageMatchCount(string NameToSearch, Guid? DataId)
        {
            var ProposalToDebarSearchResult = 
                GetProposalToDebarPageMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = ProposalToDebarSearchResult.ProposalToDebar.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public ERRProposalToDebarPageSiteData GetProposalToDebarPageMatch(
            string NameToSearch, Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            ERRProposalToDebarPageSiteData ProposalToDebarSearchResult =
                _UOW.ERRProposalToDebarRepository.FindById(DataId);

            UpdateMatchStatus(ProposalToDebarSearchResult.ProposalToDebar, NameToSearch);

            var DebarList = ProposalToDebarSearchResult.ProposalToDebar.Where(
               ProposalToDebarList => ProposalToDebarList.Matched > 0).ToList();

            ProposalToDebarSearchResult.ProposalToDebar = DebarList;

            return ProposalToDebarSearchResult;
        }
        #endregion

        #region AdequateAssuranceList
        
        public string GetAdequateAssuranceListPageMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var AdequateAssuranceSearchResult = 
                GetAdequateAssuranceListPageMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = AdequateAssuranceSearchResult.AdequateAssurances.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public AdequateAssuranceListSiteData GetAdequateAssuranceListPageMatch(
            string NameToSearch, Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            AdequateAssuranceListSiteData AdequateAssuranceList =
                _UOW.AdequateAssuranceListRepository.FindById(DataId);

            UpdateMatchStatus(AdequateAssuranceList.AdequateAssurances, NameToSearch);

            var AssuranceList = AdequateAssuranceList.AdequateAssurances.Where(
               debarredList => debarredList.Matched > 0).ToList();

            AdequateAssuranceList.AdequateAssurances = AssuranceList;

            return AdequateAssuranceList;
        }
        #endregion

        #region ClinicalInvestigatorDisqualificationProceedings
        
        public string GetDisqualifionProceedingsMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var DisqualificationSearchResult = 
                GetDisqualificationProceedingsMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = DisqualificationSearchResult.DisqualifiedInvestigatorList.
                    Where(
                    x => x.Matched == counter).Count();

                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }
        
        public ClinicalInvestigatorDisqualificationSiteData
            GetDisqualificationProceedingsMatch(string NameToSearch, Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            ClinicalInvestigatorDisqualificationSiteData DisqualificationSearchResult =
                _UOW.ClinicalInvestigatorDisqualificationRepository.FindById(DataId);

            UpdateMatchStatus(DisqualificationSearchResult.DisqualifiedInvestigatorList,
                NameToSearch);

            var DisqualifiedList = DisqualificationSearchResult.DisqualifiedInvestigatorList.
                Where(
               Dlist => Dlist.Matched > 0).ToList();

            DisqualificationSearchResult.DisqualifiedInvestigatorList = DisqualifiedList;

            return DisqualificationSearchResult;
        }
        #endregion

        #region CBERClinicalInvestigator

        public string GetCBERClinicalInvestigatorPageMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var CBERSearchResult = 
                GetCBERClinicalInvestigatorPageMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = CBERSearchResult.ClinicalInvestigator.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }
        
        public CBERClinicalInvestigatorInspectionSiteData 
            GetCBERClinicalInvestigatorPageMatch(string NameToSearch, Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            CBERClinicalInvestigatorInspectionSiteData CBERSearchResult =
                _UOW.CBERClinicalInvestigatorRepository.FindById(DataId);

            UpdateMatchStatus(CBERSearchResult.ClinicalInvestigator, NameToSearch);

            var CBERList = CBERSearchResult.ClinicalInvestigator.Where(
               CBERClinicalList => CBERClinicalList.Matched > 0).ToList();

            CBERSearchResult.ClinicalInvestigator = CBERList;

            return CBERSearchResult;
        }
        #endregion

        #region PHSSite
        public string GetPHSAdministrativeMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var PHSData = GetPHSAdministrativeSiteMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = PHSData.PHSAdministrativeSiteData.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }

            return MatchStatus;
        }

        public PHSAdministrativeActionListingSiteData GetPHSAdministrativeSiteMatch(
            string NameToSearch, 
            Guid? DataId)
        {
            PHSAdministrativeActionListingSiteData PHSSiteData =
                _UOW.PHSAdministrativeActionListingRepository.FindById(DataId);

            string[] Name = NameToSearch.Split(' ');

            UpdateMatchStatus(PHSSiteData.PHSAdministrativeSiteData, NameToSearch);

            var PHSData = PHSSiteData.PHSAdministrativeSiteData.Where(
                PHS => PHS.Matched > 0).ToList();

            PHSSiteData.PHSAdministrativeSiteData = PHSData;

            return PHSSiteData;
        }
        #endregion

        #region ExclusionDatabaseSearch

        public string GetExclusionDatabaseSearchPageMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var ExclusionSearchResult = GetExclusionDatabaseSearchPageMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = ExclusionSearchResult.ExclusionSearchList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public ExclusionDatabaseSearchPageSiteData GetExclusionDatabaseSearchPageMatch(
            string NameToSearch, Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            ExclusionDatabaseSearchPageSiteData ExclusionSearchResult =
                _UOW.ExclusionDatabaseSearchRepository.FindById(DataId);

            UpdateMatchStatus(ExclusionSearchResult.ExclusionSearchList, NameToSearch);

            var ExclusionSearchList = ExclusionSearchResult.ExclusionSearchList.Where(
               ExclusionList => ExclusionList.Matched > 0).ToList();

            ExclusionSearchResult.ExclusionSearchList = ExclusionSearchList;

            return ExclusionSearchResult;
        }
        #endregion

        #region CorporateIntegrityAgreement

        public string GetCIAPageMatchCount(string NameToSearch, Guid? DataId)
        {
            var CIASearchResult = GetCIAPageMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = CIASearchResult.CIAListSiteData.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public CorporateIntegrityAgreementListSiteData GetCIAPageMatch(
            string NameToSearch, Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            CorporateIntegrityAgreementListSiteData CIASearchResult =
                _UOW.CorporateIntegrityAgreementRepository.FindById(DataId);

            UpdateMatchStatus(CIASearchResult.CIAListSiteData, NameToSearch);

            var DebarList = CIASearchResult.CIAListSiteData.Where(
               debarredList => debarredList.Matched > 0).ToList();

            CIASearchResult.CIAListSiteData = DebarList;

            return CIASearchResult;
        }

        #endregion

        #region SystemForAwardManagement

        public string GetSAMMatchCount(string NameToSearch, Guid? DataId)
        {
            var SAMSearchResult = GetSAMMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = SAMSearchResult.SAMSiteData.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public SystemForAwardManagementPageSiteData GetSAMMatch(string NameToSearch,
            Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            SystemForAwardManagementPageSiteData SAMSiteSearchResult =
                _UOW.SystemForAwardManagementRepository.FindById(DataId);

            UpdateMatchStatus(SAMSiteSearchResult.SAMSiteData, NameToSearch);

            var SAMList = SAMSiteSearchResult.SAMSiteData.Where(
               SAMDataList => SAMDataList.Matched > 0).ToList();

            SAMSiteSearchResult.SAMSiteData = SAMList;

            return SAMSiteSearchResult;
        }
        #endregion

        #region SpeciallyDesignatedNations

        public string GetSpeciallyDesignatedNationalsMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var SDNSiteData = GetSpeciallyDesignatedNationsMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound =
                    SDNSiteData.SDNListSiteData.
                    Where(x => x.Matched == counter).Count();

                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public SpeciallyDesignatedNationalsListSiteData GetSpeciallyDesignatedNationsMatch(
            string NameToSearch, Guid? DataId)
        {
            var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.FindById(DataId);

            string[] Name = NameToSearch.Split(' ');

            UpdateMatchStatus(SDNSiteData.SDNListSiteData,
                NameToSearch);

            var CIISiteData = SDNSiteData.SDNListSiteData.
                Where(CIIData => CIIData.Matched > 0).ToList();

            SDNSiteData.SDNListSiteData = CIISiteData;

            return SDNSiteData;
        }
        #endregion

        void UpdateMatchStatus(IEnumerable<SiteDataItemBase> items, string NameToSearch)
        {
            string[] namesInNameToSearch = NameToSearch.Split(' ');
            foreach (SiteDataItemBase item in items)
            {
                int Count = 0;
                foreach (string name in namesInNameToSearch)
                {
                    if (item.FullName.ToLower().Contains(name.ToLower()))
                    {
                        Count += 1;
                    }
                }
                item.Matched = Count;
            }
        }
        
        #region Save and Update Approved/Rejected records
        public bool SaveRecordStatus(SaveSearchResult Result)
        {
            Result.CreatedOn = DateTime.Now;
            _UOW.SaveSearchResultRepository.Add(Result);
            return true;
        }
        
        public FDADebarPageSiteData GetStatusOfFDASiteRecords(
            FDADebarPageSiteData FDASiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.FDADebarPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null) 
                return FDASiteData;

            foreach (DebarredPerson debarredPerson in FDASiteData.DebarredPersons)
            {
                foreach(SaveSearchDetails SearchDetails in 
                    SavedSearchResult.saveSearchDetails)
                {
                    if (debarredPerson.RowNumber == SearchDetails.RowNumber)
                        debarredPerson.Status = SearchDetails.Status;
                }
            }
            return FDASiteData;
        }

        public ClinicalInvestigatorInspectionSiteData GetStatusOfClinicalSiteRecords(
            ClinicalInvestigatorInspectionSiteData ClinicalSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.ClinicalInvestigatorInspectionPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return ClinicalSiteData;

            foreach (ClinicalInvestigator Investigator in 
                ClinicalSiteData.ClinicalInvestigatorInspectionList)
            {
                foreach(SaveSearchDetails SearchDetails in 
                    SavedSearchResult.saveSearchDetails)
                {
                    if (Investigator.RowNumber == SearchDetails.RowNumber)
                        Investigator.Status = SearchDetails.Status;
                }
            }
            return ClinicalSiteData;
        }

        public FDAWarningLettersSiteData GetStatusOfFDAWarningSiteRecords(
            FDAWarningLettersSiteData FDAWarningLetterSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.ERRProposalToDebarPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return FDAWarningLetterSiteData;

            foreach (FDAWarningLetter FDAWarningLetterData in
                FDAWarningLetterSiteData.FDAWarningLetterList)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (FDAWarningLetterData.RowNumber == SearchDetails.RowNumber)
                        FDAWarningLetterData.Status = SearchDetails.Status;
                }
            }
            return FDAWarningLetterSiteData;
        }

        public ERRProposalToDebarPageSiteData GetStatusOfProposalToDebarSiteRecords(
            ERRProposalToDebarPageSiteData ProposalToDebarSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.ERRProposalToDebarPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return ProposalToDebarSiteData;

            foreach (ProposalToDebar proposalToDebar in 
                ProposalToDebarSiteData.ProposalToDebar)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (proposalToDebar.RowNumber == SearchDetails.RowNumber)
                        proposalToDebar.Status = SearchDetails.Status;
                }
            }
            return ProposalToDebarSiteData;
        }

        public AdequateAssuranceListSiteData GetStatusOfAssuranceSiteRecords(
            AdequateAssuranceListSiteData AssuranceSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.AdequateAssuranceListPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return AssuranceSiteData;

            foreach (AdequateAssuranceList AssuranceList in
                AssuranceSiteData.AdequateAssurances)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (AssuranceList.RowNumber == SearchDetails.RowNumber)
                        AssuranceList.Status = SearchDetails.Status;
                }
            }
            return AssuranceSiteData;
        }

        public ClinicalInvestigatorDisqualificationSiteData 
            GetStatusOfDisqualificationSiteRecords(
            ClinicalInvestigatorDisqualificationSiteData DisqualificationSiteData,
            string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.AdequateAssuranceListPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return DisqualificationSiteData;

            foreach (DisqualifiedInvestigator DisqualifiedList 
                in DisqualificationSiteData.DisqualifiedInvestigatorList)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (DisqualifiedList.RowNumber == SearchDetails.RowNumber)
                        DisqualifiedList.Status = SearchDetails.Status;
                }
            }
            return DisqualificationSiteData;
        }

        public CBERClinicalInvestigatorInspectionSiteData GetStatusOfCBERSiteRecords(
            CBERClinicalInvestigatorInspectionSiteData CBERSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.CBERClinicalInvestigatorInspectionPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return CBERSiteData;

            foreach (CBERClinicalInvestigator CBERList in
                CBERSiteData.ClinicalInvestigator)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (CBERList.RowNumber == SearchDetails.RowNumber)
                        CBERList.Status = SearchDetails.Status;
                }
            }
            return CBERSiteData;
        }

        public PHSAdministrativeActionListingSiteData GetStatusOfPHSSiteRecords(
            PHSAdministrativeActionListingSiteData PHSSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.PHSAdministrativeActionListingPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return PHSSiteData;

            foreach (PHSAdministrativeAction PHSData in 
                PHSSiteData.PHSAdministrativeSiteData)
            {
                foreach(SaveSearchDetails SearchDetails in 
                    SavedSearchResult.saveSearchDetails)
                {
                    if (PHSData.RowNumber == SearchDetails.RowNumber)
                        PHSData.Status = SearchDetails.Status;
                }
            }
            return PHSSiteData;
        }

        public ExclusionDatabaseSearchPageSiteData GetStatusOfExclusionSiteRecords(
            ExclusionDatabaseSearchPageSiteData ExclusionSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.ExclusionDatabaseSearchPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return ExclusionSiteData;

            foreach (ExclusionDatabaseSearchList ExclusionList in
                ExclusionSiteData.ExclusionSearchList)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (ExclusionList.RowNumber == SearchDetails.RowNumber)
                        ExclusionList.Status = SearchDetails.Status;
                }
            }
            return ExclusionSiteData;
        }

        public CorporateIntegrityAgreementListSiteData GetStatusOfCIASiteRecords(
            CorporateIntegrityAgreementListSiteData CIASiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.CorporateIntegrityAgreementsListPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return CIASiteData;

            foreach (CIAList CIADataList in
                CIASiteData.CIAListSiteData)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (CIADataList.RowNumber == SearchDetails.RowNumber)
                        CIADataList.Status = SearchDetails.Status;
                }
            }
            return CIASiteData;
        }

        public SystemForAwardManagementPageSiteData GetStatusOfSAMSiteRecords(
            SystemForAwardManagementPageSiteData SAMSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.SystemForAwardManagementPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return SAMSiteData;

            foreach (SystemForAwardManagement SAMData in SAMSiteData.SAMSiteData)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (SAMData.RowNumber == SearchDetails.RowNumber)
                        SAMData.Status = SearchDetails.Status;
                }
            }
            return SAMSiteData;
        }

        public SpeciallyDesignatedNationalsListSiteData GetStatusOfSDNSiteRecords(
            SpeciallyDesignatedNationalsListSiteData SDNSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.SpeciallyDesignedNationalsListPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return SDNSiteData;

            foreach(SDNList SDNListData in SDNSiteData.SDNListSiteData)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (SDNListData.RowNumber == SearchDetails.RowNumber)
                        SDNListData.Status = SearchDetails.Status;
                }
            }
            return SDNSiteData;
        }
        #endregion
    }
}
