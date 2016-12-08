using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DDAS.Models.Entities.Domain.SiteData;
using System.Text.RegularExpressions;
using Utilities;
using System.IO;
using Utilities.WordTemplate;

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

        public ComplianceForm UpdateSingleSiteFromComplianceForm(string NameToSearch,
            Guid? ComplianceFormId, SiteEnum Enum,
            ILog log)
        {
            NameToSearch = RemoveExtraCharacters(NameToSearch);
            var ScanData = new SiteScanData(_UOW, _SearchEngine);

            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var InvestigatorDetails = form.InvestigatorDetails.Where(
                Investigator => Investigator.Name.ToLower() == NameToSearch.ToLower()).
                FirstOrDefault();

            var ExistingSiteData = InvestigatorDetails.SiteDetails.Where(
            x => x.SiteEnum == Enum).First();

            try
            {
                var SiteData = ScanData.GetSiteScanData(Enum, NameToSearch, log);

                GetMatchStatus(Enum, NameToSearch, SiteData.DataId, ExistingSiteData);

                ExistingSiteData.HasExtractionError = false;
                ExistingSiteData.ExtractionErrorMessage = null;

                _UOW.ComplianceFormRepository.UpdateCollection(form);

                return form;
            }
            catch (Exception e)
            {
                log.WriteLog("Data extract failed. ErrorMessage: " + e.ToString());
                ExistingSiteData.HasExtractionError = true;
                ExistingSiteData.ExtractionErrorMessage = 
                    "Site restore was attempted at: " + 
                    DateTime.Now + 
                    " and was failed";
                return form;
            }
        }
        
        //Patrick 27Nov2016 - check with Pradeep if alt code is available?
        public void AddMandatorySitesToComplianceForm(ComplianceForm compForm, ILog log)
        {
            SearchQuery searchQuery = SearchSites.GetNewSearchQuery();

            var ScanData = new SiteScanData(_UOW, _SearchEngine);

            int SrNo = 0;
            foreach (SearchQuerySite site in searchQuery.SearchSites)
            {
                SrNo += 1;
                
                var siteScan = new SiteScan();

                var siteSourceToAdd = new SiteSource();


                if (site.ExtractionMode.ToLower() == "db")
                    //Patrick-Pradeep 02Dec2016 -  Exception is raised in GetSiteScanData therefore will not return null
                    siteScan = ScanData.GetSiteScanData(site.SiteEnum, "", log);

                if(siteScan != null)
                {
                    siteSourceToAdd.DataExtractedOn = siteScan.DataExtractedOn;
                    siteSourceToAdd.SiteSourceUpdatedOn = siteScan.SiteLastUpdatedOn;
                    //Patrick Is this required?
                    siteSourceToAdd.SiteDataId = siteScan.DataId; 
                }

                siteSourceToAdd.CreatedOn = DateTime.Now;
                //The Id and DisplayPosition are identical when form is created.
                //DisplayPosition may change at the client side.
                siteSourceToAdd.Id = SrNo;
                siteSourceToAdd.DisplayPosition = SrNo;
              
                siteSourceToAdd.SiteEnum = site.SiteEnum;
                siteSourceToAdd.SiteUrl = site.SiteUrl;
                siteSourceToAdd.SiteName = site.SiteName;
                siteSourceToAdd.SiteShortName = site.SiteShortName;
                siteSourceToAdd.IsMandatory = site.Mandatory;
                siteSourceToAdd.IsOptional = site.IsOptional;
                siteSourceToAdd.ExtractionMode = site.ExtractionMode;
                siteSourceToAdd.Deleted = false;

                compForm.SiteSources.Add(siteSourceToAdd);
            }
        }

        public ComplianceForm GetSearchSummary(ComplianceForm form, ILog log)
        {
            form.Active = true;
            if (form.RecId == null)
            {
                form.SearchStartedOn = DateTime.Now;
            }

            var complianceForm = new ComplianceForm();
            var Investigators = new List<InvestigatorSearched>();
            
            foreach(InvestigatorSearched Investigator in form.InvestigatorDetails)
            {
                GetInvestigatorSearchedDetails(Investigator, log);
                Investigators.Add(Investigator);
            }
            form.InvestigatorDetails = Investigators;

            var CreateComplianceForm = new ComplianceFormService(_UOW);
            if (form.RecId == null)
                CreateComplianceForm.CreateComplianceForm(form);
            else
                _UOW.ComplianceFormRepository.UpdateCollection(form);

            //form.RecId = CreateComplianceForm.GetComplianceFormId()

            Investigators = null;
            form.InvestigatorDetails = Investigators;

            return form;
        }

        public InvestigatorSearched GetInvestigatorSearchedDetails(
            InvestigatorSearched Investigator, ILog log)
        {
            var NameToSearch = RemoveExtraCharacters(Investigator.Name);

            var SiteDetails = new List<SitesIncludedInSearch>();

            SiteScanData ScanData = new SiteScanData(_UOW, _SearchEngine);
            var SiteScanList = ScanData.GetSiteScanSummary(NameToSearch, log);

            foreach(SiteScan Site in SiteScanList)
            {
                var SiteIncludedInSearch = new SitesIncludedInSearch();
                GetSiteDetails(Site, NameToSearch, SiteIncludedInSearch);
                SiteDetails.Add(SiteIncludedInSearch);
                Investigator.SiteDetails = SiteDetails;
            }
            return Investigator;
        }

        public SitesIncludedInSearch GetSiteDetails(SiteScan Site, string NameToSearch, 
            SitesIncludedInSearch SiteIncludedInSearch)
        {
            SiteIncludedInSearch.SiteEnum = Site.SiteEnum;

            if (Site.HasErrors == false)
            {
                var TempSite = GetMatchStatus(Site.SiteEnum,
                    NameToSearch, Site.DataId, SiteIncludedInSearch);
            }
            else
            {
                SiteIncludedInSearch.HasExtractionError = true;
                SiteIncludedInSearch.ExtractionErrorMessage = Site.ErrorDescription;
            }

            SiteIncludedInSearch.SiteUrl = Site.SiteUrl;
            SiteIncludedInSearch.SiteName = Site.SiteName;

            return SiteIncludedInSearch;
        }

        //old ?
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

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetDisqualifionProceedingsMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorPageMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSAdministrativeMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabaseSearchPageMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIAPageMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSAMMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSpeciallyDesignatedNationalsMatchCount(NameToSearch,
                        DataId, Site);

                default: throw new Exception("Invalid Enum");
            }
        }

        #region FDADebarSite
        
        //Altv in devp
        public SitesIncludedInSearch GetFDADebarPageMatchCount(string NameToSearch, 
            Guid? DataId, SitesIncludedInSearch Site)
        {
            //var FDASearchResult = GetFDADebarPageMatch(NameToSearch, DataId);

            FDADebarPageSiteData FDASearchResult =
                _UOW.FDADebarPageRepository.FindById(DataId);

            //Site.DataExtractedOn = FDASearchResult.CreatedOn;
            Site.DataExtractedOn = FDASearchResult.SiteLastUpdatedOn;

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

            //Site.DataExtractedOn = ClinicalSiteData.CreatedOn;
            Site.DataExtractedOn = ClinicalSiteData.SiteLastUpdatedOn;

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
                    "IdNumber: " + Investigator.IdNumber + "~" +
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
        #endregion

        #region FDAWarningLetters
        
        public SitesIncludedInSearch GetFDAWarningLettersMatchCount(string NameToSearch, 
            Guid? DataId, SitesIncludedInSearch Site)
        {
            FDAWarningLettersSiteData FDASearchResult =
                _UOW.FDAWarningLettersRepository.FindById(DataId);

            //Site.DataExtractedOn = FDASearchResult.CreatedOn;
            Site.DataExtractedOn = FDASearchResult.SiteLastUpdatedOn;

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
            Site.ExtractionMode = "Live";
            return Site;
        }
        #endregion

        #region ProposalToDebar
        public SitesIncludedInSearch GetProposalToDebarPageMatchCount(string NameToSearch, 
            Guid? DataId, SitesIncludedInSearch Site)
        {
            ERRProposalToDebarPageSiteData ERRSiteData =
                _UOW.ERRProposalToDebarRepository.FindById(DataId);

            //Site.DataExtractedOn = ERRSiteData.CreatedOn;
            Site.DataExtractedOn = ERRSiteData.SiteLastUpdatedOn;

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

            //Site.DataExtractedOn = AdequateListSearchResult.CreatedOn;
            Site.DataExtractedOn = AdequateListSearchResult.SiteLastUpdatedOn;

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
        
        public SitesIncludedInSearch GetDisqualifionProceedingsMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site)
        {
            ClinicalInvestigatorDisqualificationSiteData DisqualificationSiteData =
                _UOW.ClinicalInvestigatorDisqualificationRepository.FindById(DataId);

            //Site.DataExtractedOn = DisqualificationSiteData.CreatedOn;
            Site.DataExtractedOn = DisqualificationSiteData.SiteLastUpdatedOn;

            UpdateMatchStatus(DisqualificationSiteData.DisqualifiedInvestigatorList, NameToSearch);

            var DisqualificationList = DisqualificationSiteData.DisqualifiedInvestigatorList.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (DisqualificationList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = DisqualificationList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (DisqualifiedInvestigator Investigator in DisqualificationList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = Investigator.RowNumber;
                MatchedRecord.Matched = Investigator.Matched;
                MatchedRecord.RecordDetails =
                    //"RowNumber: " + person.RowNumber + "~" +
                    "FullName: " + Investigator.FullName + "~" +
                    "Name: " + Investigator.Name + "~" +
                    "Center: " + Investigator.Center + "~" +
                    "DateOfStatus: " + Investigator.DateOfStatus + "~" +
                    "DateNIDPOEIssued: " + Investigator.DateNIDPOEIssued + "~" +
                    "DateNOOHIssued: " + Investigator.DateNOOHIssued + "~" +
                    "LinkToNIDPOELetter: " + Investigator.LinkToNIDPOELetter + "~" +
                    "LinkToNOOHLetter: " + Investigator.LinkToNOOHLetter;

                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;
            Site.ExtractionMode = "Live";

            return Site;
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

            //Site.DataExtractedOn = CBERSearchResult.CreatedOn;
            Site.DataExtractedOn = CBERSearchResult.SiteLastUpdatedOn;

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

            //Site.DataExtractedOn = PHSSearchResult.CreatedOn;
            Site.DataExtractedOn = PHSSearchResult.SiteLastUpdatedOn;

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

            //Site.DataExtractedOn = ExclusionSearchResult.CreatedOn;
            Site.DataExtractedOn = ExclusionSearchResult.SiteLastUpdatedOn;

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

            //Site.DataExtractedOn = CIASearchResult.CreatedOn;
            Site.DataExtractedOn = CIASearchResult.SiteLastUpdatedOn;

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
            SystemForAwardManagementPageSiteData SAMSearchResult =
                _UOW.SystemForAwardManagementRepository.FindById(DataId);

            //Site.DataExtractedOn = SAMSearchResult.CreatedOn;
            Site.DataExtractedOn = SAMSearchResult.SiteLastUpdatedOn;

            UpdateMatchStatus(SAMSearchResult.SAMSiteData, NameToSearch);

            var SAMList = SAMSearchResult.SAMSiteData.Where(
               SDNListData => SDNListData.Matched > 0).ToList();

            if (SAMList == null)
                return Site;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = SAMList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    Site.FullMatchCount += MatchesFound;
                else
                    Site.PartialMatchCount += MatchesFound;
            }

            List<MatchedRecordsPerSite> MatchedRecords =
                new List<MatchedRecordsPerSite>();

            foreach (SystemForAwardManagement SAMData in SAMList)
            {
                var MatchedRecord = new MatchedRecordsPerSite();
                MatchedRecord.RowNumber = SAMData.RowNumber;
                MatchedRecord.Matched = SAMData.Matched;
                MatchedRecord.RecordDetails =
                    "FullName: " + SAMData.FullName + "~" +
                    "Name: " + SAMData.Entity + "~" +
                    "Duns: " + SAMData.Duns + "~" +
                    "HasActiveExclusion: " + SAMData.HasActiveExclusion + "~" +
                    "ExpirationDate: " + SAMData.ExpirationDate + "~" +
                    "PurposeOfRegistration: " + SAMData.HasActiveExclusion + "~" +
                    "CAGECode: " + SAMData.HasActiveExclusion + "~" +
                    "DoDAAC: " + SAMData.HasActiveExclusion + "~" +
                    "DelinquentFederalDebt: " + SAMData.HasActiveExclusion + "~" +
                    "Classification: " + SAMData.HasActiveExclusion + "~" +
                    "ActivationDate: " + SAMData.HasActiveExclusion + "~" +
                    "TerminationDate: " + SAMData.HasActiveExclusion;
                MatchedRecords.Add(MatchedRecord);
            }
            Site.MatchedRecords = MatchedRecords;
            Site.CreatedOn = DateTime.Now;
            Site.ExtractionMode = "Live";

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

        public SitesIncludedInSearch GetSpeciallyDesignatedNationalsMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site)
        {
            SpeciallyDesignatedNationalsListSiteData SDNSearchResult =
                _UOW.SpeciallyDesignatedNationalsRepository.FindById(DataId);

            //Site.DataExtractedOn = SDNSearchResult.CreatedOn;
            Site.DataExtractedOn = SDNSearchResult.SiteLastUpdatedOn;

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
        #endregion

        void UpdateMatchStatus(IEnumerable<SiteDataItemBase> items, string NameToSearch)
        {
            //0020 - 007E,
            //var name = Regex.Replace(NameToSearch, @"[^\u0020-\u007E]+", string.Empty);
            NameToSearch = RemoveExtraCharacters(NameToSearch);
            string[] Names = NameToSearch.Split(' ');
            foreach (SiteDataItemBase item in items)
            {
                if (item.FullName != null)
                {
                    //if (item.FullName.Trim().Length > 3)
                    //{
                        string FullName = RemoveExtraCharacters(item.FullName);
                        int Count = 0;
                        string[] TempName = FullName.Split(' ');

                        for (int Index = 0; Index < Names.Length; Index++)
                        {
                            var temp = Names[Index];
                            if (temp != null)
                            {
                                if (temp != "")
                                {
                                    for (int Counter = 0; Counter < TempName.Length; Counter++)
                                    {
                                        TempName[Counter] = RemoveExtraCharacters(TempName[Counter]);

                                        bool FullNameComponentIsEqualsToNameComponentAndIsNotNull =
                                        (TempName[Counter].ToLower().Equals(Names[Index].ToLower()) 
                                        && TempName[Counter] != null);

                                        bool FullNameComponentStartWith = (TempName[Counter].ToLower().
                                        StartsWith(Names[Index].ToLower()));

                                        if (FullNameComponentIsEqualsToNameComponentAndIsNotNull)
                                        {
                                            Count += 1;
                                        }
                                    }
                                }
                            }
                        }
                        if (Count > 1)
                            item.Matched = Count;
                    //}
                    
                }
            }
        }

        //Patrick: further refactoring may not require this method.
        public List<MatchedRecord> ConvertToMatchedRecords(IEnumerable<SiteDataItemBase> records)
        {
            List<MatchedRecord> MatchedRecords = new List<MatchedRecord>();

            foreach (SiteDataItemBase record in records)
            {
                var MatchedRecord = new MatchedRecord();
                MatchedRecord.RowNumber = record.RowNumber;
                MatchedRecord.MatchCount = record.Matched;
                MatchedRecord.RecordDetails = record.RecordDetails;

                MatchedRecords.Add(MatchedRecord);
            }
            return MatchedRecords;
        }

        #region GetMatchedRecords for a given site

        public SitesIncludedInSearch GetMatchedRecords(string NameToSearch,
            Guid? ComplianceFormId, SiteEnum Enum)
        {
            NameToSearch = RemoveExtraCharacters(NameToSearch);

            var complainceForm =
                _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var InvestigatorDetails = complainceForm.InvestigatorDetails.Where(
                form => form.Name.ToLower() == NameToSearch.ToLower()).FirstOrDefault();

            var MatchingRecordDetails = InvestigatorDetails.SiteDetails.Where(
                site => site.SiteEnum == Enum).FirstOrDefault();

            return MatchingRecordDetails;
        }

        #endregion

        #region Save and Update Approved/Rejected records
        public bool SaveRecordStatus(string NameToSearch, SitesIncludedInSearch Site,
            Guid? ComplianceFormId)
        {
            NameToSearch = RemoveExtraCharacters(NameToSearch);

            var ComplianceFormDetails = 
                _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (ComplianceFormDetails == null)
                return false;

            var ExistingInvestigator = ComplianceFormDetails.InvestigatorDetails.Where(
                x => x.Name.ToLower() == NameToSearch.ToLower()).FirstOrDefault();

            var ExistingSiteDetails = ExistingInvestigator.SiteDetails.Where(site =>
                site.SiteEnum == Site.SiteEnum).FirstOrDefault();

            ExistingSiteDetails.UpdatedOn = DateTime.Now;

            var ExistingRecords = ExistingSiteDetails.MatchedRecords;

            var ApprovedOrRejectedRecords = Site.MatchedRecords.Where(record =>
            record.Status == "Approve" || record.Status == "Reject").ToList();

            int IssuesFound = 0;
            foreach (MatchedRecordsPerSite Record in ApprovedOrRejectedRecords)
            {
                if (Record.Status.ToLower() == "approve")
                    IssuesFound += 1;
            }
            ExistingSiteDetails.IssuesFound = IssuesFound;
            ExistingSiteDetails.IssuesFoundStatus = "issue(s) identified: " +
                    ExistingSiteDetails.IssuesFound;

            ExistingSiteDetails.MatchedRecords = Site.MatchedRecords;

            int SiteCount = 0;
            var TotalIssuesIdentified = ExistingInvestigator.SiteDetails.Where(x =>
            x.IssuesFound > 0).Count();

            IssuesFound = 0;
            foreach (SitesIncludedInSearch SiteInSearch in ExistingInvestigator.SiteDetails)
            {
                IssuesFound += SiteInSearch.IssuesFound;
                SiteCount += 1;
                ExistingInvestigator.TotalIssuesFound = IssuesFound;
            }
            _UOW.ComplianceFormRepository.UpdateCollection(ComplianceFormDetails);

            return true;
        }

        #endregion

        public string RemoveExtraCharacters(string Name)
        {
            //string CharactersToRemove = ".,/:";
            //return Name.Replace(CharactersToRemove, "");
            return Regex.Replace(Name, "[,.]", "");
        }

        public string AddSpaceBetweenWords(string Name)
        {
            string res = Regex.Replace(Name, "[A-Z]", " $0").Trim();
            return res;
        }

        #region ByPradeep
        //Pradeep 1Dec2016

        public List<ComplianceForm> ReadUploadedFileData(string FilePath, ILog log)
        {
            var ComplianceForms = new List<ComplianceForm>();

            var readUploadedExcelFile = new ReadUploadedExcelFile();

            var DataFromExcelFile = readUploadedExcelFile.
                ReadData(FilePath);

            foreach (RowData row in DataFromExcelFile)
            {
                var form = GetNewComplianceForm(log);

                var Investigators = new List<InvestigatorSearched>();
                var Investigator = new InvestigatorSearched();
                Investigator.Name = row.DetailsInEachRow[0]; //PI
                Investigator.Role = "PI";
                form.ProjectNumber = row.DetailsInEachRow[1];
                form.SponsorProtocolNumber = row.DetailsInEachRow[2];
                form.Address = row.DetailsInEachRow[3];
                form.Country = row.DetailsInEachRow[4];

                Investigators.Add(Investigator);

                for (int Index = 5; Index < row.DetailsInEachRow.Count; Index++)
                {
                    var Inv = new InvestigatorSearched();
                    Inv.Name = row.DetailsInEachRow[Index]; //SIs
                    Inv.Role = "SI";
                    Investigators.Add(Inv);
                }
                form.InvestigatorDetails = Investigators;
                ComplianceForms.Add(form);
            }
            return ComplianceForms;
        }

        #endregion

            #region ByPatrick

            //Patrick 27Nov2016 
        public ComplianceForm GetNewComplianceForm(ILog log)
        {
            ComplianceForm newForm = new ComplianceForm();
            newForm.SearchStartedOn = DateTime.Now;
            AddMandatorySitesToComplianceForm(newForm, log);

            return newForm;
        }

        //3Dec2016
        public MemoryStream GenerateComplianceForm(Guid? ComplianceFormId)
        {
            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var UtilitiesObject = new ReplaceTextFromWordTemplate();


            var FileName = form.InvestigatorDetails.FirstOrDefault().Name + ".docx";

            return UtilitiesObject.ReplaceTextFromWord(form, FileName);
        }

        public string GenerateComplianceFormAlt(Guid? ComplianceFormId)
        {
            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var UtilitiesObject = new ReplaceTextFromWordTemplate();

            var FileName = @"C:\Development\p926-ddas\DDAS.API\Downloads\";

            var PI = RemoveExtraCharacters(form.InvestigatorDetails.FirstOrDefault().Name);

            FileName += PI + ".docx";

            var stream = UtilitiesObject.ReplaceTextFromWord(form, FileName);

            return @"Downloads\" + PI + ".docx";
        }

        public ComplianceForm ScanUpdateComplianceForm(ComplianceForm frm, ILog log)
        {
            //Creates or Updates form
            //Remove Inv + Sites if marked for delete:
            RemoveDeleteMarkedItemsFromFormCollections(frm);
            
            //Check and Search if required:
            AddMatchingRecords(frm, log);


            if(frm.RecId != null)
                _UOW.ComplianceFormRepository.UpdateCollection(frm); //Update
            else
                _UOW.ComplianceFormRepository.Add(frm); //Insert

            return frm;
        }
        
        public ComplianceForm UpdateComplianceForm(ComplianceForm frm)
        {
            //Creates or Updates form
            //Remove Inv + Sites if marked for delete:
            RemoveDeleteMarkedItemsFromFormCollections(frm); 
            //Patrick 02Dec2016:
            if (frm.RecId == null){
                _UOW.ComplianceFormRepository.Add(frm);
            }
            else
            {
                UpdateFindings(frm);
                _UOW.ComplianceFormRepository.UpdateCollection(frm);
            }
            return frm;
        }
        
        public List<PrincipalInvestigatorDetails> getPrincipalInvestigatorNComplianceFormDetails()
        {
            var retList = new List<PrincipalInvestigatorDetails>();

            var compForms = _UOW.ComplianceFormRepository.GetAll();

            foreach (ComplianceForm compForm in compForms)
            {
                var form = _UOW.ComplianceFormRepository.FindById(compForm.RecId);

                var item = new PrincipalInvestigatorDetails();
                //item.Active = compForm.Active;
                item.Address = compForm.Address;
                item.Country = compForm.Country;
                item.ProjectNumber = compForm.ProjectNumber;
                item.SponsorProtocolNumber = compForm.SponsorProtocolNumber;
                item.RecId = compForm.RecId;
                item.SearchStartedOn = compForm.SearchStartedOn;
                if (compForm.InvestigatorDetails.Count > 0)
                {
                    item.PrincipalInvestigator = compForm.InvestigatorDetails.FirstOrDefault().Name;
                }

                
                item.Status = "";
               

                retList.Add(item);
            }
            return retList;
        }

        public InvestigatorSearched getInvestigatorSiteSummary(string compFormId, int InvestigatorId)
        {
            Guid gCompFormId = Guid.Parse(compFormId);
            var compForm = _UOW.ComplianceFormRepository.FindById(gCompFormId);
            if (compForm == null)
            {
                return null;
            }
            else
            {
                return getInvestigatorSiteSummary(compForm, InvestigatorId);
            }
         }

        public InvestigatorSearched getInvestigatorSiteSummary(ComplianceForm compForm, int InvestigatorId)
        {
            InvestigatorSearched retInv = new InvestigatorSearched();
            // inv.SitesSearched.Find(x => x.siteEnum == site.SiteEnum);
            InvestigatorSearched invInCompForm = 
                compForm.InvestigatorDetails.Find(
                x => x.Id == InvestigatorId);

            if (invInCompForm == null)
            {
                return null;
            }
            //?? for manual sites ???
            foreach (SiteSource site in compForm.SiteSources)
            {
                var searchStatus = new SiteSearchStatus();
                searchStatus.DisplayPosition = site.DisplayPosition;
                searchStatus.siteEnum = site.SiteEnum;
                searchStatus.SiteUrl = site.SiteUrl;
                searchStatus.SiteName = site.SiteName;
                
                var searchStatusInCompForm = 
                    invInCompForm.SitesSearched.Find(
                    x => x.siteEnum == site.SiteEnum);

                if (searchStatusInCompForm == null)
                {
                    searchStatus.ExtractionErrorMessage = "Site not searched";
                }
                else
                {
                    searchStatus.FullMatchCount = searchStatusInCompForm.FullMatchCount;
                    searchStatus.IssuesFound = searchStatusInCompForm.IssuesFound;
                    searchStatus.PartialMatchCount = searchStatusInCompForm.PartialMatchCount;
                    searchStatus.ReviewCompleted = searchStatusInCompForm.ReviewCompleted;

                }

                retInv.SitesSearched.Add(searchStatus);
            }

            return retInv;
        }

        public ComplianceForm UpdateFindings(ComplianceForm form)
        {
            foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
            {
                Investigator.TotalIssuesFound = 0;

                foreach (SiteSearchStatus searchStatus in Investigator.SitesSearched)
                {
                    var ListOfFindings = form.Findings;

                    var Findings = ListOfFindings.Where(
                        x => x.SiteEnum == searchStatus.siteEnum).ToList();

                    int IssuesFound = 0;
                    int InvId = 0;
                    foreach(Finding Finding in Findings)
                    {
                        if (Finding != null && Finding.IsAnIssue &&
                            Finding.InvestigatorSearchedId == Investigator.Id)
                        {
                            InvId = Finding.InvestigatorSearchedId;
                            IssuesFound += 1;
                            searchStatus.IssuesFound = IssuesFound;
                        }
                    }
                    Investigator.TotalIssuesFound += IssuesFound;

                    var Site = form.SiteSources.Find
                        (x => x.SiteEnum == searchStatus.siteEnum);

                    if(IssuesFound > 0 && Investigator.Id == InvId)
                        Site.IssuesIdentified = true;
                }
            }
            return form;
        }

        private void AddMatchingRecords(ComplianceForm frm, ILog log)
        {
            int InvestigatorId = 1;

            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                var ListOfSiteSearchStatus = new List<SiteSearchStatus>();    

                foreach (SiteSource site in frm.SiteSources)
                {
                    SiteSearchStatus searchStatus = null;

                    if (inv.SitesSearched != null)
                    {
                        searchStatus =
                            inv.SitesSearched.Find(x => x.siteEnum == site.SiteEnum);
                    }
                        
                    if (!site.IsOptional &&
                        searchStatus == null || searchStatus.HasExtractionError == true)
                    {
                        if (searchStatus == null)
                        {
                            searchStatus = new SiteSearchStatus();
                            searchStatus.siteEnum = site.SiteEnum;
                        }

                        try
                        {
                            // Not processed,  search now.
                            //var matchedRecords = GetMatchedRecords(
                            //    site.SiteEnum, site.SiteDataId, inv.Name, log);

                            var matchedRecords = GetMatchedRecords(
                                site, searchStatus, inv.Name, log);

                            
                            //To-Do: convert matchedRecords to Findings


                            inv.Id = InvestigatorId;


                            foreach (MatchedRecord rec in matchedRecords)
                            {
                                var finding = new Finding();
                                finding.MatchCount = rec.MatchCount;
                                finding.InvestigatorSearchedId = inv.Id;
                                finding.SourceNumber = site.DisplayPosition;
                                finding.SiteEnum = site.SiteEnum; //Pradeep 2Dec2016
                                finding.RecordDetails = rec.RecordDetails;
                                finding.RowNumberInSource = rec.RowNumber;

                                //Patrick 04Dec2016
                                finding.IsMatchedRecord = true;
                                finding.DateOfInspection = site.SiteSourceUpdatedOn;
                                finding.InvestigatorName = inv.Name;
                                frm.Findings.Add(finding);
                            }

                            searchStatus.HasExtractionError = false;
                            searchStatus.ExtractionErrorMessage = "";
                        }
                        catch (Exception ex)
                        {
                            searchStatus.HasExtractionError = true;
                            searchStatus.ExtractionErrorMessage = "Data Extraction not successful";
                            log.WriteLog("Data extraction failed. Details: " + ex.Message);
                            // Log -- ex.Message + ex.InnerException.Message
                        }
                        finally
                        {
                            ListOfSiteSearchStatus.Add(searchStatus);
                        }
                    }
                }
                inv.SitesSearched = ListOfSiteSearchStatus;
                InvestigatorId += 1;
            }
        }

        private void RemoveDeleteMarkedItemsFromFormCollections(ComplianceForm frm)
        {
            frm.InvestigatorDetails.RemoveAll(x => x.Deleted == true);

            //Remove Delete Site's SiteSearchStatus from each remaining Investigator
            var sitesRemoved = frm.SiteSources.Where(x => x.Deleted == true);
            foreach (SiteSource site in sitesRemoved)
            {
                foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
                {
                    inv.SitesSearched.RemoveAll(x => x.siteEnum == site.SiteEnum);
                }
            }
            frm.SiteSources.RemoveAll(x => x.Deleted == true);

            //findings -  Delete Source = site and selected = false, manual is set on the client side
            frm.Findings.RemoveAll(x => x.IsMatchedRecord == false && x.Selected == false);
        }

        public List<MatchedRecord> GetMatchedRecords(SiteSource site, 
            SiteSearchStatus searchStatus, string NameToSearch, ILog log)
        {
            switch (site.SiteEnum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchedRecords(site.SiteDataId, 
                        NameToSearch, searchStatus);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInvestigatorPageMatchedRecords(site.SiteDataId, 
                        NameToSearch, searchStatus);

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLettersPageMatchedRecords(site.SiteDataId,
                        NameToSearch, searchStatus);

                case SiteEnum.ERRProposalToDebarPage:
                    return GetERRProposalToDebarPageMatchedRecords(site.SiteDataId,
                        NameToSearch, searchStatus);

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssurancePageMatchedRecords(site.SiteDataId,
                        NameToSearch, searchStatus);

                //case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                //    return GetDisqualifionProceedingsMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorPageMatchedRecords(site.SiteDataId,
                        NameToSearch, searchStatus);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSAdministrativeActionPageMatchedRecords(site.SiteDataId,
                        NameToSearch, searchStatus);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabasePageMatchedRecords(site.SiteDataId,
                        NameToSearch, searchStatus);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIAPageMatchedRecords(site.SiteDataId,
                        NameToSearch, searchStatus);

                //case SiteEnum.SystemForAwardManagementPage:
                //    return GetSAMMatchCount(NameToSearch, DataId, Site);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSDNPageMatchedRecords(site.SiteDataId,
                        NameToSearch, searchStatus);

                default: throw new Exception("Invalid Enum");
            }
        }

        public void GetFullAndPartialMatchCount(
            IEnumerable<SiteDataItemBase> DebarList, SiteSearchStatus searchStatus,
            string NameToSearch)
        {
            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = DebarList.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound > 0 && counter == Name.Length)
                    searchStatus.FullMatchCount += MatchesFound;
                else
                    searchStatus.PartialMatchCount += MatchesFound;
            }
        }

        //Alt for GetFDADebarPageMatchCount  
        public List<MatchedRecord> GetFDADebarPageMatchedRecords(Guid? SiteDataId, 
            string NameToSearch, SiteSearchStatus searchStatus)
        {
            FDADebarPageSiteData FDASearchResult =
                _UOW.FDADebarPageRepository.FindById(SiteDataId);

            UpdateMatchStatus(FDASearchResult.DebarredPersons, NameToSearch);  //updates list with match count

            var DebarList = FDASearchResult.DebarredPersons.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (DebarList == null)
                return null;

            GetFullAndPartialMatchCount(DebarList, searchStatus, NameToSearch); //updates full and partial match counts

            //Patrick - review later - move full/partial count to ...
            //string[] Name = NameToSearch.Split(' ');

            //for (int counter = 1; counter <= Name.Length; counter++)
            //{
            //    int MatchesFound = DebarList.Where(
            //        x => x.Matched == counter).Count();
            //    if (MatchesFound > 0 && counter == Name.Length)
            //        Site.FullMatchCount += MatchesFound;
            //    else
            //        Site.PartialMatchCount += MatchesFound;
            //}


            //Patrick: Further refactoring possible, ConvertToMatchedRecords may not be required:
            return ConvertToMatchedRecords(DebarList);

            //List<MatchedRecord> MatchedRecords =
            //    new List<MatchedRecord>();

            //foreach (DebarredPerson person in DebarList)
            //{
            //    var MatchedRecord = new MatchedRecord();
            //    MatchedRecord.RowNumber = person.RowNumber;
            //    MatchedRecord.MatchCount = person.Matched;
            //    MatchedRecord.RecordDetails = person.RecordDetails;


            //    MatchedRecords.Add(MatchedRecord);
            //}

            //Site.MatchedRecords = MatchedRecords;
            //Site.CreatedOn = DateTime.Now;

            //return Site;

        }

        //added on 1Dec2016 Pradeep, Yet to add Live sites below..

        public List<MatchedRecord> GetClinicalInvestigatorPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, SiteSearchStatus searchStatus)
        {
            ClinicalInvestigatorInspectionSiteData CIILSearchResult =
                _UOW.ClinicalInvestigatorInspectionListRepository.FindById(SiteDataId);

            UpdateMatchStatus(CIILSearchResult.ClinicalInvestigatorInspectionList, NameToSearch);  //updates list with match count

            var ClinicalInvestigatorList = CIILSearchResult.ClinicalInvestigatorInspectionList.Where(
               ClinicalList => ClinicalList.Matched > 0).ToList();

            if (ClinicalInvestigatorList == null)
                return null;

            GetFullAndPartialMatchCount(ClinicalInvestigatorList, searchStatus, NameToSearch);

            return ConvertToMatchedRecords(ClinicalInvestigatorList);
        }

        public List<MatchedRecord> GetFDAWarningLettersPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, SiteSearchStatus searchStatus)
        {

            _SearchEngine.Load(SiteEnum.FDAWarningLettersPage, NameToSearch, "");
            var siteData = _SearchEngine.SiteData;

            //UpdateMatchStatus(FDAWarningSearchResult.FDAWarningLetterList, NameToSearch);  //updates list with match count
            UpdateMatchStatus(siteData, NameToSearch);  //updates list with match count

            //var FDAWarningLetterList = 
            //    FDAWarningSearchResult.FDAWarningLetterList.Where(
            //    FDAList => FDAList.Matched > 0).ToList();

            //if (FDAWarningLetterList == null)
            //    return null;

            if (siteData == null)
                return null;

            GetFullAndPartialMatchCount(siteData, searchStatus, NameToSearch);

            return ConvertToMatchedRecords(siteData);
        }

        public List<MatchedRecord> GetERRProposalToDebarPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, SiteSearchStatus searchStatus)
        {
            ERRProposalToDebarPageSiteData ERRSearchResult =
                _UOW.ERRProposalToDebarRepository.FindById(SiteDataId);

            UpdateMatchStatus(ERRSearchResult.ProposalToDebar, NameToSearch);  //updates list with match count

            var ERRList = ERRSearchResult.ProposalToDebar.Where(
               ErrList => ErrList.Matched > 0).ToList();

            if (ERRList == null)
                return null;

            GetFullAndPartialMatchCount(ERRList, searchStatus, NameToSearch);

            return ConvertToMatchedRecords(ERRList);
        }

        public List<MatchedRecord> GetAdequateAssurancePageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, SiteSearchStatus searchStatus)
        {
            AdequateAssuranceListSiteData AdequateAssuranceSearchResult =
                _UOW.AdequateAssuranceListRepository.FindById(SiteDataId);

            UpdateMatchStatus(AdequateAssuranceSearchResult.AdequateAssurances, NameToSearch);  //updates list with match count

            var AdequateAssuranceList = 
                AdequateAssuranceSearchResult.AdequateAssurances.Where(
                AssuranceList => AssuranceList.Matched > 0).ToList();

            if (AdequateAssuranceList == null)
                return null;

            GetFullAndPartialMatchCount(AdequateAssuranceList, searchStatus, NameToSearch);

            return ConvertToMatchedRecords(AdequateAssuranceList);
        }

        public List<MatchedRecord> GetCBERClinicalInvestigatorPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, SiteSearchStatus searchStatus)
        {
            CBERClinicalInvestigatorInspectionSiteData CBERSearchResult =
                _UOW.CBERClinicalInvestigatorRepository.FindById(SiteDataId);

            UpdateMatchStatus(CBERSearchResult.ClinicalInvestigator, NameToSearch);  //updates list with match count

            var ClinicalInvestigatorList = CBERSearchResult.ClinicalInvestigator.Where(
               CBERList => CBERList.Matched > 0).ToList();

            if (ClinicalInvestigatorList == null)
                return null;

            GetFullAndPartialMatchCount(ClinicalInvestigatorList, searchStatus, NameToSearch);

            return ConvertToMatchedRecords(ClinicalInvestigatorList);
        }

        public List<MatchedRecord> GetExclusionDatabasePageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, SiteSearchStatus searchStatus)
        {
            ExclusionDatabaseSearchPageSiteData ExclusionSearchResult =
                _UOW.ExclusionDatabaseSearchRepository.FindById(SiteDataId);

            UpdateMatchStatus(ExclusionSearchResult.ExclusionSearchList, NameToSearch);  //updates list with match count

            var ExclusionDBList = ExclusionSearchResult.ExclusionSearchList.Where(
               ExclusionList => ExclusionList.Matched > 0).ToList();

            if (ExclusionDBList == null)
                return null;

            GetFullAndPartialMatchCount(ExclusionDBList, searchStatus, NameToSearch);

            return ConvertToMatchedRecords(ExclusionDBList);
        }

        public List<MatchedRecord> GetPHSAdministrativeActionPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, SiteSearchStatus searchStatus)
        {
            PHSAdministrativeActionListingSiteData PHSSearchResult =
                _UOW.PHSAdministrativeActionListingRepository.FindById(SiteDataId);

            UpdateMatchStatus(PHSSearchResult.PHSAdministrativeSiteData, NameToSearch);  //updates list with match count

            var PHSList = PHSSearchResult.PHSAdministrativeSiteData.Where(
               PHSData => PHSData.Matched > 0).ToList();

            if (PHSList == null)
                return null;

            GetFullAndPartialMatchCount(PHSList, searchStatus, NameToSearch);

            return ConvertToMatchedRecords(PHSList);
        }

        public List<MatchedRecord> GetCIAPageMatchedRecords(Guid? SiteDataId,
                    string NameToSearch, SiteSearchStatus searchStatus)
        {
            CorporateIntegrityAgreementListSiteData CIASearchResult =
                _UOW.CorporateIntegrityAgreementRepository.FindById(SiteDataId);

            UpdateMatchStatus(CIASearchResult.CIAListSiteData, NameToSearch);  //updates list with match count

            var CIAList = CIASearchResult.CIAListSiteData.Where(
               CIAData => CIAData.Matched > 0).ToList();

            if (CIAList == null)
                return null;

            GetFullAndPartialMatchCount(CIAList, searchStatus, NameToSearch);

            return ConvertToMatchedRecords(CIAList);
        }

        public List<MatchedRecord> GetSDNPageMatchedRecords(Guid? SiteDataId,
                    string NameToSearch, SiteSearchStatus searchStatus)
        {
            SpeciallyDesignatedNationalsListSiteData SDNSearchResult =
                _UOW.SpeciallyDesignatedNationalsRepository.FindById(SiteDataId);

            UpdateMatchStatus(SDNSearchResult.SDNListSiteData, NameToSearch);  //updates list with match count

            var SDNList = SDNSearchResult.SDNListSiteData.Where(
               SDNData => SDNData.Matched > 0).ToList();

            if (SDNList == null)
                return null;

            GetFullAndPartialMatchCount(SDNList, searchStatus, NameToSearch);

            return ConvertToMatchedRecords(SDNList);

        }

        #endregion
    }
}
