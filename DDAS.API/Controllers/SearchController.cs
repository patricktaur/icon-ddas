using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Web.Http;
using DDAS.Models.Enums;
using System;
using DDAS.Models;
using DDAS.API.Identity;
using Microsoft.AspNet.Identity;
using Utilities;
using System.Web;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DDAS.API.Controllers
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private ISearchEngine _SearchEngine;
        private ISearchSummary _SearchSummary;
        private IUnitOfWork _UOW;
        private ILog _log;

        private string DataExtractionLogFile =
            System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"];

        public SearchController(ISearchEngine search, ISearchSummary SearchSummary,
            IUnitOfWork uow, ILog log)
        {
            _SearchEngine = search;
            _SearchSummary = SearchSummary;
            _UOW = uow;
            _log = log;
        }

        [Route("AddNewRole")]
        [HttpPost]
        public IHttpActionResult CreateRole(IdentityRole role)
        {
            //IdentityRole role = new IdentityRole(roleName);
            RoleStore roleStore = new RoleStore(_UOW);
            roleStore.CreateAsync(role);
            return Ok();
        }


        //[Authorize] //(Roles = "")]
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
                    um.CreateAsync(IdUser, user.pwd);





                    um.AddToRole(IdUser.Id, user.RoleName);
                }
                else
                {
                    um.AddToRole(IdUser.Id, user.RoleName);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Ok();
        }

        //[Authorize] //(Roles="User")]
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


        [Authorize]
        [Route("GetBulkSearchSummaryResult")]
        [HttpPost]
        public IHttpActionResult GetMultipleSearchSummaryResult()
        {
            string root = HttpContext.Current.Server.MapPath("~/App_Data/");
            var provider = new MultipartFormDataStreamProvider(root);

            var task = Request.Content.ReadAsMultipartAsync(provider);

            return Ok();
        }


        [Route("GetSearchSummaryResult")]
        [HttpGet]
        public IHttpActionResult GetSearchSummaryResult(string NameToSearch)
        {
            //var query = new NameToSearchQuery();
            //query.NameToSearch = NameToSearch;

            //_log = new LogText(DataExtractionLogFile, true);
            //_log.LogStart();
            //_log.WriteLog(DateTime.Now.ToString(), "Extract Data starts");

            //_SearchEngine.Load(query.NameToSearch);

            //_log.WriteLog(DateTime.Now.ToString(), "Extract Data ends");
            //_log.WriteLog("=================================================================================");
            //_log.LogEnd();

            var SearchResults = _SearchSummary.GetSearchSummary(NameToSearch);
            return Ok(SearchResults);
        }




        [Route("GetSearchSummaryDetails")]
        [HttpGet]
        public IHttpActionResult GetSearchSummaryDetailsXXX(string NameToSearch, string RecId,
            SiteEnum siteEnum)
        {
            var query = new SearchDetailsQuery();
            query.NameToSearch = NameToSearch;
            query.RecId = Guid.Parse(RecId);
            query.siteEnum = siteEnum;

            switch (query.siteEnum)
            {

                case SiteEnum.FDADebarPage:
                    var SearchDetails = _SearchSummary.
                        GetFDADebarPageMatch(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(SearchDetails);

                //refactor GetStatusOfFDASiteRecords as per the new design
                //return Ok(
                //    _SearchSummary.GetStatusOfFDASiteRecords(SearchDetails,
                //    query.NameToSearch));

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    var ClinicalSearchDetails = _SearchSummary.
                        GetClinicalInvestigatorSiteMatch(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(ClinicalSearchDetails);
                //Refactor this as per new design
                //return Ok(_SearchSummary.
                //    GetStatusOfClinicalSiteRecords(ClinicalSearchDetails,
                //    query.NameToSearch));

                case SiteEnum.FDAWarningLettersPage:
                    var FDAWarningLetterDetails = _SearchSummary.
                        GetFDAWarningLettersMatch(
                        query.NameToSearch, query.RecId, query.siteEnum);

                    return Ok(FDAWarningLetterDetails);
                //return Ok(_SearchSummary.
                //    GetStatusOfFDAWarningSiteRecords(FDAWarningLetterDetails,
                //    query.NameToSearch));

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

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    var DisqualificationDetails = _SearchSummary.
                        GetDisqualificationProceedingsMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.
                        GetStatusOfDisqualificationSiteRecords(DisqualificationDetails,
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

                case SiteEnum.SystemForAwardManagementPage:
                    var SAMDetails = _SearchSummary.
                        GetSAMMatch(
                        query.NameToSearch, query.RecId);

                    return Ok(_SearchSummary.
                        GetStatusOfSAMSiteRecords(SAMDetails,
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


        [Route("SaveSearchResult")]
        [HttpPost]
        public IHttpActionResult SaveSearchResults(SitesIncludedInSearch result,
            Guid? ComplianceFormId)
        {
            return Ok(_SearchSummary.SaveRecordStatus(
                result, ComplianceFormId));
        }

        [Route("Upload")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostFormData()
        
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                  
                }

                List<TestClass> tests = new List<TestClass>();
                //tests.Add(new TestClass {MatchSummary = "AAA", NameToSearch="BBB", ProcessedSummary="CCC", SearchDate="DDD" });
                TestClass test1 = new TestClass();
                test1.MatchSummary = "AAA";
                test1.NameToSearch = "NNNNN";
                test1.ProcessedSummary = "PPPP";
                test1.SearchDate = "1 Jan 2016";
                tests.Add(test1);

                TestClass test2 = new TestClass();
                test2.MatchSummary = "AAA";
                test2.NameToSearch = "NNNNN";
                test2.ProcessedSummary = "PPPP";
                test2.SearchDate = "1 Jan 2016";
                tests.Add(test2);

                //return Request.CreateResponse(HttpStatusCode.OK(tests));

                //return Request.CreateResponse<Employee>(HttpStatusCode.OK, emp);  
                var response = Request.CreateResponse(HttpStatusCode.OK);
               
                response.Content = new StringContent(JArray.FromObject(tests).ToString(), Encoding.UTF8, "application/json");

                return response;
                //return Request.CreateResponse(response);
            }
            catch (System.Exception e)
            {
                
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }

        public class UserDetails
        {
            public string  UserName{get;set;}
            public string pwd { get; set; }
            public string RoleName { get; set; }
        }

        class TestClass
    {
        public string NameToSearch { get; set; }
        public string SearchDate { get; set; }
        public string MatchSummary { get; set; }
        public string ProcessedSummary { get; set; }
   
    }

    }
