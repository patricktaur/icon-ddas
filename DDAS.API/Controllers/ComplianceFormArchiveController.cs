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
        private IComplianceFormArchiveService _compFormArchiveService;
        private IUnitOfWork _UOW;
        //private ILog _log;
        private IConfig _config;
        private FileDownloadResponse _fileDownloadResponse;

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
            _compFormArchiveService = compFormArchiveService;
        }
        #region Queries
        
        [Route("ComplianceFormWithReviewDateFilters")]
        [HttpPost]
        public IHttpActionResult GetComplianceFormWithReviewDateFilter(ComplianceFormArchiveFilter CompFormFilter)
        {
            try
            {
                using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
                {
                    var result = _compFormArchiveService.GetComplianceFormsFromFiltersWithReviewDates(CompFormFilter);
                    return Ok(result);
                }
            }
            catch (Exception Ex)
            {

                return Ok(Ex.ToString());
            }
        }
        #endregion
        #region Archive
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

                var retMsg = _compFormArchiveService.ArchiveComplianceFormsWithSearchDaysGreaterThan(days);
                return Ok(retMsg);
            }
        }
        #endregion


        private string CurrentUser()
        {
            return User.Identity.GetUserName();
        }

        private string GetCallerName([CallerMemberName] string caller = null)
        {
            return caller;
        }

    }
}
