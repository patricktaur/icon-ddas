using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DDAS.API.Controllers
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        private IAppAdminService _AppAdminService;
        private string ErrorScreenCaptureFolder;
        private string _RootPath;
        public AdminController(IAppAdminService AppAdmin)
        {
            _AppAdminService = AppAdmin;

            _RootPath = HttpRuntime.AppDomainAppPath;

            ErrorScreenCaptureFolder = _RootPath +
                System.Configuration.ConfigurationManager.AppSettings["ErrorScreenCaptureFolder"];
            //ErrorScreenCaptureFolder = @"DataFiles\ErrorScreenCapture";

        }

        [Route("GetDataExtractionHistory")]
        [HttpGet]
        public IHttpActionResult GetDataExtractionHistory()
        {
            return Ok(_AppAdminService.GetDataExtractionHistory());
        }

        #region Add/Delete sites

        [Route("AddSite")]
        [HttpPost]
        public IHttpActionResult AddSite(SitesToSearch Site)
        {
            _AppAdminService.AddSitesInDbCollection(Site);
            return Ok();
        }
        [Route("GetSiteSources")]
        [HttpGet]
        public IHttpActionResult GetSiteSources()
        {
            return Ok(_AppAdminService.GetAllSiteSources());

        }

        [Route("GetSiteSource")]
        [HttpGet]
        public IHttpActionResult GetSiteSource(string RecId)
        {
            var Id = Guid.Parse(RecId);
            return Ok(_AppAdminService.GetSingleSiteSource(Id));
        }

        [Route("SaveSiteSource")]
        [HttpPost]
        public IHttpActionResult UpdateSiteSource(SitesToSearch SiteSource)
        {
            return Ok(_AppAdminService.UpdateSiteSource(SiteSource));
        }

        [Route("DeleteSiteSource")]
        [HttpGet]
        public IHttpActionResult DeleteSiteSource(string RecId)
        {
            var Id = Guid.Parse(RecId);
            _AppAdminService.DeleteSiteSource(Id);
            return Ok(true);
        }

        #endregion

        #region Add/Delete/Get Country

        [Route("AddCountry")]
        [HttpPost]
        public IHttpActionResult AddCountry(Country country)
        {
            var result = _AppAdminService.AddCountry(country);
            if (result)
                return Ok("Country: " + country.CountryName +
                    " is added successfully");
            else
                return Ok("could not add the Country");
        }

        [Route("GetCountries")]
        [HttpGet]
        public IHttpActionResult GetCountries()
        {
            var Countries = _AppAdminService.GetCountries();
            return Ok(Countries);
        }

        [Route("GetCountry")]
        [HttpGet]
        public IHttpActionResult GetCountry(Guid? RecId)
        {
            var Country = _AppAdminService.GetCountry(RecId);
            return Ok(Country);
        }

        [Route("DeleteCountry")]
        [HttpGet]
        public IHttpActionResult DeleteCountry(string RecId)
        {
            var Id = Guid.Parse(RecId);
            _AppAdminService.DeleteCountry(Id);
            return Ok(true);
        }
        #endregion

        #region Add/Delete/Get SponsorProtocol

        [Route("AddSponsorProtocol")]
        [HttpPost]
        public IHttpActionResult AddSponsor(SponsorProtocol sponsor)
        {
            var result = _AppAdminService.AddSponsor(sponsor);
            if (result)
                return Ok("sponsor: " + sponsor.SponsorProtocolNumber +
                    " added successfully");
            else
                return Ok("could not add sponsor protocol");
        }

        [Route("GetSponsorProtocols")]
        [HttpGet]
        public IHttpActionResult GetSponsorProtocols()
        {
            return Ok(_AppAdminService.GetSponsorProtocols());
        }

        [Route("GetSponsorProtocol")]
        [HttpGet]
        public IHttpActionResult GetSponsorProtocol(string RecId)
        {
            var Id = Guid.Parse(RecId);
            return Ok(_AppAdminService.GetSponsorProtocol(Id));
        }

        [Route("DeleteSponsorProtocol")]
        [HttpGet]
        public IHttpActionResult DeleteSponsorProtocol(string RecId)
        {
            var Id = Guid.Parse(RecId);
            _AppAdminService.DeleteSponsor(Id);
            return Ok(true);
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
            return Ok(_AppAdminService.GetDefaultSites());
        }

        [Route("GetDefaultSite")]
        [HttpGet]
        public IHttpActionResult GetDefaultSite(string RecId)
        {
            var Id = Guid.Parse(RecId);
            var site = _AppAdminService.GetSingleDefaultSite(Id);
            return Ok(site);
        }

        [Route("SaveDefaultSite")]
        [HttpPost]
        public IHttpActionResult UpdateDefaultSource(DefaultSite DefaultSite)
        {
            return Ok(_AppAdminService.UpdateDefaultSite(DefaultSite));
        }

        [Route("DeleteDefaultSite")]
        [HttpGet]
        public IHttpActionResult DeleteDefaultSite(string RecId)
        {
            var Id = Guid.Parse(RecId);
            _AppAdminService.DeleteDefaultSite(Id);
            return Ok(true);
        }
        #endregion
    }
}
