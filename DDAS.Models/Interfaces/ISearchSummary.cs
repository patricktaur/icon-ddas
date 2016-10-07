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
        bool SaveRecordStatus(SaveSearchResult Result);
        FDADebarPageSiteData 
            GetStatusOfFDASiteRecords(FDADebarPageSiteData FDASiteData,
            string NameToSeach);
        ClinicalInvestigatorInspectionSiteData GetStatusOfClinicalSiteRecords(
            ClinicalInvestigatorInspectionSiteData ClinicalSiteData, 
            string NameToSearch);
        PHSAdministrativeActionListingSiteData GetStatusOfPHSSiteRecords(
            PHSAdministrativeActionListingSiteData PHSSiteData, string NameToSearch);
    }
}
