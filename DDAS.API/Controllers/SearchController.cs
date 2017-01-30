﻿using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System.Web.Http;
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
using Utilities.WordTemplate;

namespace DDAS.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private ISearchEngine _SearchEngine;
        private ISearchService _SearchService;
        private IUnitOfWork _UOW;
        private ILog _log;

        private string DataExtractionLogFile;
        private string UploadsFolder;
        private string ComplianceFormFolder;
        private string ExcelTemplateFolder;
        
        private string RootPath;
        private string WordTemplateFolder;     

        public SearchController(ISearchEngine search, ISearchService SearchSummary,
            IUnitOfWork uow)
        {
            _SearchEngine = search;
            _SearchService = SearchSummary;
            _UOW = uow;
            _log = new DummyLog(); //Need to refactor

            //_userName = User.Identity.GetUserName(); //returns null in constructor, returns correct value in method.
           
            RootPath = HttpRuntime.AppDomainAppPath;

            DataExtractionLogFile = RootPath +
                System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"];

            UploadsFolder = RootPath +
                System.Configuration.ConfigurationManager.AppSettings["UploadsFolder"];

            ComplianceFormFolder = RootPath +
                System.Configuration.ConfigurationManager.AppSettings["ComplianceFormFolder"];

            ExcelTemplateFolder = RootPath +
                System.Configuration.ConfigurationManager.AppSettings["ExcelTemplateFolder"];

            WordTemplateFolder = RootPath +
                System.Configuration.ConfigurationManager.AppSettings["WordTemplateFolder"];
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
                var userName = User.Identity.GetUserName();
                //CustomMultipartFormDataStreamProvider provider = 
                //    new CustomMultipartFormDataStreamProvider(UploadsFolder);

                var provider = new MultipartFormDataStreamProvider(UploadsFolder);

                await Request.Content.ReadAsMultipartAsync(provider);

                var complianceForms = new List<ComplianceForm>();

                List<string> ValidationMessages = new List<string>();
                

                foreach (MultipartFileData file in provider.FileData)
                {
                    var DataInExcelFile = 
                        _SearchService.ReadDataFromExcelFile(file.LocalFileName);

                    ValidationMessages =
                        _SearchService.ValidateExcelInputs(DataInExcelFile);

                    if (ValidationMessages.Count > 0)
                    {
                        //unable to make the uploader handle list of strings, therefore this ListToString workaround:
                        return
                            Request.CreateResponse(HttpStatusCode.OK, ListToString(ValidationMessages));
                    }

                    var forms = _SearchService.ReadUploadedFileData(DataInExcelFile,
                        _log, userName, file.LocalFileName);

                    foreach(ComplianceForm form in forms)
                    {
                        complianceForms.Add(
                            _SearchService.ScanUpdateComplianceForm(form, _log));
                    }
                }
                //return Request.CreateResponse(HttpStatusCode.OK, complianceForms);
                return Request.CreateResponse(HttpStatusCode.OK, "");

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
 
        private class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }


        //[Authorize(Roles ="user")]
        [Route("GetPrincipalInvestigators")]
        [HttpGet]
        public IHttpActionResult GetPrincipalInvestigators()
        {
            return Ok(
                _SearchService.
                getAllPrincipalInvestigators());
        }

        [Route("GetMyActivePrincipalInvestigators")]
        [HttpGet]
        public IHttpActionResult GetMyActivePrincipalInvestigators()
        {
             var UserName = User.Identity.GetUserName();
            return Ok(
                _SearchService.getPrincipalInvestigators(UserName, true));
         }


        [Route("GetMyReviewPendingPrincipalInvestigators")]
        [HttpGet]
        public IHttpActionResult GetMyReviewPendingPrincipalInvestigators()
        {
            var UserName = User.Identity.GetUserName();
            return Ok(
                _SearchService.getPrincipalInvestigators(UserName, true, false));
        }

        [Route("GetMyClosedPrincipalInvestigators")]
        [HttpGet]
        public IHttpActionResult GetMyClosedPrincipalInvestigators()
        {
            var UserName = User.Identity.GetUserName();
            return Ok(
                _SearchService.getPrincipalInvestigators(UserName, false));
        }

        [Route("GetMyReviewCompletedPrincipalInvestigators")]
        [HttpGet]
        public IHttpActionResult GetMyReviewCompletedPrincipalInvestigators()
        {
            var UserName = User.Identity.GetUserName();
            return Ok(
                _SearchService.getPrincipalInvestigators(UserName, true, true));
        }


        [Route("GetInvestigatorSiteSummary")]
        [HttpGet]
        public IHttpActionResult GetInvestigatorSiteSummary(string formId, int investigatorId)
        {
            return Ok(
                _SearchService.
                    getInvestigatorSiteSummary(formId, investigatorId));
        }

        //Pradeep 4Jan2017
        [Route("ComplianceFormFilters")]
        [HttpPost]
        public IHttpActionResult GetComplianceFormFilterResults(ComplianceFormFilter CompFormFilter)
        {
            return Ok(_SearchService.GetComplianceFormsFromFilters(CompFormFilter));
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
            var UserName = User.Identity.GetUserName();
            if (formId == null)
            {
                return Ok(_SearchService.GetNewComplianceForm(_log, UserName));
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


        [Route("SaveAssignedToData")]
        [HttpGet]
        public IHttpActionResult SaveAssginedToData(string AssignedTo, bool Active,
            string ComplianceFormId)
        {
            var RecId = Guid.Parse(ComplianceFormId);
            _SearchService.UpdateAssignedToData(AssignedTo, Active, RecId);
            return Ok(true);
        }

 
        [Route("GenerateComplianceForm")]
        [HttpGet]
        public IHttpActionResult GenerateComplianceForm(string ComplianceFormId)
        {
            try
            {
                Guid? RecId = Guid.Parse(ComplianceFormId);

                //var FilePath = _SearchService.GenerateComplianceFormAlt(
                //    RecId, WordTemplateFolder, ComplianceFormFolder);

                IWriter writer = new CreateComplianceFormWord();

                var FilePath = _SearchService.GenerateComplianceForm(
                    ComplianceFormFolder, WordTemplateFolder, RecId,
                    writer, ".docx");

                string path = FilePath.Replace(RootPath, "");

                return Ok(path);

            }
            catch (Exception e)
            {
                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                //    "Error Details: " + e.Message);
                return  Content(HttpStatusCode.BadRequest, e.Message);
            }
        
        }

        [Route("GenerateComplianceFormPDF")]
        [HttpGet]
        public IHttpActionResult GenerateComplianceFormPDF(string ComplianceFormId)
        {
            try
            {

                Guid? RecId = Guid.Parse(ComplianceFormId);

                //var FilePath = _SearchService.GenerateComplianceFormAlt(
                //    RecId, WordTemplateFolder, ComplianceFormFolder);

                IWriter writer = new CreateComplianceFormPDF();

                var FilePath = _SearchService.GenerateComplianceForm(
                    ComplianceFormFolder, WordTemplateFolder, RecId,
                    writer, ".pdf");

                string path = FilePath.Replace(RootPath, "");

                return Ok(path);

            }
            catch (Exception e)
            {
                //return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                //    "Error Details: " + e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
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
            var localFilePath = UploadsFolder + "SITE LIST REQUEST FORM_Updated.docx";
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
        [HttpPut]
        public IHttpActionResult CloseComplianceForm(Guid ComplianceFormId)
        {
            //compare AssignedTo to loggedInUser
            //validation: all reviews must be completed.

            ComplianceForm form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            form.Active = false;
            _UOW.ComplianceFormRepository.UpdateCollection(form);

            return Ok(true);
        }

        [Route("OpenComplianceForm")]
        [HttpPut]
        public IHttpActionResult OpenComplianceForm(Guid ComplianceFormId)
        {
           
            ComplianceForm form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);
            form.Active = true;
            _UOW.ComplianceFormRepository.UpdateCollection(form);

            return Ok(true);
        }


        [Route("DeleteComplianceForm")]
        [HttpGet]
        public IHttpActionResult DeleteComplianceForm(string ComplianceFormId)
        {
            Guid? RecId = Guid.Parse(ComplianceFormId);
            _UOW.ComplianceFormRepository.DropComplianceForm(RecId);
            return Ok(true);
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

        string ListToString(List<string> lst)
        {
            string retValue = "";
            foreach(string l in lst)
            {
                retValue += l + "---";
            }
            return retValue;
        }

    }

    public class UserDetails
    {
        public string UserName { get; set; }
        public string pwd { get; set; }

        public List<IdentityRole> Role { get; set; }

    }
}
