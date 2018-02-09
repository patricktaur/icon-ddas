using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IExtractData
    {
        void ExtractDataSingleSite(SiteEnum siteEnum, string userName);
        List<ExtractionStatus> GetLatestExtractionStatus();
        IEnumerable<string> GetSitesWhereDataExtractionEarlierThan(int Hour = 32);
    }
}
