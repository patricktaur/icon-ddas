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


<<<<<<< HEAD
        ComplianceForm ScanUpdateComplianceForm(ComplianceForm form, ILog log,
            string ErrorScreenCaptureFolder);
=======
        ComplianceForm GetNewComplianceForm(ILog log, string UserName);
 
        ComplianceForm ScanUpdateComplianceForm(ComplianceForm form, ILog log, 
            string ErrorScreenCaptureFolder, string siteType = "db");
>>>>>>> a581a415be30b4f10a7e9b73f5bcd2960dab82dc
        ComplianceForm UpdateComplianceForm(ComplianceForm form);
        void UpdateAssignedToData(string AssignedTo, bool Active, Guid? RecId);
        void UpdateExtractionQuePosition(Guid formId, int Position, DateTime ExtractionStartedAt, DateTime ExtractionEstimatedCompletion);

        List<PrincipalInvestigator> GetComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter);

  


        InvestigatorSearched getInvestigatorSiteSummary(string compFormId, int InvestigatorId);
        


        ComplianceForm RollUpSummary(ComplianceForm form);

        MemoryStream GenerateComplianceForm(Guid? ComplianceFormId);

        //To be removed
        string GenerateComplianceFormAlt(Guid? ComplianceFormId, string TemplatesFolder, string DownloadFolder);

        List<string> ValidateExcelInputs(List<string> ExcelInputRow, int Row);
        List<List<string>> ReadDataFromExcelFile(string FilePath);
        List<ComplianceForm> ReadUploadedFileData(List<List<string>> ExcelInputData, 
            ILog log, string UserName, string FilePath);

        string GenerateComplianceForm(
            string DownloadFolder, string TemplateFolder,
            Guid? ComplianceFormId, IWriter writer,
            string FileExtension);
    }
}
