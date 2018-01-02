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
using DDAS.Models.Enums;

namespace DDAS.Services.AuditService
{
    public class AuditService : IAudit
    {
        private IUnitOfWork _UOW;
        private IEMailService _EMailService;

        public AuditService(IUnitOfWork UOW, IEMailService EmailService)
        {
            _UOW = UOW;
            _EMailService = EmailService;
        }

        public bool RequestQC(ComplianceForm Form)
        {
            foreach(Review Review in Form.Reviews)
            {
                if (Review.RecId == null && 
                    Review.Status == ReviewStatusEnum.QCRequested &&
                    Review.PreviousReviewId == null)
                {
                    Review.RecId = Guid.NewGuid();

                    var PreviousReview = Form.Reviews.Find(x =>
                    x.AssigendTo.ToLower() == Review.AssignedBy.ToLower());

                    Review.PreviousReviewId = PreviousReview.RecId;
                }
                else if(Review.RecId == null)
                    Review.RecId = Guid.NewGuid();
            }

            _UOW.ComplianceFormRepository.UpdateCollection(Form);
            //SendAuditRequestedMail(Audit.Auditor, Audit.RequestedBy);
            return true;
        }

        public List<QCListViewModel> ListQCs()
        {
            var Forms = _UOW.ComplianceFormRepository.GetAll();

            if (Forms.Count == 0)
                return null;

            Forms = Forms.Where(x => 
                x.IsReviewCompleted == true)
                .ToList();

            var AllQCs = new List<QCListViewModel>();

            foreach (ComplianceForm Form in Forms)
            {
                var Reviews = Form.Reviews.Where(x =>
                x.Status == ReviewStatusEnum.QCRequested ||
                x.Status == ReviewStatusEnum.QCInProgress ||
                x.Status == ReviewStatusEnum.QCFailed ||
                x.Status == ReviewStatusEnum.QCPassed)
                .ToList();

                foreach (Review Review in Reviews)
                {
                    var QCViewModel = new QCListViewModel();
                    //SetComplianceFormDetails(AuditViewModel, audit.ComplianceFormId);
                    QCViewModel.RecId = Review.RecId;
                    QCViewModel.ComplianceFormId = Form.RecId.Value;
                    QCViewModel.PrincipalInvestigator =
                        Form.InvestigatorDetails.FirstOrDefault().Name;
                    QCViewModel.ProjectNumber = Form.ProjectNumber;
                    QCViewModel.ProjectNumber2 = Form.ProjectNumber2;
                    QCViewModel.QCVerifier = Review.AssigendTo;
                    QCViewModel.Status = Review.Status;
                    QCViewModel.CompletedOn = Review.CompletedOn;
                    QCViewModel.Requestor = Review.AssignedBy;
                    QCViewModel.RequestedOn = Review.AssignedOn;

                    AllQCs.Add(QCViewModel);
                }
            }
            return AllQCs;
        }

        private void SetComplianceFormDetails(QCListViewModel AuditViewModel, Guid ComplianceFormId)
        {
            var ComplianceForm = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            AuditViewModel.PrincipalInvestigator =
                ComplianceForm.InvestigatorDetails.FirstOrDefault().Name;

            AuditViewModel.ProjectNumber = ComplianceForm.ProjectNumber;
            AuditViewModel.ProjectNumber2 = ComplianceForm.ProjectNumber2;
        }

        public ComplianceForm GetQC(Guid ComplianceFormId, string AssignedTo)
        {
            var Form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var Review = Form.Reviews.Find(x =>
            x.AssigendTo.ToLower() == AssignedTo.ToLower() &&
            x.AssignedBy.ToLower() == Form.AssignedTo.ToLower() &&
            x.Status == ReviewStatusEnum.QCRequested);

            if (Review != null && Review.StartedOn == null)
            {
                Review.Status = ReviewStatusEnum.QCInProgress;
                Review.StartedOn = DateTime.Now;
                Review.ReviewerRole = ReviewerRoleEnum.QCVerifier;
                _UOW.ComplianceFormRepository.UpdateCollection(Form);
            }
            return Form;
        }

        public bool SaveQC(ComplianceForm Form)
        {
            throw new NotImplementedException();
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
