using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IAppAdminService
    {
        List<DataExtractionHistory> GetDataExtractionHistory();
        List<DataExtractionHistory> GetDataExtractionPerSite(SiteEnum Enum);
        void DeleteExtractionEntry(SiteEnum Enum, Guid? RecId);
        bool LaunchLiveScanner(string exeFolder);
        LIveSiteScannerMemoryModel LiveScannerInfo();
        List<LiveSiteScannerProcessModel> getLiveScannerProcessorsInfo();
        bool KillLiveSiteScanner(int HowMany = 1);
        void AddSitesInDbCollection(SearchQuerySite Site);
    }
}
