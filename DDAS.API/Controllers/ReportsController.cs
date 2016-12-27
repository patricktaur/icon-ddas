using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DDAS.API.Controllers
{
    [RoutePrefix("api/Reports")]
    public class ReportsController : ApiController
    {
        private ISearchService _SearchSummary;
        private IUnitOfWork _UOW;

        public ReportsController(ISearchService SearchSummary, IUnitOfWork UOW)
        {
            _SearchSummary = SearchSummary;
            _UOW = UOW;
        }

        [Route("GetNamesFromClosedComplianceForms")]
        [HttpGet]
        public List<ComplianceForm> GetNamesFromClosedComplianceForm()
        {
            var ComplianceForms =
                _UOW.ComplianceFormRepository.FindActiveComplianceForms(false);

            foreach (ComplianceForm form in ComplianceForms)
            {
                foreach (InvestigatorSearched Investigator in form.InvestigatorDetails)
                    Investigator.SiteDetails = null;
            }

            return ComplianceForms;
        }

        [Route("ActivateComplianceForm")]
        [HttpGet]
        public IHttpActionResult CloseComplianceForm(string ComplianceFormId)
        {
            Guid? RecId = Guid.Parse(ComplianceFormId);
            ComplianceForm form = _UOW.ComplianceFormRepository.FindById(RecId);

            form.Active = true;
            _UOW.ComplianceFormRepository.UpdateCollection(form);

            return Ok(true);
        }

        [Route("DeleteComplianceForm")]
        [HttpGet]
        public IHttpActionResult DeleteComplianceForm(string ComplianceFormId)
        {
            Guid? RecId = Guid.Parse(ComplianceFormId);
            _UOW.ComplianceFormRepository.DropComplianceForm(RecId);
            var Forms = GetNamesFromClosedComplianceForm();
            return Ok(Forms);
        }
    }
}