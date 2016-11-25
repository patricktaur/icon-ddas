using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using System;

namespace DDAS.Models.Interfaces
{
    public interface ISearchSummary
    {
        ComplianceForm GetSearchSummary(ComplianceForm form, ILog log);

        ComplianceForm UpdateSingleSiteFromComplianceForm(string NameToSearch,
            Guid? ComplianceFormId, SiteEnum Enum, ILog log);

        SitesIncludedInSearch GetMatchedRecords(
            string NameToSearch, Guid? DataId, SiteEnum Enum);

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

        SitesIncludedInSearch GetSpeciallyDesignatedNationalsMatchCount(
            string NameToSearch, Guid? DataId, SitesIncludedInSearch Site);

        bool SaveRecordStatus(string NameToSearch, 
            SitesIncludedInSearch Result, Guid? ComplianceFormId);
    }
}
