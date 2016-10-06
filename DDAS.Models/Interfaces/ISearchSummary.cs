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
        FDADebarPageSiteData 
            GetStatusOfFDASiteDataRecords(FDADebarPageSiteData FDASiteData);
        bool SaveRecordStatus(SaveSearchResult Result);
    }
}
