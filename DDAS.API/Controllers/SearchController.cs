using DDAS.Models.Entities.Domain;
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
using System.Linq;
using DDAS.Models.Enums;
using DDAS.API.Helpers;
using DDAS.Models.ViewModels;
using System.Text;

namespace DDAS.API.Controllers
{
    [Authorize(Roles = "user, admin")]
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        //private ISearchEngine _SearchEngine;
        private ISearchService _SearchService;
        private IUnitOfWork _UOW;
        //private ILog _log;
        private IConfig _config;
        private FileDownloadResponse _fileDownloadResponse;

        //private string DataExtractionLogFile;
        //private string UploadsFolder;
        //private string ComplianceFormFolder;
        //private string ExcelTemplateFolder;
        //private string ErrorScreenCaptureFolder;
        //private string AttachmentsFolder;
        //private string WordTemplateFolder;

        private string _RootPath;

        public SearchController(
            IUnitOfWork UOW,
            ISearchService SearchSummary, 
            IConfig Config)
        {
            _RootPath = HttpRuntime.AppDomainAppPath;
            _UOW = UOW;
            _config = Config;
            _SearchService = SearchSummary;
            _fileDownloadResponse = new FileDownloadResponse();
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

            try
            {   
                var userName = User.Identity.GetUserName();

                //to retain name of the file-to-be-saved/uploaded, 
                //by default a guid is generated as filename for security reasons
                //CustomMultipartFormDataStreamProvider provider = 
                //    new CustomMultipartFormDataStreamProvider(UploadsFolder); 

                var provider = new MultipartFormDataStreamProvider(_config.UploadsFolder);

                await Request.Content.ReadAsMultipartAsync(provider);

                var complianceForms = new List<ComplianceForm>();

                List<string> ValidationMessages = new List<string>();

                foreach (MultipartFileData file in provider.FileData)
                {
                    string FilePathWithGUID = file.LocalFileName;
                    string UploadedFileName = file.Headers.ContentDisposition.FileName;

                    if (UploadedFileName.StartsWith("\"") && UploadedFileName.EndsWith("\""))
                        UploadedFileName = UploadedFileName.Trim('"');
                    if (UploadedFileName.Contains(@"/") || UploadedFileName.Contains(@"\"))
                        UploadedFileName = Path.GetFileName(UploadedFileName);

                    var excelInput =
                            _SearchService.ReadDataFromExcelFile(FilePathWithGUID);

                    if (excelInput.ExcelInputRows.Count >= 0)
                    {
                        if (excelInput.ExcelInputRows.Count == 0)
                            return Request.CreateResponse(HttpStatusCode.OK,
                                "No records found");

                        if(excelInput.ExcelInputRows.SelectMany(
                            x => x.ErrorMessages.Where(
                                y => y.ToLower()
                                .Contains("errors found"))).Count() > 0)
                        {
                            //unable to make the uploader handle list of strings, 
                            //therefore this ListToString workaround:
                            return
                                Request.CreateResponse(HttpStatusCode.OK,
                                ListToString(excelInput));
                        }
                    }

                    var forms = _SearchService.ReadUploadedFileData(
                        excelInput,
                        userName, 
                        FilePathWithGUID, 
                        UploadedFileName);

                    //var extQuery = new Services.LiveScan.ExtractionQueries(_UOW, 2);
                    //DateTime nextEstimatedLiveScanCompletion = extQuery.getNextExtractionCompletion();

                    foreach (ComplianceForm form in forms)
                    {
                        //form.ExtractionEstimatedCompletion = nextEstimatedLiveScanCompletion;
                        complianceForms.Add(
                            _SearchService.ScanUpdateComplianceForm(
                                form));
                        //nextEstimatedLiveScanCompletion = nextEstimatedLiveScanCompletion.AddSeconds(extQuery.AverageExtractionTimeInSecs * 3);
                    }
                }
                //return Request.CreateResponse(HttpStatusCode.OK, complianceForms);
                
                return Request.CreateResponse(HttpStatusCode.OK, "ok");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Error Details: " + e.Message);
            }
            finally
            {

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

        [Route("UploadAttachments")]
        [HttpPost]
        //public async Task<HttpResponseMessage> UploadAttachments(ComplianceForm form)
        public async Task<HttpResponseMessage> UploadAttachments(string SessionId)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                var userName = User.Identity.GetUserName();
                //CustomMultipartFormDataStreamProvider provider = 
                //    new CustomMultipartFormDataStreamProvider(UploadsFolder);

                var provider = new MultipartFormDataStreamProvider(_config.UploadsFolder);

                await Request.Content.ReadAsMultipartAsync(provider);

                var Attachments = new List<Attachment>();

                List<string> ValidationMessages = new List<string>();

                foreach (MultipartFileData file in provider.FileData)
                {
                    string FilePathWithGUID = file.LocalFileName;
                    string UploadedFileName = file.Headers.ContentDisposition.FileName;
                    if (UploadedFileName.StartsWith("\"") && UploadedFileName.EndsWith("\""))
                    {
                        UploadedFileName = UploadedFileName.Trim('"');
                    }
                    if (UploadedFileName.Contains(@"/") || UploadedFileName.Contains(@"\"))
                    {
                        UploadedFileName = Path.GetFileName(UploadedFileName);
                    }

                    //File.Move(file.LocalFileName, Path.Combine(StoragePath, fileName));
                    var Attachment = new Attachment();
                    Attachment.Title = "";
                    Attachment.FileName = UploadedFileName;
                    Attachment.GeneratedFileName = FilePathWithGUID;
                }
                //_SearchService.AddAttachmentsToFindings(form);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Error Details: " + e.Message);
            }
        }

        [Route("UploadComplianceFormAttachments")]
        [HttpPost]
        //public async Task<HttpResponseMessage> UploadAttachments(ComplianceForm form)
        public async Task<HttpResponseMessage> UploadComplianceFormAttachments(string ComplianceFormId)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                var userName = User.Identity.GetUserName();
                //CustomMultipartFormDataStreamProvider provider = 
                //    new CustomMultipartFormDataStreamProvider(UploadsFolder);

                var provider = new MultipartFormDataStreamProvider(_config.UploadsFolder);

                await Request.Content.ReadAsMultipartAsync(provider);

                var Attachments = new List<Attachment>();

                List<string> ValidationMessages = new List<string>();

                foreach (MultipartFileData file in provider.FileData)
                {
                    string FilePathWithGUID = file.LocalFileName;
                    string UploadedFileName = file.Headers.ContentDisposition.FileName;
                    if (UploadedFileName.StartsWith("\"") && UploadedFileName.EndsWith("\""))
                    {
                        UploadedFileName = UploadedFileName.Trim('"');
                    }
                    if (UploadedFileName.Contains(@"/") || UploadedFileName.Contains(@"\"))
                    {
                        UploadedFileName = Path.GetFileName(UploadedFileName);
                    }

                    var Attachment = new Attachment();
                    Attachment.Title = "";
                    Attachment.FileName = UploadedFileName;
                    Attachment.GeneratedFileName = FilePathWithGUID;
                }
                //_SearchService.AddAttachmentsToFindings(form);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Error Details: " + e.Message);
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
                _SearchService.getPrincipalInvestigators(UserName,  true, true));
        }

        [Route("GetInvestigatorSiteSummary")]
        [HttpGet]
        public IHttpActionResult GetInvestigatorSiteSummary(string formId, int investigatorId)
        {
            return Ok(
                _SearchService.
                    getInvestigatorSiteSummary(formId, investigatorId));
        }

        //getInstituteFindingsSummary
        [Route("GetInstituteFindingsSummary")]
        [HttpGet]
        public IHttpActionResult getInstituteFindingsSummary(string formId)
        {
            Guid gCompFormId = Guid.Parse(formId);
            return Ok(
                _SearchService.
                    getInstituteFindingsSummary(gCompFormId));
        }

        [Route("GetSingleComponentMatches")]
        [HttpGet]
        public IHttpActionResult GetSinlgeComponentMatches(
            SiteEnum Enum, 
            Guid? SiteDataId,
            string NameComponent)
        {
            return Ok();
        }

        [Route("GetSingleComponentMatchedRecords")]
        [HttpGet]
        public IHttpActionResult GetSingleComponentMatchRecords(
            string SiteDataId, SiteEnum SiteEnum, string FullName)
        {
            try
            {
                var Id = Guid.Parse(SiteDataId);
       
                //SiteEnum siteEnum = (SiteEnum)Enum;
                return Ok(
                    _SearchService.GetSingleComponentMatchedRecords(
                        Id, SiteEnum, FullName));
            }
            catch(Exception e)
            {
                return Ok(e.ToString());
            }
        }

        [Route("ComplianceFormFilters")]
        [HttpPost]
        public IHttpActionResult GetComplianceFormFilterResults(ComplianceFormFilter CompFormFilter)
        {
            try
            {
                return Ok(_SearchService.GetComplianceFormsFromFilters(CompFormFilter));
            }
            catch (Exception Ex)
            {
                return Ok(Ex.ToString());
            }
        }

        [Route("ClosedComplianceFormFilters")]
        [HttpPost]
        public IHttpActionResult GetClosedComplianceFormFilters(
            ComplianceFormFilter CompFormFilter)
        {
            try
            {
                var AssignedTo = User.Identity.GetUserName();
                return Ok(
                    _SearchService.GetClosedComplianceFormsFromFilters(
                        CompFormFilter, AssignedTo));
            }
            catch (Exception Ex)
            {
                return Ok(Ex.ToString());
            }
        }

        [Route("UnAssignedComplianceForms")]
        [HttpGet]
        public IHttpActionResult GetUnAssignedComplianceForms()
        {
            return Ok(_SearchService.GetUnAssignedComplianceForms());
        }

        #region Patrick

        [Route("GetComplianceForm")]
        [HttpGet]
        public IHttpActionResult GetComplianceForm(string formId = "")  //returns previously generated form or empty form  
        {
            var UserName = User.Identity.GetUserName();
            if (formId == null)
            {
                return Ok(_SearchService.GetNewComplianceForm(UserName));
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
                   
                    UpdateFormToCurrentVersion.
                        UpdateComplianceFormToCurrentVersion(compForm);

                    var Review = compForm.Reviews.FirstOrDefault();
                    if (Review != null &&
                        Review.Status == ReviewStatusEnum.SearchCompleted &&
                        compForm.AssignedTo.ToLower() == User.Identity.GetUserName().ToLower())
                    {
                        Review.StartedOn = DateTime.Now;
                        Review.Status = ReviewStatusEnum.ReviewInProgress;
                        //_UOW.ComplianceFormRepository.UpdateCollection(compForm);
                    }
                    _UOW.ComplianceFormRepository.UpdateCollection(compForm);

                    
                }
                return Ok(compForm);
            }
        }

        //Patrick: 16Jan2018
        //_UOW.ComplianceFormRepository.UpdateCollection(frm);
        [Route("UpdateQCEditComplianceForm")]
        [HttpPost]
        public IHttpActionResult UpdateQCEditComplianceForm(ComplianceForm form)
        {
            return Ok(_SearchService.UpdateQC(form));
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
            if (form.InvestigatorDetails.First().Name == "" ||
                form.InvestigatorDetails.First().Name == null)
                return Ok("PI name cannot be empty");

            var result = _SearchService.ScanUpdateComplianceForm(form);
            return Ok(result);
        }
        #endregion

        [Route("SaveAssignedToData")]
        [HttpGet]
        public IHttpActionResult SaveAssginedToData(string AssignedTo, string AssignedFrom,
            string ComplianceFormId)
        {
            //var AssignedBy = User.Identity.GetUserName();
            //var RecId = Guid.Parse(ComplianceFormId);
            //_SearchService.UpdateAssignedToData(AssignedTo, AssignedBy, Active, RecId);
            //return Ok(true);
            try
            {
                if (AssignedFrom == null)
                {
                    AssignedFrom = "";
                }
                var AssignedBy = User.Identity.GetUserName();
                var RecId = Guid.Parse(ComplianceFormId);
                _SearchService.UpdateAssignedTo(RecId, AssignedBy, AssignedFrom, AssignedTo);

                return Ok(true);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
            }
        }

        [Route("ClearAssignedTo")]
        [HttpGet]
                                                                                 
        public IHttpActionResult ClearAssginedTo(string ComplianceFormId, string AssignedFrom)
        {
            var AssignedBy = User.Identity.GetUserName();
            var RecId = Guid.Parse(ComplianceFormId);
            _SearchService.UpdateAssignedTo(RecId, AssignedBy, AssignedFrom, "");

            return Ok(true);
        }


        [Route("GetUploadsFolderPath")]
        [HttpGet]
        public IHttpActionResult GetUploadsFolderPath()
        {
            string FilePath = _config.UploadsFolder;
            string path = FilePath.Replace(_RootPath, "");
            return Ok(path);
        }

        [Route("DownloadUploadedFile")]
        [HttpGet]
        public HttpResponseMessage DownloadUploadedFile(string GeneratedFileName)
        {
            HttpResponseMessage Response = null;

            if (!File.Exists(_config.UploadsFolder + GeneratedFileName))
                Response = Request.CreateResponse(HttpStatusCode.Gone);
            else
            {
                Response = Request.CreateResponse(HttpStatusCode.OK);

                var UserAgent = Request.Headers.UserAgent.ToString();
                var Browser = IdentifyBrowser.GetBrowserType(UserAgent);

                byte[] ByteArray = 
                    File.ReadAllBytes(_config.UploadsFolder + GeneratedFileName);

                Response.Content = new ByteArrayContent(ByteArray);

                Response.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment");

                Response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                Response.Content.Headers.Add("Browser", Browser);
                Response.Content.Headers.Add("Access-Control-Expose-Headers", "Browser");
            }
            return Response;
        }

        #region Updates from Client
      
        [Route("UpdateCompFormGeneralNInvestigators")]
        [HttpPost]
        public IHttpActionResult UpdateCompFormGeneralNInvestigatorsNOptionalSites(ComplianceForm form)
        {
            return Ok(_SearchService.UpdateCompFormGeneralNInvestigatorsNOptionalSites(form));   
        }

        [Route("UpdateFindings")]
        [HttpPost]
        public IHttpActionResult UpdateFindings(UpdateFindigs updateFindings)
        {
            return Ok(_SearchService.UpdateFindings(updateFindings));
            //return Ok( _UOW.ComplianceFormRepository.UpdateFindings(updateFindings));
        }

        [Route("UpdateInstituteFindings")]
        [HttpPost]
        public IHttpActionResult UpdateInstituteFindings(UpdateInstituteFindings FindingsModel)
        {
               return Ok(_SearchService.UpdateInstituteFindings(FindingsModel));
        }

        #endregion

        [Route("GenerateComplianceForm")]
        [HttpGet]
        public HttpResponseMessage GenerateComplianceForm(string ComplianceFormId)
        {
            HttpResponseMessage response = null;

            var localFilePath = 
                _config.WordTemplateFolder + "ComplianceFormTemplate.docx";

            if (!File.Exists(localFilePath))
            {
               //return Ok("Could not find Compliance form template");
               response = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                var UserAgent = Request.Headers.UserAgent.ToString();
                var Browser = IdentifyBrowser.GetBrowserType(UserAgent);

                response = Request.CreateResponse(HttpStatusCode.OK);

                Guid? RecId = Guid.Parse(ComplianceFormId);

                IWriter writer = new CreateComplianceFormWord();

                string FileName = null;

                //var FilePath = _SearchService.GenerateComplianceForm(
                //    RecId, 
                //    writer, 
                //    ".docx", 
                //    out FileName);

                //string path = FilePath.Replace(_RootPath, "");
                //return Ok(path);

                //'out FileName' is retained in case the code needs to be rolled back
                //to return file path to the client
                var memoryStream = _SearchService.GenerateComplianceForm(
                    RecId,
                    writer,
                    ".docx",
                    out FileName);

                response.Content = new ByteArrayContent(memoryStream.ToArray());

                response.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment");

                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/ms-word");

                response.Content.Headers.ContentDisposition.FileName = FileName;

                var FileNameHeader = FileName + " " + Browser;
                //add custom headers to the response
                //easy for angular2 to read this header
                response.Content.Headers.Add("Filename", FileNameHeader);
                //response.Content.Headers.Add("Browser", Browser);
                response.Content.Headers.Add("Access-Control-Expose-Headers", "Filename");
                //response.Content.Headers.Add("Access-Control-Expose-Headers", "Browser");
            }
            return response;
        }

        [Route("GenerateComplianceFormPDF")]
        [HttpGet]
        public HttpResponseMessage GenerateComplianceFormPDF(string ComplianceFormId)
        {
            HttpResponseMessage response = null;

            var localFilePath = 
                _config.WordTemplateFolder + "ComplianceFormTemplate.docx";

            if (!File.Exists(localFilePath))
            {
                //return Ok("Could not find Compliance form template");
                response = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                var UserAgent = Request.Headers.UserAgent.ToString();
                var Browser = IdentifyBrowser.GetBrowserType(UserAgent);

                Guid? RecId = Guid.Parse(ComplianceFormId);

                IWriter writer = new CreateComplianceFormPDF();

                string FileName = null;

                //var FilePath = _SearchService.GenerateComplianceForm(
                //    RecId,
                //    writer,
                //    ".pdf",
                //    out FileName);

                //string path = FilePath.Replace(_RootPath, "");
                //return Ok();

                var memoryStream = _SearchService.GenerateComplianceForm(
                    RecId,
                    writer,
                    ".pdf",
                    out FileName);

                response.Content = new ByteArrayContent(memoryStream.ToArray());

                response.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment");

                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/pdf");

                response.Content.Headers.ContentDisposition.FileName = FileName;

                var FileNameHeader = FileName + " " + Browser;
                //add custom headers to the response
                //easy for angular2 to read this header
                response.Content.Headers.Add("Filename", FileNameHeader);
                //response.Content.Headers.Add("Browser", Browser);
                response.Content.Headers.Add("Access-Control-Expose-Headers", "Filename");
                //response.Content.Headers.Add("Access-Control-Expose-Headers", "Browser");
            }
            return response;
        }

        //[Route("GenerateOutputFile")]
        //[HttpGet]
        //public HttpResponseMessage GenerateOutputFile()
        //{
        //    HttpResponseMessage response = null;

        //    if(!File.Exists(
        //        _config.ExcelTempateFolder + "Output_File_Template.xlsx"))
        //    {
        //        response = Request.CreateResponse(HttpStatusCode.Gone);
        //    }
        //    else
        //    {
        //        var UserAgent = Request.Headers.UserAgent.ToString();
        //        var Browser = GetBrowserType(UserAgent);

        //        response = Request.CreateResponse(HttpStatusCode.OK);

        //        var GenerateOutputFile =
        //            new GenerateOutputFile(
        //                _config.ExcelTempateFolder + "Output_File_Template.xlsx");

        //        var forms = _UOW.ComplianceFormRepository.GetAll();

        //        //var FilePath = _SearchService.GenerateOutputFile(
        //        //    GenerateOutputFile,
        //        //    forms);

        //        //var Path = FilePath.Replace(RootPath, "");

        //        var memoryStream =
        //            _SearchService.GenerateOutputFile(GenerateOutputFile, forms);
                
        //        response.Content = new ByteArrayContent(memoryStream.ToArray());

        //        response.Content.Headers.ContentDisposition =
        //            new ContentDispositionHeaderValue("attachment");

        //        response.Content.Headers.ContentType =
        //            new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        //        var OutputFileName = "OutputFile_" +
        //            DateTime.Now.ToString("dd_MMM_yyyy HH_mm") +
        //            ".xlsx";

        //        response.Content.Headers.ContentDisposition.FileName = OutputFileName;

        //        //add custom headers to the response
        //        //easy for angular2 to read this header
        //        response.Content.Headers.Add("Filename", OutputFileName);
        //        response.Content.Headers.Add("Browser", Browser);
        //        response.Content.Headers.Add("Access-Control-Expose-Headers", "Filename");
        //        response.Content.Headers.Add("Access-Control-Expose-Headers", "Browser");
        //    }
        //    return response;
        //}

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
        [HttpPost]
        public HttpResponseMessage DownloadComplianceForm()
        {
            HttpResponseMessage result = null;
            //var localFilePath = HttpContext.Current.Server.MapPath("~/timetable.jpg");
            var localFilePath = _config.ExcelTempateFolder + "Output_File_Template.xlsx";
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

                result.Content.Headers.ContentType = 
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                result.Content.Headers.ContentDisposition.FileName = "Compliance Form";

                //add custom headers to the response
                result.Content.Headers.Add("Filename", "OutputFile_Test");
                result.Content.Headers.Add("Access-Control-Expose-Headers", "Filename");
            }
            return result;
        }
     
        [Route("CloseComplianceForm")]
        [HttpPut]
        public IHttpActionResult CloseComplianceForm(Guid ComplianceFormId)
        {
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
            //return Ok(SearchSites.GetNewSearchQuery());
            var test = _UOW.SiteSourceRepository.GetAll().OrderBy(x => x.SiteName);
            return Ok(_UOW.SiteSourceRepository.GetAll().OrderBy(x => x.SiteName).ToList());           
        }

        [Route("CurrentReviewStatus")]
        [HttpGet]
        public IHttpActionResult GetCurrentReviewStatus(string ComplianceFormId)
        {
            var FormId = Guid.Parse(ComplianceFormId);
            var Form = _UOW.ComplianceFormRepository.FindById(FormId);

            var Review = Form.Reviews.LastOrDefault();

            if (Review == null)
                throw new Exception("Review collection cannot be empty!");

            var CurrentReviewStatus = new CurrentReviewStatusViewModel();

            if (Review.Status == ReviewStatusEnum.SearchCompleted ||
                Review.Status == ReviewStatusEnum.ReviewInProgress ||
                Review.Status == ReviewStatusEnum.ReviewCompleted)
            {
                CurrentReviewStatus.ReviewerRecId = Review.RecId.Value;
                CurrentReviewStatus.QCVerifierRecId = null;
                CurrentReviewStatus.CurrentReview = Review;
            }
            else if(Review.Status == ReviewStatusEnum.QCRequested ||
                Review.Status == ReviewStatusEnum.QCInProgress ||
                Review.Status == ReviewStatusEnum.QCFailed)
            {
                CurrentReviewStatus.QCVerifierRecId = Review.RecId.Value;
                CurrentReviewStatus.CurrentReview = Review;
                var ReviewCompleted = Form.Reviews.Find(x => 
                    x.Status == ReviewStatusEnum.ReviewCompleted);
                if(ReviewCompleted != null)
                    CurrentReviewStatus.ReviewerRecId = ReviewCompleted.RecId;
                else
                    CurrentReviewStatus.ReviewerRecId = null;
            }
            else if(Review.Status == ReviewStatusEnum.QCCorrectionInProgress)
            {
                var QCReview = Form.Reviews.Find(x =>
                    x.Status == ReviewStatusEnum.QCFailed);
                if (QCReview != null)
                    CurrentReviewStatus.QCVerifierRecId = QCReview.RecId;
                else
                    CurrentReviewStatus.QCVerifierRecId = null;
                var ReviewCompleted = Form.Reviews.Find(x =>
                    x.Status == ReviewStatusEnum.ReviewCompleted);
                if (ReviewCompleted != null)
                    CurrentReviewStatus.ReviewerRecId = ReviewCompleted.RecId;
                else
                    CurrentReviewStatus.ReviewerRecId = null;
                CurrentReviewStatus.CurrentReview = Review;
            }
            else if (Review.Status == ReviewStatusEnum.Completed)
            {
                var QCReview = Form.Reviews.Find(x =>
                    x.Status == ReviewStatusEnum.QCFailed || 
                    x.Status == ReviewStatusEnum.QCPassed);
                if (QCReview != null)
                    CurrentReviewStatus.QCVerifierRecId = QCReview.RecId;
                else
                    CurrentReviewStatus.QCVerifierRecId = null;

                var ReviewCompleted = Form.Reviews.Find(x =>
                    x.Status == ReviewStatusEnum.ReviewCompleted);
                if (ReviewCompleted != null)
                    CurrentReviewStatus.ReviewerRecId = ReviewCompleted.RecId;
                else
                    CurrentReviewStatus.ReviewerRecId = null;
                CurrentReviewStatus.CurrentReview = Review;
            }
            return Ok(CurrentReviewStatus);
        }

        [Route("GetAttachmentsList")]
        [HttpGet]
        public IHttpActionResult GetAttachmentsList(string formId)
        {
            var folder = HttpContext.Current.Server.MapPath("~/DataFiles/Attachments/" + formId);

            string[] FileList = new string[0];

            if (Directory.Exists(folder))
            {
                FileList = Directory.GetFiles(folder).Select(file => Path.GetFileName(file)).ToArray();

                return Ok(FileList);
            }
            else
                return Ok(FileList);
            //string[] files = Directory.GetFiles(dir).Select(file => Path.GetFileName(file)).ToArray(); – 
        }

        [Route("DownloadAttachmentFile")]
        [HttpGet]
        public HttpResponseMessage DownloadAttachmentFile(string formId, string fileName)
        {

            var folder = HttpContext.Current.Server.MapPath("~/DataFiles/Attachments/" + formId);
            var fileNameWithPath = folder + "/" + fileName;

            


            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(fileNameWithPath, FileMode.Open, FileAccess.Read);
            //stream.ReadTimeout = 25000;
            //stream.WriteTimeout = 25000;
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("Filename");
            result.Content.Headers.ContentDisposition.FileName = fileName;

            var UserAgent = Request.Headers.UserAgent.ToString();
            var Browser = IdentifyBrowser.GetBrowserType(UserAgent);
            var FileNameHeader = fileName + " " + Browser;
            result.Content.Headers.Add("Filename", FileNameHeader);
            result.Content.Headers.Add("Access-Control-Expose-Headers", "Filename");

            return result;
        }



        //[Route("GetSessionId")]
        //[HttpGet]
        //public string GetSessionId()
        //{
        //    var retValue = Guid.NewGuid().ToString().Replace("-", "A");

        //    return retValue;
        //}


        //[Route("GetSessionId")]
        //[HttpGet]
        //public IHttpActionResult GetSessionId()
        //{
        //    return Ok("abc");
        //}

        #region Download Data Files
        [Route("DownloadDataFiles")]
        [HttpGet]
        public IHttpActionResult GetDownloadedDataFiles(int SiteEnum)
        {
            var DataFiles = _SearchService.GetDataFiles(SiteEnum);

            DataFiles.ForEach(DataFile =>
            {
                DataFile.FullPath = 
                DataFile.FullPath.Replace(_RootPath, "");
            });
            return Ok(DataFiles);
        }
        #endregion

        [Route("ExportToiSprint")]
        [HttpGet]
        public IHttpActionResult ExportToiSprint(string ComplianceFormId)
        {
            return Ok();
        }

        string ListToString(ExcelInput excelInput)
        {
            string retValue = "";

            var excelRows = excelInput.ExcelInputRows;
            foreach(ExcelInputRow row in excelRows)
            {
                foreach(string Value in row.ErrorMessages)
                {
                    retValue += Value + "---";
                }
            }
            return retValue;
        }

        string ListToString(List<List<string>> lst)
        {
            string retValue = "";
            foreach(List<string> l in lst)
            {
                foreach(string s in l)
                {
                    retValue += s + "---";
                }
                //retValue += l + "---";
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
