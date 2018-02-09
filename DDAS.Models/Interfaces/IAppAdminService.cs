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
    public interface IAppAdminService
    {
        //List<DataExtractionHistory> GetDataExtractionHistory();
        List<ExtractionStatus> GetDataExtractionPerSite(SiteEnum Enum);
        void DeleteExtractionEntry(SiteEnum Enum, Guid? RecId);

        bool LaunchLiveScanner(string exeFolder);
        LiveSiteScannerMemoryModel LiveScannerInfo();
        List<LiveSiteScannerProcessModel> getLiveScannerProcessorsInfo();
        bool KillLiveSiteScanner(int HowMany = 1);

        void AddSitesInDbCollection(SitesToSearch Site);
        SitesToSearch GetSingleSiteSource(Guid? RecId);
        List<SitesToSearch> GetAllSiteSources();
        bool UpdateSiteSource(SitesToSearch SiteSource);
        void DeleteSiteSource(Guid? RecId);

        List<CountryViewModel> GetCountries();
        Country GetCountry(Guid? RecId);
        bool AddCountry(Country country);
        void DeleteCountry(Guid? RecId);

        bool AddSponsor(SponsorProtocol sponsor);
        void DeleteSponsor(Guid? RecId);
        List<SponsorProtocolViewModel> GetSponsorProtocols();
        SponsorProtocol GetSponsorProtocol(Guid? RecId);

        //bool AddDefaultSite(DefaultSite site);
        List<DefaultSitesViewModel> GetDefaultSites();
        DefaultSite GetSingleDefaultSite(Guid? RecId);
        bool UpdateDefaultSite(DefaultSite defaultSite);
        void DeleteDefaultSite(Guid? RecId);

        List<UploadsViewModel> GetUploadedFiles();
        bool DeleteUploadedFile(string GeneratedFileName);
        bool DeleteAllUploadedFiles();

        List<OutputFileViewModel> GetOutputFiles();

        List<ExceptionLoggerViewModel> GetExceptionLogs();

        List<ExtractionLogViewModel> GetExtractionLog();

        List<LogWSDDASViewModel> GetiSprintToDDASLog();

        List<LogWSiSprintViewModel> GetDDtoiSprintLog();

        //List<CBERClinicalInvestigator> GetCBERData();

        //IEnumerable<string> GetSitesWhereDataExtractionEarlierThan(int Hour = 32);
    }
}
