using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DDAS.Models.Entities.Domain.SiteData;
using Utilities;

namespace DDAS.Services.Search
{
    public class SearchService : ISearchSummary
    {
        private IUnitOfWork _UOW;
        private ILog _log;
        private ISearchEngine _SearchEngine;

        public SearchService(IUnitOfWork uow, ILog log, 
            ISearchEngine SearchEngine)
        {
            _UOW = uow;
            _log = log;
            _SearchEngine = SearchEngine;
        }

        public SearchSummary GetSearchSummary(string NameToSearch)
        {
            SearchSummary searchSummary = new SearchSummary();
            var searchSummaryItems = new List<SearchSummaryItem>();

            NameToSearch = NameToSearch.Replace(",", "");

            SiteScanData ScanData = new SiteScanData(_UOW, _log, _SearchEngine);
            List<SiteScan> SiteScanList = ScanData.GetSiteScanSummary(NameToSearch);

            searchSummary.NameToSearch = NameToSearch;

            ComplianceForm complianceForm = new ComplianceForm();
            var SitesForNameToSearch = new List<SitesIncludedInSearch>();

            complianceForm.NameToSearch = NameToSearch;

            foreach (SiteScan Site in SiteScanList)
            {
                var SiteForNameToSearch = new SitesIncludedInSearch();
                SiteForNameToSearch.SiteEnum = Site.SiteEnum;

                var SummaryItem = new SearchSummaryItem();

                var TempSite = GetMatchStatus(Site.SiteEnum, 
                    NameToSearch, Site.DataId, SiteForNameToSearch);
                //???
                SummaryItem.FullMatch = TempSite.FullMatchCount;
                SummaryItem.PartialMatch = TempSite.PartialMatchCount;
                SummaryItem.SiteEnum = Site.SiteEnum;
                SummaryItem.SiteName = Site.SiteName;
                SummaryItem.SiteUrl = Site.SiteUrl;
                SummaryItem.RecId = Site.DataId;

                searchSummaryItems.Add(SummaryItem);

                SiteForNameToSearch.SiteUrl = Site.SiteUrl;
                SitesForNameToSearch.Add(SiteForNameToSearch);
            }
            searchSummary.SearchSummaryItems = searchSummaryItems;

            complianceForm.SiteDetails = SitesForNameToSearch;

            ComplianceFormService service = new ComplianceFormService(_UOW);
            service.CreateComplianceForm(complianceForm);

            searchSummary.ComplianceFormId = 
                service.GetComplianceFormId(NameToSearch);

            return searchSummary;
        }

        public SitesIncludedInSearch GetMatchStatus(SiteEnum Enum, string NameToSearch, 
            Guid? DataId, SitesIncludedInSearch Site)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInvestigatorMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLettersMatchCount(NameToSearch, DataId, Site);

                //case SiteEnum.ERRProposalToDebarPage:
                //    return GetProposalToDebarPageMatchCount(NameToSearch, DataId);

                //case SiteEnum.AdequateAssuranceListPage:
                //    return GetAdequateAssuranceListPageMatchCount(NameToSearch, DataId);

                //case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                //    return GetDisqualifionProceedingsMatchCount(NameToSearch, DataId);

                //case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                //    return GetCBERClinicalInvestigatorPageMatchCount(NameToSearch, DataId);

                //case SiteEnum.PHSAdministrativeActionListingPage:
                //    return GetPHSAdministrativeMatchCount(NameToSearch, DataId);

                //case SiteEnum.ExclusionDatabaseSearchPage:
                //    return GetExclusionDatabaseSearchPageMatchCount(NameToSearch, DataId);

                //case SiteEnum.CorporateIntegrityAgreementsListPage:
                //    return GetCIAPageMatchCount(NameToSearch, DataId);

                //case SiteEnum.SystemForAwardManagementPage:
                //    return GetSAMMatchCount(NameToSearch, DataId);

                //case SiteEnum.SpeciallyDesignedNationalsListPage:
                //    return GetSpeciallyDesignatedNationalsMatchCount(NameToSearch, 
                //        DataId);

