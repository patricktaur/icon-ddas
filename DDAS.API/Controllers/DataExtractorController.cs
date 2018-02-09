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

namespace DDAS.API.Controllers
{
    //[Authorize(Roles = "admin")]
    [RoutePrefix("api/DataExtractor")]
    public class DataExtractorController : ApiController
    {
        ISearchEngine _SearchEngine;
        private IUnitOfWork _UOW;
        private IExtractData _ExtractData;
        public DataExtractorController(IExtractData ExtractData)
        {
            _ExtractData = ExtractData;
        }

        #region DataExtraction
        [Authorize(Roles = "app-admin, admin")]
        [Route("ExtractDataFromSingleSite")]
        [HttpGet]
        public IHttpActionResult ExtractDataFromSingleSite(SiteEnum siteEnum)
        {
            try
            {
                var userName = User.Identity.GetUserName();
                _ExtractData.ExtractDataSingleSite(siteEnum, userName);

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Error");
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

    }
}
