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
using System.Collections.Generic;
using System.Web;


namespace DDAS.API.Controllers
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private ISearchEngine _SearchEngine;
        private ISearchService _SearchSummary;
        private ISiteSummary _SiteSummary;
        private IUnitOfWork _UOW;
        private ILog _log;

        private string DataExtractionLogFile =
            System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"];

        private string UploadFolder =
            System.Configuration.ConfigurationManager.AppSettings["UploadFolder"];

        public SearchController(ISearchEngine search, ISearchService SearchSummary,
            IUnitOfWork uow, ILog log, ISiteSummary SiteSummary)
        {
            _SearchEngine = search;
            _SearchSummary = SearchSummary;
            _UOW = uow;
            _log = new LogText(DataExtractionLogFile);
            _SiteSummary = SiteSummary;
        }

        #region MoveToAccountsController
        [Route("GetAllUsers")]
        [HttpGet]
        public IHttpActionResult GetUser(string UserName)
        {
            var User = _UOW.UserRepository.GetAllUsers();
            if (User != null)
            {
                return Ok(User);
            }
            else
                return Ok("no users found!");
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
                var role = new List<IdentityRole>();
                role = user.Role;
                IdentityUser IdUsertemp = um.FindByName(IdUser.UserName);
                if (IdUsertemp == null)
                {
                    um.CreateAsync(IdUser, user.pwd);

                    um.AddToRole(IdUser.Id, role[0].Name);
                }
                else
                {
                    um.AddToRole(IdUser.Id, role[0].Name);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return Ok("User: " + user.UserName + " has been added");
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
        #endregion


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
                //string root = HttpContext.Current.Server.MapPath("~/App_Data");

                CustomMultipartFormDataStreamProvider provider = 
                    new CustomMultipartFormDataStreamProvider(UploadFolder);

                await Request.Content.ReadAsMultipartAsync(provider);

                var complianceForms = new List<ComplianceForm>();
                string[] FileContent = null;

                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);

                    FileContent = File.ReadAllLines(file.LocalFileName);
                    Trace.WriteLine(FileContent);
                    _log.WriteLog("FileContent Length: " + FileContent.Length);

                    var forms = _SearchSummary.ReadUploadedFileData(file.LocalFileName,
                        _log);

                    foreach(ComplianceForm form in forms)
                    {
                        complianceForms.Add(
                            _SearchSummary.ScanUpdateComplianceForm(form, _log));
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, complianceForms);
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
        public List<ComplianceForm> GetNamesFromOpenComplianceForm()
        {
            var ComplianceForms =
                _UOW.ComplianceFormRepository.FindActiveComplianceForms(true);

            foreach(ComplianceForm form in ComplianceForms)
            {
                foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
                    Investigator.SiteDetails = null;
            }

            return ComplianceForms;
        }

        [Route("GetPrincipalInvestigators")]
        [HttpGet]
        public IHttpActionResult GetPrincipalInvestigators()
        {
            return Ok(
                _SearchSummary.
                getPrincipalInvestigatorNComplianceFormDetails());
        }

        //GetInvestigatorSiteSummary/?formId=' + formId + "&investigatorId=" + investigatorId)
        [Route("GetInvestigatorSiteSummary")]
        [HttpGet]
        public IHttpActionResult GetInvestigatorSiteSummary(string formId, int investigatorId)
        {
            return Ok(
                _SearchSummary.
                    getInvestigatorSiteSummary(formId, investigatorId));
        }


        #region Patrick
        //Patrick:27Nov2016

        //[Route("PrincipalInvestigators")]
        //[HttpGet]
        //public IHttpActionResult PrincipalInvestigators()  
        //{
        //    return Ok(_SearchSummary.getPrincipalInvestigatorNComplianceFormDetails());

        //}

        [Route("GetComplianceFormA")]
        [HttpGet]
        public IHttpActionResult GetComplianceForm(string formId = "")  //returns previously generated form or empty form  
        {
            //Patrick 02Dec2016
            //_log.LogStart();
            //try
            //{
            if (formId.Length == 0)
            {
                return Ok(_SearchSummary.GetNewComplianceForm(_log));
            }
            else
            {
                Guid? gFormId = Guid.Parse(formId);
                var compForm = _UOW.ComplianceFormRepository.FindById(gFormId);
                if (compForm == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(compForm);
                }
                
                //return Ok(_UOW.ComplianceFormRepository.FindById(formId));
            }
           
            //}
            //catch (Exception e)
            //{
            //    _log.WriteLog("ErrorMessage: " + e.ToString());
            //    return InternalServerError(e);
            //}
            //finally
            //{
            //    _log.WriteLog("=================================================================================");
            //    _log.LogEnd();
            //}
        }

        [Route("SaveComplianceForm")]
        [HttpPost]
        public IHttpActionResult UpdateComplianceForm(ComplianceForm form)
        {
            return Ok(_SearchSummary.UpdateComplianceForm(form));
        }

        [Route("ScanSaveComplianceForm")]
        [HttpPost]
        public IHttpActionResult ScanUpdateComplianceForm(ComplianceForm form)
        {
            _log.LogStart();
            var result = _SearchSummary.ScanUpdateComplianceForm(form, _log);
            _log.WriteLog("=================================================================================");
            _log.LogEnd();
            return Ok(result);
        }

      

        #endregion


        //Called by Angular single name search.
        [Route("GetComplianceForm")]
        [HttpPost]
        public IHttpActionResult GetComplianceForm(ComplianceForm form)
        {
            _log.LogStart();
            try
            {
                var SearchResults = GetSearchSummaryDetailsForSingleName(form);
                return Ok(SearchResults);
            }
            catch (Exception e)
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
            ComplianceForm form)
        {

            return _SearchSummary.GetSearchSummary(form, _log);

        }

        [Route("GetSearchSummaryResult")]
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

        [Route("GetSearchSummaryDetails")]
        [HttpGet]
        public IHttpActionResult GetSearchSummaryDetailsXXX(string NameToSearch,
            string RecId, SiteEnum siteEnum)
        {
            var query = new SearchDetailsQuery();
            query.NameToSearch = NameToSearch;
            query.RecId = Guid.Parse(RecId);
            query.siteEnum = siteEnum;

            switch (query.siteEnum)
            {
                case SiteEnum.FDADebarPage:
                    var SearchDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(SearchDetails);

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    var ClinicalSearchDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(ClinicalSearchDetails);

                case SiteEnum.FDAWarningLettersPage:
                    var FDAWarningLetterDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(FDAWarningLetterDetails);

                case SiteEnum.ERRProposalToDebarPage:
                    var ProposalToDebarDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(ProposalToDebarDetails);

                case SiteEnum.AdequateAssuranceListPage:
                    var AssuranceDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(AssuranceDetails);

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    var DisqualificationDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(DisqualificationDetails);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    var CBERDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(CBERDetails);

                case SiteEnum.PHSAdministrativeActionListingPage:
                    var PHSSearchDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(PHSSearchDetails);

                case SiteEnum.ExclusionDatabaseSearchPage:
                    var ExclusionDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(ExclusionDetails);

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    var CIADetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(CIADetails);

                case SiteEnum.SystemForAwardManagementPage:
                    var SAMDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(SAMDetails);

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    var SDNSearchDetails = _SearchSummary.
                        GetMatchedRecords(query.NameToSearch,
                        query.RecId, siteEnum);

                    return Ok(SDNSearchDetails);

                default:
                    throw new Exception("wrong enum");
            }
        }

        [Route("ExtractDataForSingleSite")]
        [HttpGet]
        public IHttpActionResult ExtractSingleSite(string NameToSearch,
            string ComplianceFormId, SiteEnum Enum)
        {
            _log.LogStart();

            Guid? RecId = Guid.Parse(ComplianceFormId);
            var form = 
                _SearchSummary.UpdateSingleSiteFromComplianceForm(
                    NameToSearch, RecId, Enum, _log);

            _log.WriteLog("=================================================================================");
            _log.LogEnd();

            return Ok(form);
        }

        [Route("SaveSearchResult")]
        [HttpPost]
        public IHttpActionResult SaveSearchResults(string NameToSearch,
            SitesIncludedInSearch result, Guid? ComplianceFormId)
        {
            return Ok(
                _SearchSummary.SaveRecordStatus(
                    NameToSearch, result, ComplianceFormId));
        }


        [Route("GenerateComplianceForm")]
        [HttpGet]
        public IHttpActionResult GenerateComplianceForm(string ComplianceFormId)
        {
            Guid? RecId = Guid.Parse(ComplianceFormId);
            var form = new GenerateComplianceForm(_UOW);
            form.GetComplianceForm(RecId);
            return Ok();
        }

        [Route("DownloadComplianceForm")]
        [HttpGet]
        public HttpResponseMessage GetTestFile()
        {
            HttpResponseMessage result = null;
            //var localFilePath = HttpContext.Current.Server.MapPath("~/timetable.jpg");
            var localFilePath = UploadFolder + "SITE LIST REQUEST FORM_Updated.docx";
            if (!File.Exists(localFilePath))
            {
                result = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                // Serve the file to the client
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = "Compliance Form";
            }
            return result;
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
            var Forms = GetNamesFromOpenComplianceForm();
            return Ok(Forms);
        }

    }

    public class UserDetails
    {
        public string UserName { get; set; }
        public string pwd { get; set; }

        public List<IdentityRole> Role { get; set; }

    }

}
