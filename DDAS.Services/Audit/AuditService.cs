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
            _UOW.ComplianceFormRepository.UpdateCollection(Form);
            SendQCRequestedMail(Form, "");
            return true;
        }

        public bool RequestQC(Guid ComplianceFormId, Review review, string URL)
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
            form.QCStatus = QCCompletedStatusEnum.InProgress;
            _UOW.ComplianceFormRepository.UpdateCollection(form);
            SendQCRequestedMail(form, URL);
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

                foreach(InvestigatorSearched Investigator in 
                    Form.InvestigatorDetails.Where(x => x.Role.ToLower() == "sub i"))
                {
                    QCViewModel.SubInvestigators.Add(Investigator.Name);
                }

                QCViewModel.PrincipalInvestigator =
                    Form.InvestigatorDetails.First().Name;
                QCViewModel.ProjectNumber = Form.ProjectNumber;
                QCViewModel.ProjectNumber2 = Form.ProjectNumber2;
                QCViewModel.QCVerifier = Form.QCVerifier;
                QCViewModel.QCVerifierFullName = GetUserFullName(Form.QCVerifier);
                QCViewModel.Status = QCReview.Status;
                QCViewModel.QCStatus = Form.QCStatus;
                QCViewModel.CompletedOn = QCReview.CompletedOn;
                QCViewModel.Requester = Form.Reviewer;
                QCViewModel.RequesterFullName = GetUserFullName(Form.Reviewer);
                QCViewModel.RequestedOn = QCReview.AssignedOn;

                AllQCs.Add(QCViewModel);
            }
            if (AllQCs.Count > 0)
                return AllQCs.OrderByDescending(x => x.RequestedOn).ToList();

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
            if (Form == null)
            {
                throw new Exception("Compliance Form cannot be accessed");
            }
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

            var QCCompletedReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.QCCompleted);

            var QCCorrectionReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.QCCorrectionInProgress);

            var CompletedReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.Completed);

            if (QCCompletedReview != null &&
                QCCorrectionReview == null &&
                CompletedReview == null &&
                Form.AssignedTo.ToLower() == LoggedInUserName)
            {
                Form.Reviews.Add(new Review()
                {
                    RecId = Guid.NewGuid(),
                    AssignedOn = QCCompletedReview.CompletedOn.Value,
                    AssigendTo = QCCompletedReview.AssignedBy,
                    AssignedBy = QCCompletedReview.AssigendTo,
                    StartedOn = DateTime.Now,
                    Status = ReviewStatusEnum.QCCorrectionInProgress,
                    PreviousReviewId = QCCompletedReview.RecId
                });
                _UOW.ComplianceFormRepository.UpdateCollection(Form);
            }
            return Form;
        }

        public ComplianceForm SubmitQC(ComplianceForm Form, string URL)
        {
            var CurrentQCReview = Form.Reviews.LastOrDefault();

            if (CurrentQCReview == null)
                throw new Exception("Review cannot be empty");

            if (CurrentQCReview.Status == ReviewStatusEnum.QCCorrectionInProgress)
            {
                UpdateReviewStatus(CurrentQCReview,
                    Form.Findings.Where(x => x.IsAnIssue).ToList());
            }
            else if (CurrentQCReview.Status == ReviewStatusEnum.QCCompleted)
            {
                //CurrentQCReview.Status = ReviewStatusEnum.QCCompleted;
                CurrentQCReview.CompletedOn = DateTime.Now;
                SendQCSubmitMail(CurrentQCReview.AssignedBy,
                    CurrentQCReview.AssigendTo,
                    Form.InvestigatorDetails.First().Name,
                    (Form.ProjectNumber + " " + Form.ProjectNumber2).Trim());
            }

            _UOW.ComplianceFormRepository.UpdateCollection(Form);
            return Form;
        }

        private void UpdateReviewStatus(Review Review,
            List<Finding> FindingsWithIssues)
        {
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
                QCSummary.SourceId = finding.SiteSourceId.Value;
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

        public bool Undo(Guid ComplianceFormId, UndoEnum undoEnum, string UndoComment)
        {
            switch (undoEnum)
            {
                case UndoEnum.UndoQCRequest:
                    return UndoQCRequest(ComplianceFormId, UndoComment);
                case UndoEnum.UndoQCSubmit:
                    return UndoQCSubmit(ComplianceFormId);
                case UndoEnum.UndoQCResponse:
                    return UndoQCResponse(ComplianceFormId);
                case UndoEnum.UndoCompleted:
                    return UndoCompleted(ComplianceFormId);
                default: throw new Exception("invalid UndoEnum");
            }
        }

        private bool UndoQCRequest(Guid ComplianceFormId, string UndoComment)
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

                SendUndoQCRequestMail(QCRequestedReview.AssignedBy, 
                    QCRequestedReview.AssigendTo,
                    Form.InvestigatorDetails.First().Name,
                    (Form.ProjectNumber + " " + Form.ProjectNumber2).Trim(),
                    QCRequestedReview.ReviewCategory, UndoComment);
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
                x.Status == ReviewStatusEnum.QCCompleted ||
                x.Status == ReviewStatusEnum.Completed);

            if (QCFailedOrPassedReview != null)
            {
                QCFailedOrPassedReview.Status = ReviewStatusEnum.QCInProgress;
                QCFailedOrPassedReview.CompletedOn = null;
                _UOW.ComplianceFormRepository.UpdateCollection(Form);
                SendUndoQCSubmitMail(QCFailedOrPassedReview.AssignedBy,
                    QCFailedOrPassedReview.AssigendTo,
                    Form.InvestigatorDetails.First().Name,
                    (Form.ProjectNumber + " " + Form.ProjectNumber2).Trim());
                return true;
            }
            else
                return false;
        }

        private bool UndoQCResponse(Guid ComplianceFormId)
        {
            var Form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (Form == null)
                throw new Exception("Could not find compliance form");

            var QCFailedReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.QCCompleted);

            var CompletedReview = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.Completed);

            if (QCFailedReview != null && CompletedReview != null)
            {
                CompletedReview.Status = ReviewStatusEnum.QCCorrectionInProgress;
                CompletedReview.CompletedOn = null;
                _UOW.ComplianceFormRepository.UpdateCollection(Form);
                return true;
            }
            else
                return false;
        }

        private bool UndoCompleted(Guid ComplianceFormId)
        {
            var Form = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            var ReviewCompleted = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.ReviewCompleted);

            var Completed = Form.Reviews.Find(x =>
                x.Status == ReviewStatusEnum.Completed);

            if (ReviewCompleted == null && Completed != null)
                Completed.Status = ReviewStatusEnum.ReviewCompleted;

            _UOW.ComplianceFormRepository.UpdateCollection(Form);

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
                case CommentCategoryEnum.ExcludeFinding: return "Exclude Finding";
                case CommentCategoryEnum.NotAccepted: return "Not Accepted";
                default: throw new Exception("Invalid CommentCategoryEnum");
            }
        }

        #region QC Mails

        private void SendQCRequestedMail(ComplianceForm Form, string URL)
        {
            var QCReview = Form.Reviews.Find(x => x.Status == ReviewStatusEnum.QCRequested);

            if (QCReview == null)
                throw new Exception("Could not find QCRequested review. Send QC Request mail failed");

            var PIName = "";
            if (Form.InvestigatorDetails.Count > 0)
                PIName = Form.InvestigatorDetails.First().Name;

            var ProjectNumber = "";
            if (Form.ProjectNumber2 != null)
                ProjectNumber = Form.ProjectNumber + " " + Form.ProjectNumber2;
            else
                ProjectNumber = Form.ProjectNumber;

            var User = _UOW.UserRepository.GetAll()
                .Find(x => x.UserName.ToLower() == QCReview.AssigendTo.ToLower());

            if (User == null)
                throw new Exception("invalid username");

            var UserEMail = User.EmailId;
            var Subject = "QC Request - " + QCReview.ReviewCategory + "_" +
                ProjectNumber + "_" + PIName;

            var Link = URL + "/login?returnUrl=start/qc/edit-qc/";
            Link += Form.RecId + "/" + QCReview.AssigendTo + "/end";

            var MailBody = "Dear " + User.UserFullName + ",<br/><br/>";
            MailBody += GetUserFullName(QCReview.AssignedBy) + " has requested you to review a compliance search outcome. <br/><br/>";
            MailBody += "Please login to DDAS application and navigate to \"QC Check\" to start the review. <br/><br/>";
            MailBody += Link + "<br/><br/>";
            MailBody += "Yours Sincerely,<br/>";
            MailBody += GetUserFullName(QCReview.AssignedBy);

            //link requirements:site url + /login?returnUrl=start/ + page path + /end
            //example:
            //http://localhost:3000/login?returnUrl=start/qc/edit-qc/a0cd3a08-8d76-45dc-a2d0-4c7a13726abd/admin1/end

            SendMail(UserEMail, Subject, MailBody);
        }

        private void SendUndoQCRequestMail(string AssignedBy, string AssignedTo, string PI,
            string ProjectNumber, string ReviewCategory, string UndoComment)
        {
            var User = _UOW.UserRepository.FindByUserName(AssignedTo);

            if (User == null)
                throw new Exception("invalid username");

            var UserEMail = User.EmailId;
            var Subject = "Undo QC Request - " + ReviewCategory + "_" +
                ProjectNumber + "_" + PI;
            var MailBody = "Dear " + User.UserFullName + ",<br/><br/> ";

            MailBody += GetUserFullName(AssignedBy) + " has recalled the review request ";

            if (UndoComment != null)
                MailBody += " as <b>" + UndoComment + "</b>";

            MailBody += "<br/><br/>";
            MailBody += "Yours Sincerely,<br/>";
            MailBody += GetUserFullName(AssignedBy);

            SendMail(UserEMail, Subject, MailBody);
        }

        private void SendQCSubmitMail(string AssignedBy, string AssignedTo, string PI,
            string ProjectNumber)
        {
            var User = _UOW.UserRepository.GetAll()
                .Find(x => x.UserName.ToLower() == AssignedBy.ToLower());

            if (User == null)
                throw new Exception("invalid username");

            var UserEMail = User.EmailId;
            var Subject = "QC Complete - " + ProjectNumber + "_" + PI;
            var MailBody = "Dear " + User.UserFullName + ",<br/><br/>";
            MailBody += "Your QC review request has been completed by " + GetUserFullName(AssignedTo) + ". <br/><br/>";
            MailBody += "Please login to DDAS application and navigate to \"QC Check\" to view the observations/comments. <br/><br/>";
            MailBody += "Please login to DDAS application and navigate to \"QC Check\" to view the observations/comments. <br/><br/>";
            MailBody += "Yours Sincerely,<br/>";
            MailBody += GetUserFullName(AssignedTo);

            SendMail(UserEMail, Subject, MailBody);
        }

        private void SendUndoQCSubmitMail(string AssignedBy, string AssignedTo, string PI,
            string ProjectNumber)
        {
            var User = _UOW.UserRepository.FindByUserName(AssignedBy);

            if (User == null)
                throw new Exception("invalid username");

            var UserEMail = User.EmailId;
            var Subject = "Undo QC Complete - " + ProjectNumber + "_" + PI;
            var MailBody = "Dear " + User.UserFullName + ",<br/><br/>";
            MailBody += GetUserFullName(AssignedTo) + " has recalled the QC submit. <br/><br/>";
            MailBody += "Yours Sincerely,<br/>";
            MailBody += GetUserFullName(AssignedTo);

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
        
        #endregion

        private string GetQCCompletedSummary(ComplianceForm Form)
        {
            var QCCompletedSummary = "<b>Comment Type:</b> Comment A";

            return QCCompletedSummary;
        }

        private string GetUserFullName(string AssignedTo)
        {
            if (AssignedTo == null || AssignedTo == "")
                return null;

            var User = _UOW.UserRepository.GetAll()
                .Find(x => x.UserName.ToLower() == AssignedTo.ToLower());

            return
                User != null ? User.UserFullName : null;
        }
    }
}
