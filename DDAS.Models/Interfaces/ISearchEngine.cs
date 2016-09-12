using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System.Collections.Generic;

namespace DDAS.Models.Interfaces
{
    public interface ISearchEngine
    {
        SearchResult SearchByName(string NameToSearch);
        SearchResult SearchByName(string NameToSearch, List<SiteEnum> siteEnums);
        ResultAtSite SearchByName(string NameToSearch, SiteEnum siteEnum);
        SearchResult SearchByName(SearchQuery searchQuery);
        SearchQuery GetNewSearchQuery();
    }
}
