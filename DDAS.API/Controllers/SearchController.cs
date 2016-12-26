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
        private ISearchService _SearchService;
        private ISiteSummary _SiteSummary;
        private IUnitOfWork _UOW;
        private ILog _log;

        private string DataExtractionLogFile =
            System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"];

        private string UploadFolder =
            System.Configuration.ConfigurationManager.AppSettings["UploadFolder"];

        private string DownloadFolder =
            System.Configuration.ConfigurationManager.AppSettings["DownloadFolder"];

        private string TemplatesFolder =
            System.Configuration.ConfigurationManager.AppSettings["TemplateFolder"];

        private string AppDataFolder = HttpContext.Current.Server.MapPath("~/App_Data");

        public SearchController(ISearchEngine search, ISearchService SearchSummary,
            IUnitOfWork uow, ISiteSummary SiteSummary)
        {
            _SearchEngine = search;
            _SearchService = SearchSummary;
            _UOW = uow;
            _log = new DummyLog(); //Need to refactor
            _log = new LogText(DataExtractionLogFile);
            _SiteSummary = SiteSummary;
        }

        #region MoveToAccountsController
        [Authorize(Roles = "admin")]
        [Route("GetUsers")]
        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            var Users = _UOW.UserRepository.GetAllUsers();
   
            if (Users != null)
            {
                return Ok(Users);
            }
            else
                return Ok("no users found!");
        }

        [Route("GetUser")]
        [HttpGet]
        public IHttpActionResult GetUser(string UserId)
        {
            Guid? gUserId = Guid.Parse(UserId);
            var User = _UOW.UserRepository.FindById(gUserId); 
            //important: the User object must be mapped to Userview to eliminate security fields (hash code etc) 
            if (User != null)
            {
                return Ok(User);
            }
            else
                return Ok("No user found!");
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

       

        [Route("SaveUser")]
        [HttpPost]
        public IHttpActionResult SaveUser(User user)
        {

            _UOW.UserRepository.UpdateUser(user);

            return Ok(user);
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

        [Route("GetAllRoles")]
        [HttpGet]
        public IHttpActionResult GetAllRoles()
        {
            var roles = _UOW.RoleRepository.GetAll();
            return Ok(roles);
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
                var userName = User.Identity.GetUserName();
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
                    //_log.WriteLog("FileContent Length: " + FileContent.Length);

                    var forms = _SearchService.ReadUploadedFileData(file.LocalFileName,
                        _log);

                    if (forms == null)
                        return 
                            Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid File");

                    foreach(ComplianceForm form in forms)
                    {
                        complianceForms.Add(
                            _SearchService.ScanUpdateComplianceForm(form, _log));
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

        //[Authorize(Roles ="user")]
        [Route("GetPrincipalInvestigators")]
        [HttpGet]
        public IHttpActionResult GetPrincipalInvestigators()
        {
            return Ok(
                _SearchService.
                getPrincipalInvestigatorNComplianceFormDetails());
        }

        //GetInvestigatorSiteSummary/?formId=' + formId + "&investigatorId=" + investigatorId)
        [Route("GetInvestigatorSiteSummary")]
        [HttpGet]
        public IHttpActionResult GetInvestigatorSiteSummary(string formId, int investigatorId)
        {
            return Ok(
                _SearchService.
                    getInvestigatorSiteSummary(formId, investigatorId));
        }

        #region Patrick

        [Route("GetComplianceFormA")]
        [HttpGet]
        public IHttpActionResult GetComplianceForm(string formId = "")  //returns previously generated form or empty form  
        {
            //Patrick 02Dec2016
            //_log.LogStart();
            //try
            //{
            var userName = User.Identity.GetUserName();
            if (formId == null)
            {
                return Ok(_SearchService.GetNewComplianceForm(_log));
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
            }
        }

        [Route("SaveComplianceForm")]
        [HttpPost]
        public IHttpActionResult UpdateComplianceForm(ComplianceForm form)
        {
            return Ok(_SearchService.UpdateComplianceForm(form));
        }

        [Route("ScanSaveComplianceForm")]
        [HttpPost]
        public IHttpActionResult ScanUpdateComplianceForm(ComplianceForm form)
        {
            _log.LogStart();
            var result = _SearchService.ScanUpdateComplianceForm(form, _log);
            _log.WriteLog("=================================================================================");
            _log.LogEnd();
            return Ok(result);
        }
        #endregion

        //Called by Angular single name search
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

        //??? 
        public ComplianceForm GetSearchSummaryDetailsForSingleName(
            ComplianceForm form)
        {
            return _SearchService.GetSearchSummary(form, _log);
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
            return Ok();
            //var query = new SearchDetailsQuery();
            //query.NameToSearch = NameToSearch;
            //query.RecId = Guid.Parse(RecId);
            //query.siteEnum = siteEnum;

            //switch (query.siteEnum)
            //{
            //    case SiteEnum.FDADebarPage:
            //        var SearchDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(SearchDetails);

            //    case SiteEnum.ClinicalInvestigatorInspectionPage:
            //        var ClinicalSearchDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(ClinicalSearchDetails);

            //    case SiteEnum.FDAWarningLettersPage:
            //        var FDAWarningLetterDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(FDAWarningLetterDetails);

            //    case SiteEnum.ERRProposalToDebarPage:
            //        var ProposalToDebarDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(ProposalToDebarDetails);

            //    case SiteEnum.AdequateAssuranceListPage:
            //        var AssuranceDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(AssuranceDetails);

            //    case SiteEnum.ClinicalInvestigatorDisqualificationPage:
            //        var DisqualificationDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(DisqualificationDetails);

            //    case SiteEnum.CBERClinicalInvestigatorInspectionPage:
            //        var CBERDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(CBERDetails);

            //    case SiteEnum.PHSAdministrativeActionListingPage:
            //        var PHSSearchDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(PHSSearchDetails);

            //    case SiteEnum.ExclusionDatabaseSearchPage:
            //        var ExclusionDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(ExclusionDetails);

            //    case SiteEnum.CorporateIntegrityAgreementsListPage:
            //        var CIADetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(CIADetails);

            //    case SiteEnum.SystemForAwardManagementPage:
            //        var SAMDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(SAMDetails);

            //    case SiteEnum.SpeciallyDesignedNationalsListPage:
            //        var SDNSearchDetails = _SearchService.
            //            GetMatchedRecords(query.NameToSearch,
            //            query.RecId, siteEnum);

            //        return Ok(SDNSearchDetails);

            //    default:
            //        throw new Exception("wrong enum");
            //}
        }

        [Route("ExtractDataForSingleSite")]
        [HttpGet]
        public IHttpActionResult ExtractSingleSite(string NameToSearch,
            string ComplianceFormId, SiteEnum Enum)
        {
            _log.LogStart();

            Guid? RecId = Guid.Parse(ComplianceFormId);
            var form = 
                _SearchService.UpdateSingleSiteFromComplianceForm(
                    NameToSearch, RecId, Enum, _log);

            _log.WriteLog("=================================================================================");
            _log.LogEnd();

            return Ok(form);
        }


        [Route("GenerateComplianceForm")]
        [HttpGet]
        public IHttpActionResult GenerateComplianceForm(string ComplianceFormId)
        {
            try
            {
               
                Guid? RecId = Guid.Parse(ComplianceFormId);

                var FilePath = _SearchService.GenerateComplianceFormAlt(
                    Guid.Parse(ComplianceFormId), TemplatesFolder, DownloadFolder);

                return Ok(FilePath);
            }
            catch (Exception e)
            {
                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                //    "Error Details: " + e.Message);
                return  Content(HttpStatusCode.BadRequest, e.Message);
            }
        
        }

        //3Dec2016
        [Route("DownloadComplianceForm")]
        [HttpGet]
        public HttpResponseMessage DownloadForm(string ComplianceFormId = null)
        {
            HttpResponseMessage result = null;

            if (ComplianceFormId == null)
            {
                result = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                // Serve the file to the client
                result = Request.CreateResponse(HttpStatusCode.OK);
                var form = _SearchService.GenerateComplianceForm(Guid.Parse(ComplianceFormId));

                result.Content = new ByteArrayContent(form.ToArray());

                result.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment");

                result.Content.Headers.ContentType = new MediaTypeHeaderValue(
                    "application/vnd.ms-word");
                    
                result.Content.Headers.ContentDisposition.FileName = "Compliance Form.docx";
            }
            return result;
        }

        //Not required
        [Route("TestDownload")]
        [HttpGet]
        public HttpResponseMessage DownloadComplianceForm()
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

                result.Content = new StreamContent(
                    new FileStream(localFilePath, FileMode.Open, FileAccess.Read));

                result.Content.Headers.ContentDisposition = 
                    new ContentDispositionHeaderValue("attachment");

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

        [Route("GetAllCountries")]
        [HttpGet]
        public IHttpActionResult GetAllCountries()
        {
            var Countries = new CountryList();
            return Ok(Countries.GetCountries);
        }

        [Route("GetSiteSources")]
        [HttpGet]
        public IHttpActionResult GetSiteSources()
        {
            return Ok(SearchSites.GetNewSearchQuery());
        }

    }

    public class UserDetails
    {
        public string UserName { get; set; }
        public string pwd { get; set; }

        public List<IdentityRole> Role { get; set; }

    }

}
