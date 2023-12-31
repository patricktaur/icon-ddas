﻿using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDAS.Models.Interfaces
{
    public interface ISearchService
    {

        ComplianceForm GetComplianceForm(Guid ComplianceFormId);
        List<PrincipalInvestigator> getAllPrincipalInvestigators();
        List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active);
        List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active = true, bool ReviewCompleted = true);
        List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, ReviewStatusEnum ReviewStatus);
        List<PrincipalInvestigator> getPrincipalInvestigators(List<ComplianceForm> Forms);



        ComplianceForm GetNewComplianceForm(string UserName, string InputSource);

        void AddMatchingRecords(ComplianceForm frm);

        ComplianceForm ScanUpdateComplianceForm(ComplianceForm form);

        ComplianceForm UpdateComplianceForm(ComplianceForm form);
        bool UpdateComplianceFormNIgnoreIfNotFound(ComplianceForm form);
        void UpdateAssignedTo(Guid? RecId, string  AssignedBy, string AssignedFrom, string AssignedTo);
        void UpdateAssignedTo(string AssignedBy, AssignComplianceFormsTo AssignComplianceFormsTo);


        void UpdateExtractionQuePosition(Guid formId, int Position, DateTime ExtractionStartedAt, DateTime ExtractionEstimatedCompletion);

        ComplianceForm UpdateCompFormGeneralNInvestigatorsNOptionalSites(ComplianceForm form);
        bool UpdateFindings(UpdateFindigs updateFindings);
        bool UpdateInstituteFindings(UpdateInstituteFindings InstitueFinding);

        List<PrincipalInvestigator> GetComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter);
        List<PrincipalInvestigator> GetComplianceFormsFromFiltersWithReviewDates(
            ComplianceFormFilter CompFormFilter);

        

        List<PrincipalInvestigator> GetClosedComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter, string AssignedTo);

        List<PrincipalInvestigator> GetUnAssignedComplianceForms();

        InvestigatorSearched getInvestigatorSiteSummary(string compFormId, int InvestigatorId);

        List<InstituteFindingsSummaryViewModel> getInstituteFindingsSummary(Guid CompFormId);

        ComplianceForm RollUpSummary(ComplianceForm form);
        bool UpdateRollUpSummary(Guid formId);

        void AddLiveScanFindings(ComplianceForm frm);

        MemoryStream GenerateComplianceForm(Guid? ComplianceFormId);

        //List<string> ValidateExcelInputs(List<string> ExcelInputRow, int Row);
        ExcelInput ReadDataFromExcelFile(string FilePathWithGUID);
        List<ComplianceForm> ReadUploadedFileData(ExcelInput ExcelInputData, 
            string UserName, string FilePathWithGUID, string UploadedFileName);

        MemoryStream GenerateComplianceForm(
            Guid? ComplianceFormId, 
            IWriter writer,
            string FileExtension,
            out string FileName);

        bool AddAttachmentsToFindings(ComplianceForm from);

        List<Finding> GetSingleComponentMatchedRecords(
            Guid? SiteDataId,
            SiteEnum Enum,
            string FullName);

        //string GenerateOutputFile(
        //    IGenerateOutputFile GenerateOutputFile,
        //    List<ComplianceForm> forms);

        MemoryStream GenerateOutputFile(
            IGenerateOutputFile GenerateOutputFile,
            List<ComplianceForm> forms);

        bool UpdateQC(ComplianceForm Form);

        iSprintResponseModel.DDtoIsprintResponse ExportDataToIsprint(Guid ComplianceFormId);
        iSprintResponseModel.DDtoIsprintResponse ExportDataToIsprint(ComplianceForm Form);

        string GetUserFullName(string UserName);
    }
}
