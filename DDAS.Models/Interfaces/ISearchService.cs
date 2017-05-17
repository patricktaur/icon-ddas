using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDAS.Models.Interfaces
{
    public interface ISearchService
    {
  
        List<PrincipalInvestigator> getAllPrincipalInvestigators();
        List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active);
        List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active = true, bool ReviewCompleted = true);

        ComplianceForm GetNewComplianceForm(string UserName);

        void AddMatchingRecords(ComplianceForm frm);

        ComplianceForm ScanUpdateComplianceForm(ComplianceForm form);

        ComplianceForm UpdateComplianceForm(ComplianceForm form);
        bool UpdateComplianceFormNIgnoreIfNotFound(ComplianceForm form);
        void UpdateAssignedToData(string AssignedTo, bool Active, Guid? RecId);
       
        void UpdateExtractionQuePosition(Guid formId, int Position, DateTime ExtractionStartedAt, DateTime ExtractionEstimatedCompletion);

        ComplianceForm UpdateCompFormGeneralNInvestigatorsNOptionalSites(ComplianceForm form);
        bool UpdateFindings(UpdateFindigs updateFindings);
        bool UpdateInstituteFindings(UpdateInstituteFindings InstitueFinding);

        List<PrincipalInvestigator> GetComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter);

        InvestigatorSearched getInvestigatorSiteSummary(string compFormId, int InvestigatorId);

        List<InstituteFindingsSummaryViewModel> getInstituteFindingsSummary(Guid CompFormId);

        ComplianceForm RollUpSummary(ComplianceForm form);
        bool UpdateRollUpSummary(Guid formId);

        void AddLiveScanFindings(ComplianceForm frm);

        MemoryStream GenerateComplianceForm(Guid? ComplianceFormId);

        List<string> ValidateExcelInputs(List<string> ExcelInputRow, int Row);
        ExcelInput ReadDataFromExcelFile(string FilePathWithGUID);
        List<ComplianceForm> ReadUploadedFileData(ExcelInput ExcelInputData, 
            string UserName, string FilePathWithGUID, string UploadedFileName);

        string GenerateComplianceForm(
            Guid? ComplianceFormId, IWriter writer,
            string FileExtension);

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

    }
}
