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

            var review = Form.Reviews.Find(x => x.ReviewerRole == ReviewerRoleEnum.QCVerifier);

            Form.Comments.Add(new Comment()
            {
                ReviewId = review.RecId.Value
            });

            //foreach(Finding finding in Form.Findings)
            //{
            //    if(finding.Comments.Count == 0)
            //    {
            //        var Comment = new Comment();
            //        var Review =
            //            Form.Reviews.Find(x =>
            //            x.ReviewerRole == ReviewerRoleEnum.Reviewer);
            //        Comment.ReviewId = Review.RecId.Value;
            //        finding.Comments.Add(Comment);

            //        var Comment1 = new Comment();
            //        var Review1 =
            //            Form.Reviews.Find(x =>
            //            x.ReviewerRole == ReviewerRoleEnum.QCVerifier);
            //        Comment1.ReviewId = Review1.RecId.Value;
            //        finding.Comments.Add(Comment1);
            //    }
            //}

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
            //Save QC is currently equivalent to submitting the QC
            _UOW.ComplianceFormRepository.UpdateCollection(Form);
            return true;
        }

        public List<QCSummaryViewModel> ListQCSummary(Guid ComplianceFormId)
        {
            var Form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (Form == null)
                throw new Exception("No compliance forms found!");

            var QCReview = Form.Reviews.Find(x =>
            x.ReviewerRole == ReviewerRoleEnum.QCVerifier);

            var QCSummaryList = new List<QCSummaryViewModel>();

            foreach (Finding finding in Form.Findings.Where(x => x.IsAnIssue))
            {
                foreach(Comment comment in finding.Comments)
                {
                    if(comment.FindingComment != null || 
                        comment.CategoryEnum == CommentCategoryEnum.Accepted)
                    {
                        var QCSummary = new QCSummaryViewModel();
                        QCSummary.Investigator =
                            finding.InvestigatorName;
                        QCSummary.SourceName =
                            Form.SiteSources.Find(x =>
                            x.Id == finding.SiteSourceId).SiteShortName;
                        QCSummary.CategoryEnumString = 
                            GetCategoryEnumString(comment.CategoryEnum);

                        if (finding.ReviewId == QCReview.RecId)
                            QCSummary.Comment = finding.Observation + " " +
                                comment.FindingComment;
                        else
                            QCSummary.Comment = comment.FindingComment;

                        QCSummaryList.Add(QCSummary);
                    }
                }
            }
            return QCSummaryList;
        }

        private string GetCategoryEnumString(CommentCategoryEnum Enum)
        {
            switch (Enum)
            {
                case CommentCategoryEnum.Minor: return "Minor";
                case CommentCategoryEnum.Major: return "Major";
                case CommentCategoryEnum.Critical: return "Critical";
                case CommentCategoryEnum.Suggestion: return "Suggestion";
                case CommentCategoryEnum.Others: return "Others";
                default: throw new Exception("Invalid CommentCategoryEnum");
            }
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
