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


namespace DDAS.API.Controllers
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private ISearchEngine _SearchEngine;
        private ISearchSummary _SearchSummary;
        private ISiteSummary _SiteSummary;
        private IUnitOfWork _UOW;
        private ILog _log;

        private string DataExtractionLogFile =
            System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"];

        public SearchController(ISearchEngine search, ISearchSummary SearchSummary,
            IUnitOfWork uow, ILog log, ISiteSummary SiteSummary)
        {
            _SearchEngine = search;
            _SearchSummary = SearchSummary;
            _UOW = uow;
            _log = log;
            _SiteSummary = SiteSummary;
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

        [Route("GetSearchSummaryResult")]
        [HttpGet]
        public IHttpActionResult GetSearchSummaryResult(string NameToSearch)
        {
            var SearchResults = GetSearchSummaryDetailsForSingleName(NameToSearch);
            return Ok(SearchResults);
        }

        [Route("GetSearchSummary")]
        [HttpGet]
        public IHttpActionResult GetSearchSummary(string NameToSearch, string ComplianceFormId)
        {
            var Query = new NameToSearchQuery();
            Query.NameToSearch = NameToSearch;
            Query.ComplianceFormId = Guid.Parse(ComplianceFormId);

            return Ok(
                _SiteSummary.GetSearchSummaryStatus(
                    Query.NameToSearch, Query.ComplianceFormId));
        }

        public ComplianceForm GetSearchSummaryDetailsForSingleName(
            string NameToSearch)
        {
            return _SearchSummary.GetSearchSummary(NameToSearch);
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
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(SearchDetails);

                //refactor GetStatusOfFDASiteRecords as per the new design
                //return Ok(
                //    _SearchSummary.GetStatusOfFDASiteRecords(SearchDetails,
                //    query.NameToSearch));

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    var ClinicalSearchDetails = _SearchSummary.
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(ClinicalSearchDetails);
                //Refactor this as per new design
                //return Ok(_SearchSummary.
                //    GetStatusOfClinicalSiteRecords(ClinicalSearchDetails,
                //    query.NameToSearch));

                case SiteEnum.FDAWarningLettersPage:
                    var FDAWarningLetterDetails = _SearchSummary.
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(FDAWarningLetterDetails);
                //return Ok(_SearchSummary.
                //    GetStatusOfFDAWarningSiteRecords(FDAWarningLetterDetails,
                //    query.NameToSearch));

                case SiteEnum.ERRProposalToDebarPage:
                    var ProposalToDebarDetails = _SearchSummary.
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(ProposalToDebarDetails);

                case SiteEnum.AdequateAssuranceListPage:
                    var AssuranceDetails = _SearchSummary.
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(AssuranceDetails);

                //case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                //    var DisqualificationDetails = _SearchSummary.
                //        GetDisqualificationProceedingsMatch(
                //        query.NameToSearch, query.RecId);

                //    return Ok(_SearchSummary.
                //        GetStatusOfDisqualificationSiteRecords(DisqualificationDetails,
                //        query.NameToSearch));

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    var CBERDetails = _SearchSummary.
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(CBERDetails);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    var PHSSearchDetails = _SearchSummary.
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(PHSSearchDetails);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    var ExclusionDetails = _SearchSummary.
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(ExclusionDetails);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    var CIADetails = _SearchSummary.
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(CIADetails);

                //case SiteEnum.SystemForAwardManagementPage:
                //    var SAMDetails = _SearchSummary.
                //        GetSAMMatch(
                //        query.NameToSearch, query.RecId);

                //    return Ok(_SearchSummary.
                //        GetStatusOfSAMSiteRecords(SAMDetails,
                //        query.NameToSearch));

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    var SDNSearchDetails = _SearchSummary.
                        GetMatchedRecords(
                        query.NameToSearch, query.RecId, siteEnum);

                    return Ok(SDNSearchDetails);

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
        public Task<List<ComplianceForm>> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //string root = HttpContext.Current.Server.MapPath("~\App_Data");
            
            //var Sites = new List<SitesIncludedInSearch>();
            //var Site = new SitesIncludedInSearch();

            //Site.SiteEnum = 0;
            //Site.SiteName = "FDA Debar Page";
            //Site.SiteUrl = "http://www.fda.gov/ICECI/EnforcementActions/FDADebarmentList/default.htm";
            //Site.FullMatchCount = 1;
            //Site.PartialMatchCount = 0;
            //Site.ScannedOn = DateTime.Now;
            //Site.LastUpdatedOn = DateTime.Now;
            //Site.CreatedOn = DateTime.Now;
            //Site.Findings = "";
            //Site.IssuesIdentified = false;
            //Sites.Add(Site);
            var ComplianceFormList = new List<ComplianceForm>();
            var complianceForm = new ComplianceForm();
            complianceForm.NameToSearch = "Martin Luther King";
            complianceForm.ProjectNumber = "AA113";
            complianceForm.Country = "UK";
            complianceForm.Address = "#221B Baker Street";
            complianceForm.SearchStartedOn = DateTime.Now;
            complianceForm.Sites_FullMatchCount = 1;
            complianceForm.Sites_PartialMatchCount = 0;
            complianceForm.SponsorProtocolNumber = "ED12C";
            
            complianceForm.SiteDetails = null;
            ComplianceFormList.Add(complianceForm);

            var AnotherComplianceForm = new ComplianceForm();
            AnotherComplianceForm.NameToSearch = "James Bond";
            AnotherComplianceForm.ProjectNumber = "007";
            AnotherComplianceForm.Country = "UK";
            AnotherComplianceForm.Address = "MI5 London";
            AnotherComplianceForm.SearchStartedOn = DateTime.Now;
            AnotherComplianceForm.Sites_FullMatchCount = 1;
            AnotherComplianceForm.Sites_PartialMatchCount = 20;
            AnotherComplianceForm.SponsorProtocolNumber = "MI5";

            AnotherComplianceForm.SiteDetails = null;
            ComplianceFormList.Add(AnotherComplianceForm);

            return Task.FromResult(ComplianceFormList);
            
        }


        [Route("GetNamesFromOpenComplianceForms")]
        [HttpGet]
        public IHttpActionResult GetNamesFromOpenComplianceForm()
        {

            var ComplianceFormList = new List<ComplianceForm>();
            var complianceForm = new ComplianceForm();
            complianceForm.NameToSearch = "Martin Luther King";
            complianceForm.ProjectNumber = "AA113";
            complianceForm.Country = "UK";
            complianceForm.Address = "#221B Baker Street";
            complianceForm.SearchStartedOn = DateTime.Now;
            complianceForm.Sites_FullMatchCount = 1;
            complianceForm.Sites_PartialMatchCount = 0;
            complianceForm.SponsorProtocolNumber = "ED12C";

            complianceForm.SiteDetails = null;
            ComplianceFormList.Add(complianceForm);

            var AnotherComplianceForm = new ComplianceForm();
            AnotherComplianceForm.NameToSearch = "James Bond";
            AnotherComplianceForm.ProjectNumber = "007";
            AnotherComplianceForm.Country = "UK";
            AnotherComplianceForm.Address = "MI5 London";
            AnotherComplianceForm.SearchStartedOn = DateTime.Now;
            AnotherComplianceForm.Sites_FullMatchCount = 1;
            AnotherComplianceForm.Sites_PartialMatchCount = 20;
            AnotherComplianceForm.SponsorProtocolNumber = "MI5";

            AnotherComplianceForm.SiteDetails = null;
            ComplianceFormList.Add(AnotherComplianceForm);

            return Ok(ComplianceFormList);
        }


        public int GetMeANumberAsync()
        {
            return 10 * 10;

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
