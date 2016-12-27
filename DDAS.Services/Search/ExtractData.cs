using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System.Collections.Generic;

namespace DDAS.Services.Search
{
    public class ExtractData
    {
        private ISearchEngine _searchEngine;
        public ExtractData(ISearchEngine SearchEngine)
        {
            _searchEngine = SearchEngine;
        }

        public void ExtractDataSingleSite(SiteEnum siteEnum, 
            string DownloadFolder, ILog log)
        {
            _searchEngine.ExtractData(siteEnum, DownloadFolder, log);
        }

        public void ExtractDataAllDBSites(List<SearchQuerySite> Sites,
            string DownloadFolder, ILog log)
        {
            _searchEngine.ExtractData(Sites, DownloadFolder, log);
        }
    }
}
