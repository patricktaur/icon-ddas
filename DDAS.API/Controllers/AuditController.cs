using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DDAS.API.Controllers
{
    [Authorize(Roles = "user, admin")]
    [RoutePrefix("api/Audit")]
    public class AuditController : ApiController
    {
        private IAudit _Audit;
        public AuditController(IAudit Audit)
        {
            _Audit = Audit;
        }

        [Route("RequestAudit")]
        [HttpPost]
        public IHttpActionResult RequestAudit(Audit Audit)
        {
            return Ok(_Audit.RequestAudit(Audit));
        }

        [Route("GetAudit")]
        [HttpGet]
        public IHttpActionResult GetAudit(string Id)
        {
            var AuditId = Guid.Parse(Id);
            return Ok(_Audit.GetAudit(AuditId));
        }

        [Route("ListAudits")]
        [HttpGet]
        public IHttpActionResult ListAudits()
        {
            return Ok(_Audit.ListAudits());
        }

        [Route("SaveAudit")]
        [HttpPost]
        public IHttpActionResult SaveAudit(Audit audit)
        {
            return Ok(_Audit.SaveAudit(audit));
        }
    }
}