using DDAS.API.Helpers;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
namespace DDAS.API.Controllers
{
    [Authorize(Roles = "user, admin")]
    [RoutePrefix("api/QC")]
    public class AuditController : ApiController
    {
        private IAudit _Audit;
        public AuditController(IAudit Audit)
        {
            _Audit = Audit;
        }

        [Route("RequestQC")]
        [HttpPost]
        public IHttpActionResult RequestQC(ComplianceForm Form)
        {
            return Ok(_Audit.RequestQC(Form));
        }

        [Route("RequestQC1")]
        [HttpPost]
        // public async Task<HttpResponseMessage> PostFormData()
        public async Task<HttpResponseMessage> RequestQC1()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string URL = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
            URL = URL.Replace(HttpContext.Current.Request.UrlReferrer.AbsolutePath, "");
            //get Temp Folder:
            var attachmentsFolder = HttpContext.Current.Server.MapPath("~/DataFiles/Attachments/");
            var tempFolder = attachmentsFolder + "TEMP-" + Guid.NewGuid();
            Directory.CreateDirectory(tempFolder);

            //Upload files:
            CustomMultipartFormDataStreamProvider provider =
                new CustomMultipartFormDataStreamProvider(tempFolder);

            var result = await Request.Content.ReadAsMultipartAsync(provider);
            var compFormId = result.FormData["ComplianceFormId"];
            var strReview = result.FormData["Review"];
            Review review = JsonConvert.DeserializeObject <Review> (strReview);
            if (review == null)
            {
                throw new Exception("Review object expected");
            }

            //Rename folder:

            string fileSaveLocation = attachmentsFolder + compFormId;
            //Remove folder if it was created by previous QC Request Action:
            if (Directory.Exists(fileSaveLocation))
            {
                Directory.Delete(fileSaveLocation, true);
            }
            Directory.Move(tempFolder, fileSaveLocation);

            var guidCompForm = Guid.Parse(compFormId);
            _Audit.RequestQC(guidCompForm, review, URL);

            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }

        [Route("GetQC")]
        [HttpGet]
        public IHttpActionResult GetAudit(string Id, string AssignedTo)
        {
            try
            {
                var RecId = Guid.Parse(Id);
                var CompForm = _Audit.GetQC(RecId, AssignedTo, User.Identity.GetUserName().ToLower());
                UpdateFormToCurrentVersion
                    .UpdateComplianceFormToCurrentVersion(CompForm);

                return Ok(CompForm);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ListQCs")]
        [HttpGet]
        public IHttpActionResult ListQCs()
        {
            return Ok(_Audit.ListQCs());
        }

        [Route("SubmitQC")]
        [HttpPost]
        public IHttpActionResult SaveAudit(ComplianceForm Form)
        {
            string URL = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
            URL = URL.Replace(HttpContext.Current.Request.UrlReferrer.AbsolutePath, "");
            return Ok(_Audit.SubmitQC(Form, URL));
        }

        [Route("ListQCSummary")]
        [HttpGet]
        public IHttpActionResult ListQCSummary(string FormId)
        {
            var Id = Guid.Parse(FormId);
            return Ok(_Audit.ListQCSummary(Id));
        }

        [Route("Undo")]
        [HttpGet]
        public IHttpActionResult Undo(
            string ComplianceFormId, UndoEnum undoEnum, string UndoComment)
        {
            var Id = Guid.Parse(ComplianceFormId);
            var Result = _Audit.Undo(Id, undoEnum, UndoComment);
            return Ok(Result);
        }

        private class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }
    }
}