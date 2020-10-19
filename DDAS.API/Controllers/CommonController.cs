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
    [Authorize]
    [RoutePrefix("api/Common")]
    public class CommonController : ApiController
    {
        private IAppAdminService _AppAdminService;
        private string _RootPath;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _logMode;

        public CommonController(IAppAdminService AppAdmin)
        {
            _AppAdminService = AppAdmin;

            _RootPath = HttpRuntime.AppDomainAppPath;
            _logMode = System.Configuration.ConfigurationManager.AppSettings["LogMode"];

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
