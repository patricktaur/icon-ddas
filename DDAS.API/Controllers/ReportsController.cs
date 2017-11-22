using DDAS.API.Helpers;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Utilities;

namespace DDAS.API.Controllers
{
    [Authorize(Roles = "user, admin")]
    [RoutePrefix("api/Reports")]
    public class ReportsController : ApiController
    {
        private ISearchService _SearchSummary;
        private IUnitOfWork _UOW;
        private IConfig _config;
        private string _RootPath;
        private IReport _Report;

        public ReportsController(ISearchService SearchSummary, 
            IUnitOfWork UOW, 
            IConfig Config,
            IReport Report)
        {
            _RootPath = HttpRuntime.AppDomainAppPath;
            _SearchSummary = SearchSummary;
            _UOW = UOW;
            _config = Config;
            _Report = Report;
        }

        [Route("GetNamesFromClosedComplianceForms")]
        [HttpGet]
        public List<ComplianceForm> GetNamesFromClosedComplianceForm()
        {
            var ComplianceForms =
                _UOW.ComplianceFormRepository.FindActiveComplianceForms(false);

            foreach (ComplianceForm form in ComplianceForms)
            {
                foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
                    Investigator.SiteDetails = null;
            }

            return ComplianceForms;
        }

        [Route("ActivateComplianceForm")]
        [HttpGet]
        public IHttpActionResult CloseComplianceForm(string ComplianceFormId)
        {
            Guid? RecId = Guid.Parse(ComplianceFormId);
            ComplianceForm form = _UOW.ComplianceFormRepository.FindById(RecId);

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
            var Forms = GetNamesFromClosedComplianceForm();
            return Ok(Forms);
        }

        [Route("GenerateOutputFileTest")]
        [HttpPost]
        public IHttpActionResult GenerateOutputFile(ComplianceFormFilter CompFormFilter)
        {
            //HttpResponseMessage response = null;

            if (!File.Exists(
                _config.ExcelTempateFolder + "Output_File_Template.xlsx"))
            {
                return Ok("Could not find Output file template");
               // response = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                //var UserAgent = Request.Headers.UserAgent.ToString();
                //var Browser = GetBrowserType(UserAgent);

                //response = Request.CreateResponse(HttpStatusCode.OK);

                var GenerateOutputFile =
                    new GenerateOutputFile(
                        _config.ExcelTempateFolder + "Output_File_Template.xlsx");

                var fromDate = CompFormFilter.SearchedOnFrom.Value;

                var allForms = _UOW.ComplianceFormRepository.GetAll();

                var forms = allForms
                    .Where(x => (x.StatusEnum == Models.Enums.ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified
                    || x.StatusEnum == Models.Enums.ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified))
                    .OrderBy(x => x.SearchStartedOn).ToList();

                var forms1 = forms;

                if (CompFormFilter.SearchedOnFrom != null)
                {
                    DateTime startDate;
                    startDate = CompFormFilter.SearchedOnFrom.Value.Date;
                    forms1 = forms.Where(x =>
                   x.SearchStartedOn >= startDate)
                   .ToList();
                }

                var forms2 = forms1;

                if (CompFormFilter.SearchedOnTo != null)
                {
                    DateTime endDate;
                    endDate = CompFormFilter.SearchedOnTo.Value.Date.AddDays(1);
                    forms2 = forms1.Where(x =>
                    x.SearchStartedOn <
                    endDate)
                    .ToList();
                }

                var FilePath = 
                    _SearchSummary.GenerateOutputFile(GenerateOutputFile, forms);

                //string path = FilePath.Replace(_RootPath, "");
                //return Ok(path);
                return Ok();

                //var memoryStream =
                //    _SearchSummary.GenerateOutputFile(GenerateOutputFile, forms);

                //response.Content = new ByteArrayContent(memoryStream.ToArray());

                //response.Content.Headers.ContentDisposition =
                //    new ContentDispositionHeaderValue("attachment");

                //response.Content.Headers.ContentType =
                //    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                //var OutputFileName = "OutputFile_" +
                //    DateTime.Now.ToString("dd_MMM_yyyy HH_mm") +
                //    ".xlsx";

                //response.Content.Headers.ContentDisposition.FileName = OutputFileName;

                ////add custom headers to the response
                ////easy for angular2 to read this header
                //response.Content.Headers.Add("Filename", OutputFileName);
                //response.Content.Headers.Add("Browser", Browser);
                //response.Content.Headers.Add("Access-Control-Expose-Headers", "Filename");
                //response.Content.Headers.Add("Access-Control-Expose-Headers", "Browser");
            }
            //return response;
        }

