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
using System.Runtime.CompilerServices;
using System.Configuration;

namespace DDAS.API.Controllers
{
    //[Authorize(Roles = "user, admin")]
    [RoutePrefix("api/archive")]
    public class ComplianceFormArchiveController : ApiController
    {
        //private ISearchEngine _SearchEngine;
        private ISearchService _SearchService;
        private IComplianceFormArchiveService _compFormArchiveSerive;
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

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //private Stopwatch _stopWatch;
        private string _logMode;
        public ComplianceFormArchiveController(
            IUnitOfWork UOW,
            ISearchService SearchSummary,
            IComplianceFormArchiveService compFormArchiveService,
            IConfig Config)
        {
            _RootPath = HttpRuntime.AppDomainAppPath;
            _UOW = UOW;
            _config = Config;
            _SearchService = SearchSummary;
            _fileDownloadResponse = new FileDownloadResponse();
            _logMode = System.Configuration.ConfigurationManager.AppSettings["LogMode"];
            _compFormArchiveSerive = compFormArchiveService;
        }
       
        //Archive:
        [Route("ArchiveCompFormsWithSearchDaysGreaterThan")]
        [HttpGet]
        public IHttpActionResult ArchiveCompFormsWithSearchDaysGreaterThan(int days)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
               if (days < 180)
                {
                    return Ok("Not Archived. Days cannot be less than 180");
                }

                var retMsg = _compFormArchiveSerive.ArchiveComplianceFormsWithSearchDaysGreaterThan(days);
                return Ok(retMsg);
            }
        }

        string ListToString(ExcelInput excelInput)
        {
            string retValue = "";

            var excelRows = excelInput.ExcelInputRows;
            foreach (ExcelInputRow row in excelRows)
            {
                foreach (string Value in row.ErrorMessages)
                {
                    retValue += Value + "---";
                }
            }
            return retValue;
        }

        string ListToString(List<List<string>> lst)
        {
            string retValue = "";
            foreach (List<string> l in lst)
            {
                foreach (string s in l)
                {
                    retValue += s + "---";
                }
                //retValue += l + "---";
            }
            return retValue;
        }

        //private void LogStart(string callerName)
        //{
        //    _stopWatch.Start();
        //    _logGUID = shortGUID();
        //    Logger.Info(String.Format("{0} | {1} | {2} Start ", callerName, _logGUID, CurrentUser()));

        //}

        //private void LogEnd(string callerName)
        //{
        //    _stopWatch.Stop();
        //    Logger.Info(String.Format("{0} | {1} | Stop | {2} | Elapsed ms: {3}", callerName, _logGUID, CurrentUser(), _stopWatch.ElapsedMilliseconds));

        //}

        //private void LogDebug(string callerName, string exceptionMessage)
        //{
        //    _stopWatch.Stop();
        //    Logger.Debug(String.Format("{0} | {1} | Stop | {2} | Elapsed ms: {3} | ERROR: {4}", callerName, _logGUID, CurrentUser(), _stopWatch.ElapsedMilliseconds, exceptionMessage));

        //}


        //private string shortGUID()
        //{
        //    var guid = Guid.NewGuid();
        //    var base64Guid = Convert.ToBase64String(guid.ToByteArray());

        //    // Replace URL unfriendly characters with better ones
        //    base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

        //    // Remove the trailing ==
        //    return base64Guid.Substring(0, base64Guid.Length - 2);
        //}

        private string CurrentUser()
        {
            return User.Identity.GetUserName();
        }

        private string GetCallerName([CallerMemberName] string caller = null)
        {
            return caller;
        }

    }

    public class UserDetails1
    {
        public string UserName { get; set; }
        public string pwd { get; set; }

        public List<IdentityRole> Role { get; set; }

    }
}
