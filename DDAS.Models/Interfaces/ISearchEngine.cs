using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using System.Collections.Generic;

namespace DDAS.Models.Interfaces
{
    public interface ISearchEngine
    {
        //SearchResult SearchByName(string NameToSearch);
        //SearchResult SearchByName(string NameToSearch, List<SiteEnum> siteEnums);
        
        bool SearchByName(string NameToSearch, SiteEnum siteEnum);
        SearchResult SearchByName(SearchQuery searchQuery);
        //SearchQuery GetNewSearchQuery();
        void Load(string NameToSearch, string DownloadFolder, ILog log);
        void Load(SearchQuery query, string DownloadFolder, ILog log);
        void Load(SiteEnum siteEnum, string NameToSearch, string DownloadFolder);

        IEnumerable<SiteDataItemBase> SiteData { get; }

        void SaveData();
    }
}
