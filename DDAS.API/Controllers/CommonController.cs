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
    [Authorize]
    [RoutePrefix("api/Common")]
    public class CommonController : ApiController
    {
        private IAppAdminService _AppAdminService;
        private string ErrorScreenCaptureFolder;
        private string _RootPath;
        public CommonController(IAppAdminService AppAdmin)
        {
            _AppAdminService = AppAdmin;

            _RootPath = HttpRuntime.AppDomainAppPath;
        }

        [Route("GetSiteSources")]
        [HttpGet]
        public IHttpActionResult GetSiteSources()
        {
            return Ok(_AppAdminService.GetAllSiteSources());
        }
    }
}
