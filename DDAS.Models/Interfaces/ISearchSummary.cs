using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface ISearchSummary
    {
        SearchSummary GetSearchSummary(NameToSearchQuery query);
        FDADebarPageSiteData 
            GetFDADebarPageMatch(string NameToSearch, Guid? DataId);
        PHSAdministrativeActionListingSiteData
            GetPHSAdministrativeSiteMatch(string NameToSearch, Guid? DataId);
        ClinicalInvestigatorInspectionSiteData
            GetClinicalInvestigatorSiteMatch(string NameToSearch, Guid? DataId);
        SpeciallyDesignatedNationalsListSiteData GetSpeciallyDesignatedNationsMatch(
            string NameToSearch, Guid? DataId);
        ERRProposalToDebarPageSiteData GetProposalToDebarPageMatch(
            string NameToSearch, Guid? DataId);
        AdequateAssuranceListSiteData GetAdequateAssuranceListPageMatch(
            string NameToSearch, Guid? DataId);
        CBERClinicalInvestigatorInspectionSiteData
            GetCBERClinicalInvestigatorPageMatch(string NameToSearch, Guid? DataId);
        ExclusionDatabaseSearchPageSiteData GetExclusionDatabaseSearchPageMatch(
            string NameToSearch, Guid? DataId);
        CorporateIntegrityAgreementListSiteData GetCIAPageMatch(
            string NameToSearch, Guid? DataId);

        bool SaveRecordStatus(SaveSearchResult Result);

        FDADebarPageSiteData 
            GetStatusOfFDASiteRecords(FDADebarPageSiteData FDASiteData,
            string NameToSeach);
        ClinicalInvestigatorInspectionSiteData GetStatusOfClinicalSiteRecords(
            ClinicalInvestigatorInspectionSiteData ClinicalSiteData, 
            string NameToSearch);
        PHSAdministrativeActionListingSiteData GetStatusOfPHSSiteRecords(
            PHSAdministrativeActionListingSiteData PHSSiteData, string NameToSearch);
        
        SpeciallyDesignatedNationalsListSiteData GetStatusOfSDNSiteRecords(
            SpeciallyDesignatedNationalsListSiteData SDNSiteData, string NameToSearch);

        ERRProposalToDebarPageSiteData GetStatusOfProposalToDebarSiteRecords(
            ERRProposalToDebarPageSiteData ProposalToDebarSiteData, string NameToSearch);

        AdequateAssuranceListSiteData GetStatusOfAssuranceSiteRecords(
            AdequateAssuranceListSiteData AssuranceSiteData, string NameToSearch);

        CBERClinicalInvestigatorInspectionSiteData GetStatusOfCBERSiteRecords(
            CBERClinicalInvestigatorInspectionSiteData CBERSiteData, string NameToSearch);

        ExclusionDatabaseSearchPageSiteData GetStatusOfExclusionSiteRecords(
            ExclusionDatabaseSearchPageSiteData ExclusionSiteData, string NameToSearch);

        CorporateIntegrityAgreementListSiteData GetStatusOfCIASiteRecords(
            CorporateIntegrityAgreementListSiteData CIASiteData, string NameToSearch);
    }
}
