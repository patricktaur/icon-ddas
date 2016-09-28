using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Services.Search
{
    public class Search : ISearchSummary
    {
        private IUnitOfWork _UOW;

        public Search(IUnitOfWork uow)
        {
            _UOW = uow;
        }
        public SearchSummary GetSearchSummary(SearchQuery query)
        {
            SearchSummary searchSummary = new SearchSummary();

            foreach(SearchQuerySite Site in query.SearchSites)
            {
                var searchSummaryItem = new SearchSummaryItem();
                searchSummaryItem.MatchStatus = GetMatchStatus(Site.SiteEnum);
            }
            return null;
        }

        public string GetMatchStatus(SiteEnum Enum)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchStatus();
            }
            return null;
        }

        public string GetFDADebarPageMatchStatus()
        {
            var FDASearchResult = _UOW.FDADebarPageRepository.GetAll();
             
            return null;
        }

        public string GetMatchWeightage(string NameToSearch)
        {
            return null;
        }
    }
}
