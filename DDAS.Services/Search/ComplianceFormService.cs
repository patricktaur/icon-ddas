using DDAS.Models;
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
        private int _NumberOfRunningExtractionProcesses = 4;

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
        public List<List<string>> ReadDataFromExcelFile(string FilePathWithGUID)
        {
            var readExcelData = new ReadUploadedExcelFile();

            var Validations = new List<List<string>>();
            var ValidationMessages = new List<string>();
            var ComplianceFormDetails = new List<List<string>>();

            int RowIndex = 2;
            while (true)
            {
                var ExcelRow = readExcelData.ReadDataFromExcel(FilePathWithGUID, RowIndex);

                if(ExcelRow.Where(x => x.Contains("cannot find column")).Count() > 0)
                {
                    Validations.Add(ExcelRow);
                    break;
                }

                if (ExcelRow[0] == null || ExcelRow[0] == "")
                {
                    break;
                }

                //Validate RowData
                ValidationMessages = ValidateExcelInputs(ExcelRow, RowIndex);

                if (ValidationMessages.Count > 0)
                {
                    Validations.Add(ValidationMessages);
                    break;
                }

                if(ExcelRow[1].ToLower() == "principal")
                {
                    RowIndex += 1;
                    var SubInvestigator = readExcelData.ReadDataFromExcel(FilePathWithGUID, RowIndex);

                    while (SubInvestigator[1].ToLower() == "sub")
                    {
                        ValidationMessages = ValidateExcelInputs(SubInvestigator, RowIndex);

                        if (ValidationMessages.Count > 0)
                        {
                            Validations.Add(ValidationMessages);
                            //break;
                        }

                        foreach (string Inv in SubInvestigator)
                        {
                            ExcelRow.Add(Inv);
                        }
                        RowIndex += 1;
                        SubInvestigator = readExcelData.ReadDataFromExcel(FilePathWithGUID, RowIndex);
                    }
                }
                ComplianceFormDetails.Add(ExcelRow);
            }
            if(Validations.Count > 0)
            {
                List<string> temp = new List<string>() {"errors found"};
                Validations.Add(temp);
            }
            return Validations.Count > 0 ? Validations : ComplianceFormDetails;

        }
        //Pradeep 1Dec2016
        public List<ComplianceForm> ReadUploadedFileData(List<List<string>> DataFromExcelFile, 
            ILog log,
            string UserName, 
            string FilePathWithGUID,
            string UploadedFileName)
        {
            var ComplianceForms = new List<ComplianceForm>();

            for (int Counter = 0; Counter < DataFromExcelFile.Count; Counter++)
            {
                var DetailsInEachRow = DataFromExcelFile[Counter];

                var Investigator = new InvestigatorSearched();

                var form = GetNewComplianceForm(log, UserName);
                form.AssignedTo = UserName;
                form.UploadedFileName = UploadedFileName;
                form.GeneratedFileName = Path.GetFileName(FilePathWithGUID);
                form.ProjectNumber = DetailsInEachRow[4];
                form.SponsorProtocolNumber = DetailsInEachRow[5];
                form.Institute = DetailsInEachRow[6];
                form.Address = DetailsInEachRow[7];
                form.Country = DetailsInEachRow[8];
                int InvId = 1;
                if (DetailsInEachRow[1].ToLower() == "principal")
                {
                    
                    Investigator.Id = InvId;
                    InvId += 1;
                    Investigator.Name = DetailsInEachRow[0];
                    Investigator.Role = DetailsInEachRow[1];
                    Investigator.MedicalLiceseNumber = DetailsInEachRow[2];
                    Investigator.Qualification = DetailsInEachRow[3];
                   
                    form.InvestigatorDetails.Add(Investigator);

                    if (DetailsInEachRow.Count > 9)
                    {
                        for(int Index = 9; Index < DetailsInEachRow.Count; Index++)
                        {
                            var Inv = new InvestigatorSearched();

                            Inv.Id = InvId;
                            InvId += 1;
                            Inv.Name = DetailsInEachRow[Index];
                            Inv.Role = DetailsInEachRow[Index + 1];
                            Inv.MedicalLiceseNumber = DetailsInEachRow[Index + 2];
                            Inv.Qualification = DetailsInEachRow[Index + 3];


                            Index += 8;
                            form.InvestigatorDetails.Add(Inv);
                        }
                    }
                }
                ComplianceForms.Add(form);
            }
            return ComplianceForms;
        }
        #endregion

        public void UpdateAssignedToData(string AssignedTo, bool Active,
            Guid? RecId)
        {
            //var form = _UOW.ComplianceFormRepository.FindById(RecId);
            //form.AssignedTo = AssignedTo;
            //form.Active = Active;
            //_UOW.ComplianceFormRepository.UpdateCollection(form);

            _UOW.ComplianceFormRepository.UpdateAssignedTo(RecId.Value, AssignedTo);

        }

        //used by Excel File Upload method.
        public ComplianceForm ScanUpdateComplianceForm(ComplianceForm frm, ILog log,  string ErrorScreenCaptureFolder, string siteType = "db")
        {
            //Creates or Updates form
            //Remove Inv + Sites if marked for delete:
            //RemoveDeleteMarkedItemsFromFormCollections(frm);

            AddMissingSearchStatusRecords(frm);
            //Check and Search if required:
            AddMatchingRecords(frm, log,  ErrorScreenCaptureFolder, siteType);
            //AddLiveScanFindings(frm, log, ErrorScreenCaptureFolder, siteType);

            //return SaveComplianceForm(frm);

            RollUpSummary(frm);
            //UpdateRollUpSummary(frm.RecId.Value);
            frm.ExtractionEstimatedCompletion = getEstimatedExtractionCompletion();

            if (frm.RecId != null)
                _UOW.ComplianceFormRepository.UpdateCollection(frm); //Update
            else
                _UOW.ComplianceFormRepository.Add(frm); //Insert
            return frm;
        }

        public bool AddAttachmentsToFindings(ComplianceForm form)
        {
            UpdateComplianceForm(form);
            return true;
        }

        //Used by Client Save button, updates - general section, Inviestigators and Sites collection.
        public ComplianceForm UpdateComplianceForm(ComplianceForm frm)
        {
            //Creates or Updates form
            //Remove Inv + Sites if marked for delete:
            RemoveDeleteMarkedItemsFromFormCollections(frm);

            AddMissingSearchStatusRecords(frm);

            return SaveComplianceForm(frm);
            //if (frm.RecId != null)
            //{
            //    _UOW.ComplianceFormRepository.UpdateComplianceForm(frm.RecId.Value, frm);
            //}
            //else
            //{
            //    _UOW.ComplianceFormRepository.Add(frm);
            //}
            //return frm;

        }

        public bool UpdateComplianceFormNIgnoreIfNotFound(ComplianceForm form) {
            //Check if Form exists.
            //Forms can get deleted by other operations
            //Therefore ignore if not found.
            var formToUpdate = _UOW.ComplianceFormRepository.FindById(form.RecId);
           
            if (formToUpdate == null)
            {
                return false;
            }
            else
            {
                 _UOW.ComplianceFormRepository.UpdateCollection(form);
                return true;
            }
        }

        private ComplianceForm SaveComplianceForm(ComplianceForm frm)
        {
            //retain Assignedto //
            if (frm.RecId != null)
            {
                var formFromDB = _UOW.ComplianceFormRepository.FindById(frm.RecId);
                if (formFromDB == null)
                {
                    throw new Exception("Compliance Form not found in data base: " + frm.ProjectNumber);
                }
                //AssignedTo can change after the form is downloaded by client.  
                frm.AssignedTo = formFromDB.AssignedTo;

                //REcords are found only when the form is returned from the client after the extractor saved the comp form.
                var findingsFromLiveExtraction = formFromDB.Findings.Where(f => !frm.Findings.Any(f2 => (f2.Id == f.Id)));
                frm.Findings.AddRange(findingsFromLiveExtraction);
            }

            //Guid added to findings added by client.  Null Guid identifies records that are added by client.
            var findingsAddedByClient = frm.Findings.Where(f => f.Id == null).ToList();
            findingsAddedByClient.ForEach(f => f.Id = Guid.NewGuid());
            

            RollUpSummary(frm);

            //if (frm.ExtractionPendingInvestigatorCount == 0)
            //{
            //    frm.ExtractionEstimatedCompletion = null;
            //}


            //set frm.ExtractionEstimatedCompletion, will be overwritten when the form is added to the Queue
            if (frm.ExtractionPendingInvestigatorCount > 0 && frm.ExtractionEstimatedCompletion == null)
            {
                var formWithMaxExtractionEstimatedDate = _UOW.ComplianceFormRepository.GetAll().OrderByDescending(o => o.ExtractionEstimatedCompletion).FirstOrDefault();
                
                if (formWithMaxExtractionEstimatedDate != null)
                {
                    var maxDate = formWithMaxExtractionEstimatedDate.ExtractionEstimatedCompletion;
                    var estimatedCompletionSecs = frm.ExtractionPendingInvestigatorCount * 3 * 15;
                    DateTime baseDate;
                    if (maxDate != null && maxDate > DateTime.Now)
                    {
                        baseDate = maxDate.Value;
                    }
                    else
                    {
                        baseDate = DateTime.Now;
                    }
                    var completionAt = baseDate.AddSeconds(estimatedCompletionSecs);
                    frm.ExtractionEstimatedCompletion = completionAt;
                }
            }

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

        public void UpdateExtractionQuePosition(Guid formId, int Position, DateTime ExtractionStartedAt, DateTime ExtractionEstimatedCompletion)
        {
            var form = _UOW.ComplianceFormRepository.FindById(formId);
            form.ExtractionQuePosition = Position;
            form.ExtractionQueStart = ExtractionStartedAt;
            form.ExtractionEstimatedCompletion = ExtractionEstimatedCompletion;
            _UOW.ComplianceFormRepository.UpdateCollection(form);
        }

        private DateTime getEstimatedExtractionCompletion()
        {
            List<ComplianceForm> forms = _UOW.ComplianceFormRepository.GetAll();

            //var sitesToScanCount = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(
            //    s => s.ExtractionMode == "Live"
            //    && s.ExtractedOn == null
            //    && !(s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
            //    ))).ToList().OrderBy(o => o.SearchStartedOn).Count();

            var formsToScanCount = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(
              s => s.ExtractionMode == "Live"
              && s.ExtractedOn == null
              && !(s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
              ))).ToList().Count();

            int totCount = 0;
            var formsForLiveScan = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(
               s => s.ExtractionMode == "Live"
               && s.ExtractedOn == null
               && !(s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
               ))).ToList();
            foreach (ComplianceForm form in formsForLiveScan)
            {
                foreach(InvestigatorSearched inv in form.InvestigatorDetails.Where(i => i.ExtractionPendingSiteCount > 0))
                {
                    totCount += inv.SitesSearched.Count(
                       s => s.ExtractionMode == "Live"
                    && s.ExtractedOn == null
                     && !(s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified));
                }
            }
          

            var estimatedCompletionSecs = (totCount * 15) / _NumberOfRunningExtractionProcesses;
            if (estimatedCompletionSecs < 60)
            {
                estimatedCompletionSecs = 60;
            }
            var completionAt = DateTime.Now.AddSeconds(estimatedCompletionSecs);
            return completionAt;

        }
         /*
           


         */

        #region Client side update

        // Called by ComplianceForm Save
        public ComplianceForm UpdateCompFormGeneralNInvestigatorsNOptionalSites(ComplianceForm form, ILog log, string ErrorScreenCaptureFolder)
        {
            if (form.RecId == null)
            {
                AddMissingSearchStatusRecords(form);
                AddMatchingRecords(form, log, ErrorScreenCaptureFolder, "db");

                RollUpSummary(form);
                _UOW.ComplianceFormRepository.Add(form);
                return form;
            }
            else
            {
                var dbForm = _UOW.ComplianceFormRepository.FindById(form.RecId);
                if (dbForm != null)
                {
                    dbForm.ProjectNumber = form.ProjectNumber;
                    dbForm.SponsorProtocolNumber = form.SponsorProtocolNumber;
                    dbForm.Institute = form.Institute;
                    dbForm.Address = form.Address;
                    dbForm.Country = form.Country;

                    //Delete Investigator not found in client collection
                    foreach (InvestigatorSearched inv in dbForm.InvestigatorDetails)
                    {
                        var clInv = form.InvestigatorDetails.Find(x => x.Id == inv.Id);
                        if (clInv == null)
                        {
                            //not found, delete from DB
                            inv.Deleted = true;
                            //remove all Findings for the deleted Investigator
                            dbForm.Findings.RemoveAll(x => x.InvestigatorSearchedId == inv.Id);
                        }
                    }
                    dbForm.InvestigatorDetails.RemoveAll(x => x.Deleted == true);

                    //InvestigatorUpdate or add:
                    var invId = 1;
                    foreach (InvestigatorSearched clInv in form.InvestigatorDetails)
                    {

                        var dbInv = dbForm.InvestigatorDetails.Find(x => x.Id == clInv.Id);
                        if (dbInv != null)
                        {
                            dbInv.Name = clInv.Name;
                            dbInv.Qualification = clInv.Qualification;
                            dbInv.Role = clInv.Role;
                            dbInv.InvestigatorId = clInv.InvestigatorId;
                            //dbInv.Id = invId;
                        }
                        else
                        {
                            //Not found in DB, add
                            //clInv.Id = invId;
                            dbForm.InvestigatorDetails.Add(clInv);
                        }
                        invId += 1;
                    }

                    //Remove Optional Sites.
                    //Remove Optional sites not found in client collection

                    //Site add if not found:
                    foreach (SiteSource clSite in form.SiteSources)
                    {
                        var dbInv = dbForm.SiteSources.Find(x => x.SiteEnum == clSite.SiteEnum);
                        if (dbInv == null)
                        {
                            //Not found, add
                            dbForm.SiteSources.Add(clSite);
                        }
                    }

                    foreach (SiteSource site in dbForm.SiteSources)
                    {
                        var clSite = form.SiteSources.Find(x => x.SiteEnum == site.SiteEnum);
                        if (clSite.Deleted == true)
                        {
                            site.Deleted = true;
                        }

                        //if (clSite == null)
                        //{
                        //    //not found, delete from DB
                        //    site.Deleted = true;
                        //}
                    }
                    dbForm.SiteSources.RemoveAll(x => x.Deleted == true);

                    //Correct DisplayPosition etc
                    AddMissingSearchStatusRecords(dbForm);
                    RemoveOrphanedSearchStatusRecords(dbForm);
                    RemoveOrphanedFindings(dbForm);

                    // DisplayPosition, RowNumberInSource nos need adjustment when a site is deleted.
                    AdjustDisplayPositionOfSiteSources(dbForm);
                    CorrectDisplayPositionOfSearchStatusRecords(dbForm);
                    CorrectSourceNumberInFindings(dbForm);
                    //Check and Search if required:
                    if (dbForm.ExtractionPendingInvestigatorCount > 0)
                    {
                        dbForm.ExtractionEstimatedCompletion = getEstimatedExtractionCompletion();
                    }
                    AddMatchingRecords(dbForm, log, ErrorScreenCaptureFolder, "db");

                    RollUpSummary(dbForm);
                    _UOW.ComplianceFormRepository.UpdateCollection(dbForm);
                    return dbForm;
                }
                else
                {
                    return null;
                }
            }
            
        }

        // Called by Findings.
        public bool UpdateFindings(UpdateFindigs updateFindings)
        {
            //Called by client - Findings page.
            //Retrieves form from db and replaces view related values (ReviewCompleted, corresponding Findings) 
            
            //get Comp form from db.
            var form = _UOW.ComplianceFormRepository.FindById(updateFindings.FormId);
            if (form != null)
            {
                
                //Set  Review Completed value:
                foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
                {
                    if (Investigator.Id == updateFindings.InvestigatorSearchedId)
                    {
                        foreach (SiteSearchStatus s in Investigator.SitesSearched)
                        {
                            if (s.siteEnum == updateFindings.SiteEnum)
                            {
                                s.ReviewCompleted = updateFindings.ReviewCompleted;
                            }
                        }
                    }
                }

                //Findings
                //Remove manually added Findings (IsMatched = false) from db and add again from the client
                form.Findings.RemoveAll(
                    x => x.InvestigatorSearchedId == updateFindings.InvestigatorSearchedId
                    && x.SiteEnum == updateFindings.SiteEnum
                    && x.IsMatchedRecord == false);
                //Add all IsMatchedRecord = false records from client
                form.Findings.AddRange(updateFindings.Findings.Where(x => x.IsMatchedRecord == false));

                var matchedRecords = updateFindings.Findings.Where(x => x.InvestigatorSearchedId == updateFindings.InvestigatorSearchedId
                   && x.SiteEnum == updateFindings.SiteEnum
                   && x.IsMatchedRecord == true);

                //Replace existing generated records (IsMatchedRecord = true) records with records received from client.
                foreach (var rec in matchedRecords)
                {
                    var findingInForm = form.Findings.Find(x => x.Id == rec.Id);
                    if (findingInForm != null)
                    {
                        findingInForm.Observation = rec.Observation;
                        findingInForm.IsAnIssue = rec.IsAnIssue;
                        findingInForm.Selected = rec.Selected;
                     }
                }

                RollUpSummary(form);

                _UOW.ComplianceFormRepository.UpdateCollection(form);
                return true;
            }
            else
            {
                return false;
            }

            
            
            //_UOW.ComplianceFormRepository.UpdateInvestigator(updateFindings.FormId, updateFindings.InvestigatorSearched);
            //_UOW.ComplianceFormRepository.UpdateFindings(updateFindings);


           // return true;
        }

        #endregion

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
            NameToSearch = RemoveExtraCharacters(NameToSearch);
            string[] Name = NameToSearch.Split(' ');
            foreach (SiteDataItemBase item in items)
            {
                if (item.FullName != null)
                {
                    //if (item.FullName.Trim().Length > 3)
                    //{
                    string FullName = RemoveExtraCharacters(item.FullName);
                    int Count = 0;
                    string[] FullNameDB = FullName.Split(' ');

                    for (int Index = 0; Index < Name.Length; Index++)
                    {
                        var temp = Name[Index];
                        if (temp != null)
                        {
                            if (temp != "")
                            {
                                for (int Counter = 0; Counter < FullNameDB.Length; Counter++)
                                {
                                    FullNameDB[Counter] = RemoveExtraCharacters(FullNameDB[Counter]);

                                    bool FullNameComponentIsEqualsToNameComponentAndIsNotNull =
                                    (FullNameDB[Counter] != null && 
                                    FullNameDB[Counter].ToLower().Equals(Name[Index].ToLower())
                                    );

                                    bool FullNameComponentStartWith = (FullNameDB[Counter].ToLower().
                                    StartsWith(Name[Index].ToLower()));

                                    if (FullNameComponentIsEqualsToNameComponentAndIsNotNull)
                                    {
                                        Count += 1;
                                        break;
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
                if (record.DateOfInspection.HasValue)
                {
                    MatchedRecord.DateOfInspection = record.DateOfInspection;
                }
               
                MatchedRecords.Add(MatchedRecord);
            }
            return MatchedRecords;
        }

        public ComplianceForm RollUpSummary(ComplianceForm form)  //previously UpdateFindings
        {

            int SiteCount = form.SiteSources.Count;
            int FullMatchesFoundInvestigatorCount = 0;
            int PartialMatchesFoundInvestigatorCount = 0;

            int IssuesFoundInvestigatorCount = 0;
            int ReviewCompletedInvestigatorCount = 0;
            int ExtractedInvestigatorCount = 0;
            int ExtractionErrorInvestigatorCount = 0;
            int ExtractionPendingInvestigatorCount = 0;

            //Pradeep 20Dec2016
            form.PartialMatchesFoundInvestigatorCount = 0;
            form.FullMatchesFoundInvestigatorCount = 0;
            form.IssuesFoundInvestigatorCount = 0;
            form.ReviewCompletedInvestigatorCount = 0;
            form.ExtractedInvestigatorCount = 0;

           var AllSites = form.SiteSources;
            AllSites.ToList().ForEach(x => x.IssuesIdentified = false);
            foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
            {
                Investigator.TotalIssuesFound = 0;

                int PartialMatchSiteCount = 0;
                int FullMatchSiteCount = 0;
                int IssuesFoundSiteCount = 0;
                int ReviewCompletedSiteCount = 0;
                int ExtractionPendingSiteCount = 0;

                //Pradeep 20Dec2016
                Investigator.Sites_PartialMatchCount = 0;
                Investigator.Sites_FullMatchCount = 0;
                Investigator.IssuesFoundSiteCount = 0;
                Investigator.ReviewCompletedSiteCount = 0;

                
                foreach (SiteSearchStatus searchStatus in Investigator.SitesSearched)
                {
                    if (
                        //searchStatus.ExtractionMode.ToLower() == "db" 
                        //|| searchStatus.ExtractionMode.ToLower() == "live") && searchStatus.ExtractedOn == null
                        searchStatus.ExtractedOn == null
                        && searchStatus.ExtractionMode.ToLower() == "live"
                        && !(searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                        )
                    {
                        searchStatus.ExtractionPending = true;
                        ExtractionPendingSiteCount += 1;
                    }
                    searchStatus.IssuesFound = 0; //Pradeep 20Dec2016

                    var ListOfFindings = form.Findings;

                    var Findings = ListOfFindings.Where(
                        x => x.SiteEnum == searchStatus.siteEnum).ToList();

                    int IssuesFound = 0;
                    int InvId = 0;
                    foreach (Finding Finding in Findings)
                    {
                        if (Finding != null && Finding.IsAnIssue &&
                            Finding.InvestigatorSearchedId == Investigator.Id 
                             )
                        {
                            InvId = Finding.InvestigatorSearchedId;
                            IssuesFound += 1;
                            
                        }
                    }
                    searchStatus.IssuesFound = IssuesFound;
                    Investigator.TotalIssuesFound += IssuesFound;

                    var Site = form.SiteSources.Find
                        (x => x.SiteEnum == searchStatus.siteEnum);

                    if (IssuesFound > 0 && Investigator.Id == InvId)
                        Site.IssuesIdentified = true;
                   //else
                    //    Site.IssuesIdentified = false;

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
                Investigator.ExtractionPendingSiteCount = ExtractionPendingSiteCount;

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
                if (Investigator.ReviewCompletedSiteCount == SiteCount)
                {
                     ReviewCompletedInvestigatorCount += 1;
                }
                if (Investigator.ExtractedOn != null)
                {
                    ExtractedInvestigatorCount += 1;
                }

                if (Investigator.ExtractionErrorSiteCount > 0)
                {
                    ExtractionErrorInvestigatorCount += 1;
                }

                if (Investigator.ExtractionPendingSiteCount > 0)
                {
                    ExtractionPendingInvestigatorCount += 1;
                }
            }

            form.PartialMatchesFoundInvestigatorCount = PartialMatchesFoundInvestigatorCount;
            form.FullMatchesFoundInvestigatorCount = FullMatchesFoundInvestigatorCount;
            form.IssuesFoundInvestigatorCount = IssuesFoundInvestigatorCount;
            form.ReviewCompletedInvestigatorCount = ReviewCompletedInvestigatorCount;
            form.ExtractedInvestigatorCount = ExtractedInvestigatorCount;
            form.ExtractionErrorInvestigatorCount = ExtractionErrorInvestigatorCount;
            form.ExtractionPendingInvestigatorCount = ExtractionPendingInvestigatorCount;

            return form;
        }

        public bool UpdateRollUpSummary(Guid formId)  
        {
            var form = _UOW.ComplianceFormRepository.FindById(formId);
            RollUpSummary(form);
            _UOW.ComplianceFormRepository.UpdateComplianceForm(formId, form);
            return true;
        }

        public void AddMatchingRecords(ComplianceForm frm, ILog log, string ErrorScreenCaptureFolder, string siteType)
        {
            int InvestigatorId = 1;
            frm.ExtractedOn = DateTime.Now; //last extracted on
            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                //var ListOfSiteSearchStatus = new List<SiteSearchStatus>();

                var InvestigatorName = RemoveExtraCharacters(inv.Name);

                var ComponentsInInvestigatorName =
                    InvestigatorName.Trim().Split(' ').Count();

                inv.ExtractedOn = DateTime.Now;
                inv.HasExtractionError = true; // until set to false.
                inv.IssuesFoundSiteCount = 0;
                inv.ReviewCompletedCount = 0;

                bool HasExtractionError = false; //for rollup value for Investigator
                int ExtractionErrorSiteCount = 0;
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

                    if (!(searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified ))
                    {
                        if (searchStatus.HasExtractionError == true || searchStatus.ExtractedOn == null)
                        {
                            searchRequired = true;
                        }
                    }

                    //10Feb2017-todo: siteSource.ExtractionMode.ToLower() = "db") //get db Sites only, live sites are extracted through windows service
                    if (searchRequired == true && 
                        siteSource.ExtractionMode.ToLower() == siteType)
                    {
                        try
                        {
                            //clear previously added matching records.
                            frm.Findings.RemoveAll(x => (x.InvestigatorSearchedId == inv.Id) && (x.SiteEnum == searchStatus.siteEnum) && x.IsMatchedRecord == true);

                            DateTime? SiteLastUpdatedOn = null;
                            var MatchedRecords = GetMatchedRecords(siteSource, InvestigatorName, log,
                               ErrorScreenCaptureFolder,  ComponentsInInvestigatorName,
                               out SiteLastUpdatedOn);

                            siteSource.SiteSourceUpdatedOn = SiteLastUpdatedOn;

                            GetFullAndPartialMatchCount(MatchedRecords, searchStatus, ComponentsInInvestigatorName);

                            inv.Sites_PartialMatchCount +=
                                searchStatus.PartialMatchCount;
                            inv.Sites_FullMatchCount +=
                                searchStatus.FullMatchCount;

                            //inv.Id = InvestigatorId;

                            //To-Do: convert matchedRecords to Findings
                            foreach (MatchedRecord rec in MatchedRecords)
                            {
                                var finding = new Finding();
                                finding.Id = Guid.NewGuid();
                                finding.IsFullMatch = rec.IsFullMatch;
                                finding.MatchCount = rec.MatchCount;
                                finding.InvestigatorSearchedId = inv.Id;
                                finding.SourceNumber = siteSource.DisplayPosition;
                                finding.SiteEnum = siteSource.SiteEnum; 

                                finding.RecordDetails = rec.RecordDetails;
                                finding.RowNumberInSource = rec.RowNumber;

                                finding.IsMatchedRecord = true;
                                if (rec.DateOfInspection.HasValue)
                                {
                                    finding.DateOfInspection = rec.DateOfInspection;
                                }
                                
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

                            searchStatus.ExtractedOn = DateTime.Now;
                            if (MatchedRecords.Count == 0)
                            {
                                searchStatus.ReviewCompleted = true;
                            }

                            //ListOfSiteSearchStatus.Add(searchStatus);
                        }
                        catch (Exception ex)
                        {
                            HasExtractionError = true;  //for rollup to investigator
                            ExtractionErrorSiteCount += 1;
                            searchStatus.HasExtractionError = true;
                            searchStatus.ExtractionErrorMessage = 
                                "Data Extraction not successful - " + ex.Message;
                            log.WriteLog("Data extraction failed. Details: " + ex.Message);
                            // Log -- ex.Message + ex.InnerException.Message
                        }
                        finally
                        {
                           
                        }
                    }
                }
                inv.ExtractionErrorSiteCount = ExtractionErrorSiteCount;
                inv.HasExtractionError = HasExtractionError;
                //inv.SitesSearched = ListOfSiteSearchStatus;
                InvestigatorId += 1;
            }
        }

        public void AddLiveScanFindings(ComplianceForm frm, ILog log, string ErrorScreenCaptureFolder)
        {
            string siteType = "live";

            //int InvestigatorId = 1;
            frm.ExtractedOn = DateTime.Now; //last extracted on
            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                //var ListOfSiteSearchStatus = new List<SiteSearchStatus>();

                var InvestigatorName = RemoveExtraCharacters(inv.Name);

                var ComponentsInInvestigatorName =
                    InvestigatorName.Trim().Split(' ').Count();

                //inv.ExtractedOn = DateTime.Now;
                //inv.HasExtractionError = true; // until set to false.
                //inv.IssuesFoundSiteCount = 0;
                //inv.ReviewCompletedCount = 0;

                //bool HasExtractionError = false; //for rollup value for Investigator
                //int ExtractionErrorSiteCount = 0;
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

                    if (!(searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || searchStatus.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified))
                    {
                        if (searchStatus.HasExtractionError == true || searchStatus.ExtractedOn == null)
                        {
                            searchRequired = true;
                        }
                    }

                    //10Feb2017-todo: siteSource.ExtractionMode.ToLower() = "db") //get db Sites only, live sites are extracted through windows service
                    if (searchRequired == true &&
                        siteSource.ExtractionMode.ToLower() == siteType)
                    {
                        try
                        {
                            //clear previously added matching records.
                            frm.Findings.RemoveAll(x => (x.InvestigatorSearchedId == inv.Id) && (x.SiteEnum == searchStatus.siteEnum) && x.IsMatchedRecord == true);

                            //new:
                            DateTime? SiteLastUpdatedOn1 = null;
                          
                                var findings = getFindings(siteSource, InvestigatorName, inv.Id, log,
                               ErrorScreenCaptureFolder, ComponentsInInvestigatorName,
                               out SiteLastUpdatedOn1);

                            if (findings.Count > 0)
                            {
                                _UOW.ComplianceFormRepository.AddFindings(frm.RecId.Value, findings);
                            }
                               
  
                            DateTime? SiteLastUpdatedOn = null;
                            //var MatchedRecords = GetMatchedRecords(siteSource, InvestigatorName, log,
                            //   ErrorScreenCaptureFolder, ComponentsInInvestigatorName,
                            //   out SiteLastUpdatedOn);

                            siteSource.SiteSourceUpdatedOn = SiteLastUpdatedOn;

                            //searchStatus - full / partial match count
                            //GetFullAndPartialMatchCount(MatchedRecords, searchStatus, ComponentsInInvestigatorName);
                            //record.MatchCount >= ComponentsInInvestigatorName
                            searchStatus.FullMatchCount = findings.Where(x => x.MatchCount >= ComponentsInInvestigatorName).Count();
                            searchStatus.PartialMatchCount = findings.Count - searchStatus.FullMatchCount;


                            //Handeled in Rollup.
                            //inv.Sites_PartialMatchCount +=
                            //    searchStatus.PartialMatchCount;
                            //inv.Sites_FullMatchCount +=
                            //    searchStatus.FullMatchCount;

                            //??
                            //inv.Id = InvestigatorId;

                            //To-Do: convert matchedRecords to Findings
                            //foreach (MatchedRecord rec in MatchedRecords)
                            //{
                            //    var finding = new Finding();
                            //    finding.Id = Guid.NewGuid();
                            //    finding.MatchCount = rec.MatchCount;
                            //    finding.InvestigatorSearchedId = inv.Id;
                            //    finding.SourceNumber = siteSource.DisplayPosition;
                            //    finding.SiteEnum = siteSource.SiteEnum;

                            //    finding.RecordDetails = rec.RecordDetails;
                            //    finding.RowNumberInSource = rec.RowNumber;

                            //    finding.IsMatchedRecord = true;
                            //    if (rec.DateOfInspection.HasValue)
                            //    {
                            //        finding.DateOfInspection = rec.DateOfInspection;
                            //    }

                            //    finding.InvestigatorName = inv.Name;
                            //    finding.Links = rec.Links;

                            //    frm.Findings.Add(finding);
                            //}

                            //Review:
                            //siteSource.SiteSourceUpdatedOn' is the date of update at the time of creation of CompForm
                            //
                            //replace  '= siteSource.SiteSourceUpdatedOn' SiteSourceUpdatedOn at the time of data extraction
                            searchStatus.SiteSourceUpdatedOn = siteSource.SiteSourceUpdatedOn;
                            searchStatus.HasExtractionError = false;
                            searchStatus.ExtractionErrorMessage = "";
                            searchStatus.ExtractionPending = false;
                            searchStatus.ExtractedOn = DateTime.Now;
                            //if (MatchedRecords.Count == 0)
                            //{
                            //    searchStatus.ReviewCompleted = true;
                            //}
                            if (findings.Count == 0)
                            {
                                searchStatus.ReviewCompleted = true;
                            }

                            

                            

                            //ListOfSiteSearchStatus.Add(searchStatus);
                        }
                        catch (Exception ex)
                        {
                            //HasExtractionError = true;  //for rollup to investigator
                            //ExtractionErrorSiteCount += 1;
                            searchStatus.ExtractionPending = true;
                            searchStatus.ExtractedOn = null;

                            searchStatus.HasExtractionError = true;
                            searchStatus.ExtractionErrorMessage =
                                "Data Extraction not successful - " + ex.Message;
                            log.WriteLog("Data extraction failed. Details: " + ex.Message);
                            // Log -- ex.Message + ex.InnerException.Message
                        }
                        finally
                        {

                        }
                        UpdateSearchStatus(frm.RecId.Value, inv.Id, searchStatus);
                    }
                }

                //handled
                //inv.ExtractionErrorSiteCount = ExtractionErrorSiteCount;
                //inv.HasExtractionError = HasExtractionError;
                //inv.SitesSearched = ListOfSiteSearchStatus;
                //InvestigatorId += 1;
                
            }
        }

        private List<Finding> getFindings(SiteSource siteSource, string InvestigatorName, int InvestigatorId, ILog log,  string ErrorScreenCaptureFolder, int ComponentsInInvestigatorName, out DateTime? SiteLastUpdatedOn)
        {

            var retFindings = new List<Finding>(); 
            var MatchedRecords = GetMatchedRecords(siteSource, InvestigatorName, log,
                               ErrorScreenCaptureFolder, ComponentsInInvestigatorName,
                               out SiteLastUpdatedOn);

            //To-Do: convert matchedRecords to Findings
            foreach (MatchedRecord rec in MatchedRecords)
            {
                var finding = new Finding();
                finding.Id = Guid.NewGuid();
                finding.MatchCount = rec.MatchCount;
                finding.InvestigatorSearchedId = InvestigatorId;
                finding.SourceNumber = siteSource.DisplayPosition;
                finding.SiteEnum = siteSource.SiteEnum;

                finding.RecordDetails = rec.RecordDetails;
                finding.RowNumberInSource = rec.RowNumber;

                finding.IsMatchedRecord = true;
                if (rec.DateOfInspection.HasValue)
                {
                    finding.DateOfInspection = rec.DateOfInspection;
                }

                finding.InvestigatorName = InvestigatorName;
                finding.Links = rec.Links;

                retFindings.Add(finding);
            }
            return retFindings;
        }

        private bool UpdateSearchStatus(Guid formId, int InvestigatyorId, SiteSearchStatus siteStatus)
        {
            var dbForm = _UOW.ComplianceFormRepository.FindById(formId);

            foreach (InvestigatorSearched inv in dbForm.InvestigatorDetails)
            {
                if (inv.Id == InvestigatyorId)
                {
                    foreach (SiteSearchStatus ss in inv.SitesSearched)
                    {
                        if (ss.siteEnum == siteStatus.siteEnum)
                        {
                            //replace values
                            ss.DisplayPosition = siteStatus.DisplayPosition;
                            ss.ExtractedOn = siteStatus.ExtractedOn;
                            ss.HasExtractionError = siteStatus.HasExtractionError;
                            ss.ExtractionErrorMessage = siteStatus.ExtractionErrorMessage;
                            //ss.ExtractionPending ??
                            ss.FullMatchCount = siteStatus.FullMatchCount;
                            //ss.HasExtractionError
                            //ss.IssuesFound
                            ss.PartialMatchCount = siteStatus.PartialMatchCount;
                            ss.ReviewCompleted = siteStatus.ReviewCompleted;
                            ss.SiteSourceUpdatedOn = siteStatus.SiteSourceUpdatedOn;
                            ss.ExtractionPending = siteStatus.ExtractionPending;
                            _UOW.ComplianceFormRepository.UpdateComplianceForm(formId, dbForm);
                            break;
                        }
                    }
                }

                
            }
           
            return true;
        }

        #region Save related


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

        //When Site is removed, (Optional site), the corresponding SearchStatus of all Investigators become orphanced.
        private void RemoveOrphanedSearchStatusRecords(ComplianceForm frm)
        {
            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                foreach (SiteSearchStatus siteSearchStatus in inv.SitesSearched)
                {
                    var siteSource = frm.SiteSources.Find(x => x.SiteEnum == siteSearchStatus.siteEnum);
                    if (siteSource == null) // Site Source not available, remove 
                    {
                        siteSearchStatus.DisplayPosition = -1;
                    }
                }
                inv.SitesSearched.RemoveAll(x => x.DisplayPosition == -1);
            }
        }

        private void AdjustDisplayPositionOfSiteSources(ComplianceForm frm)
        {
            //When intermediate sites are deleted.
            var SrNo = 0;
            foreach (SiteSource site in frm.SiteSources)
            {
                SrNo += 1;
                site.DisplayPosition = SrNo;
            }
        }

        private void CorrectDisplayPositionOfSearchStatusRecords(ComplianceForm frm)
        {
            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                foreach (SiteSearchStatus siteSearchStatus in inv.SitesSearched)
                {
                    siteSearchStatus.DisplayPosition = frm.SiteSources.Find(x => x.SiteEnum == siteSearchStatus.siteEnum).DisplayPosition;
 
                }
             }
        }

        private void CorrectSourceNumberInFindings(ComplianceForm frm)
        {
            foreach (Finding finding in frm.Findings)
            {
                finding.SourceNumber = frm.SiteSources.Find(x => x.SiteEnum == finding.SiteEnum).DisplayPosition;
            }
        }

        //When Site is removed, (Optional site), the Findings for those sites become orphanced.
        private void RemoveOrphanedFindings(ComplianceForm frm)
        {
            //list.RemoveAll( item => !list2.Contains(item));
           

            foreach (Finding finding in frm.Findings)
            {
                var siteSource = frm.SiteSources.Find(x => x.SiteEnum == finding.SiteEnum);
                if (siteSource == null) // Site Source not available, remove 
                {
                     if (siteSource == null) // Site Source not available, remove 
                    {
                        finding.MatchCount = -1;  //used as a flag to delete records
                    }
                }
             }
            frm.Findings.RemoveAll(x => x.MatchCount == -1);
        }
        #endregion

        public List<MatchedRecord> GetMatchedRecords(SiteSource site,
            string NameToSearch, ILog log, string ErrorScreenCaptureFolder,
            int ComponentsInInvestigatorName, out DateTime? SiteLastUpdatedOn)
        {
            switch (site.SiteEnum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInvestigatorPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLettersPageMatchedRecords(
                        site.SiteDataId, NameToSearch,
                        ErrorScreenCaptureFolder,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.ERRProposalToDebarPage:
                    return GetERRProposalToDebarPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssurancePageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetClinicalInvestigatorDisqualificationPageMatchedRecords(
                        site.SiteDataId, NameToSearch,
                        ErrorScreenCaptureFolder,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSAdministrativeActionPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabasePageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCIAPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSAMPageMatchedRecords(site.SiteDataId, NameToSearch,
                        ErrorScreenCaptureFolder,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSDNPageMatchedRecords(site.SiteDataId,
                        NameToSearch,
                        ComponentsInInvestigatorName,
                        out SiteLastUpdatedOn);

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
                {
                    SearchStatus.FullMatchCount += 1;
                    record.IsFullMatch = true;
                }
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
        }

        public List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active=true, bool ReviewCompleted = false)
        {
            var retList = new List<PrincipalInvestigator>();

            retList = getPrincipalInvestigators(AssignedTo, Active);

            return retList.Where(x =>  x.ReviewCompleted == ReviewCompleted).ToList();
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
            item.ExtractionErrorInvestigatorCount = compForm.ExtractionErrorInvestigatorCount;
            item.ExtractionPendingInvestigatorCount = compForm.ExtractionPendingInvestigatorCount;
            item.EstimatedExtractionCompletionWithin = compForm.EstimatedExtractionCompletionWithin;

            foreach (InvestigatorSearched Investigator in compForm.InvestigatorDetails)
            {
                if(Investigator.Role.ToLower() == "sub")
                {
                    var SubInv = new SubInvestigator();
                    SubInv.Name = Investigator.Name;
                    SubInv.Status = Investigator.Status;
                    SubInv.StatusEnum = Investigator.StatusEnum;
                    item.SubInvestigators.Add(SubInv);
                }
            }
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
                inv.Name == CompFormFilter.InvestigatorName).ToList()).ToList();
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

            if (CompFormFilter.SearchedOnFrom != null) 
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

            IWriter writer = new CreateComplianceFormWord();

            GenerateComplianceForm(DownloadFolder, TemplateFolder, 
                ComplianceFormId, writer,
                ".docx");
 
            return DownloadFolder;
        }
        #endregion

        #region ComplianceFormGeneration - both PDF and Word
        public string GenerateComplianceForm(
            string DownloadFolder, string TemplateFolder,
            Guid? ComplianceFormId, IWriter writer, 
            string FileExtension)
        {
            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var PI = RemoveSpecialCharacters(form.InvestigatorDetails.FirstOrDefault().Name);

            var ProjectNumber = RemoveSpecialCharacters(form.ProjectNumber);

            var GeneratedFileName = ProjectNumber + "_" + PI + FileExtension;

            var GeneratedFileNameNPath = DownloadFolder + GeneratedFileName;

            writer.Initialize(TemplateFolder, GeneratedFileNameNPath);

            writer.WriteParagraph("INVESTIGATOR COMPLIANCE SEARCH FORM");

            writer.AddFormHeaders(form.ProjectNumber, form.SponsorProtocolNumber,
                form.Institute, form.Address);

            //InvestigatorDetailsTable
            writer.WriteParagraph("Investigators:");

            string[] TableHeaders = InvestigatorTableHeaders();

            writer.AddTableHeaders(TableHeaders, 4, 1);

            foreach(InvestigatorSearched Investigator in form.InvestigatorDetails)
            {
                string[] CellValues = new string[]
                {
                    Investigator.Name,
                    Investigator.Qualification,
                    Investigator.MedicalLiceseNumber,
                    Investigator.Role
                };
                writer.FillUpTable(CellValues);
            }
            writer.SaveChanges();

            //SitesTable
            writer.WriteParagraph(
                "Relevant sources of Investigator information, " +
                "against which this Investigator has been checked.");

            TableHeaders = SitesTableHeaders();
            writer.AddTableHeaders(TableHeaders, 5, 2);

            int ColumnIndex = 1;
            int RowIndex = 1;
            foreach(SiteSource Site in form.SiteSources)
            {
                if (!Site.IsMandatory)
                    continue;

 				string SiteSourceUpdatedOn = "";

                if (Site.SiteSourceUpdatedOn != null)
                    SiteSourceUpdatedOn =
                        Site.SiteSourceUpdatedOn.Value.ToString("dd MMM yyyy");

                string[] CellValues = new string[]
                {
                    RowIndex.ToString(),
                    Site.SiteName,
                    SiteSourceUpdatedOn,
                    Site.SiteUrl,
                    Site.IssuesIdentified ? "Yes" : "No"
                };

                writer.FillUpTable(CellValues);

                RowIndex += 1;
                ColumnIndex += 1;
            }
            writer.SaveChanges();

            //AdditionalSitesTable
            writer.WriteParagraph("Addtional sources:");
            TableHeaders = SitesTableHeaders();
            writer.AddTableHeaders(TableHeaders, 5, 3);

            foreach(SiteSource Site in form.SiteSources)
            {
                if(!Site.IsMandatory)
                {
                    string SiteSourceUpdatedOn = "";

                    if (Site.SiteSourceUpdatedOn != null)
                        SiteSourceUpdatedOn =
                            Site.SiteSourceUpdatedOn.Value.ToString("dd MMM yyyy");

                    string[] CellValues = new string[]
                    {
                        RowIndex.ToString(),
                        Site.SiteName,
                        SiteSourceUpdatedOn,
                        Site.SiteUrl,
                        Site.IssuesIdentified ? "Yes" : "No"
                    };
                    writer.FillUpTable(CellValues);
                    RowIndex += 1;
                }
            }

            writer.SaveChanges();

            //FindingsTable
            writer.WriteParagraph(
                "Additional details for issues (Yes) identified above:");

            TableHeaders = FindingsTableHeaders();
            writer.AddTableHeaders(TableHeaders, 4, 4);

            foreach(Finding finding in form.Findings)
            {
                string DateOfInspection = "";
                if (finding.DateOfInspection != null)
                    DateOfInspection =
                        finding.DateOfInspection.Value.ToString("dd MMM yyyy");

                //if (finding.Selected)
                //{
                //    string[] CellValues = new string[]
                //    {
                //        finding.SourceNumber.ToString(),
                //        finding.InvestigatorName,
                //        DateOfInspection,
                //        finding.Observation
                //    };
                //    writer.FillUpTable(CellValues);
                //}
                //Pradeep: 20Feb2017:
                if (finding.Selected)
                {
                    string Observation = finding.Observation;

                    string[] CellValues = new string[]
                    {
                        finding.SourceNumber.ToString(),
                        finding.InvestigatorName,
                        DateOfInspection,
                        Observation
                    };
                    writer.FillUpTable(CellValues);
                }
            }
            writer.SaveChanges();

            //SearchedByTable
            writer.WriteParagraph("Search Performed By:");
            writer.AddSearchedBy(form.AssignedTo, DateTime.Now.ToString("dd MMM yyyy"));

            writer.SaveChanges();

            writer.AddFooterPart("Updated On: " + form.UpdatedOn.ToString("dd MMM yyyy"));

            writer.CloseDocument();

            //writer.AttachFile(@"C:\Development\test.pdf", GeneratedFileNameNPath);

            return DownloadFolder + GeneratedFileName;
        }

        private string[] InvestigatorTableHeaders()
        {
            return new string[]
            {
                "Investigator Name",
                "Qualification",
                "Medical License Number",
                "Role"
            };
        }

        private string[] SitesTableHeaders()
        {
            return new string[]
            {
                "Source #",
                "Source Name",
                "Source Date",
                "WebLink",
                "Issues Identified"
            };
        }

        private string[] FindingsTableHeaders()
        {
            return new string[]
            {
                "Source#",
                "Investigator Name",
                "Date Of Inspection/Action",
                "Description of findings"
            };
        }

        #endregion

        #region ByPatrick

        //3Dec2016
        public MemoryStream GenerateComplianceForm(Guid? ComplianceFormId)
        {
            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var UtilitiesObject = new CreateComplianceFormWord();

            var FileName = form.InvestigatorDetails.FirstOrDefault().Name + ".docx";

            return UtilitiesObject.CreateComplianceForm(form, FileName);
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

        //public ComplianceForm UpdateFindings(ComplianceForm form)
        //{
        //    foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
        //    {
        //        Investigator.TotalIssuesFound = 0;

        //        foreach (SiteSearchStatus searchStatus in Investigator.SitesSearched)
        //        {
        //            var ListOfFindings = form.Findings;

        //            var Findings = ListOfFindings.Where(
        //                x => x.SiteEnum == searchStatus.siteEnum).ToList();

        //            int IssuesFound = 0;
        //            int InvId = 0;
        //            foreach (Finding Finding in Findings)
        //            {
        //                if (Finding != null && Finding.IsAnIssue &&
        //                    Finding.InvestigatorSearchedId == Investigator.Id)
        //                {
        //                    InvId = Finding.InvestigatorSearchedId;
        //                    IssuesFound += 1;
        //                    searchStatus.IssuesFound = IssuesFound;
        //                }
        //            }
        //            Investigator.TotalIssuesFound += IssuesFound;

        //            var Site = form.SiteSources.Find
        //                (x => x.SiteEnum == searchStatus.siteEnum);

        //            if (IssuesFound > 0 && Investigator.Id == InvId)
        //                Site.IssuesIdentified = true;
        //        }
        //    }
        //    return form;
        //}


        public List<MatchedRecord> GetFDADebarPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            FDADebarPageSiteData FDASearchResult =
                _UOW.FDADebarPageRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                FDASearchResult.DebarredPersons,
                InvestigatorName);  //updates list with match count

            var DebarList = FDASearchResult.DebarredPersons.Where(
               debarredList => debarredList.Matched > 0).ToList();

            SiteLastUpdatedOn = FDASearchResult.SiteLastUpdatedOn;

            if (DebarList == null)
                return null;

            //Patrick: Further refactoring possible, ConvertToMatchedRecords may not be required:
            return ConvertToMatchedRecords(DebarList);

        }

        public List<MatchedRecord> GetClinicalInvestigatorPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            ClinicalInvestigatorInspectionSiteData CIILSearchResult =
                _UOW.ClinicalInvestigatorInspectionListRepository.FindById(SiteDataId);

            UpdateMatchStatus(CIILSearchResult.ClinicalInvestigatorInspectionList,
                InvestigatorName);  //updates list with match count

            var ClinicalInvestigatorList = CIILSearchResult.ClinicalInvestigatorInspectionList.Where(
               ClinicalList => ClinicalList.Matched > 0).ToList();

            SiteLastUpdatedOn = CIILSearchResult.SiteLastUpdatedOn;

            if (ClinicalInvestigatorList == null)
                return null;

            return ConvertToMatchedRecords(ClinicalInvestigatorList);
        }

        public List<MatchedRecord> GetFDAWarningLettersPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, string ErrorScreenCaptureFolder,
            int ComponentsInInvestigatorName, out DateTime? SiteLastUpdatedOn)
        {
            DateTime? temp = null; 
            _SearchEngine.ExtractData(
                SiteEnum.FDAWarningLettersPage, NameToSearch, 
                ErrorScreenCaptureFolder, MatchCountLowerLimit, out temp);

            var siteData = _SearchEngine.SiteData;
            SiteLastUpdatedOn = temp;
             
            var baseSiteData = _SearchEngine.baseSiteData;

            UpdateMatchStatus(siteData, NameToSearch);

            var FDAWarningLetterList =
                siteData.Where(
                FDAList => FDAList.Matched > 0);

            if (siteData == null)
                return null;

            return ConvertToMatchedRecords(FDAWarningLetterList);
        }

        public List<MatchedRecord> GetERRProposalToDebarPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            ERRProposalToDebarPageSiteData ERRSearchResult =
                _UOW.ERRProposalToDebarRepository.FindById(SiteDataId);

            UpdateMatchStatus(ERRSearchResult.ProposalToDebar,
                InvestigatorName);  //updates list with match count

            var ERRList = ERRSearchResult.ProposalToDebar.Where(
               ErrList => ErrList.Matched > 0).ToList();

            SiteLastUpdatedOn = ERRSearchResult.SiteLastUpdatedOn;

            if (ERRList == null)
                return null;

            return ConvertToMatchedRecords(ERRList);
        }

        public List<MatchedRecord> GetAdequateAssurancePageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            AdequateAssuranceListSiteData AdequateAssuranceSearchResult =
                _UOW.AdequateAssuranceListRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                AdequateAssuranceSearchResult.AdequateAssurances,
                InvestigatorName);  //updates list with match count

            var AdequateAssuranceList =
                AdequateAssuranceSearchResult.AdequateAssurances.Where(
                AssuranceList => AssuranceList.Matched > 0).ToList();

            SiteLastUpdatedOn = AdequateAssuranceSearchResult.SiteLastUpdatedOn;

            if (AdequateAssuranceList == null)
                return null;

            return ConvertToMatchedRecords(AdequateAssuranceList);
        }

      public List<MatchedRecord> GetClinicalInvestigatorDisqualificationPageMatchedRecords(
            Guid? SiteDataId, string NameToSearch, string ErrorScreenCaptureFolder,
            int ComponentsInInvestigatorName, out DateTime? SiteLastUpdatedOn)
        {
            DateTime? temp = null;
            _SearchEngine.ExtractData(
                SiteEnum.ClinicalInvestigatorDisqualificationPage, NameToSearch,
                ErrorScreenCaptureFolder, MatchCountLowerLimit, out temp);

            var siteData = _SearchEngine.SiteData;
            SiteLastUpdatedOn = temp;

            var baseSiteData = _SearchEngine.baseSiteData;

            UpdateMatchStatus(siteData, NameToSearch);

            var DisqualificationSiteData =
                siteData.Where(site => site.Matched > 0);

            if (siteData == null)
                return null;

            return ConvertToMatchedRecords(DisqualificationSiteData);
        }

        public List<MatchedRecord> GetCBERClinicalInvestigatorPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            CBERClinicalInvestigatorInspectionSiteData CBERSearchResult =
                _UOW.CBERClinicalInvestigatorRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                CBERSearchResult.ClinicalInvestigator,
                InvestigatorName);  //updates list with match count

            var ClinicalInvestigatorList = CBERSearchResult.ClinicalInvestigator.Where(
               CBERList => CBERList.Matched > 0).ToList();

            SiteLastUpdatedOn = CBERSearchResult.SiteLastUpdatedOn;

            if (ClinicalInvestigatorList == null)
                return null;

            return ConvertToMatchedRecords(ClinicalInvestigatorList);
        }

        public List<MatchedRecord> GetExclusionDatabasePageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName, int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            ExclusionDatabaseSearchPageSiteData ExclusionSearchResult =
                _UOW.ExclusionDatabaseSearchRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                ExclusionSearchResult.ExclusionSearchList,
                InvestigatorName);  //updates list with match count

            var ExclusionDBList = ExclusionSearchResult.ExclusionSearchList.Where(
               ExclusionList => ExclusionList.Matched > 0).ToList();

            SiteLastUpdatedOn = ExclusionSearchResult.SiteLastUpdatedOn;

            if (ExclusionDBList == null)
                return null;

            return ConvertToMatchedRecords(ExclusionDBList);
        }

        public List<MatchedRecord> GetPHSAdministrativeActionPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName, int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            PHSAdministrativeActionListingSiteData PHSSearchResult =
                _UOW.PHSAdministrativeActionListingRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                PHSSearchResult.PHSAdministrativeSiteData,
                InvestigatorName);  //updates list with match count

            var PHSList = PHSSearchResult.PHSAdministrativeSiteData.Where(
               PHSData => PHSData.Matched > 0).ToList();

            SiteLastUpdatedOn = PHSSearchResult.SiteLastUpdatedOn;

            if (PHSList == null)
                return null;

            return ConvertToMatchedRecords(PHSList);
        }

        public List<MatchedRecord> GetCIAPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName,
            int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            CorporateIntegrityAgreementListSiteData CIASearchResult =
                _UOW.CorporateIntegrityAgreementRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                CIASearchResult.CIAListSiteData,
                InvestigatorName);  //updates list with match count

            var CIAList = CIASearchResult.CIAListSiteData.Where(
               CIAData => CIAData.Matched > 0).ToList();

            SiteLastUpdatedOn = CIASearchResult.SiteLastUpdatedOn;

            if (CIAList == null)
                return null;

            return ConvertToMatchedRecords(CIAList);
        }

       public List<MatchedRecord> GetSAMPageMatchedRecords(Guid? SiteDataId,
            string NameToSearch, string ErrorScreenCaptureFolder, 
            int ComponentsInIvestigatorName, out DateTime? SiteLastUpdatedOn)
        {
            DateTime? temp = null;
            _SearchEngine.ExtractData(SiteEnum.SystemForAwardManagementPage, NameToSearch,
                ErrorScreenCaptureFolder, MatchCountLowerLimit, out temp);

            var siteData = _SearchEngine.SiteData;
            SiteLastUpdatedOn = temp;
            var baseSiteData = _SearchEngine.baseSiteData;

            UpdateMatchStatus(siteData, NameToSearch);

            var DisqualificationSiteData =
                siteData.Where(site => site.Matched > 0);

            if (siteData == null)
                return null;

            return ConvertToMatchedRecords(DisqualificationSiteData);
        }

        public List<MatchedRecord> GetSDNPageMatchedRecords(Guid? SiteDataId,
            string InvestigatorName, int ComponentsInInvestigatorName,
            out DateTime? SiteLastUpdatedOn)
        {
            SpeciallyDesignatedNationalsListSiteData SDNSearchResult =
                _UOW.SpeciallyDesignatedNationalsRepository.FindById(SiteDataId);

            UpdateMatchStatus(
                SDNSearchResult.SDNListSiteData,
                InvestigatorName);

            var SDNList = SDNSearchResult.SDNListSiteData.Where(
               SDNData => SDNData.Matched > 0).ToList();

            SiteLastUpdatedOn = SDNSearchResult.SiteLastUpdatedOn;            

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
        public List<string> ValidateExcelInputs(List<string> ExcelInputRow, int Row)
        {
            var ValidationMessages = new List<string>();

            string ValidationMessage = null;

            var PrincipalInv = ExcelInputRow[0].Split(' ').Count();

            if (Row == 2 && ExcelInputRow[1].ToLower() != "principal")
            {
                ValidationMessages.Add(
                    "RowNumber: 1 - First Investigator must be a Principal Investigator");
            }

            if (ExcelInputRow[0].Trim() == "")
            {
                ValidationMessage = "RowNumber: " + Row + 
                    " - Investigator Name is null!";
                ValidationMessages.Add(ValidationMessage);
            }
            if(ExcelInputRow[0].ToLower().Contains("cannot find column"))
            {
                ValidationMessages.Add(ExcelInputRow[0]);
            }
            if(PrincipalInv <= 1)
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Investigator Name must have atleast two components " +
                    "separated with a space!";
                ValidationMessages.Add(ValidationMessage);
            }
            if(IsNumericOrHasSpecialCharacters(ExcelInputRow[0]))
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Investigator Name should not have any " +
                    "numbers or special characters!";
                ValidationMessages.Add(ValidationMessage);
            }
            if(ExcelInputRow[0].Length > 100)
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Investigator Name exceeds max character(100) limit!";
                ValidationMessages.Add(ValidationMessage);
            }
            if(ExcelInputRow[1].ToLower() != "principal" &&
                ExcelInputRow[1].ToLower() != "sub")
            {
                ValidationMessages.Add( "RowNumber: " + Row +
                    " - Role column should have either 'Principal' or 'Sub'");
            }
            if (ExcelInputRow[1].Length > 10)
            {
                ValidationMessages.Add("RowNumber: " + Row +
                    " - Role column exceeds max character(9) limit");
            }
            if (ExcelInputRow[2].Length > 100)
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Investigator ML Number exceeds max character(100) limit!";
                ValidationMessages.Add(ValidationMessage);
            }
            if (ExcelInputRow[2].ToLower().Contains("cannot find column"))
            {
                ValidationMessages.Add(ExcelInputRow[2]);
            }
            if (ExcelInputRow[3].Length > 100)
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Investigator Qualification exceeds max character(100) limit!";
                ValidationMessages.Add(ValidationMessage);
            }
            if (ExcelInputRow[3].ToLower().Contains("cannot find column"))
            {
                ValidationMessages.Add(ExcelInputRow[3]);
            }
            if (IsNumericOrHasSpecialCharacters(ExcelInputRow[3]))
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Qualification should not have any " +
                    "numbers or special characters!";
                ValidationMessages.Add(ValidationMessage);
            }

            if (ExcelInputRow[1].ToLower() == "sub")
                return ValidationMessages;

            if (ExcelInputRow[4] == "")
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Project Number is mandatory!";
                ValidationMessages.Add(ValidationMessage);
            }
            if(ExcelInputRow[4].Length > 100)
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Project Number exceeds max character(100) limit!";
                ValidationMessages.Add(ValidationMessage);
            }
            if(HasSpecialCharacters(ExcelInputRow[4]))
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Project Number should not have any special characters!";
                ValidationMessages.Add(ValidationMessage);
            }
            if (!IsValidProjectNumber(ExcelInputRow[4]))
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Change the project number format to - \"1234/5678\"";
                ValidationMessages.Add(ValidationMessage);
            }
            if (ExcelInputRow[4].ToLower().Contains("cannot find column"))
            {
                ValidationMessages.Add(ExcelInputRow[4]);
            }
            if (ExcelInputRow[5].Length > 100)
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Sponsor protocol number exceeds max character(100) limit!";
                ValidationMessages.Add(ValidationMessage);
            }
            if (ExcelInputRow[5].ToLower().Contains("cannot find column"))
            {
                ValidationMessages.Add(ExcelInputRow[5]);
            }
            if (ExcelInputRow[6].Length > 100)
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Institute Name exceeds max character(100) limit!";
                ValidationMessages.Add(ValidationMessage);
            }
            if (ExcelInputRow[6].ToLower().Contains("cannot find column"))
            {
                ValidationMessages.Add(ExcelInputRow[6]);
            }
            if (ExcelInputRow[7].Length > 500)
            {
                ValidationMessage = "RowNumber: " + Row +
                    " - Address exceeds max character(500) limit!";
                ValidationMessages.Add(ValidationMessage);
            }
            if (ExcelInputRow[7].ToLower().Contains("cannot find column"))
            {
                ValidationMessages.Add(ExcelInputRow[7]);
            }
            return ValidationMessages;
        }

        private bool IsNumericOrHasSpecialCharacters(string Value)
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
                    c == '*' || 
                    c == '_' || c == '&' || 
                    c == '@' || c == '!' || 
                    c == '%' || c == '^')
                    return true;
            }
            return false;
        }

        private bool IsValidProjectNumber(string Value)
        {
            string Expression = "^\\d{4}/\\d{4}$";
            return Regex.IsMatch(Value, Expression);
        }

      
        #endregion
    }
}
