using DDAS.API.Helpers;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DDAS.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using Utilities;
using DDAS.API.Helpers;
using System.Collections.ObjectModel;

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
        private FileDownloadResponse _fileDownloadResponse;
        private CSVConvertor _csvConvertor;

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
            _fileDownloadResponse = new FileDownloadResponse();
            _csvConvertor = new CSVConvertor();
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

                if (!IsAdmin)
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
        public IHttpActionResult GetInvestigationCompletedReport(ReportFilters Filters, string mode = "view")
        {

            //var Report = _Report.GetInvestigationsReport(Filters);
            //return Ok(Report);


            var report = _Report.GetInvestigationsReport(Filters);
            var list = report.ReportByUsers;
            switch (mode)
            {
                case "view":
                    return Ok(report);
                case "csv":
                    var fileName = "Investigations_Completed.csv";
                    var newLine = System.Environment.NewLine;
                    StringBuilder sb = new StringBuilder();
                    //Header:
                    var header = " ";
                    foreach (var fld in list[0].ReportItems)
                    {
                        header += ", " + fld.ReportPeriod;
                    }
                    sb.AppendLine(header);

                    foreach (var lineItem in list)
                    {
                        string content = "";
                        content += "\"" + lineItem.UserName + "\"";
                        foreach (var fld in lineItem.ReportItems)
                        {
                            content += ", " + fld.Value;
                        }
                        sb.AppendLine(content);
                    }

                    return ResponseMessage(_fileDownloadResponse.GetResponse(Request, sb.ToString(), fileName));
                default:
                    return Ok(report);
            }
        }

        [Route("OpenInvestigationsReport")]
        [HttpGet]
        public IHttpActionResult GetOpenInvestigations(string mode = "view")
        {
            //var OpenInvestigations = _Report.GetOpenInvestigations();
            //return Ok(OpenInvestigations);

            var list = _Report.GetOpenInvestigations();
            switch (mode)
            {
                case "view":
                    return Ok(list);
                case "csv":
                    //Assigned To	Count	Earliest	Latest
                    var fileName = "Investigations_Open.csv";
                    var headers = new List<string> { "Assigned To", "Count", "Earliest", "Latest" };
                    return ResponseMessage(_fileDownloadResponse.GetResponse(Request, list, fileName, headers));
                default:
                    return Ok(list);
            }
        }

        [Route("AdminDashboard")]
        [HttpGet]
        public IHttpActionResult GetAdminDashboard(string mode = "view")
        {
            //return Ok(_Report.GetAdminDashboard());

            var list = _Report.GetAdminDashboard();

            switch (mode)
            {
                case "view":
                    return Ok(list);
                case "csv":
                    //User	Opening Balance	Compliance Forms Uploaded	Compliance Forms Completed	Closing Balance
                    var headers = new List<string> { "User", "Opening Balance", "Compliance Forms Uploaded", "Compliance Forms Completed", "Closing Balance" };
                    return ResponseMessage(_fileDownloadResponse.GetResponse(Request, list, "AdminDashboard.csv", headers));
                default:
                    return Ok(list);
            }
        }


        [Route("AssignmentHistory")]
        [HttpPost]
        public IHttpActionResult GetAssignmentHistory(ReportFilterViewModel ReportFilter, string mode = "view")
        {
            if (ReportFilter.ToDate != null)
            {
                ReportFilter.ToDate = ReportFilter.ToDate.Date.AddDays(1);
            }

            //return Ok(
            //_Report.GetAssignmentHistory(ReportFilter));

            //var list = _Report.GetAssignmentHistory(ReportFilter));
            var list = _Report.GetAssignmentHistory(ReportFilter);

            switch (mode)
            {
                case "view":
                    return Ok(list);
                case "csv":
                    var headers = new List<string> { "Principal Investigator", "Sub Investigator Count", "Proj No 1", "Proj No 2", "Search Started On", "Re-assigned On", "Re-assigned From", "Assigned By", "Assigned To", };
                    return ResponseMessage(_fileDownloadResponse.GetResponse(Request, list, "Report.csv", headers));
                default:
                    return Ok(list);
            }
        }



        [Route("InvestigatorReviewCompletedTime")]
        [HttpPost]
        public IHttpActionResult
            GetInvestigatorReviewCompletedTime(ReportFilterViewModel ReportFilter, string mode = "view")
        {
            if (ReportFilter.ToDate != null)
            {
                ReportFilter.ToDate = ReportFilter.ToDate.Date.AddDays(1);
            }


            var list = _Report.GetInvestigatorsReviewCompletedTime(ReportFilter);

            switch (mode)
            {
                case "view":
                    return Ok(list);
                case "csv":

                    // //Investigator	Role	Project Number	Search Started On	Review Completed On	
                    //Assigned To	Full Matches	Patrial Matches	Single Matches	Issues Status	TimeTaken (in Minutes) to Complete Review
                    var headers = new List<string> { "Investigator",
                        "Role", "Project Number-1",
                        "Project Number-2", "Search Started On",
                        "Review Completed On",  "Assigned To",
                        "Full Matches", "Patrial Matches",
                        "Single Matches",  "Issues Status",
                        "TimeTaken (in Minutes) to Complete Review"   };
                    return ResponseMessage(_fileDownloadResponse.GetResponse(Request, list, "InvestigatorReviewCompletedTime.csv", headers));
                default:
                    return Ok(list);
            }

        }

        [Route("InvestigatorByFinding")]
        [HttpPost]
        public IHttpActionResult GetInvestigatorsByFinding(ReportFilterViewModel ReportFilter, string mode = "view")
        {
            if (ReportFilter.ToDate != null)
            {
                ReportFilter.ToDate = ReportFilter.ToDate.Date.AddDays(1);
            }

            //return Ok(
            //    _Report.GetInvestigatorByFinding(ReportFilter));

            var list = _Report.GetInvestigatorByFinding(ReportFilter);

            switch (mode)
            {
                case "view":
                    return Ok(list);
                case "csv":

                    //Project Number	Investigator	Role	Review Completed By	Review Completed On	Site Short Name	Findings
                    var headers = new List<string> { "Project Number-1", "Project Number-2", "Investigator", "Role", "Review Completed By", "Review Completed On", "Site Short Name", "Findings" };
                    return ResponseMessage(_fileDownloadResponse.GetResponse(Request, list, "Investigators_By_Findings.csv", headers));
                default:
                    return Ok(list);
            }
        }

        [Route("StudySpecificInvestigators")]
        [HttpPost]
        public IHttpActionResult GetStudySpecificInvestigators(ReportFilterViewModel ReportFilter, string mode = "view")
        {
            if (ReportFilter.ToDate != null)
            {
                ReportFilter.ToDate = ReportFilter.ToDate.Date.AddDays(1);
            }

            //return Ok(
            //    _Report.GetStudySpecificInvestigators(ReportFilter));

            var list = _Report.GetStudySpecificInvestigators(ReportFilter);

            switch (mode)
            {
                case "view":
                    return Ok(list);
                case "csv":

                    var headers = new List<string> { "Project Number-1", "Project Number-2",
                    "Sponsor Protocol Number-1", "Sponsor Protocol Number-2",
                    "Investigator Name", "Role",
                    "Medical License Number", "Institute", "Country",
                    "Review Completed", "Finding Status", "Assigned To" };
            
                    return ResponseMessage(_fileDownloadResponse.GetResponse(Request, list, "Investigators_By_Findings.csv", headers));
                default:
                    return Ok(list);
            }
        }
    }
}