        [Route("GenerateOutputFile")]
        [HttpPost]
        public HttpResponseMessage GenerateOutputFileXXX(ComplianceFormFilter CompFormFilter)
        {
            HttpResponseMessage response = null;

            if (!File.Exists(
                _config.ExcelTempateFolder + "Output_File_Template.xlsx"))
            {
                 response = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                var UserAgent = Request.Headers.UserAgent.ToString();
                var Browser = IdentifyBrowser.GetBrowserType(UserAgent);

                response = Request.CreateResponse(HttpStatusCode.OK);

                var IsAdmin = User.IsInRole("admin");
                var UserName = User.Identity.GetUserName();

                var GenerateOutputFile =
                    new GenerateOutputFile(
                        _config.ExcelTempateFolder + "Output_File_Template.xlsx");

                var fromDate = CompFormFilter.SearchedOnFrom.Value;

                var allForms = _UOW.ComplianceFormRepository.GetAll();

                if(!IsAdmin)
                {
                    allForms = _UOW.ComplianceFormRepository.GetAll().Where(x =>
                    x.AssignedTo == UserName).ToList();
                }

                var forms = 
                    allForms.OrderBy(x => x.SearchStartedOn).ToList();
                //.Where(x => (x.StatusEnum == Models.Enums.ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified
                //|| x.StatusEnum == Models.Enums.ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified))

                var forms1 = forms;

                if (CompFormFilter.SearchedOnFrom != null)
                {
                    DateTime startDate;
                    startDate = CompFormFilter.SearchedOnFrom.Value.Date;
                    forms1 = forms.Where(x =>
                   x.SearchStartedOn >= startDate)
                   .ToList();
                }

                var forms2 = forms1;

                if (CompFormFilter.SearchedOnTo != null)
                {
                    DateTime endDate;
                    endDate = CompFormFilter.SearchedOnTo.Value.Date.AddDays(1);
                    forms2 = forms1.Where(x =>
                    x.SearchStartedOn <
                    endDate)
                    .ToList();
                }

                var memoryStream =
                    _SearchSummary.GenerateOutputFile(GenerateOutputFile, forms2);

                response.Content = new ByteArrayContent(memoryStream.ToArray());

                response.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment");

                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                var OutputFileName = "OutputFile_" +
                    DateTime.Now.ToString("dd_MMM_yyyy_HH_mm") +
                    ".xlsx";

                response.Content.Headers.ContentDisposition.FileName = OutputFileName;

                var FileName = OutputFileName + " " + Browser;
                //add custom headers to the response
                //easy for angular2 to read this header
                response.Content.Headers.Add("Filename", FileName);
                //response.Content.Headers.Add("Browser", Browser);
                response.Content.Headers.Add("Access-Control-Expose-Headers", "Filename");
                //response.Content.Headers.Add("Access-Control-Expose-Headers", "Browser");
            }
            return response;
        }

        [Route("UserManual")]
        [HttpGet]
        public HttpResponseMessage DownloadUserManual()
        {
            HttpResponseMessage response = null;
            var FilePath =
                _config.ExcelTempateFolder +
                "User Manual - ICON - DDAS - Draft.pdf";

            if (!File.Exists(
                FilePath))
            {
                response = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                var UserAgent = Request.Headers.UserAgent.ToString();
                var Browser = IdentifyBrowser.GetBrowserType(UserAgent);

                response = Request.CreateResponse(HttpStatusCode.OK);

                var byteArray = File.ReadAllBytes(FilePath);
                var memoryStream = new MemoryStream();
                memoryStream.Write(byteArray, 0, byteArray.Length);

                response.Content = new ByteArrayContent(memoryStream.ToArray());

                response.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment");

                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/pdf");

                var FileName = "ICON_User_Manual.pdf";

                response.Content.Headers.ContentDisposition.FileName = FileName;

                var Name = FileName + " " + Browser;
                //add custom headers to the response
                //easy for angular2 to read this header
                response.Content.Headers.Add("Filename", Name);
                //response.Content.Headers.Add("Browser", Browser);
                response.Content.Headers.Add("Access-Control-Expose-Headers", "Filename");
                //response.Content.Headers.Add("Access-Control-Expose-Headers", "Browser");
            }
            return response;
        }

        [Route("InvestigationsCompletedReport")]
        [HttpPost]
        public IHttpActionResult GetInvestigationCompletedReport(ReportFilters Filters)
        {
            //if (Filters.ToDate != null)
            //{
            //    Filters.ToDate = Filters.ToDate.Date.AddDays(1);
            //}

            var Report = _Report.GetInvestigationsReport(Filters);
            return Ok(Report);
        }

        [Route("OpenInvestigationsReport")]
        [HttpGet]
        public IHttpActionResult GetOpenInvestigations()
        {
            var OpenInvestigations = _Report.GetOpenInvestigations();
            return Ok(OpenInvestigations);
        }

        [Route("AdminDashboard")]
        [HttpGet]
        public IHttpActionResult GetAdminDashboard()
        {
            return Ok(_Report.GetAdminDashboard());
        }

        [Route("AssignmentHistory")]
        [HttpGet]
        public IHttpActionResult GetAssignmentHistory()
        {
            return Ok(
                _Report.GetAssignmentHistory());
        }

        [Route("StudySpecificInvestigators")]
        [HttpGet]
        public IHttpActionResult GetStudySpecificInvestigators(string ProjectNumber)
        {
            return Ok(
                _Report.GetStudySpecificInvestigators(ProjectNumber));
        }
    }
}