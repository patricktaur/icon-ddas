﻿using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utilities;
using Utilities.WordTemplate;

namespace DDAS.Services.Search
{
    public class ComplianceFormService : ISearchService
    {
        private IUnitOfWork _UOW;
        private ISearchEngine _SearchEngine;

        private const int MatchCountLowerLimit = 2;

        public ComplianceFormService(IUnitOfWork uow, ISearchEngine SearchEngine)
        {
            _UOW = uow;
            _SearchEngine = SearchEngine;
        }

        #region ComplianceFormCreationNUpdates

        //Patrick 27Nov2016 
        public ComplianceForm GetNewComplianceForm(ILog log, string UserName)
        {
            ComplianceForm newForm = new ComplianceForm();
            newForm.AssignedTo = UserName;
            newForm.SearchStartedOn = DateTime.Now;
            AddMandatorySitesToComplianceForm(newForm, log);

            return newForm;
        }

        #region ByPradeep
        //13Jan2017
        public List<List<string>> ReadDataFromExcelFile(string FilePath)
        {
            var readExcelData = new ReadUploadedExcelFile();
            return readExcelData.ReadData(FilePath);
        }
        //Pradeep 1Dec2016
        public List<ComplianceForm> ReadUploadedFileData(List<List<string>> DataFromExcelFile, 
            ILog log,
            string UserName, 
            string FilePath)
        {
            var ComplianceForms = new List<ComplianceForm>();

            //foreach (RowData row in DataFromExcelFile)
            for(int Counter = 0; Counter < DataFromExcelFile.Count; Counter++)
            {
                var DetailsInEachRow = DataFromExcelFile[Counter];

                var form = GetNewComplianceForm(log, UserName);

                form.AssignedTo = UserName;

                form.UploadedFileName = Path.GetFileName(FilePath);

                var Investigators = new List<InvestigatorSearched>();
                var Investigator = new InvestigatorSearched();
                Investigator.Name = DetailsInEachRow[0];
                Investigator.MedicalLiceseNumber = DetailsInEachRow[1];
                Investigator.Qualification = DetailsInEachRow[2];
                Investigator.Role = "Principal";
                form.ProjectNumber = DetailsInEachRow[3];
                form.SponsorProtocolNumber = DetailsInEachRow[4];
                form.Institute = DetailsInEachRow[5];
                form.Address = DetailsInEachRow[6];
                form.Country = DetailsInEachRow[7];

                Investigators.Add(Investigator);

                for (int Index = 8; Index < DetailsInEachRow.Count; Index++)
                {
                    var Inv = new InvestigatorSearched();
                    Inv.Name = DetailsInEachRow[Index]; //SIs
                    Inv.Role = "Sub";
                    Inv.MedicalLiceseNumber = DetailsInEachRow[Index + 1];
                    Inv.Qualification = DetailsInEachRow[Index + 2];
                    Investigators.Add(Inv);
                    Index += 2;
                }
                form.InvestigatorDetails = Investigators;
                ComplianceForms.Add(form);
            }
            return ComplianceForms;
        }
        #endregion

        public void UpdateAssignedToData(string AssignedTo, bool Active,
            Guid? RecId)
        {
            var form = _UOW.ComplianceFormRepository.FindById(RecId);
            form.AssignedTo = AssignedTo;
            form.Active = Active;
            _UOW.ComplianceFormRepository.UpdateCollection(form);
        }

        public ComplianceForm ScanUpdateComplianceForm(ComplianceForm frm, ILog log)
        {
            //Creates or Updates form
            //Remove Inv + Sites if marked for delete:
            RemoveDeleteMarkedItemsFromFormCollections(frm);

            AddMissingSearchStatusRecords(frm);
            //Check and Search if required:
            AddMatchingRecords(frm, log);

            RollUpSummary(frm);

            if (frm.RecId != null)
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

            AddMissingSearchStatusRecords(frm);

            RollUpSummary(frm);
            //Patrick 02Dec2016:
            if (frm.RecId == null)
            {
                _UOW.ComplianceFormRepository.Add(frm);
            }
            else
            {
                _UOW.ComplianceFormRepository.UpdateCollection(frm);
            }
            return frm;
        }

