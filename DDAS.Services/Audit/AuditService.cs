using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models;
using DDAS.Models.ViewModels;
using Utilities.EMail;

namespace DDAS.Services.AuditService
{
    public class AuditService : IAudit
    {
        private IUnitOfWork _UOW;
        private IEMailService _EMailService;

        private const string Accepted = "Accepted";
        private const string Rejected = "Rejected";

        public AuditService(IUnitOfWork UOW, IEMailService EmailService)
        {
            _UOW = UOW;
            _EMailService = EmailService;
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
            SendAuditRequestedMail(Audit.Auditor, Audit.RequestedBy);
            return true;
        }

        private void SendAuditRequestedMail(string SendMailTo, string AuditRequestedBy)
        {
            var User = _UOW.UserRepository.GetAll()
                .Find(x => x.UserName.ToLower() == SendMailTo.ToLower());

            if (User == null)
                throw new Exception("invalid username");

            var UserEMail = User.EmailId;
            var Subject = "DDAS Audit Request";
            var MailBody = "Dear " + User.UserName + ",<br/><br/> ";
            MailBody += AuditRequestedBy + " has requested you to audit a compliance form. <br/><br/>";
            MailBody += "Please login to DDAS application and navigate to \"QC Audits\" to start the audit. <br/><br/>";
            MailBody += "Yours Sincerely,<br/>";
            MailBody += "DDAS Team";

            SendMail(UserEMail, Subject, MailBody);
        }

        private void SendAuditCompletedMail(string SendMailTo, string AuditCompletedBy)
        {
            var User = _UOW.UserRepository.GetAll()
                .Find(x => x.UserName.ToLower() == SendMailTo.ToLower());

            if (User == null)
                throw new Exception("invalid username");

            var UserEMail = User.EmailId;
            var Subject = "DDAS Audit Response";
            var MailBody = "Dear " + User.UserName + ",<br/><br/> ";
            MailBody += "Your audit request has been completed by " + AuditCompletedBy + "<br/>";
            MailBody += "Please login to DDAS application to view the results.<br/><br/>";
            MailBody += "Yours Sincerely,<br/>";
            MailBody += "DDAS Team";

            SendMail(UserEMail, Subject, MailBody);
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

            if (audit.IsSubmitted)
                SendAuditCompletedMail(audit.RequestedBy, audit.Auditor);

            return true;
        }

        private void SendMail(string To, string Subject, string Body)
        {
            var EMail = new EMailModel();
            EMail.To.Add(To);
            EMail.Subject = Subject;
            EMail.Body = Body;
            _EMailService.SendMail(EMail);
        }
    }
}
