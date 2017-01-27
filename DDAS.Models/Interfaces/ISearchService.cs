using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDAS.Models.Interfaces
{
    public interface ISearchService
    {
        //ComplianceForm GetSearchSummary(ComplianceForm form, ILog log);

        //ComplianceForm UpdateSingleSiteFromComplianceForm(string NameToSearch,
        //    Guid? ComplianceFormId, SiteEnum Enum, ILog log);

        List<PrincipalInvestigator> getAllPrincipalInvestigators();
        List<PrincipalInvestigator> getPrincipalInvestigators(string AssignedTo, bool Active);

        //Patrick 27Nov2016
        ComplianceForm GetNewComplianceForm(ILog log, string UserName);

        ComplianceForm ScanUpdateComplianceForm(ComplianceForm form, ILog log);
        ComplianceForm UpdateComplianceForm(ComplianceForm form);

        //Pradeep 5Jan2017
        List<PrincipalInvestigator> GetComplianceFormsFromFilters(
            ComplianceFormFilter CompFormFilter);

        void UpdateAssignedToData(string AssignedTo, bool Active,
            Guid? RecId);

        //Patrick 03Dec2016
        InvestigatorSearched getInvestigatorSiteSummary(string compFormId, int InvestigatorId);
        
        //Pradeep 1Dec2016
        //List<ComplianceForm> ReadUploadedFileData(string FilePath, ILog log, string UserName);

        ComplianceForm RollUpSummary(ComplianceForm form);

        MemoryStream GenerateComplianceForm(Guid? ComplianceFormId);

        //To be removed
        string GenerateComplianceFormAlt(Guid? ComplianceFormId, string TemplatesFolder, string DownloadFolder);

        //13Jan2017
        List<string> ValidateExcelInputs(List<List<string>> ExcelInputRow);
        List<List<string>> ReadDataFromExcelFile(string FilePath);
        List<ComplianceForm> ReadUploadedFileData(List<List<string>> ExcelInputData, 
            ILog log, string UserName, string FilePath);

        //24Jan2017
        string GenerateComplianceForm(
            string DownloadFolder, string TemplateFolder,
            Guid? ComplianceFormId, IWriter writer,
            string FileExtension);
    }
}
