using DDAS.API.Helpers;
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
    [RoutePrefix("api/QC")]
    public class AuditController : ApiController
    {
        private IAudit _Audit;
        public AuditController(IAudit Audit)
        {
            _Audit = Audit;
        }

        [Route("RequestQC")]
        [HttpPost]
        public IHttpActionResult RequestQC(ComplianceForm Form)
        {
            return Ok(_Audit.RequestQC(Form));
        }

        [Route("GetQC")]
        [HttpGet]
        public IHttpActionResult GetAudit(string Id, string AssignedTo)
        {
            var RecId = Guid.Parse(Id);
            var CompForm = _Audit.GetQC(RecId, AssignedTo);
            UpdateFormToCurrentVersion
                .UpdateComplianceFormToCurrentVersion(CompForm);

            return Ok(CompForm);
        }

        [Route("ListQCs")]
        [HttpGet]
        public IHttpActionResult ListQCs()
        {
            return Ok(_Audit.ListQCs());
        }

        [Route("SaveQC")]
        [HttpPost]
        public IHttpActionResult SaveAudit(ComplianceForm Form)
        {
            return Ok(_Audit.SaveQC(Form));
        }
    }
}