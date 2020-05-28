using DDAS.Models;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


using Microsoft.AspNet.Identity;
using System.Web;
using DDAS.Data.Mongo;

namespace DDAS.API.Controllers
{

    //[Authorize(Roles = "admin")]
    [Authorize(Roles = "user, admin, app-admin")]
    [RoutePrefix("api/DataExtractor")]
    public class DataExtractorController : ApiController
    {
        ISearchEngine _SearchEngine;
        private IUnitOfWork _UOW;
        private IDataExtractorService _ExtractData;
        private ILog _log;
        private string _RootPath;
        public DataExtractorController(IDataExtractorService ExtractData, IUnitOfWork UOW)
        {
            _UOW = UOW;
            _ExtractData = ExtractData;
            _RootPath = HttpRuntime.AppDomainAppPath;
            _log = new DBLog(_UOW, "DataExtractorController", true);
        }

        #region DataExtraction
        [Authorize(Roles = "app-admin, admin")]
        [Route("ExtractDataFromSingleSite")]
        [HttpGet]
        public IHttpActionResult ExtractDataFromSingleSite(SiteEnum siteEnum)
        {
            try
            {
                //var userName = User.Identity.GetUserName();
                //_ExtractData.ExtractDataSingleSite(siteEnum, userName);
                var ExtractorExePath = _RootPath + @"bin\DDAS.DataExtractor.exe";

                int SiteNumber = (int)siteEnum;

                //_ExtractData.ExtractThruShell(SiteNumber, ExtractorExePath);
                _ExtractData.ExtractDataSingleSite(siteEnum, _log);

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Error: " + ex.Message);
            }
        }

        #endregion
        [Authorize(Roles = "app-admin, admin")]
        [Route("GetLatestExtractionStatus")]
        [HttpGet]
        public IHttpActionResult GetLatestExtractionStatus()
        {
            var fromDate = DateTime.Now.AddMonths(-9).Date;
            var toDate = DateTime.Now.AddDays(1).Date;
            return Ok(_ExtractData.GetLatestExtractionStatus(fromDate, toDate));
        }

        [Authorize(Roles = "app-admin, admin, user")]
        [Route("GetDataExtractionErrorSiteCount")]
        [HttpGet]
        public IHttpActionResult GetDataExtractionErrorSiteCount()
        {
            var count = _ExtractData.GetSitesWhereDataExtractionEarlierThan(32).ToList().Count;
            return Ok(count);
        }

        #region Download Data Files
        [Route("DownloadDataFiles")]
        [HttpGet]
        public IHttpActionResult GetDownloadedDataFiles(int SiteEnum)
        {
            var DataFiles = _ExtractData.GetDataFiles(SiteEnum);

            DataFiles.ForEach(DataFile =>
            {
                DataFile.FullPath =
                DataFile.FullPath.Replace(_RootPath, "");
            });
            return Ok(DataFiles);
        }
        #endregion

        #region getExtractedData

        [Route("GetFDAWarningLetterSiteData")]
        [HttpGet]
        public IHttpActionResult GetFDAWarningLetterSiteData()
        {
            return Ok(_ExtractData.GetFDAWarningLetterSiteData());
        }


        [Route("GetFDADebarPageSiteData")]
        [HttpGet]
        public IHttpActionResult GetFDADebarPageSiteData()
        {
            return Ok(_ExtractData.GetFDADebarPageSiteData());
        }

        [Route("GetERRProposalToDebarPageSiteData")]
        [HttpGet]
        public IHttpActionResult GetERRProposalToDebarPageSiteData()
        {
            return Ok(_ExtractData.GetERRProposalToDebarPageSiteData());
        }


        [Route("GetAdequateAssuranceListSiteData")]
        [HttpGet]
        public IHttpActionResult GetAdequateAssuranceListSiteData()
        {
            return Ok(_ExtractData.GetAdequateAssuranceListSiteData());
        }

        [Route("GetClinicalInvestigatorDisqualificationSiteData")]
        [HttpGet]
        public IHttpActionResult GetClinicalInvestigatorDisqualificationSiteData()
        {
            return Ok(_ExtractData.GetClinicalInvestigatorDisqualificationSiteData());
        }

        [Route("GetPHSAdministrativeActionListingSiteData")]
        [HttpGet]
        public IHttpActionResult GetPHSAdministrativeActionListingSiteData()
        {
            return Ok(_ExtractData.GetPHSAdministrativeActionListingSiteData());
        }

        [Route("GetCBERClinicalInvestigatorInspectionSiteData")]
        [HttpGet]
        public IHttpActionResult GetCBERClinicalInvestigatorInspectionSiteData()
        {
            return Ok(_ExtractData.GetCBERClinicalInvestigatorInspectionSiteData());
        }

        [Route("GetCorporateIntegrityAgreementListSiteData")]
        [HttpGet]
        public IHttpActionResult GetCorporateIntegrityAgreementListSiteData()
        {
            return Ok(_ExtractData.GetCorporateIntegrityAgreementListSiteData());
        }


        [Route("GetClinicalInvestigatorInspectionCDERSiteData")]
        [HttpGet]
        public IHttpActionResult GetClinicalInvestigatorInspectionCDERSiteData()
        {
            return Ok(_ExtractData.GetClinicalInvestigatorInspectionSiteData());
        }

        [Route("GetExclusionDatabaseSearchPageSiteData")]
        [HttpGet]
        public IHttpActionResult GetExclusionDatabaseSearchPageSiteData()
        {
            return Ok(_ExtractData.GetExclusionDatabaseSearchPageSiteData());
        }
        [Route("GetSpeciallyDesignatedNationalsSiteData")]
        [HttpGet]
        public IHttpActionResult GetSpeciallyDesignatedNationalsSiteData()
        {
            return Ok(_ExtractData.GetSpeciallyDesignatedNationalsSiteData());
        }

        [Route("GetSystemForAwardManagementPageSiteData")]
        [HttpGet]
        public IHttpActionResult GetSystemForAwardManagementPageSiteData()
        {
            return Ok(_ExtractData.GetSystemForAwardManagementPageSiteData());
        }


        #endregion
    }
}
