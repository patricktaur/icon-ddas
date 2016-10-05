using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Web.Http;
using DDAS.Models.ViewModels.Search;
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
                    //return Ok(_SearchSummary.GetFDADebarPageMatch(query));

                    var SearchDetails = _SearchSummary.GetFDADebarPageMatch(
                    query.NameToSearch,
                    query.RecId);
                    return null;
                    break;
            case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return null;
                    break;
            case SiteEnum.FDAWarningLettersPage:
                    return null;
                    break;
            default:
                    return null;
                    throw new Exception("wrong enum");
            }


            
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

        [Route("SearchHistory")]
        [HttpGet]
        public IHttpActionResult SearchHistory()
        {
            
            SearchHistoryViewModel[] result = new SearchHistoryViewModel[] {
                new SearchHistoryViewModel {searchedBy="Ravi", searchedOn="25-Mar-2016", searchName="Tom Jerry", searchCount=9 },
                new SearchHistoryViewModel {searchedBy="Shyam", searchedOn="18-Feb-2016", searchName="Venkatesh Verma", searchCount=8},
                new SearchHistoryViewModel {searchedBy="Ram", searchedOn="15-Jan-2016", searchName="Vanita Venkatraman", searchCount=7},

                   new SearchHistoryViewModel {searchedBy="Ravi", searchedOn="10-Jan-2016", searchName="Patrick Taur", searchCount=9 },
                new SearchHistoryViewModel {searchedBy="Shyam", searchedOn="18-Feb-2016", searchName="Pradeep Chavan", searchCount=8},
                new SearchHistoryViewModel {searchedBy="Ram", searchedOn="10-Jan-2016", searchName="Divya Sunil", searchCount=7},

                   new SearchHistoryViewModel {searchedBy="Ravi", searchedOn="08-Jan-2016", searchName="Manasa Jois", searchCount=9 },
                new SearchHistoryViewModel {searchedBy="Shyam", searchedOn="07-Feb-2016", searchName="Rajani A B C", searchCount=8},
                new SearchHistoryViewModel {searchedBy="Ram", searchedOn="06-Jan-2016", searchName="Sindhu M", searchCount=7},

                 new SearchHistoryViewModel {searchedBy="Ravi", searchedOn="05-Mar-2016", searchName="Sathya Muth", searchCount=9 },

            };
            
            return Ok();
        }

        [Route("StudyNumbers")]
        [HttpGet]
        public IHttpActionResult StudyNumbers()
        {
            StudyNumberViewModel[] result = new StudyNumberViewModel[]
            {
                new StudyNumberViewModel { StudyNumber = "9999/8888"},
                 new StudyNumberViewModel { StudyNumber = "8888/7777"},
                  new StudyNumberViewModel { StudyNumber = "7777/6666"},
                   new StudyNumberViewModel { StudyNumber = "6666/5555"},
                    new StudyNumberViewModel { StudyNumber = "5555/4444"},
                     new StudyNumberViewModel { StudyNumber = "4444/3333"}
            };

            return Ok(result);
        }

    }
}