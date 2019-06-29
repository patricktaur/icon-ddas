using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IDataExtractorService
    {
        void ExtractDataSingleSite(SiteEnum siteEnum, string userName);

        void ExtractDataSingleSite(SiteEnum siteEnum, ILog log);

        void ExtractThruShell(Int32 siteNumber, string ExePath);

        List<ExtractionStatus> GetLatestExtractionStatus(DateTime fromDate, DateTime toDate);
        IEnumerable<string> GetSitesWhereDataExtractionEarlierThan(int Hour = 32);
        List<DownloadDataFilesViewModel> GetDataFiles(int Enum);

        #region getExtractedData

        FDADebarPageSiteData GetFDADebarPageSiteData();
        ERRProposalToDebarPageSiteData GetERRProposalToDebarPageSiteData();
        AdequateAssuranceListSiteData GetAdequateAssuranceListSiteData();
        ClinicalInvestigatorDisqualificationSiteData GetClinicalInvestigatorDisqualificationSiteData();
        PHSAdministrativeActionListingSiteData GetPHSAdministrativeActionListingSiteData();
        CBERClinicalInvestigatorInspectionSiteData GetCBERClinicalInvestigatorInspectionSiteData();
        CorporateIntegrityAgreementListSiteData GetCorporateIntegrityAgreementListSiteData();
        #endregion
    }
}
