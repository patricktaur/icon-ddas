using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
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

        ComplianceForm GetNewComplianceForm(ILog log, string UserName);

        void AddMatchingRecords(ComplianceForm frm, ILog log, string ErrorScreenCaptureFolder, string siteType);

        ComplianceForm ScanUpdateComplianceForm(ComplianceForm form, ILog log, 
            string ErrorScreenCaptureFolder, string siteType = "db");

        ComplianceForm UpdateComplianceForm(ComplianceForm form);
        bool UpdateComplianceFormNIgnoreIfNotFound(ComplianceForm form);
        void UpdateAssignedToData(string AssignedTo, bool Active, Guid? RecId);
       
        void UpdateExtractionQuePosition(Guid formId, int Position, DateTime ExtractionStartedAt, DateTime ExtractionEstimatedCompletion);

        ComplianceForm UpdateCompFormGeneralNInvestigatorsNOptionalSites(ComplianceForm form, ILog log, string ErrorScreenCaptureFolder);
        bool UpdateFindings(UpdateFindigs updateFindings);

        List<PrincipalInvestigator> GetComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter);

        InvestigatorSearched getInvestigatorSiteSummary(string compFormId, int InvestigatorId);

        ComplianceForm RollUpSummary(ComplianceForm form);

        MemoryStream GenerateComplianceForm(Guid? ComplianceFormId);

        //To be removed
        string GenerateComplianceFormAlt(Guid? ComplianceFormId, string TemplatesFolder, string DownloadFolder);

        List<string> ValidateExcelInputs(List<string> ExcelInputRow, int Row);
        List<List<string>> ReadDataFromExcelFile(string FilePathWithGUID);
        List<ComplianceForm> ReadUploadedFileData(List<List<string>> ExcelInputData, 
            ILog log, string UserName, string FilePathWithGUID, string UploadedFileName);

        string GenerateComplianceForm(
            string DownloadFolder, string TemplateFolder,
            Guid? ComplianceFormId, IWriter writer,
            string FileExtension);

        bool AddAttachmentsToFindings(ComplianceForm from);
    }
}
