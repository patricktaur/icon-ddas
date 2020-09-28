using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Runtime.CompilerServices;
using Microsoft.AspNet.Identity;
using DDAS.API.Helpers;
namespace DDAS.API.Controllers
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        private IAppAdminService _AppAdminService;
        private string ErrorScreenCaptureFolder;
        private string _RootPath;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _logMode;

        public AdminController(IAppAdminService AppAdmin)
        {
            _AppAdminService = AppAdmin;

            _RootPath = HttpRuntime.AppDomainAppPath;

            ErrorScreenCaptureFolder = _RootPath +
                System.Configuration.ConfigurationManager.AppSettings["ErrorScreenCaptureFolder"];
            //ErrorScreenCaptureFolder = @"DataFiles\ErrorScreenCapture";
            _logMode = System.Configuration.ConfigurationManager.AppSettings["LogMode"];

        }



        //[Route("GetDataExtractionHistory")]
        //[HttpGet]
        //public IHttpActionResult GetDataExtractionHistory()
        //{
        //    return Ok(_AppAdminService.GetDataExtractionHistory());
        //}

        //[Route("GetDataExtractionErrorSiteCount")]
        //[HttpGet]
        //public IHttpActionResult GetDataExtractionErrorSiteCount()
        //{
        //    return Ok(_AppAdminService.GetSitesWhereDataExtractionEarlierThan(32).ToList().Count);
        //}

        [Route("GetExtractionLog")]
        [HttpGet]
        public IHttpActionResult GetExtractionLog()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.GetExtractionLog());
            }
        }

        #region Add/Delete sites

        [Route("AddSite")]
        [HttpPost]
        public IHttpActionResult AddSite(SitesToSearch Site)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                _AppAdminService.AddSitesInDbCollection(Site);
                return Ok();
            }
        }

        [Route("GetSiteSources")]
        [HttpGet]
        public IHttpActionResult GetSiteSources()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.GetAllSiteSources());
            }
        }

        [Route("GetSiteSource")]
        [HttpGet]
        public IHttpActionResult GetSiteSource(string RecId)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Id = Guid.Parse(RecId);
                return Ok(_AppAdminService.GetSingleSiteSource(Id));
            }
        }

        [Route("SaveSiteSource")]
        [HttpPost]
        public IHttpActionResult UpdateSiteSource(SitesToSearch SiteSource)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.UpdateSiteSource(SiteSource));
            }
        }

        [Route("DeleteSiteSource")]
        [HttpGet]
        public IHttpActionResult DeleteSiteSource(string RecId)
        {
            try
            {
                using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
                {
                    var Id = Guid.Parse(RecId);
                    _AppAdminService.DeleteSiteSource(Id);
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Add/Delete/Get Country

        [Route("AddCountry")]
        [HttpPost]
        public IHttpActionResult AddCountry(Country country)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var result = _AppAdminService.AddCountry(country);
                if (result)
                    return Ok("Country: " + country.CountryName +
                        " is added successfully");
                else
                    return Ok("could not add the Country");
            }
        }

        [Route("GetCountries")]
        [HttpGet]
        public IHttpActionResult GetCountries()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Countries = _AppAdminService.GetCountries();
                return Ok(Countries);
            }
        }

        [Route("GetCountry")]
        [HttpGet]
        public IHttpActionResult GetCountry(Guid? RecId)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Country = _AppAdminService.GetCountry(RecId);
                return Ok(Country);
            }
        }

        [Route("DeleteCountry")]
        [HttpGet]
        public IHttpActionResult DeleteCountry(string RecId)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Id = Guid.Parse(RecId);
                _AppAdminService.DeleteCountry(Id);
                return Ok(true);
            }
        }
        #endregion

        #region Add/Delete/Get SponsorProtocol

        [Route("AddSponsorProtocol")]
        [HttpPost]
        public IHttpActionResult AddSponsor(SponsorProtocol sponsor)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var result = _AppAdminService.AddSponsor(sponsor);
                return Ok(result);
                //if (result)
                //    return Ok("sponsor: " + sponsor.SponsorProtocolNumber +
                //        " added successfully");
                //else
                //    return Ok("could not add sponsor protocol");
            }
        }

        [Route("GetSponsorProtocols")]
        [HttpGet]
        public IHttpActionResult GetSponsorProtocols()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.GetSponsorProtocols());
            }
        }

        [Route("GetSponsorProtocol")]
        [HttpGet]
        public IHttpActionResult GetSponsorProtocol(string RecId)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Id = Guid.Parse(RecId);
                return Ok(_AppAdminService.GetSponsorProtocol(Id));
            }
        }

        [Route("DeleteSponsorProtocol")]
        [HttpGet]
        public IHttpActionResult DeleteSponsorProtocol(string RecId)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Id = Guid.Parse(RecId);
                _AppAdminService.DeleteSponsor(Id);
                return Ok(true);
            }
        }
        #endregion

        #region Add/Delete/Get DefaultSite
        //[Route("AddDefaultSite")]
        //[HttpPost]
        //public IHttpActionResult AddDefaultSite(DefaultSite DefaultSite)
        //{

        //    return Ok(_AppAdminService.AddDefaultSite(DefaultSite));
        //}

        [Route("GetDefaultSites")]
        [HttpGet]
        public IHttpActionResult GetDefaultSites()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.GetDefaultSites());
            }
        }

        [Route("GetDefaultSite")]
        [HttpGet]
        public IHttpActionResult GetDefaultSite(string RecId)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Id = Guid.Parse(RecId);
                var site = _AppAdminService.GetSingleDefaultSite(Id);
                return Ok(site);
            }
        }

        [Route("SaveDefaultSite")]
        [HttpPost]
        public IHttpActionResult SaveOrUpdateDefaultSource(DefaultSite DefaultSite)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Status = _AppAdminService.UpdateDefaultSite(DefaultSite);
                return Ok(Status);
            }
        }

        [Route("DeleteDefaultSite")]
        [HttpGet]
        public IHttpActionResult DeleteDefaultSite(string RecId)
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                var Id = Guid.Parse(RecId);
                _AppAdminService.DeleteDefaultSite(Id);
                return Ok(true);
            }
        }
        #endregion

        #region GetExceptionLogs

        [Route("GetExceptionLogs")]
        [HttpGet]
        public IHttpActionResult GetExceptionLogs()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.GetExceptionLogs());
            }
        }

        #endregion

        #region iSprint to DDAS Log

        [Route("iSprintToDDASLog")]
        [HttpGet]
        public IHttpActionResult GetiSprintToDDASLog()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.GetiSprintToDDASLog());
            }
        }
        #endregion

        [Route("DDAStoiSprintLog")]
        [HttpGet]
        public IHttpActionResult GetDDAStoiSprintLog()
        {
            using (new TimeMeasurementBlock(Logger, _logMode, CurrentUser(), GetCallerName()))
            {
                return Ok(_AppAdminService.GetDDtoiSprintLog());
            }
        }

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
