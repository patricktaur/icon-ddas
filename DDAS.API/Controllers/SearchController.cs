using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Web.Http;
using DDAS.Models.Enums;
using System;
using DDAS.Models;
using DDAS.API.Identity;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Utilities;
using System.Net.Http.Headers;
using DDAS.Services.Search;
using System.Web;

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

        //private string UploadFolder =
        //    System.Configuration.ConfigurationManager.AppSettings["UploadFolder"];

        public SearchController(ISearchEngine search, ISearchSummary SearchSummary,
            IUnitOfWork uow, ILog log, ISiteSummary SiteSummary)
        {
            _SearchEngine = search;
            _SearchSummary = SearchSummary;
            _UOW = uow;
            _log = new LogText(DataExtractionLogFile);
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

        [Route("Upload")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostFormData()
        {
            _log.LogStart();
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                // Read the form data
                //string root = HttpContext.Current.Server.MapPath("~/App_Data");
                string root = "C:\\Development\\DDAS_Uploads";
                CustomMultipartFormDataStreamProvider provider = 
                    new CustomMultipartFormDataStreamProvider(root);

                await Request.Content.ReadAsMultipartAsync(provider);

                string[] FileContent = null;
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);

                    FileContent = File.ReadAllLines(file.LocalFileName);
                    _log.WriteLog("FileContent Length: " + FileContent.Length);
                    for (int Counter = 1; Counter < FileContent.Length; Counter++)
                    {
                        _log.WriteLog(" Name: " + FileContent[Counter]);
                        GetSearchSummaryDetailsForSingleName(FileContent[Counter]);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Completed");
            }
            catch (Exception e)
            {
                _log.WriteLog("ErrorMessage: " + e.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, 
                    "Error Details: " + e.Message);
            }
            finally
            {
                _log.WriteLog("=================================================================================");
                _log.LogEnd();
            }
        }

        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }

        [Route("GetNamesFromOpenComplianceForms")]
        [HttpGet]
        public IHttpActionResult GetNamesFromOpenComplianceForm()
        {
            var ComplianceForms =
                _UOW.ComplianceFormRepository.FindActiveComplianceForms(true);

            foreach (ComplianceForm form in ComplianceForms)
                form.SiteDetails = null;

            return Ok(ComplianceForms);
        }

        [Route("GetComplianceForm")]
        [HttpGet]
        public IHttpActionResult GetSearchSummaryResult(string NameToSearch)
        {
            _log.LogStart();
            try
            {
                var SearchResults = GetSearchSummaryDetailsForSingleName(NameToSearch);
                return Ok(SearchResults);
            }
            catch(Exception e)
            {
                _log.WriteLog("ErrorMessage: " + e.ToString());
                return InternalServerError(e);
            }
            finally
            {
                _log.WriteLog("=================================================================================");
                _log.LogEnd();
            }
        }

        public ComplianceForm GetSearchSummaryDetailsForSingleName(
            string NameToSearch)
        {
            
            var form = _SearchSummary.GetSearchSummary(NameToSearch, _log);
            return form;
        }

        [Route("GetSearchSummaryResult")]
        [HttpGet]
        public IHttpActionResult GetSearchSummary(string ComplianceFormId)
        {
            var Query = new NameToSearchQuery();
            Query.ComplianceFormId = Guid.Parse(ComplianceFormId);

            return Ok(
                _SiteSummary.GetSearchSummaryStatus(Query.ComplianceFormId));
        }

        [Route("GetSearchSummaryDetails")]
        [HttpGet]
        public IHttpActionResult GetSearchSummaryDetailsXXX(string RecId,
            SiteEnum siteEnum)
        {
            var query = new SearchDetailsQuery();
            //query.NameToSearch = NameToSearch;
            query.RecId = Guid.Parse(RecId);
            query.siteEnum = siteEnum;

            switch (query.siteEnum)
            {
                case SiteEnum.FDADebarPage:
                    var SearchDetails = _SearchSummary.
                        GetMatchedRecords(
                        query.RecId, siteEnum);

                    return Ok(SearchDetails);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    var ClinicalSearchDetails = _SearchSummary.
                        GetMatchedRecords(query.RecId, siteEnum);

                    return Ok(ClinicalSearchDetails);

                case SiteEnum.FDAWarningLettersPage:
                    var FDAWarningLetterDetails = _SearchSummary.
                        GetMatchedRecords(query.RecId, siteEnum);

                    return Ok(FDAWarningLetterDetails);

                case SiteEnum.ERRProposalToDebarPage:
                    var ProposalToDebarDetails = _SearchSummary.
                        GetMatchedRecords(query.RecId, siteEnum);

                    return Ok(ProposalToDebarDetails);

                case SiteEnum.AdequateAssuranceListPage:
                    var AssuranceDetails = _SearchSummary.
                        GetMatchedRecords(query.RecId, siteEnum);

                    return Ok(AssuranceDetails);

                //case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                //    var DisqualificationDetails = _SearchSummary.
                //        GetDisqualificationProceedingsMatch(
                //        query.NameToSearch, query.RecId);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    var CBERDetails = _SearchSummary.
                        GetMatchedRecords(query.RecId, siteEnum);

                    return Ok(CBERDetails);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    var PHSSearchDetails = _SearchSummary.
                        GetMatchedRecords(query.RecId, siteEnum);

                    return Ok(PHSSearchDetails);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    var ExclusionDetails = _SearchSummary.
                        GetMatchedRecords(query.RecId, siteEnum);

                    return Ok(ExclusionDetails);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    var CIADetails = _SearchSummary.
                        GetMatchedRecords(query.RecId, siteEnum);

                    return Ok(CIADetails);

                //case SiteEnum.SystemForAwardManagementPage:
                //    var SAMDetails = _SearchSummary.
                //        GetSAMMatch(
                //        query.NameToSearch, query.RecId);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    var SDNSearchDetails = _SearchSummary.
                        GetMatchedRecords(query.RecId, siteEnum);

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

        [Route("GenerateComplianceForm")]
        [HttpGet]
        public IHttpActionResult GetComplianceForm(string ComplianceFormId)
        {
            Guid? RecId = Guid.Parse(ComplianceFormId);
            var form = new GenerateComplianceForm(_UOW);
            form.GetComplianceForm(RecId);
            return Ok();
        }

        [Route("CloseComplianceForm")]
        [HttpGet]
        public IHttpActionResult CloseComplianceForm(string ComplianceFormId)
        {
            Guid? RecId = Guid.Parse(ComplianceFormId);
            ComplianceForm form = _UOW.ComplianceFormRepository.FindById(RecId);

            form.Active = false;
            _UOW.ComplianceFormRepository.UpdateCollection(form);

            return Ok(true);
        }

        [Route("DeleteComplianceForm")]
        [HttpGet]
        public IHttpActionResult DeleteComplianceForm(string ComplianceFormId)
        {
            Guid? RecId = Guid.Parse(ComplianceFormId);
            _UOW.ComplianceFormRepository.DropComplianceForm(RecId);
            return Ok();
        }
    }

    public class UserDetails
    {
        public string  UserName{get;set;}
        public string pwd { get; set; }
        public string RoleName { get; set; }
    }
}
//var AnotherComplianceForm = new ComplianceForm();
//var ComplianceFormList = new List<ComplianceForm>();

//complianceForm.NameToSearch = "Martin Luther King";
//complianceForm.ProjectNumber = "AA113";
//complianceForm.Country = "UK";
//complianceForm.Address = "#221B Baker Street";
//complianceForm.SearchStartedOn = DateTime.Now;
//complianceForm.Sites_FullMatchCount = 1;
//complianceForm.Sites_PartialMatchCount = 0;
//complianceForm.SponsorProtocolNumber = "ED12C";

//complianceForm.SiteDetails = null;
//ComplianceFormList.Add(complianceForm);

//AnotherComplianceForm.NameToSearch = "James Bond";
//AnotherComplianceForm.ProjectNumber = "007";
//AnotherComplianceForm.Country = "UK";
//AnotherComplianceForm.Address = "MI5";
//AnotherComplianceForm.SearchStartedOn = DateTime.Now;
//AnotherComplianceForm.Sites_FullMatchCount = 1;
//AnotherComplianceForm.Sites_PartialMatchCount = 10;
//AnotherComplianceForm.SponsorProtocolNumber = "MI5";

//AnotherComplianceForm.SiteDetails = null;

//ComplianceFormList.Add(AnotherComplianceForm);