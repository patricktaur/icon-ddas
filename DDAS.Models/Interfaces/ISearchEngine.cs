using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;

namespace DDAS.Models.Interfaces
{
    public interface ISearchEngine
    {
        void ExtractData(List<SitesToSearch> query, ILog log);
        void ExtractData(SiteEnum siteEnum, ILog log);
        void ExtractData(SiteEnum siteEnum, string NameToSearch, 
            int MatchCountLowerLimit,
            out DateTime? SiteLastUpdatedOn);//For Live site

        IEnumerable<SiteDataItemBase> SiteData { get; }
        BaseSiteData baseSiteData { get; }

        void SaveData();

        bool IsDataExtractionRequired(SiteEnum siteEnum, ILog log);
    }
}
