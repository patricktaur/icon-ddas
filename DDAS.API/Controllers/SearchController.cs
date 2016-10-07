using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Web.Http;
using DDAS.Models.Enums;
using System;

namespace DDAS.API.Controllers
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private ISearchEngine _SearchEngine;

        private ISearchSummary _SearchSummary;

        public SearchController(ISearchEngine search, ISearchSummary SearchSummary)
        {
            _SearchEngine = search;
            _SearchSummary = SearchSummary;
        }

        [Route("SearchResult")]
        [HttpPost]
        public IHttpActionResult SearchResult(SearchQuery query)
        {
            var searchResults = _SearchEngine.SearchByName(query);
            return Ok(searchResults);
        }

        [Route("SearchResultAtSite")]
        [HttpPost]
        public IHttpActionResult SearchResultAtSite(SearchQueryAtSite query)
        {
            var searchResults = _SearchEngine.SearchByName(query.NameToSearch, query.SiteEnum);
            return Ok(searchResults);
        }

        [Route("GetSearchSummaryResult")]
        [HttpPost]
        public IHttpActionResult GetSearchSummaryResult(NameToSearchQuery query)
        {
            var SearchResults = _SearchSummary.GetSearchSummary(query);
            return Ok(SearchResults);
        }
        
        [Route("GetSearchSummaryDetails")]
        [HttpPost]
        public IHttpActionResult GetSearchSummaryDetailsXXX(SearchDetailsQuery query)
        {
            switch (query.siteEnum) {

                case SiteEnum.FDADebarPage:
                    var SearchDetails = _SearchSummary.GetFDADebarPageMatch(
                    query.NameToSearch,
                    query.RecId);
                    return Ok(
                        _SearchSummary.GetStatusOfFDASiteDataRecords(SearchDetails));

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return Ok(_SearchSummary.GetClinicalInvestigatorSiteMatch(
                    query.NameToSearch,
                    query.RecId));

                case SiteEnum.PHSAdministrativeActionListingPage:
                    return Ok(_SearchSummary.GetPHSAdministrativeSiteMatch(
                    query.NameToSearch,
                    query.RecId));
                default:
                    throw new Exception("wrong enum");
            }
        }
        
        [Route("SaveSearchResult")]
        [HttpPost]
        public IHttpActionResult SaveSearchResults(SaveSearchResult result)
        {
            return Ok(_SearchSummary.SaveRecordStatus(result));
        }

        /*
        [Route("GetSearchSummaryDetails")]
        [HttpPost]
        public IHttpActionResult GetSearchSummaryDetails(SearchDetailsQuery query)
        {
            var SearchDetails = _SearchSummary.GetFDADebarPageMatch(
                query.NameToSearch,
                query.RecId);

            return Ok(SearchDetails);
        }
        */
        [Route("getNewSearchQuery")]
        [HttpGet]
        public IHttpActionResult newSearchQuery()
        {
            var query = _SearchEngine.GetNewSearchQuery();
             return Ok(query);
        }

      

    }
}