        //Patrick 27Nov2016 - check with Pradeep if alt code is available?
        public void AddMandatorySitesToComplianceForm(ComplianceForm compForm, ILog log)
        {
            List<SearchQuerySite> siteSources = SearchSites.GetNewSearchQuery();

            var ScanData = new SiteScanData(_UOW, _SearchEngine);

            int SrNo = 0;
            foreach (SearchQuerySite site in siteSources.Where(x => x.Mandatory == true))
            {
                SrNo += 1;

                var siteScan = new SiteScan();

                var siteSourceToAdd = new SiteSource();


                if (site.ExtractionMode.ToLower() == "db")
                    //Patrick-Pradeep 02Dec2016 -  Exception is raised in GetSiteScanData therefore will not return null
                    siteScan = ScanData.GetSiteScanData(site.SiteEnum, "", log);

                if (siteScan != null)
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
                siteSourceToAdd.ExtractionMode = site.ExtractionMode;
                siteSourceToAdd.Deleted = false;

                compForm.SiteSources.Add(siteSourceToAdd);
            }
        }

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
                MatchedRecord.Links = record.Links;
                MatchedRecords.Add(MatchedRecord);
            }
            return MatchedRecords;
        }

        public ComplianceForm RollUpSummary(ComplianceForm form)  //previously UpdateFindings
        {

            //public bool ExtractionErrorSiteCount { get; set; }
            //public int IssuesFoundSiteCount { get; set; }
            //public int ReviewCompletedSiteCount { get; set; }

            int FullMatchesFoundInvestigatorCount = 0;
            int PartialMatchesFoundInvestigatorCount = 0;

            int IssuesFoundInvestigatorCount = 0;
            int ReviewCompletedInvestigatorCount = 0;
            int ExtractedInvestigatorCount = 0;

            //Pradeep 20Dec2016
            form.PartialMatchesFoundInvestigatorCount = 0;
            form.FullMatchesFoundInvestigatorCount = 0;
            form.IssuesFoundInvestigatorCount = 0;
            form.ReviewCompletedInvestigatorCount = 0;
            form.ExtractedInvestigatorCount = 0;

            foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
            {
                Investigator.TotalIssuesFound = 0;

                int PartialMatchSiteCount = 0;
                int FullMatchSiteCount = 0;
                int IssuesFoundSiteCount = 0;
                int ReviewCompletedSiteCount = 0;

                //Pradeep 20Dec2016
                Investigator.Sites_PartialMatchCount = 0;
                Investigator.Sites_FullMatchCount = 0;
                Investigator.IssuesFoundSiteCount = 0;
                Investigator.ReviewCompletedSiteCount = 0;


                foreach (SiteSearchStatus searchStatus in Investigator.SitesSearched)
                {

                    searchStatus.IssuesFound = 0; //Pradeep 20Dec2016

                    var ListOfFindings = form.Findings;

                    var Findings = ListOfFindings.Where(
                        x => x.SiteEnum == searchStatus.siteEnum).ToList();

                    int IssuesFound = 0;
                    int InvId = 0;
                    foreach (Finding Finding in Findings)
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

                    if (IssuesFound > 0 && Investigator.Id == InvId)
                        Site.IssuesIdentified = true;
                    else
                        Site.IssuesIdentified = false;

                    //Rollup summary:
                    if (searchStatus.PartialMatchCount > 0)
                    {
                        PartialMatchSiteCount += 1;
                    }
                    if (searchStatus.FullMatchCount > 0)
                    {
                        FullMatchSiteCount += 1;
                    }
                    if (searchStatus.IssuesFound > 0)
                    {
                        IssuesFoundSiteCount += 1;
                    }
                    if (searchStatus.ReviewCompleted)
                    {
                        ReviewCompletedSiteCount += 1;
                    }
                }
                Investigator.Sites_PartialMatchCount = PartialMatchSiteCount;
                Investigator.Sites_FullMatchCount = FullMatchSiteCount;
                Investigator.IssuesFoundSiteCount = IssuesFoundSiteCount;
                Investigator.ReviewCompletedSiteCount = ReviewCompletedSiteCount;

                if (Investigator.Sites_PartialMatchCount > 0)
                {
                    PartialMatchesFoundInvestigatorCount += 1;
                }

                if (Investigator.Sites_FullMatchCount > 0)
                {
                    FullMatchesFoundInvestigatorCount += 1;
                }

                if (Investigator.IssuesFoundSiteCount > 0)
                {
                    IssuesFoundInvestigatorCount += 1;
                }
                if (Investigator.ReviewCompletedSiteCount > 0)
                {
                    ReviewCompletedInvestigatorCount += 1;
                }
            }

            form.PartialMatchesFoundInvestigatorCount = PartialMatchesFoundInvestigatorCount;
            form.FullMatchesFoundInvestigatorCount = FullMatchesFoundInvestigatorCount;
            form.IssuesFoundInvestigatorCount = IssuesFoundInvestigatorCount;
            form.ReviewCompletedInvestigatorCount = ReviewCompletedInvestigatorCount;
            form.ExtractedInvestigatorCount = 0;

            return form;
        }

        private void AddMatchingRecords(ComplianceForm frm, ILog log)
        {
            int InvestigatorId = 1;
            frm.ExtractedOn = DateTime.Now; //last extracted on
            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                var ListOfSiteSearchStatus = new List<SiteSearchStatus>();

                var InvestigatorName = RemoveExtraCharacters(inv.Name);

                var ComponentsInInvestigatorName =
                    InvestigatorName.Split(' ').Count();

                inv.ExtractedOn = DateTime.Now;
                inv.HasExtractionError = true; // until set to false.
                inv.IssuesFoundSiteCount = 0;
                inv.ReviewCompletedCount = 0;

                bool HasExtractionError = false; //for rollup value for Investigator
                foreach (SiteSource siteSource in frm.SiteSources)
                {
                    SiteSearchStatus searchStatus = null;

                    if (inv.SitesSearched != null)
                        searchStatus =
                            inv.SitesSearched.Find(x => x.siteEnum == siteSource.SiteEnum);

                    bool searchRequired = false;

                    if (searchStatus == null)
                    {
                        //SearchStatus records must be added to each Investigator before calling AddMatchingRecords
                        throw new Exception("Coding Error: Search Status is not added to Project-Investigator:"
                            + frm.ProjectNumber + "-" + inv.Name);
                    }

                    if (searchStatus.HasExtractionError == true)
                    {
                        searchRequired = true;
                    }
                    if (searchStatus.ExtractedOn == null)
                    {
                        searchRequired = true;
                    }

                    if (searchRequired == true)
                    {
                        try
                        {
                            //clear previously added matching records.
                            frm.Findings.RemoveAll(x => (x.InvestigatorSearchedId == inv.Id) && (x.SiteEnum == searchStatus.siteEnum) && x.IsMatchedRecord == true);

                            var MatchedRecords = GetMatchedRecords(siteSource, InvestigatorName, log,
                                ComponentsInInvestigatorName);

                            GetFullAndPartialMatchCount(MatchedRecords, searchStatus, ComponentsInInvestigatorName);

                            inv.Sites_PartialMatchCount +=
                                searchStatus.PartialMatchCount;
                            inv.Sites_FullMatchCount +=
                                searchStatus.FullMatchCount;

                            inv.Id = InvestigatorId;

                            //To-Do: convert matchedRecords to Findings
                            foreach (MatchedRecord rec in MatchedRecords)
                            {
                                var finding = new Finding();
                                finding.MatchCount = rec.MatchCount;
                                finding.InvestigatorSearchedId = inv.Id;
                                finding.SourceNumber = siteSource.DisplayPosition;
                                finding.SiteEnum = siteSource.SiteEnum; //Pradeep 2Dec2016

                                finding.RecordDetails = rec.RecordDetails;
                                finding.RowNumberInSource = rec.RowNumber;

                                //Patrick 04Dec2016
                                finding.IsMatchedRecord = true;
                                finding.DateOfInspection = siteSource.SiteSourceUpdatedOn;
                                finding.InvestigatorName = inv.Name;
                                finding.Links = rec.Links;
 
                                frm.Findings.Add(finding);
                            }
                            
                            //Review:
                            //siteSource.SiteSourceUpdatedOn' is the date of update at the time of creation of CompForm
                            //
                            //replace  '= siteSource.SiteSourceUpdatedOn' SiteSourceUpdatedOn at the time of data extraction
                            searchStatus.SiteSourceUpdatedOn = siteSource.SiteSourceUpdatedOn;
                            searchStatus.HasExtractionError = false;
                            searchStatus.ExtractionErrorMessage = "";
                        }
                        catch (Exception ex)
                        {
                            HasExtractionError = true;  //for rollup to investigator
                            searchStatus.HasExtractionError = true;
                            searchStatus.ExtractionErrorMessage = "Data Extraction not successful";
                            log.WriteLog("Data extraction failed. Details: " + ex.Message);
                            // Log -- ex.Message + ex.InnerException.Message
                        }
                        finally
                        {
                            searchStatus.ExtractedOn = DateTime.Now;
                            ListOfSiteSearchStatus.Add(searchStatus);
                        }
                    }
                }
                inv.HasExtractionError = HasExtractionError;
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

        private void AddMissingSearchStatusRecords(ComplianceForm frm)
        {
            //Adds SearchStatus Records if not found for an investigator.
            //This will happen when new investigator is added or new sites are added from the client side

            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                foreach (SiteSource site in frm.SiteSources)
                {
                    SiteSearchStatus searchStatus = inv.SitesSearched.Find(x => x.siteEnum == site.SiteEnum);
                    if (searchStatus == null)
                    {
                        searchStatus = new SiteSearchStatus();
                        searchStatus.siteEnum = site.SiteEnum;
                        searchStatus.SiteName = site.SiteName;
                        searchStatus.SiteUrl = site.SiteUrl;
                        searchStatus.DisplayPosition = site.DisplayPosition;
                        searchStatus.ExtractionMode = site.ExtractionMode;
                        inv.SitesSearched.Add(searchStatus);
                    }
                }
            }
        }
 
        public List<MatchedRecord> GetMatchedRecords(SiteSource site,
            string NameToSearch, ILog log,
            int ComponentsInInvestigatorName)
        {
            switch (site.SiteEnum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInvestigatorPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLettersPageMatchedRecords(
                        site.SiteDataId, NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.ERRProposalToDebarPage:
                    return GetERRProposalToDebarPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssurancePageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetClinicalInvestigatorDisqualificationPageMatchedRecords(
                        site.SiteDataId, NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSAdministrativeActionPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabasePageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIAPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSAMPageMatchedRecords(site.SiteDataId, NameToSearch,
                        ComponentsInInvestigatorName);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSDNPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName);

                default: throw new Exception("Invalid Enum");
            }
        }

        public void GetFullAndPartialMatchCount(List<MatchedRecord> Records,
            SiteSearchStatus SearchStatus, int ComponentsInInvestigatorName)
        {
            SearchStatus.FullMatchCount = 0;
            SearchStatus.PartialMatchCount = 0;
            foreach (MatchedRecord record in Records)
            {
                if (record.MatchCount >= ComponentsInInvestigatorName)
                    SearchStatus.FullMatchCount += 1;
                else
                    SearchStatus.PartialMatchCount += 1;
            }
        }

        #endregion

        #region ComplianceFormQueries

        public List<PrincipalInvestigator> getAllPrincipalInvestigators()
        {
            var retList = new List<PrincipalInvestigator>();

            var compForms = _UOW.ComplianceFormRepository.GetAll();

            foreach (ComplianceForm compForm in compForms)
            {
                var item = getPrincipalInvestigators(compForm);
                retList.Add(item);
            }
            return retList;
        }

        public List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active)
        {
            var retList = new List<PrincipalInvestigator>();
            List<ComplianceForm> compForms;
            if (AssignedTo != null && AssignedTo.Length > 0)
            {
                compForms = _UOW.ComplianceFormRepository.GetAll().Where(x => x.AssignedTo == AssignedTo).ToList();
            }
            else
            {
                compForms = _UOW.ComplianceFormRepository.GetAll();
            }

            foreach (ComplianceForm compForm in compForms.Where(x => x.Active == Active))
            {
                var item = getPrincipalInvestigators(compForm);
                retList.Add(item);
            }
            return retList;

            //return null;
        }

        public List<PrincipalInvestigator> getPrincipalInvestigatorsByFilters(string AssignedTo, string PricipalInvestigatorName = "")
        {
            var retList = new List<PrincipalInvestigator>();

            List<ComplianceForm> compForms;

           if (AssignedTo.Length > 0)
            {
                compForms = _UOW.ComplianceFormRepository.GetAll().Where(x => x.AssignedTo == AssignedTo).ToList();
            }
            else
            {
                compForms = _UOW.ComplianceFormRepository.GetAll();
            }

            //Principal Investigator
            List<ComplianceForm> compForms1;
            compForms1 = compForms;
            if (PricipalInvestigatorName.Length > 0)
            {
                compForms1 = compForms.Where(x => x.InvestigatorDetails.Any(y => (y.Name.Contains(PricipalInvestigatorName) && y.Role=="Principal"))).ToList();
            }
            else
            {
                compForms = compForms1;
            }
             

            foreach (ComplianceForm compForm in compForms1)
            {
                var item = getPrincipalInvestigators(compForm);
                retList.Add(item);
            }
            return retList;
        }

        private PrincipalInvestigator getPrincipalInvestigators(ComplianceForm compForm)
        {
            var item = new PrincipalInvestigator();
            item.Address = compForm.Address;
            item.Country = compForm.Country;
            item.ProjectNumber = compForm.ProjectNumber;
            item.SponsorProtocolNumber = compForm.SponsorProtocolNumber;
            item.RecId = compForm.RecId;
            item.Active = compForm.Active;
            item.SearchStartedOn = compForm.SearchStartedOn;
            if (compForm.InvestigatorDetails.Count > 0)
            {
                item.Name = compForm.InvestigatorDetails.FirstOrDefault().Name;
            }
            item.AssignedTo = compForm.AssignedTo;
            item.Status = compForm.Status;
            item.StatusEnum = compForm.StatusEnum;
            return item;
        }


        public List<PrincipalInvestigator> GetComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter)
        {

            if (CompFormFilter == null)
            {
                throw new Exception("Invalid CompFormFilter");
            }

            var Filter = _UOW.ComplianceFormRepository.GetAll();
            var Filter1 = Filter;


            if (CompFormFilter.InvestigatorName != null && 
                CompFormFilter.InvestigatorName != "")
            {
                var tempFilter = Filter1.Select(x => x.InvestigatorDetails.Where(inv =>
                inv.Name == CompFormFilter.InvestigatorName)).ToList();
            }

            var Filter2 = Filter1;

            if(CompFormFilter.ProjectNumber != null &&
                CompFormFilter.ProjectNumber != "")
            {
                Filter2 = Filter1.Where(x =>
                x.ProjectNumber == CompFormFilter.ProjectNumber).ToList();
            }

            var Filter3 = Filter2;

            if(CompFormFilter.SponsorProtocolNumber != null &&
                CompFormFilter.SponsorProtocolNumber != "")
            {
                Filter3 = Filter2.Where(x =>
                x.SponsorProtocolNumber.ToLower() == 
                CompFormFilter.SponsorProtocolNumber.ToLower())
                .ToList();
            }

            var Filter4 = Filter3;

            
            
            if (CompFormFilter.SearchedOnFrom != null )
                
            {
                DateTime startDate;
                startDate = CompFormFilter.SearchedOnFrom.Value.Date;
                Filter4 = Filter3.Where(x =>
               x.SearchStartedOn >= startDate)
               .ToList();
            }

            var Filter5 = Filter4;

            if (CompFormFilter.SearchedOnTo != null)
            {

                DateTime endDate ;
                endDate = CompFormFilter.SearchedOnTo.Value.Date.AddDays(1);
                Filter5 = Filter4.Where(x =>
                x.SearchStartedOn <
                endDate)
                .ToList();
            }

            var Filter6 = Filter5;

            if(CompFormFilter.Country != null &&
                CompFormFilter.Country != "")
            {
                Filter6 = Filter5.Where(x =>
                x.Country.ToLower() == CompFormFilter.Country.ToLower()).ToList();
            }

            var Filter7 = Filter6;

            if((int)CompFormFilter.Status != -1)
            {
                Filter7 = Filter6.Where(x =>
                x.StatusEnum == CompFormFilter.Status).ToList();
            }


            var ReturnList = new List<PrincipalInvestigator>();

            foreach(ComplianceForm form in Filter7)
            {
                ReturnList.Add(getPrincipalInvestigators(form));
            }
            return ReturnList;
        }

        #endregion

        #region ComplianceFormGeneration
        public string GenerateComplianceFormAlt(Guid? ComplianceFormId, string TemplateFolder, 
            string DownloadFolder)
        {
            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var UtilitiesObject = new CreateComplianceForm();

            var PI = RemoveSpecialCharacters(form.InvestigatorDetails.FirstOrDefault().Name);

            var ProjectNumber = RemoveSpecialCharacters(form.ProjectNumber);

            var GeneratedFileName = ProjectNumber + "_" + PI + ".docx";

            var GeneratedFileNameNPath = DownloadFolder + GeneratedFileName;

            var stream = UtilitiesObject.ReplaceTextFromWord(form, TemplateFolder, GeneratedFileNameNPath);

            //The following path does not work, client unable to download the file:

            //return @"App_Data\Data\Downloads\" + GeneratedFileName;
            //threfore: 

            return DownloadFolder + GeneratedFileName;
        }
        #endregion

        #region ByPatrick

        //3Dec2016
        public MemoryStream GenerateComplianceForm(Guid? ComplianceFormId)
        {
            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var UtilitiesObject = new CreateComplianceForm();

            var FileName = form.InvestigatorDetails.FirstOrDefault().Name + ".docx";

            return UtilitiesObject.ReplaceTextFromWord(form, FileName);
        }

        //Not required?
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
            retInv.Name = invInCompForm.Name;
            retInv.Role = invInCompForm.Role;
            retInv.Deleted = invInCompForm.Deleted;
            retInv.DisplayPosition = invInCompForm.DisplayPosition;
            retInv.Id = invInCompForm.Id;
            retInv.Sites_FullMatchCount = invInCompForm.Sites_FullMatchCount;
            retInv.Sites_PartialMatchCount = invInCompForm.Sites_PartialMatchCount;
            retInv.TotalIssuesFound = invInCompForm.TotalIssuesFound;

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
                    foreach (Finding Finding in Findings)
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

                    if (IssuesFound > 0 && Investigator.Id == InvId)
                        Site.IssuesIdentified = true;
                }
            }
            return form;
        }

        public List<MatchedRecord> GetFindings(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName)
        {

            return null;
        }


        public List<MatchedRecord> GetFDADebarPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName)
        {
    
            FDADebarPageSiteData FDASearchResult =
                _UOW.FDADebarPageRepository.FindById(SiteDataId);


            UpdateMatchStatus(
                FDASearchResult.DebarredPersons,
                InvestigatorName);  //updates list with match count

            var DebarList = FDASearchResult.DebarredPersons.Where(
               debarredList => debarredList.Matched > 0).ToList();

            if (DebarList == null)
                return null;

            //Patrick: Further refactoring possible, ConvertToMatchedRecords may not be required:
            return ConvertToMatchedRecords(DebarList);

        }

        public List<MatchedRecord> GetClinicalInvestigatorPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName)
        {
            ClinicalInvestigatorInspectionSiteData CIILSearchResult =
                _UOW.ClinicalInvestigatorInspectionListRepository.FindById(SiteDataId);

            UpdateMatchStatus(CIILSearchResult.ClinicalInvestigatorInspectionList,
                InvestigatorName);  //updates list with match count

            var ClinicalInvestigatorList = CIILSearchResult.ClinicalInvestigatorInspectionList.Where(
               ClinicalList => ClinicalList.Matched > 0).ToList();

            if (ClinicalInvestigatorList == null)
                return null;

            return ConvertToMatchedRecords(ClinicalInvestigatorList);
        }

        public List<MatchedRecord> GetFDAWarningLettersPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch,
            int ComponentsInInvestigatorName)
        {
            _SearchEngine.ExtractData(
                SiteEnum.FDAWarningLettersPage, NameToSearch, MatchCountLowerLimit);

            var siteData = _SearchEngine.SiteData;

            //UpdateMatchStatus(FDAWarningSearchResult.FDAWarningLetterList, NameToSearch);  //updates list with match count
            UpdateMatchStatus(siteData, NameToSearch);

            var FDAWarningLetterList =
                siteData.Where(
                FDAList => FDAList.Matched > 0);

            //if (FDAWarningLetterList == null)
            //    return null;

            if (siteData == null)
                return null;

            return ConvertToMatchedRecords(FDAWarningLetterList);
        }

        public List<MatchedRecord> GetERRProposalToDebarPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName)
        {
            ERRProposalToDebarPageSiteData ERRSearchResult =
                _UOW.ERRProposalToDebarRepository.FindById(SiteDataId);

            UpdateMatchStatus(ERRSearchResult.ProposalToDebar,
                InvestigatorName);  //updates list with match count

            var ERRList = ERRSearchResult.ProposalToDebar.Where(
               ErrList => ErrList.Matched > 0).ToList();

            if (ERRList == null)
                return null;

            return ConvertToMatchedRecords(ERRList);
        }

        public List<MatchedRecord> GetAdequateAssurancePageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName)
        {
            AdequateAssuranceListSiteData AdequateAssuranceSearchResult =
                _UOW.AdequateAssuranceListRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                AdequateAssuranceSearchResult.AdequateAssurances,
                InvestigatorName);  //updates list with match count

            var AdequateAssuranceList =
                AdequateAssuranceSearchResult.AdequateAssurances.Where(
                AssuranceList => AssuranceList.Matched > 0).ToList();

            if (AdequateAssuranceList == null)
                return null;

            return ConvertToMatchedRecords(AdequateAssuranceList);
        }

        public List<MatchedRecord> GetClinicalInvestigatorDisqualificationPageMatchedRecords(
            Guid? SiteDataId, string NameToSearch,
            int ComponentsInInvestigatorName)
        {
            _SearchEngine.ExtractData(
                SiteEnum.ClinicalInvestigatorDisqualificationPage, NameToSearch, 
                MatchCountLowerLimit);

            var siteData = _SearchEngine.SiteData;

            UpdateMatchStatus(siteData, NameToSearch);

            var DisqualificationSiteData =
                siteData.Where(site => site.Matched > 0);

            if (siteData == null)
                return null;

            return ConvertToMatchedRecords(DisqualificationSiteData);
        }

        public List<MatchedRecord> GetCBERClinicalInvestigatorPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName)
        {
            CBERClinicalInvestigatorInspectionSiteData CBERSearchResult =
                _UOW.CBERClinicalInvestigatorRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                CBERSearchResult.ClinicalInvestigator,
                InvestigatorName);  //updates list with match count

            var ClinicalInvestigatorList = CBERSearchResult.ClinicalInvestigator.Where(
               CBERList => CBERList.Matched > 0).ToList();

            if (ClinicalInvestigatorList == null)
                return null;

            return ConvertToMatchedRecords(ClinicalInvestigatorList);
        }

        public List<MatchedRecord> GetExclusionDatabasePageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName, int ComponentsInInvestigatorName)
        {
            ExclusionDatabaseSearchPageSiteData ExclusionSearchResult =
                _UOW.ExclusionDatabaseSearchRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                ExclusionSearchResult.ExclusionSearchList,
                InvestigatorName);  //updates list with match count

            var ExclusionDBList = ExclusionSearchResult.ExclusionSearchList.Where(
               ExclusionList => ExclusionList.Matched > 0).ToList();

            if (ExclusionDBList == null)
                return null;

            return ConvertToMatchedRecords(ExclusionDBList);
        }

        public List<MatchedRecord> GetPHSAdministrativeActionPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName, int ComponentsInInvestigatorName)
        {
            PHSAdministrativeActionListingSiteData PHSSearchResult =
                _UOW.PHSAdministrativeActionListingRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                PHSSearchResult.PHSAdministrativeSiteData,
                InvestigatorName);  //updates list with match count

            var PHSList = PHSSearchResult.PHSAdministrativeSiteData.Where(
               PHSData => PHSData.Matched > 0).ToList();

            if (PHSList == null)
                return null;

            return ConvertToMatchedRecords(PHSList);
        }

        public List<MatchedRecord> GetCIAPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName)
        {
            CorporateIntegrityAgreementListSiteData CIASearchResult =
                _UOW.CorporateIntegrityAgreementRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                CIASearchResult.CIAListSiteData,
                InvestigatorName);  //updates list with match count

            var CIAList = CIASearchResult.CIAListSiteData.Where(
               CIAData => CIAData.Matched > 0).ToList();

            if (CIAList == null)
                return null;

            return ConvertToMatchedRecords(CIAList);
        }

        public List<MatchedRecord> GetSAMPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, int ComponentsInIvestigatorName)
        {
            _SearchEngine.ExtractData(SiteEnum.SystemForAwardManagementPage, NameToSearch,
                MatchCountLowerLimit);

            var siteData = _SearchEngine.SiteData;

            UpdateMatchStatus(siteData, NameToSearch);

            var DisqualificationSiteData =
                siteData.Where(site => site.Matched > 0);

            if (siteData == null)
                return null;

            return ConvertToMatchedRecords(DisqualificationSiteData);
        }

        public List<MatchedRecord> GetSDNPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName, int ComponentsInInvestigatorName)
        {
            SpeciallyDesignatedNationalsListSiteData SDNSearchResult =
                _UOW.SpeciallyDesignatedNationalsRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                SDNSearchResult.SDNListSiteData,
                InvestigatorName);  //updates list with match count

            var SDNList = SDNSearchResult.SDNListSiteData.Where(
               SDNData => SDNData.Matched > 0).ToList();

            if (SDNList == null)
                return null;

            return ConvertToMatchedRecords(SDNList);

        }

        #endregion

        #region Helpers

        public string RemoveExtraCharacters(string Value)
        {
            //string CharactersToRemove = ".,/:";
            //return Name.Replace(CharactersToRemove, "");
            //string TempValue = Regex.Unescape(Value);
            return Regex.Replace(Value, "[,./]", "");
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_'
                    || c == ' ')
                    sb.Append(c);
            }
            return sb.ToString();
        }

      

        public string AddSpaceBetweenWords(string Name)
        {
            string res = Regex.Replace(Name, "[A-Z]", " $0").Trim();
            return res;
        }
        #endregion

        #region ExcelValidations
        public List<string> ValidateExcelInputs(List<List<string>> ExcelInputRows)
        {
            var ValidationMessages = new List<string>();

            string ValidationMessage = null;

            //foreach(List<string> Data in ExcelInputRows)
            for(int Counter = 0; Counter < ExcelInputRows.Count; Counter++)
            {
                var DetailsInEachRow = ExcelInputRows[Counter];

                var PrincipalInv = DetailsInEachRow[0].Split(' ').Count();

                if(DetailsInEachRow[0] == "")
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 + 
                        " Principal Investigator Name is null!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if(PrincipalInv <= 1)
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Principal Investigator Name must have atleast two components " +
                        "separated with a space!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if(IsAlpha(DetailsInEachRow[0]))
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Principal Investigator Name should not have any " +
                        "numbers or special characters!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if(DetailsInEachRow[0].Length > 100)
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Principal Investigator Name exceeds max character(100) limit!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if(DetailsInEachRow[1].Length > 100)
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Principal Investigator ML Number exceeds max character(100) limit!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if (DetailsInEachRow[2].Length > 100)
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Principal Investigator Qualification exceeds max character(100) limit!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if (DetailsInEachRow[3] == "")
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Project Number is mandatory!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if(DetailsInEachRow[3].Length > 100)
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Project Number exceeds max character(100) limit!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if(HasSpecialCharacters(DetailsInEachRow[3]))
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Project Number should not have any special characters!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if (DetailsInEachRow[4].Length > 100)
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Sponsor protocol number exceeds max character(100) limit!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if (DetailsInEachRow[5].Length > 100)
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Institute Name exceeds max character(100) limit!";
                    ValidationMessages.Add(ValidationMessage);
                }
                if (DetailsInEachRow[6].Length > 100)
                {
                    ValidationMessage = "RowNumber: " + Counter + 1 +
                        " Address exceeds max character(500) limit!";
                    ValidationMessages.Add(ValidationMessage);
                }
                for(int counter = 8; counter < DetailsInEachRow.Count(); counter++)
                {
                    if(DetailsInEachRow[counter] != "" &&
                        HasSpecialCharacters(DetailsInEachRow[counter]) ||
                        DetailsInEachRow[counter].Length > 100)
                    {
                        ValidationMessage = "RowNumber: " + Counter + 1 +
                            " Sub Investigator name/ML#/Qualification has invalid inputs!";
                        ValidationMessages.Add(ValidationMessage);
                    }
                }
            }
            return ValidationMessages;
        }

        private bool IsAlpha(string Value)
        {
            foreach (char c in Value)
            {
                if ((c >= '0' && c <= '9'))
                    return true;
            }
            return HasSpecialCharacters(Value) ? true : false;
        }

        private bool HasSpecialCharacters(string Value)
        {
            foreach (char c in Value)
            {
                if (c == '?' || c == '\\' ||
                    c == '$' || c == '#' ||
                    c == '.' || c == '*' || 
                    c == '_' || c == '&' || 
                    c == '@' || c == '!' || 
                    c == '%' || c == '^')
                    return true;
            }
            return false;
        }
        #endregion
    }
}
