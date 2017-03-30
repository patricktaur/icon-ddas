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

        //Currently ExtractData is only for DB sites

        public void ExtractDataSingleSite(SiteEnum siteEnum, ILog log)
        {
            _searchEngine.ExtractData(siteEnum, log);
        }

        public void ExtractDataAllDBSites(List<SitesToSearch> Sites, ILog log)
        {
            _searchEngine.ExtractData(Sites, log);
        }
    }
}
