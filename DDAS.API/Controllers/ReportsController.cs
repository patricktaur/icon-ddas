using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Utilities;

namespace DDAS.API.Controllers
{
    [Authorize(Roles = "user, admin")]
    [RoutePrefix("api/Reports")]
    public class ReportsController : ApiController
    {
        private ISearchService _SearchSummary;
        private IUnitOfWork _UOW;
        private IConfig _config;
        private string RootPath;

        public ReportsController(ISearchService SearchSummary, IUnitOfWork UOW, IConfig Config)
        {
            RootPath = HttpRuntime.AppDomainAppPath;
            _SearchSummary = SearchSummary;
            _UOW = UOW;
            _config = Config;
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

        [Route("GenerateOutputFile")]
        [HttpPost]
        public IHttpActionResult GenerateOutputFile(ComplianceFormFilter CompFormFilter)
        {
            var GenerateOutputFile =
                new GenerateOutputFile(
                    _config.ExcelTempateFolder + "Output_File.xlsx");

            var fromDate = CompFormFilter.SearchedOnFrom.Value;

            var allForms = _UOW.ComplianceFormRepository.GetAll();

            var forms = allForms
                .Where(x => (x.StatusEnum == Models.Enums.ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified
                || x.StatusEnum == Models.Enums.ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified))
                .OrderBy(x => x.SearchStartedOn).ToList();

            var forms1 = forms;

            if (CompFormFilter.SearchedOnFrom != null)
            {
                DateTime startDate;
                startDate = CompFormFilter.SearchedOnFrom.Value.Date;
                forms1 = forms.Where(x =>
               x.SearchStartedOn >= startDate)
               .ToList();
            }

            var forms2 = forms1;

            if (CompFormFilter.SearchedOnTo != null)
            {

                DateTime endDate;
                endDate = CompFormFilter.SearchedOnTo.Value.Date.AddDays(1);
                forms2 = forms1.Where(x =>
                x.SearchStartedOn <
                endDate)
                .ToList();
            }

            var FilePath = _SearchSummary.GenerateOutputFile(
                GenerateOutputFile,
                forms2,
                _config);

            var Path = FilePath.Replace(RootPath, "");

            return Ok(Path);
        }
    }
}