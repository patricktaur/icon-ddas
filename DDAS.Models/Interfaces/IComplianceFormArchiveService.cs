using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDAS.Models.Interfaces
{
    public interface IComplianceFormArchiveService
    {

        ComplianceFormArchive GetComplianceForm(Guid ComplianceFormId);
        List<PrincipalInvestigator> getAllPrincipalInvestigators();
        List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active);
        List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active = true, bool ReviewCompleted = true);
        List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, ReviewStatusEnum ReviewStatus);
        List<PrincipalInvestigator> getPrincipalInvestigators(List<ComplianceFormArchive> Forms);



        ComplianceFormArchive GetNewComplianceForm(string UserName, string InputSource);

        void AddMatchingRecords(ComplianceFormArchive frm);

        ComplianceFormArchive ScanUpdateComplianceForm(ComplianceFormArchive form);

        ComplianceFormArchive UpdateComplianceForm(ComplianceFormArchive form);
        bool UpdateComplianceFormNIgnoreIfNotFound(ComplianceFormArchive form);
        void UpdateAssignedTo(Guid? RecId, string  AssignedBy, string AssignedFrom, string AssignedTo);
        void UpdateAssignedTo(string AssignedBy, AssignComplianceFormsTo AssignComplianceFormsTo);


        void UpdateExtractionQuePosition(Guid formId, int Position, DateTime ExtractionStartedAt, DateTime ExtractionEstimatedCompletion);

        ComplianceFormArchive UpdateCompFormGeneralNInvestigatorsNOptionalSites(ComplianceFormArchive form);
        bool UpdateFindings(UpdateFindigs updateFindings);
        bool UpdateInstituteFindings(UpdateInstituteFindings InstitueFinding);

        List<PrincipalInvestigator> GetComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter);

        List<PrincipalInvestigator> GetClosedComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter, string AssignedTo);

        List<PrincipalInvestigator> GetUnAssignedComplianceForms();

        InvestigatorSearched getInvestigatorSiteSummary(string compFormId, int InvestigatorId);

        List<InstituteFindingsSummaryViewModel> getInstituteFindingsSummary(Guid CompFormId);

        ComplianceFormArchive RollUpSummary(ComplianceFormArchive form);
        bool UpdateRollUpSummary(Guid formId);

        void AddLiveScanFindings(ComplianceFormArchive frm);

        MemoryStream GenerateComplianceForm(Guid? ComplianceFormId);

        //List<string> ValidateExcelInputs(List<string> ExcelInputRow, int Row);
        ExcelInput ReadDataFromExcelFile(string FilePathWithGUID);
        List<ComplianceFormArchive> ReadUploadedFileData(ExcelInput ExcelInputData, 
            string UserName, string FilePathWithGUID, string UploadedFileName);

        MemoryStream GenerateComplianceForm(
            Guid? ComplianceFormId, 
            IWriter writer,
            string FileExtension,
            out string FileName);

        bool AddAttachmentsToFindings(ComplianceFormArchive from);

        List<Finding> GetSingleComponentMatchedRecords(
            Guid? SiteDataId,
            SiteEnum Enum,
            string FullName);

        //string GenerateOutputFile(
        //    IGenerateOutputFile GenerateOutputFile,
        //    List<ComplianceForm> forms);

        MemoryStream GenerateOutputFile(
            IGenerateOutputFile GenerateOutputFile,
            List<ComplianceFormArchive> forms);

        bool UpdateQC(ComplianceFormArchive Form);

        iSprintResponseModel.DDtoIsprintResponse ExportDataToIsprint(Guid ComplianceFormId);
        iSprintResponseModel.DDtoIsprintResponse ExportDataToIsprint(ComplianceFormArchive Form);

        string GetUserFullName(string UserName);
    }
}
