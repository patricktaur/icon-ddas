using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using System;

namespace DDAS.Models.Interfaces
{
    public interface ISearchSummary
    {
        ComplianceForm GetSearchSummary(string NameToSearch, ILog log);
        
        SitesIncludedInSearch GetMatchedRecords(Guid? DataId, SiteEnum Enum);

        SitesIncludedInSearch
            GetClinicalInvestigatorSiteMatch(string NameToSearch, Guid? DataId,
            SiteEnum Enum);

        SitesIncludedInSearch GetFDAWarningLettersMatch(string NameToSearch, 
            Guid? DataId, SiteEnum Enum);

        SitesIncludedInSearch GetPHSAdministrativeMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site);

        SitesIncludedInSearch GetSAMMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site);

        SitesIncludedInSearch GetProposalToDebarPageMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site);

        SitesIncludedInSearch GetAdequateAssuranceListPageMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site);

        //refactor
        SitesIncludedInSearch GetDisqualifionProceedingsMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site);

        SitesIncludedInSearch GetCBERClinicalInvestigatorPageMatchCount(
            string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site);

        SitesIncludedInSearch GetExclusionDatabaseSearchPageMatchCount(
            string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site);

        SitesIncludedInSearch GetCIAPageMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site);

        SitesIncludedInSearch GetSpeciallyDesignatedNationalsMatchCount(string NameToSearch,
            Guid? DataId, SitesIncludedInSearch Site);

        bool SaveRecordStatus(SitesIncludedInSearch Result, Guid? ComplianceFormId);

        //FDADebarPageSiteData 
        //    GetStatusOfFDASiteRecords(FDADebarPageSiteData FDASiteData,
        //    string NameToSeach);

        //ClinicalInvestigatorInspectionSiteData GetStatusOfClinicalSiteRecords(
        //    ClinicalInvestigatorInspectionSiteData ClinicalSiteData, 
        //    string NameToSearch);

        //FDAWarningLettersSiteData GetStatusOfFDAWarningSiteRecords(
        //    FDAWarningLettersSiteData FDAWarningLetterSiteData, string NameToSearch);

        //PHSAdministrativeActionListingSiteData GetStatusOfPHSSiteRecords(
        //    PHSAdministrativeActionListingSiteData PHSSiteData, string NameToSearch);

        //SpeciallyDesignatedNationalsListSiteData GetStatusOfSDNSiteRecords(
        //    SpeciallyDesignatedNationalsListSiteData SDNSiteData, string NameToSearch);

        //ERRProposalToDebarPageSiteData GetStatusOfProposalToDebarSiteRecords(
        //    ERRProposalToDebarPageSiteData ProposalToDebarSiteData, string NameToSearch);

        //AdequateAssuranceListSiteData GetStatusOfAssuranceSiteRecords(
        //    AdequateAssuranceListSiteData AssuranceSiteData, string NameToSearch);

        //ClinicalInvestigatorDisqualificationSiteData
        //    GetStatusOfDisqualificationSiteRecords(
        //    ClinicalInvestigatorDisqualificationSiteData DisqualificationSiteData,
        //    string NameToSearch);

        //CBERClinicalInvestigatorInspectionSiteData GetStatusOfCBERSiteRecords(
        //    CBERClinicalInvestigatorInspectionSiteData CBERSiteData, string NameToSearch);

        //ExclusionDatabaseSearchPageSiteData GetStatusOfExclusionSiteRecords(
        //    ExclusionDatabaseSearchPageSiteData ExclusionSiteData, string NameToSearch);

        //CorporateIntegrityAgreementListSiteData GetStatusOfCIASiteRecords(
        //    CorporateIntegrityAgreementListSiteData CIASiteData, string NameToSearch);

        //SystemForAwardManagementPageSiteData GetStatusOfSAMSiteRecords(
        //    SystemForAwardManagementPageSiteData SAMSiteData, string NameToSearch);
    }
}
