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
            foreach (Review Review in Form.Reviews)
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
                else if (Review.RecId == null)
                    Review.RecId = Guid.NewGuid();
            }

            //var QCReview = Form.Reviews.Find(x => x.ReviewerRole == ReviewerRoleEnum.QCVerifier);

            //Form.Comments.Add(new Comment()
            //{
            //    ReviewId = QCReview.RecId.Value,
            //    ReviewerCategoryEnum = CommentCategoryEnum.NotApplicable,
            //    CategoryEnum = CommentCategoryEnum.Minor
            //});

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

        public bool RequestQC(Guid ComplianceFormId, Review review)
        {

            var form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);
            var lastReview = form.Reviews.LastOrDefault();
            if (lastReview == null)
            {
                throw new Exception("Assigned to Review record expected for Compliance form: " + ComplianceFormId);
            }

            review.RecId = Guid.NewGuid();
            review.PreviousReviewId = lastReview.RecId;
            form.Reviews.Add(review);
            _UOW.ComplianceFormRepository.UpdateCollection(form);

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
                var QCReview = Form.Reviews.LastOrDefault();

                if (QCReview == null)
                    continue;
                else if (QCReview.Status == ReviewStatusEnum.SearchCompleted ||
                    QCReview.Status == ReviewStatusEnum.ReviewInProgress ||
                    QCReview.Status == ReviewStatusEnum.ReviewCompleted ||
                    QCReview.Status == ReviewStatusEnum.Completed)
                    continue;

                var QCViewModel = new QCListViewModel();
                //SetComplianceFormDetails(AuditViewModel, audit.ComplianceFormId);
                QCViewModel.RecId = QCReview.RecId;
                QCViewModel.ComplianceFormId = Form.RecId.Value;
                QCViewModel.PrincipalInvestigator =
                    Form.InvestigatorDetails.FirstOrDefault().Name;
                QCViewModel.ProjectNumber = Form.ProjectNumber;
                QCViewModel.ProjectNumber2 = Form.ProjectNumber2;
                QCViewModel.QCVerifier = Form.QCVerifier;
                QCViewModel.Status = QCReview.Status;
                QCViewModel.CompletedOn = QCReview.CompletedOn;
                QCViewModel.Requestor = Form.Reviewer;
                QCViewModel.RequestedOn = QCReview.AssignedOn;

                AllQCs.Add(QCViewModel);
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

        public ComplianceForm GetQC(Guid ComplianceFormId, string QCAssignedTo, string LoggedInUserName)
        {
            var Form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var Review = Form.Reviews.Find(x =>
            x.AssigendTo.ToLower() == QCAssignedTo.ToLower() &&
            x.AssignedBy.ToLower() == Form.AssignedTo.ToLower() &&
            x.Status == ReviewStatusEnum.QCRequested);

            if (Review != null &&
                Review.StartedOn == null &&
                Review.AssigendTo.ToLower() == LoggedInUserName.ToLower())
            {
                Review.Status = ReviewStatusEnum.QCInProgress;
                Review.StartedOn = DateTime.Now;
                Review.ReviewerRole = ReviewerRoleEnum.QCVerifier;
                _UOW.ComplianceFormRepository.UpdateCollection(Form);
            }

            var QCFailedReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.QCFailed);

            var QCCorrectionReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.QCCorrectionInProgress);

            var CompletedReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.Completed);

            if (QCFailedReview != null &&
                QCCorrectionReview == null &&
                CompletedReview == null &&
                Form.AssignedTo.ToLower() == LoggedInUserName)
            {
                Form.Reviews.Add(new Review()
                {
                    RecId = Guid.NewGuid(),
                    AssigendTo = QCFailedReview.AssignedBy,
                    AssignedBy = QCFailedReview.AssigendTo,
                    StartedOn = DateTime.Now,
                    Status = ReviewStatusEnum.QCCorrectionInProgress,
                    PreviousReviewId = QCFailedReview.RecId
                });
                _UOW.ComplianceFormRepository.UpdateCollection(Form);
            }
            return Form;
        }

        public bool SaveQC(ComplianceForm Form)
        {
            var CurrentQCReview = Form.Reviews.LastOrDefault();

            if (CurrentQCReview == null)
                throw new Exception("Review cannot be empty");

            if (CurrentQCReview.Status == ReviewStatusEnum.QCCorrectionInProgress)
            {
                UpdateReviewStatus(CurrentQCReview,
                    Form.Findings.Where(x => x.IsAnIssue).ToList());

                //CurrentQCReview.Status = ReviewStatusEnum.Completed;
            }
            else if (CurrentQCReview.Status == ReviewStatusEnum.QCPassed)
            {
                CurrentQCReview.Status = ReviewStatusEnum.Completed;
                CurrentQCReview.CompletedOn = DateTime.Now;
            }

            //Save QC is currently equivalent to submitting the QC
            _UOW.ComplianceFormRepository.UpdateCollection(Form);
            return true;
        }

        private void UpdateReviewStatus(Review Review,
            List<Finding> FindingsWithIssues)
        {
            FindingsWithIssues = FindingsWithIssues.Where(x =>
            x.Comments[0].CategoryEnum != CommentCategoryEnum.NotApplicable)
            .ToList();

            var FindingsCorrectedOrAcceptedCount = 0;

            foreach (Finding finding in FindingsWithIssues)
            {
                var comment = finding.Comments.Find(x =>
                x.ReviewerCategoryEnum == CommentCategoryEnum.CorrectionCompleted ||
                x.ReviewerCategoryEnum == CommentCategoryEnum.Accepted);

                if (comment != null)
                    FindingsCorrectedOrAcceptedCount += 1;
            }

            if (FindingsWithIssues.Count == FindingsCorrectedOrAcceptedCount)
            {
                Review.Status = ReviewStatusEnum.Completed;
                Review.CompletedOn = DateTime.Now;
            }
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
                var comment = finding.Comments[0];

                //if(comment.CategoryEnum == CommentCategoryEnum.Accepted)
                //{
                var QCSummary = new QCSummaryViewModel();
                QCSummary.Investigator =
                    finding.InvestigatorName;
                QCSummary.SourceName =
                    Form.SiteSources.Find(x =>
                    x.Id == finding.SiteSourceId).SiteShortName;
                QCSummary.CategoryEnumString =
                    GetCategoryEnumString(comment.CategoryEnum);
                QCSummary.FindingId = finding.Id; //Patrick 14Jan2017
                if (finding.ReviewId == QCReview.RecId)
                {
                    QCSummary.Type = "Finding";
                    QCSummary.Comment = finding.Observation + " " +
                        comment.FindingComment;
                    QCSummary.ResponseToQC =
                        GetCategoryEnumString(comment.ReviewerCategoryEnum);
                    QCSummaryList.Add(QCSummary);
                }
                else
                {
                    QCSummary.Type = "Comment";
                    QCSummary.Comment = comment.FindingComment;
                    QCSummary.ResponseToQC =
                        GetCategoryEnumString(comment.ReviewerCategoryEnum);
                    QCSummaryList.Add(QCSummary);
                }
                //}
            }
            return QCSummaryList;
        }

        public bool UndoQCRequest(Guid ComplianceFormId)
        {
            var Form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (Form == null)
                throw new Exception("Could not find compliance form");

            var QCRequestedReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.QCRequested);

            if (QCRequestedReview != null)
            {
                Form.Reviews.Remove(QCRequestedReview);
                _UOW.ComplianceFormRepository.UpdateCollection(Form);
                return true;
            }
            else
                return false;
        }

        private bool UndoQCSubmit(Guid ComplianceFormId)
        {
            var Form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (Form == null)
                throw new Exception("Could not find compliance form");

            var QCFailedOrPassedReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.QCFailed ||
                x.Status == ReviewStatusEnum.QCPassed ||
                x.Status == ReviewStatusEnum.Completed);

            if (QCFailedOrPassedReview != null)
            {
                QCFailedOrPassedReview.Status = ReviewStatusEnum.QCInProgress;
                _UOW.ComplianceFormRepository.UpdateCollection(Form);
                return true;
            }
            else
                return false;
        }

        //public bool Undo(Guid ComplianceFormId, UndoEnum undoEnum)
        //{
        //    switch (undoEnum)
        //    {
        //        case UndoEnum.UndoQCRequest:
        //            return UndoQCRequest(ComplianceFormId);
        //        case UndoEnum.UndoQCSubmit:
        //            return UndoQCSubmit(ComplianceFormId);
        //        case UndoEnum.UndoQCResponse:
        //            return false;
        //        default: throw new Exception("invalid UndoEnum");
        //    }
        //}
        
        private bool UndoQCResponse()
        {
            return true;
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
                case CommentCategoryEnum.CorrectionPending: return "Correction Pending";
                case CommentCategoryEnum.CorrectionCompleted: return "Correction Completed";
                case CommentCategoryEnum.Accepted: return "Accepted";
                case CommentCategoryEnum.NotApplicable: return "Not Applicable";
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
