using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;

namespace DDAS.Models.Interfaces
{
    public interface ISearchEngine
    {
        //SearchResult SearchByName(string NameToSearch);
        //SearchResult SearchByName(string NameToSearch, List<SiteEnum> siteEnums);
        
        //bool SearchByName(string NameToSearch, SiteEnum siteEnum);
        //SearchResult SearchByName(SearchQuery searchQuery);
        //SearchQuery GetNewSearchQuery();

        void Load(string NameToSearch, string DownloadFolder, ILog log);

        //void Load(SearchQuery query, string DownloadFolder, ILog log);

        void Load(
            SiteEnum siteEnum, string NameToSearch, 
            string DownloadFolder, bool IsExtractionRequired); //Pradeep 15Dec2016

        //void Load(string NameToSearch, string DownloadFolder, ILog log);
        void Load(List<SitesToSearch> query, string DownloadFolder, ILog log);
        //void Load(SiteEnum siteEnum, string NameToSearch, string DownloadFolder);

        //Pradeep 22Dec2016
        void ExtractData(List<SitesToSearch> query, string DownloadFolder, ILog log);
        void ExtractData(SiteEnum siteEnum, string DownloadFolder, ILog log);
        void ExtractData(SiteEnum siteEnum, string NameToSearch, 
            string ErrorScreenCaptureFolder, int MatchCountLowerLimit,
            out DateTime? SiteLastUpdatedOn);//For Live site

        IEnumerable<SiteDataItemBase> SiteData { get; }
        BaseSiteData baseSiteData { get; }

        void SaveData();

        bool IsDataExtractionRequired(SiteEnum siteEnum);
    }
}