                default: throw new Exception("Invalid Enum");
            }
        }

        #region FDADebarSite
        public SitesIncludedInSearch GetFDADebarPageMatchCount(string NameToSearch, 
            Guid? DataId, SitesIncludedInSearch Site)
        {
            //var FDASearchResult = GetFDADebarPageMatch(NameToSearch, DataId);

            FDADebarPageSiteData FDASearchResult =
                _UOW.FDADebarPageRepository.FindById(DataId);

            UpdateMatchStatus(FDASearchResult.DebarredPersons, NameToSearch);

            var DebarList = FDASearchResult.DebarredPersons.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (DebarList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = DebarList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (DebarredPerson person in DebarList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = person.RowNumber;
                MatchedRecord.Matched = person.Matched;
                MatchedRecord.RecordDetails =
                    //"RowNumber: " + person.RowNumber + "~" +
                    "FullName: " + person.FullName + "~" +
                    "NameOfPerson: " + person.FullName + "~" +
                    "EffectiveDate: " + person.EffectiveDate + "~" +
                    "EndOfTermOfDebarment: " + person.EndOfTermOfDebarment + "~" +
                    "FrDateText: " + person.FrDateText + "~" +
                    "VolumePage: " + person.VolumePage + "~" +
                    "DocumentLink: " + person.DocumentLink + "~" +
                    "DocumentName: " + person.DocumentName;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
        }

        public SitesIncludedInSearch GetFDADebarPageMatch(string NameToSearch, 
            Guid? DataId, SiteEnum Enum)
        {
            string[] Name = NameToSearch.Split(' ');

            //Error handling: if FDASearchResult is null
            var FDASearchResult = 
                _UOW.ComplianceFormRepository.FindById(DataId);

            var DebarList = FDASearchResult.SiteDetails.Where(
                site => site.SiteEnum == Enum).FirstOrDefault();
             
            return DebarList;
        }
        #endregion

        #region ClinicalInvestigatorInspectionSite

        public SitesIncludedInSearch GetClinicalInvestigatorMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site)
        {
            ClinicalInvestigatorInspectionSiteData ClinicalSiteData =
                _UOW.ClinicalInvestigatorInspectionListRepository.FindById(DataId);

            UpdateMatchStatus(ClinicalSiteData.ClinicalInvestigatorInspectionList, 
                NameToSearch);

            var ClinicalMatchedList = ClinicalSiteData.ClinicalInvestigatorInspectionList.
                Where(
               ClinicalList => ClinicalList.Matched > 0).ToList();

            if (ClinicalMatchedList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = ClinicalMatchedList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else if(MatchesFound != 0)
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (ClinicalInvestigator Investigator in ClinicalMatchedList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();

                MatchedRecord.RowNumber = Investigator.RowNumber;
                MatchedRecord.Matched = Investigator.Matched;
                MatchedRecord.RecordDetails =
                    //"RowNumber: " + Investigator.RowNumber + "~" +
                    "FullName: " + Investigator.FullName + "~" +
                    "Name: " + Investigator.Name + "~" +
                    "Location: " + Investigator.Location + "~" +
                    "Address: " + Investigator.Address + "~" +
                    "City: " + Investigator.City + "~" +
                    "State: " + Investigator.State + "~" +
                    "Country: " + Investigator.Country + "~" +
                    "ZipCode: " + Investigator.Zipcode + "~" +
                    "InspectionDate: " + Investigator.InspectionDate + "~" +
                    "ClassificationType: " + Investigator.ClassificationType + "~" +
                    "ClassificationCode: " + Investigator.ClassificationCode + "~" +
                    "DeficiencyCode: " + Investigator.DeficiencyCode;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
        }

        public SitesIncludedInSearch
            GetClinicalInvestigatorSiteMatch(string NameToSearch, Guid? DataId,
            SiteEnum Enum)
        {
            var ClinicalSiteMatchedDetails =
                _UOW.ComplianceFormRepository.FindById(DataId);

            var ClinicalSiteData = ClinicalSiteMatchedDetails.SiteDetails.Where(
                site => site.SiteEnum == Enum).FirstOrDefault();

            return ClinicalSiteData;
        }

        #endregion

        #region FDAWarningLetters
        
        public SitesIncludedInSearch GetFDAWarningLettersMatchCount(string NameToSearch, 
            Guid? DataId, SitesIncludedInSearch Site)
        {
            FDAWarningLettersSiteData FDASearchResult =
                _UOW.FDAWarningLettersRepository.FindById(DataId);

            UpdateMatchStatus(FDASearchResult.FDAWarningLetterList, NameToSearch);

            var WarningLettersList = FDASearchResult.FDAWarningLetterList.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (WarningLettersList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = WarningLettersList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (FDAWarningLetter WarningLetter in WarningLettersList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = WarningLetter.RowNumber;
                MatchedRecord.Matched = WarningLetter.Matched;
                MatchedRecord.RecordDetails =
                    //"Matched: " + WarningLetter.Matched + "~" +
                    "FullName: " + WarningLetter.FullName + "~" +
                    "Company: " + WarningLetter.Company + "~" +
                    "LetterIssued: " + WarningLetter.LetterIssued + "~" +
                    "IssuingOffice: " + WarningLetter.IssuingOffice + "~" +
                    "Subject: " + WarningLetter.Subject + "~" +
                    "ResponseLetterPosted: " + WarningLetter.ResponseLetterPosted + "~" +
                    "CloseOutDate: " + WarningLetter.CloseOutDate;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
        }

        public SitesIncludedInSearch GetFDAWarningLettersMatch(string NameToSearch,
            Guid? DataId, SiteEnum Enum)
        {
            string[] Name = NameToSearch.Split(' ');

            var FDASearchResult =
                _UOW.ComplianceFormRepository.FindById(DataId);

            var WarningLetters = FDASearchResult.SiteDetails.Where(
                site => site.SiteEnum == Enum).FirstOrDefault();

            return WarningLetters;
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
        public bool SaveRecordStatus(SitesIncludedInSearch Site,
            Guid? ComplianceFormId)
        {
            var ComplianceFormDetails = 
                _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (ComplianceFormDetails == null)
                return false;

            var ExistingSiteDetails = ComplianceFormDetails.SiteDetails.Where(
                x => x.SiteEnum == Site.SiteEnum).FirstOrDefault();

            ExistingSiteDetails.LastUpdatedOn = DateTime.Now;

            var ExistingRecords = ExistingSiteDetails.MatchedRecords;

            //var UpdatedRecords = Site.MatchedRecords.Where(r => r.RowNumber);

            foreach(MatchedRecordsPerSite Updatedrecord in Site.MatchedRecords)
            {
                foreach(MatchedRecordsPerSite ExistingRecord in ExistingRecords)
                {
                    if (Updatedrecord.RowNumber == ExistingRecord.RowNumber &&
                        Updatedrecord.Status != ExistingRecord.Status)
                    {
                        ExistingRecord.Issues = Updatedrecord.Issues;
                        ExistingRecord.Status = Updatedrecord.Status;
                        ExistingRecord.IssueNumber = Updatedrecord.IssueNumber;
                        ExistingRecord.Matched = Updatedrecord.Matched;
                    }
                }
            }
            //ExistingSiteDetails.Findings = Site.Findings;

            _UOW.ComplianceFormRepository.UpdateCollection(ComplianceFormDetails);

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
