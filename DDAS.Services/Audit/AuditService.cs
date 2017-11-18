using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models;
using DDAS.Models.ViewModels;

namespace DDAS.Services.AuditService
{
    public class AuditService : IAudit
    {
        private IUnitOfWork _UOW;
        private const string Accepted = "Accepted";
        private const string Rejected = "Rejected";

        public AuditService(IUnitOfWork UOW)
        {
            _UOW = UOW;
        }

        public bool RequestAudit(Audit Audit)
        {
            var ComplianceForm = 
                _UOW.ComplianceFormRepository.FindById(Audit.ComplianceFormId);

            foreach(SiteSource Site in ComplianceForm.SiteSources)
            {
                var AuditObservation = new AuditObservation();
                AuditObservation.SiteShortName = Site.SiteShortName;
                AuditObservation.SiteId = Site.Id;
                Audit.Observations.Add(AuditObservation);
            }
            _UOW.AuditRepository.Add(Audit);
            return true;
        }

        public List<AuditListViewModel> ListAudits()
        {
            var Audits = _UOW.AuditRepository.GetAll()
            .OrderBy(x => x.RequestedOn)
            .ToList();

            if (Audits.Count == 0)
                return null;

            var AllAudits = new List<AuditListViewModel>();

            foreach(Audit audit in Audits)
            {
                var AuditViewModel = new AuditListViewModel();
                SetComplianceFormDetails(AuditViewModel, audit.ComplianceFormId);
                AuditViewModel.RecId = audit.RecId.Value;
                AuditViewModel.ComplianceFormId = audit.ComplianceFormId;
                AuditViewModel.Auditor = audit.Auditor;
                AuditViewModel.AuditStatus = audit.AuditStatus;
                AuditViewModel.CompletedOn = audit.CompletedOn;
                AuditViewModel.RequestedBy = audit.RequestedBy;
                AuditViewModel.RequestedOn = audit.RequestedOn;
                AuditViewModel.AuditStatus = audit.AuditStatus;
                AuditViewModel.CompletedOn = audit.CompletedOn;

                AllAudits.Add(AuditViewModel);
            }
            return AllAudits;
        }

        private void SetComplianceFormDetails(AuditListViewModel AuditViewModel, Guid ComplianceFormId)
        {
            var ComplianceForm = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            AuditViewModel.PrincipalInvestigator =
                ComplianceForm.InvestigatorDetails.FirstOrDefault().Name;

            AuditViewModel.ProjectNumber = ComplianceForm.ProjectNumber;
        }

        public Audit GetAudit(Guid RecId)
        {
            var audit = _UOW.AuditRepository.FindById(RecId);
            return audit;
        }

        public bool SaveAudit(Audit audit)
        {
            _UOW.AuditRepository.UpdateAudit(audit);
            return true;
        }
    }
}
