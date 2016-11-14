using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DDAS.Models.Entities.Domain.SiteData;
using System.Text.RegularExpressions;

namespace DDAS.Services.Search
{
    public class SearchService : ISearchSummary
    {
        private IUnitOfWork _UOW;
        private ISearchEngine _SearchEngine;

        public SearchService(IUnitOfWork uow, 
            ISearchEngine SearchEngine)
        {
            _UOW = uow;
            _SearchEngine = SearchEngine;
        }

        public ComplianceForm GetSearchSummary(string NameToSearch, ILog log)
        {
            SearchSummary searchSummary = new SearchSummary();
            var searchSummaryItems = new List<SearchSummaryItem>();

            NameToSearch = NameToSearch.Replace(",", "");
            NameToSearch.Trim();

            SiteScanData ScanData = new SiteScanData(_UOW, _SearchEngine);
            List<SiteScan> SiteScanList = ScanData.GetSiteScanSummary(NameToSearch, log);

            searchSummary.NameToSearch = NameToSearch;

            ComplianceForm complianceForm = new ComplianceForm();
            var SitesForNameToSearch = new List<SitesIncludedInSearch>();

            complianceForm.NameToSearch = NameToSearch;

            //foreach (SiteScan Site in SiteScanList)
            for(int Counter = 0; Counter < SiteScanList.Count; Counter++)
            {
                var SiteForNameToSearch = new SitesIncludedInSearch();
                SiteForNameToSearch.SiteEnum = SiteScanList[Counter].SiteEnum;

                var SummaryItem = new SearchSummaryItem();

                var TempSite = GetMatchStatus(SiteScanList[Counter].SiteEnum, 
                    NameToSearch, SiteScanList[Counter].DataId, SiteForNameToSearch);
                //???
                SummaryItem.FullMatch = TempSite.FullMatchCount;
                SummaryItem.PartialMatch = TempSite.PartialMatchCount;
                SummaryItem.SiteEnum = SiteScanList[Counter].SiteEnum;
                SummaryItem.SiteName = SiteScanList[Counter].SiteName;
                SummaryItem.SiteUrl = SiteScanList[Counter].SiteUrl;
                SummaryItem.RecId = SiteScanList[Counter].DataId;

                searchSummaryItems.Add(SummaryItem);

                SiteForNameToSearch.MatchStatus =
                    SummaryItem.FullMatch + " full matches and " +
                    SummaryItem.PartialMatch + " partial matches";

                SiteForNameToSearch.SiteUrl = SiteScanList[Counter].SiteUrl;
                SiteForNameToSearch.SiteName = SiteScanList[Counter].SiteName;
                SitesForNameToSearch.Add(SiteForNameToSearch);
            }
            searchSummary.SearchSummaryItems = searchSummaryItems;

            complianceForm.SiteDetails = SitesForNameToSearch;

            complianceForm.Active = true;

            ComplianceFormService service = new ComplianceFormService(_UOW);
            service.CreateComplianceForm(complianceForm);

            complianceForm.SiteDetails = null;
            return complianceForm;
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

                case SiteEnum.ERRProposalToDebarPage:
                    return GetProposalToDebarPageMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssuranceListPageMatchCount(NameToSearch, DataId, Site);

                //case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                //    return GetDisqualifionProceedingsMatchCount(NameToSearch, DataId);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorPageMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSAdministrativeMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabaseSearchPageMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIAPageMatchCount(NameToSearch, DataId, Site);

                //case SiteEnum.SystemForAwardManagementPage:
                //    return GetSAMMatchCount(NameToSearch, DataId, Site);

                //case SiteEnum.SpeciallyDesignedNationalsListPage:
                //    return GetSpeciallyDesignatedNationalsMatchCount(NameToSearch, 
                //        DataId, Site);

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
        public SitesIncludedInSearch GetProposalToDebarPageMatchCount(string NameToSearch, 
            Guid? DataId, SitesIncludedInSearch Site)
        {
            ERRProposalToDebarPageSiteData ERRSiteData =
                _UOW.ERRProposalToDebarRepository.FindById(DataId);

            UpdateMatchStatus(ERRSiteData.ProposalToDebar, NameToSearch);

            var ERRList = ERRSiteData.ProposalToDebar.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (ERRList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = ERRList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (ProposalToDebar proposalToDebarRecord in ERRList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = proposalToDebarRecord.RowNumber;
                MatchedRecord.Matched = proposalToDebarRecord.Matched;
                MatchedRecord.RecordDetails =
                    //"RowNumber: " + person.RowNumber + "~" +
                    "FullName: " + proposalToDebarRecord.FullName + "~" +
                    "Name: " + proposalToDebarRecord.Name + "~" +
                    "Center: " + proposalToDebarRecord.center + "~" +
                    "Date: " + proposalToDebarRecord.date + "~" +
                    "IssuingOffice: " + proposalToDebarRecord.IssuingOffice;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
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
        
        public SitesIncludedInSearch GetAdequateAssuranceListPageMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site)
        {
            AdequateAssuranceListSiteData AdequateListSearchResult =
                _UOW.AdequateAssuranceListRepository.FindById(DataId);

            UpdateMatchStatus(AdequateListSearchResult.AdequateAssurances, NameToSearch);

            var AdequateList = AdequateListSearchResult.AdequateAssurances.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (AdequateList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = AdequateList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (AdequateAssuranceList AssuranceList in AdequateList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = AssuranceList.RowNumber;
                MatchedRecord.Matched = AssuranceList.Matched;
                MatchedRecord.RecordDetails =
                    //"RowNumber: " + person.RowNumber + "~" +
                    "FullName: " + AssuranceList.FullName + "~" +
                    "NameAndAddress: " + AssuranceList.NameAndAddress + "~" +
                    "Center: " + AssuranceList.Center + "~" +
                    "Type: " + AssuranceList.Type + "~" +
                    "ActionDate: " + AssuranceList.ActionDate + "~" +
                    "Comments: " + AssuranceList.Comments;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
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

        public SitesIncludedInSearch GetCBERClinicalInvestigatorPageMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site)
        {
            CBERClinicalInvestigatorInspectionSiteData CBERSearchResult =
                _UOW.CBERClinicalInvestigatorRepository.FindById(DataId);

            UpdateMatchStatus(CBERSearchResult.ClinicalInvestigator, NameToSearch);

            var CBERList = CBERSearchResult.ClinicalInvestigator.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (CBERList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = CBERList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (CBERClinicalInvestigator Investigator in CBERList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = Investigator.RowNumber;
                MatchedRecord.Matched = Investigator.Matched;
                MatchedRecord.RecordDetails =
                    //"RowNumber: " + person.RowNumber + "~" +
                    "FullName: " + Investigator.FullName + "~" +
                    "Name: " + Investigator.Name + "~" +
                    "Title: " + Investigator.Title + "~" +
                    "InstituteAndAddress: " + Investigator.InstituteAndAddress + "~" +
                    "InspectionStartAndEndDate: " + Investigator.InspectionStartAndEndDate + "~" +
                    "Classification: " + Investigator.Classification;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
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
        public SitesIncludedInSearch GetPHSAdministrativeMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site)
        {
            PHSAdministrativeActionListingSiteData PHSSearchResult =
                _UOW.PHSAdministrativeActionListingRepository.FindById(DataId);

            UpdateMatchStatus(PHSSearchResult.PHSAdministrativeSiteData, NameToSearch);

            var PHSList = PHSSearchResult.PHSAdministrativeSiteData.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (PHSList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = PHSList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (PHSAdministrativeAction PHSAdmin in PHSList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = PHSAdmin.RowNumber;
                MatchedRecord.Matched = PHSAdmin.Matched;
                MatchedRecord.RecordDetails =
                    //"RowNumber: " + person.RowNumber + "~" +
                    "FullName: " + PHSAdmin.FullName + "~" +
                    "FirstName: " + PHSAdmin.FirstName + "~" +
                    "LastName: " + PHSAdmin.LastName + "~" +
                    "MiddleName: " + PHSAdmin.MiddleName + "~" +
                    "DebarmentUntil: " + PHSAdmin.DebarmentUntil + "~" +
                    "NoPHSAdvisoryUntil: " + PHSAdmin.NoPHSAdvisoryUntil + "~" +
                    "CertificationOfWorkUntil: " + PHSAdmin.CertificationOfWorkUntil + "~" +
                    "SupervisionUntil: " + PHSAdmin.SupervisionUntil + "~" +
                    "RetractionOfArticle: " + PHSAdmin.RetractionOfArticle + "~" +
                    "CorrectionOfArticle: " + PHSAdmin.CorrectionOfArticle + "~" +
                    "Memo: " + PHSAdmin.Memo;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
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

        public SitesIncludedInSearch GetExclusionDatabaseSearchPageMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site)
        {
            ExclusionDatabaseSearchPageSiteData ExclusionSearchResult =
                _UOW.ExclusionDatabaseSearchRepository.FindById(DataId);

            UpdateMatchStatus(ExclusionSearchResult.ExclusionSearchList, NameToSearch);

            var ExclusionList = ExclusionSearchResult.ExclusionSearchList.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (ExclusionList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = ExclusionList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (ExclusionDatabaseSearchList ExclusionData in ExclusionList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = ExclusionData.RowNumber;
                MatchedRecord.Matched = ExclusionData.Matched;
                MatchedRecord.RecordDetails =
                    //"RowNumber: " + person.RowNumber + "~" +
                    "FullName: " + ExclusionData.FullName + "~" +
                    "FirstName: " + ExclusionData.FirstName + "~" +
                    "LastName: " + ExclusionData.LastName + "~" +
                    "MiddleName: " + ExclusionData.MiddleName + "~" +
                    "General: " + ExclusionData.General + "~" +
                    "Specialty: " + ExclusionData.Specialty + "~" +
                    "ExclusionType: " + ExclusionData.ExclusionType;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
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

        public SitesIncludedInSearch GetCIAPageMatchCount(string NameToSearch, 
            Guid? DataId, SitesIncludedInSearch Site)
        {
            CorporateIntegrityAgreementListSiteData CIASearchResult =
                _UOW.CorporateIntegrityAgreementRepository.FindById(DataId);

            UpdateMatchStatus(CIASearchResult.CIAListSiteData, NameToSearch);

            var CIAList = CIASearchResult.CIAListSiteData.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (CIAList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = CIAList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (CIAList CIAData in CIAList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = CIAData.RowNumber;
                MatchedRecord.Matched = CIAData.Matched;
                MatchedRecord.RecordDetails =
                    //"RowNumber: " + person.RowNumber + "~" +
                    "FullName: " + CIAData.FullName + "~" +
                    "Provider: " + CIAData.Provider + "~" +
                    "Status: " + CIAData.Status + "~" +
                    "City: " + CIAData.City + "~" +
                    "State: " + CIAData.State + "~" +
                    "Effective: " + CIAData.Effective;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
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

        public SitesIncludedInSearch GetSAMMatchCount(string NameToSearch, 
            Guid? DataId, SitesIncludedInSearch Site)
        {
            SpeciallyDesignatedNationalsListSiteData SDNSearchResult =
                _UOW.SpeciallyDesignatedNationalsRepository.FindById(DataId);

            UpdateMatchStatus(SDNSearchResult.SDNListSiteData, NameToSearch);

            var SDNList = SDNSearchResult.SDNListSiteData.Where(
               SDNListData => SDNListData.Matched > 0).ToList();

            if (SDNList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = SDNList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (SDNList SDNData in SDNList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = SDNData.RowNumber;
                MatchedRecord.Matched = SDNData.Matched;
                MatchedRecord.RecordDetails =
                    "FullName: " + SDNData.FullName + "~" +
                    "Name: " + SDNData.Name + "~" +
                    "PageNumber: " + SDNData.PageNumber + "~" +
                    "RecordNumber: " + SDNData.RecordNumber + "~" +
                    "WordsMatched: " + SDNData.WordsMatched;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;

            return Site;
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
            //0020 - 007E,
            var name = Regex.Replace(NameToSearch, @"[^\u0020-\u007E]+", string.Empty);
            string[] Names = name.Split(' ');
            foreach (SiteDataItemBase item in items)
            {
                if (item.FullName != null)
                {
                    if (item.FullName.Trim().Length > 3)
                    {
                        int Count = 0;
                        string[] TempName = item.FullName.Split(' ');

                        for (int Index = 0; Index < Names.Length; Index++)
                        {
                            var temp = Names[Index];
                            if (temp != null)
                            {
                                if (temp != "")
                                {
                                    for (int Counter = 0; Counter < TempName.Length; Counter++)
                                    {
                                        if (TempName[Counter].ToLower().Equals(Names[Index].ToLower()) &&
                                            TempName[Counter] != null)
                                        //TempName[Counter].ToLower().StartsWith(Name[Index].ToLower()))
                                        {
                                            Count += 1;
                                        }
                                    }
                                }
                            }
                        }
                        if (Count > 1)
                            item.Matched = Count;
                    }
                    
                    }
                }
            }

        #region GetMatchedRecords for a given site

        public SitesIncludedInSearch GetMatchedRecords(Guid? ComplianceFormId, SiteEnum Enum)
        {
            var complainceForm =
                _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var MatchingRecordDetails = complainceForm.SiteDetails.Where(
                site => site.SiteEnum == Enum).FirstOrDefault();

            return MatchingRecordDetails;
        }

        #endregion

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
                    if (Updatedrecord.RowNumber == ExistingRecord.RowNumber)
                    {
                        ExistingRecord.Observation = Updatedrecord.Observation;
                        ExistingRecord.Status = Updatedrecord.Status;
                        ExistingRecord.IssueNumber = Updatedrecord.IssueNumber;
                        ExistingRecord.Matched = Updatedrecord.Matched;
                        ExistingRecord.HiddenStatus = Updatedrecord.HiddenStatus;
                    }
                }
            }
            //ExistingSiteDetails.Findings = Site.Findings;

            _UOW.ComplianceFormRepository.UpdateCollection(ComplianceFormDetails);

            return true;
        }

        #endregion

        #region Old Code, Not in Use
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
