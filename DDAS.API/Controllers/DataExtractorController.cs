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
            return Ok(_ExtractData.GetLatestExtractionStatus());
        }
               
        [Authorize(Roles = "app-admin, admin, user")]
        [Route("GetDataExtractionErrorSiteCount")]
        [HttpGet]
        public IHttpActionResult GetDataExtractionErrorSiteCount()
        {
            return Ok(_ExtractData.GetSitesWhereDataExtractionEarlierThan(32).ToList().Count);
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


        #endregion
    }
}
