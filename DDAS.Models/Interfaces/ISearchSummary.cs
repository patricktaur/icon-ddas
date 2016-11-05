using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface ISearchSummary
    {
        SearchSummary GetSearchSummary(string query);
        //FDADebarPageSiteData 
        SitesIncludedInSearch GetFDADebarPageMatch(string NameToSearch,
            Guid? DataId, SiteEnum Enum);

        SitesIncludedInSearch
            GetClinicalInvestigatorSiteMatch(string NameToSearch, Guid? DataId,
            SiteEnum Enum);

        FDAWarningLettersSiteData GetFDAWarningLettersMatch(
            string NameToSearch, Guid? DataId);

        PHSAdministrativeActionListingSiteData
            GetPHSAdministrativeSiteMatch(string NameToSearch, Guid? DataId);

        SpeciallyDesignatedNationalsListSiteData GetSpeciallyDesignatedNationsMatch(
            string NameToSearch, Guid? DataId);

        ERRProposalToDebarPageSiteData GetProposalToDebarPageMatch(
            string NameToSearch, Guid? DataId);

        AdequateAssuranceListSiteData GetAdequateAssuranceListPageMatch(
            string NameToSearch, Guid? DataId);

        ClinicalInvestigatorDisqualificationSiteData
            GetDisqualificationProceedingsMatch(string NameToSearch, Guid? DataId);

        CBERClinicalInvestigatorInspectionSiteData
            GetCBERClinicalInvestigatorPageMatch(string NameToSearch, Guid? DataId);

        ExclusionDatabaseSearchPageSiteData GetExclusionDatabaseSearchPageMatch(
            string NameToSearch, Guid? DataId);

        CorporateIntegrityAgreementListSiteData GetCIAPageMatch(
            string NameToSearch, Guid? DataId);

        SystemForAwardManagementPageSiteData GetSAMMatch(
            string NameToSearch, Guid? DataId);

        bool SaveRecordStatus(SaveSearchResult Result);

        FDADebarPageSiteData 
            GetStatusOfFDASiteRecords(FDADebarPageSiteData FDASiteData,
            string NameToSeach);

        ClinicalInvestigatorInspectionSiteData GetStatusOfClinicalSiteRecords(
            ClinicalInvestigatorInspectionSiteData ClinicalSiteData, 
            string NameToSearch);

        FDAWarningLettersSiteData GetStatusOfFDAWarningSiteRecords(
            FDAWarningLettersSiteData FDAWarningLetterSiteData, string NameToSearch);

        PHSAdministrativeActionListingSiteData GetStatusOfPHSSiteRecords(
            PHSAdministrativeActionListingSiteData PHSSiteData, string NameToSearch);

        SpeciallyDesignatedNationalsListSiteData GetStatusOfSDNSiteRecords(
            SpeciallyDesignatedNationalsListSiteData SDNSiteData, string NameToSearch);

        ERRProposalToDebarPageSiteData GetStatusOfProposalToDebarSiteRecords(
            ERRProposalToDebarPageSiteData ProposalToDebarSiteData, string NameToSearch);

        AdequateAssuranceListSiteData GetStatusOfAssuranceSiteRecords(
            AdequateAssuranceListSiteData AssuranceSiteData, string NameToSearch);

        ClinicalInvestigatorDisqualificationSiteData
            GetStatusOfDisqualificationSiteRecords(
            ClinicalInvestigatorDisqualificationSiteData DisqualificationSiteData,
            string NameToSearch);

        CBERClinicalInvestigatorInspectionSiteData GetStatusOfCBERSiteRecords(
            CBERClinicalInvestigatorInspectionSiteData CBERSiteData, string NameToSearch);

        ExclusionDatabaseSearchPageSiteData GetStatusOfExclusionSiteRecords(
            ExclusionDatabaseSearchPageSiteData ExclusionSiteData, string NameToSearch);

        CorporateIntegrityAgreementListSiteData GetStatusOfCIASiteRecords(
            CorporateIntegrityAgreementListSiteData CIASiteData, string NameToSearch);

        SystemForAwardManagementPageSiteData GetStatusOfSAMSiteRecords(
            SystemForAwardManagementPageSiteData SAMSiteData, string NameToSearch);
    }
}
