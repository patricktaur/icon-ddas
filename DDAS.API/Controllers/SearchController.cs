using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Web.Http;
using DDAS.Models.Enums;
using System;
using DDAS.Models;
using DDAS.API.Identity;
using Microsoft.AspNet.Identity;

namespace DDAS.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private ISearchEngine _SearchEngine;
        private ISearchSummary _SearchSummary;
        private IUnitOfWork _UOW;

        public SearchController(ISearchEngine search, ISearchSummary SearchSummary,
            IUnitOfWork uow)
        {
            _SearchEngine = search;
            _SearchSummary = SearchSummary;
            _UOW = uow;
        }

        //[Authorize (Roles = "User,Admin")]
        [Route("AddUser")]
        [HttpPost]
        public IHttpActionResult GetUser(UserDetails user)
        {
            UserStore userStore = new UserStore(_UOW);
            var um = new UserManager<IdentityUser, Guid>(userStore);

            RoleStore roleStore = new RoleStore(_UOW);
            var rm = new RoleManager<IdentityRole, Guid>(roleStore);

            var IdUser = new IdentityUser();
            IdUser.UserName = user.UserName;
            IdUser.SecurityStamp = Guid.NewGuid().ToString();

            try
            {
                IdentityUser IdUsertemp = um.FindByName(IdUser.UserName);
                if (IdUsertemp == null)
                {
                    um.Create(IdUser, user.pwd);
                    um.AddToRole(IdUser.Id, user.RoleName);
                }
                else
                {
                  
                        um.AddToRole(IdUser.Id, user.RoleName);
                    
                }

            }
            catch (Exception)
            {
            }
            
            return Ok();
        }

        //[Authorize (Roles="User")]
        [Route("AddRole")]
        [HttpPost]
        public IHttpActionResult AddRole(Role role)
        {
            RoleStore roleStore = new RoleStore(_UOW);
            IdentityRole irole = new IdentityRole();
            irole.Name = role.Name;
            var rm = new RoleManager<IdentityRole, Guid>(roleStore);
            rm.Create(irole);

            return Ok();
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

        //[Authorize]
        [Route("GetSearchSummaryResult")]
        [HttpGet]
        public IHttpActionResult GetSearchSummaryResult(string NameToSearch)
        {
            var query = new NameToSearchQuery();
            query.NameToSearch = NameToSearch;
            var SearchResults = _SearchSummary.GetSearchSummary(query);
            return Ok(SearchResults);
        }

        //[Authorize]
        [Route("GetSearchSummaryDetails")]
        [HttpGet]
        public IHttpActionResult GetSearchSummaryDetailsXXX(string NameToSearch, string RecId,
            SiteEnum siteEnum)
        {
            var query = new SearchDetailsQuery();
            query.NameToSearch = NameToSearch;
            query.RecId = Guid.Parse(RecId);
            query.siteEnum = siteEnum;

            switch (query.siteEnum) {

                case SiteEnum.FDADebarPage:
                    var SearchDetails = _SearchSummary.
                        GetFDADebarPageMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(
                        _SearchSummary.GetStatusOfFDASiteRecords(SearchDetails,
                        query.NameToSearch));

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    var ClinicalSearchDetails = _SearchSummary.
                        GetClinicalInvestigatorSiteMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.
                        GetStatusOfClinicalSiteRecords(ClinicalSearchDetails,
                        query.NameToSearch));

                case SiteEnum.ERRProposalToDebarPage:
                    var ProposalToDebarDetails = _SearchSummary.
                        GetProposalToDebarPageMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.
                        GetStatusOfProposalToDebarSiteRecords(ProposalToDebarDetails,
                        query.NameToSearch));

                case SiteEnum.AdequateAssuranceListPage:
                    var AssuranceDetails = _SearchSummary.
                        GetAdequateAssuranceListPageMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.
                        GetStatusOfAssuranceSiteRecords(AssuranceDetails,
                        query.NameToSearch));

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    var CBERDetails = _SearchSummary.
                        GetCBERClinicalInvestigatorPageMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.
                        GetStatusOfCBERSiteRecords(CBERDetails,
                        query.NameToSearch));

                case SiteEnum.PHSAdministrativeActionListingPage:
                    var PHSSearchDetails = _SearchSummary.
                        GetPHSAdministrativeSiteMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.
                        GetStatusOfPHSSiteRecords(PHSSearchDetails, 
                        query.NameToSearch));

                case SiteEnum.ExclusionDatabaseSearchPage:
                    var ExclusionDetails = _SearchSummary.
                        GetExclusionDatabaseSearchPageMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.
                        GetStatusOfExclusionSiteRecords(ExclusionDetails,
                        query.NameToSearch));

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    var CIADetails = _SearchSummary.
                        GetCIAPageMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.
                        GetStatusOfCIASiteRecords(CIADetails,
                        query.NameToSearch));

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    var SDNSearchDetails = _SearchSummary.GetSpeciallyDesignatedNationsMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.GetStatusOfSDNSiteRecords(SDNSearchDetails,
                        query.NameToSearch));

                default:
                    throw new Exception("wrong enum");
            }
        }
        
        //[Authorize]
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

        //[Route("getNewSearchQuery")]
        //[HttpGet]
        //public IHttpActionResult newSearchQuery()
        //{
        //    var query = _SearchEngine.GetNewSearchQuery();
        //     return Ok(query);
        //}
        public class UserDetails
        {
            public string  UserName{get;set;}
            public string pwd { get; set; }
            public string RoleName { get; set; }

        }
      

    }
}