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
                if(site.Mandatory)
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
                    siteSourceToAdd.IsOptional = site.IsOptional;
                    siteSourceToAdd.ExtractionMode = site.ExtractionMode;
                    siteSourceToAdd.Deleted = false;

                    compForm.SiteSources.Add(siteSourceToAdd);
                }
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
            return null;
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

            if (DataFromExcelFile == null)
                return null;

            foreach (RowData row in DataFromExcelFile)
            {
                var form = GetNewComplianceForm(log);

                var Investigators = new List<InvestigatorSearched>();
                var Investigator = new InvestigatorSearched();
                Investigator.Name = row.DetailsInEachRow[0];
                Investigator.MedicalLiceseNumber = row.DetailsInEachRow[1];
                Investigator.Qualification = row.DetailsInEachRow[2];
                Investigator.Role = "Principal";
                form.ProjectNumber = row.DetailsInEachRow[3];
                form.SponsorProtocolNumber = row.DetailsInEachRow[4];
                form.Institute = row.DetailsInEachRow[5];
                form.Address = row.DetailsInEachRow[6];
                form.Country = row.DetailsInEachRow[7];

                Investigators.Add(Investigator);

                for (int Index = 8; Index < row.DetailsInEachRow.Count; Index++)
                {
                    var Inv = new InvestigatorSearched();
                    Inv.Name = row.DetailsInEachRow[Index]; //SIs
                    Inv.Role = "Sub";
                    Inv.MedicalLiceseNumber = row.DetailsInEachRow[Index + 1];
                    Inv.Qualification = row.DetailsInEachRow[Index + 2];
                    Investigators.Add(Inv);
                    Index += 2;
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

            var UtilitiesObject = new CreateComplianceForm();


            var FileName = form.InvestigatorDetails.FirstOrDefault().Name + ".docx";

            return UtilitiesObject.ReplaceTextFromWord(form, FileName);
        }

        public string GenerateComplianceFormAlt(Guid? ComplianceFormId, string DownloadsFolder)
        {
            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var UtilitiesObject = new CreateComplianceForm();

            var PI = RemoveExtraCharacters(form.InvestigatorDetails.FirstOrDefault().Name);

            DownloadsFolder += PI + ".docx";

            var stream = UtilitiesObject.ReplaceTextFromWord(form, DownloadsFolder);

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

                    if (IssuesFound > 0 && Investigator.Id == InvId)
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

                var InvestigatorName = RemoveExtraCharacters(inv.Name);

                var ComponentsInInvestigatorName = 
                    InvestigatorName.Split(' ').Count();

                foreach (SiteSource siteSource in frm.SiteSources)
                {
                    SiteSearchStatus searchStatus = null;

                    if (inv.SitesSearched != null)
                        searchStatus =
                            inv.SitesSearched.Find(x => x.siteEnum == siteSource.SiteEnum);

                    bool searchRequired = false;
                    if (searchStatus == null )
                    {
                        searchRequired = true;
                    }
                    else
                    {
                        if (searchStatus.HasExtractionError == true)
                        {
                            searchRequired = true;
                        }
                    }

                    if (searchRequired == true)
                    {
                        if (searchStatus == null)
                        {
                            searchStatus = new SiteSearchStatus();
                            searchStatus.siteEnum = siteSource.SiteEnum;
                        }

                        try
                        {
                            // Not processed,  search now.
                            //var matchedRecords = GetMatchedRecords(
                            //    site.SiteEnum, site.SiteDataId, inv.Name, log);

                            var MatchedRecords = GetMatchedRecords(
                                siteSource, InvestigatorName, log, 
                                ComponentsInInvestigatorName);

                            GetFullAndPartialMatchCount(MatchedRecords, searchStatus,
                                ComponentsInInvestigatorName);
                            //GetFullAndPartialMatchCount( DebarList, searchStatus, NameToSearch); //updates full and partial match counts


                            //To-Do: convert matchedRecords to Findings

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

                                string RecordDetails = 
                                    AddSpaceBetweenWords(rec.RecordDetails);

                                finding.RecordDetails = RecordDetails;
                                finding.RowNumberInSource = rec.RowNumber;

                                //Patrick 04Dec2016
                                finding.IsMatchedRecord = true;
                                finding.DateOfInspection = siteSource.SiteSourceUpdatedOn;
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

                //case SiteEnum.SystemForAwardManagementPage:
                //    return GetSAMMatchCount(NameToSearch, DataId, Site);

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

        //Alt for GetFDADebarPageMatchCount  
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

            _SearchEngine.Load(SiteEnum.FDAWarningLettersPage, NameToSearch, "", true);
            var siteData = _SearchEngine.SiteData;

            //UpdateMatchStatus(FDAWarningSearchResult.FDAWarningLetterList, NameToSearch);  //updates list with match count
            UpdateMatchStatus(siteData, NameToSearch);  //updates list with match count

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
            _SearchEngine.Load(SiteEnum.FDAWarningLettersPage, NameToSearch, "", true);
            var siteData = _SearchEngine.SiteData;

            UpdateMatchStatus(siteData, NameToSearch);  //updates list with match count

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
    }
}
