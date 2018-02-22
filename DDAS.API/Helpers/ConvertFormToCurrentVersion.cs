using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDAS.API.Helpers
{
    public class UpdateFormToCurrentVersion
    {
        public static void UpdateComplianceFormToCurrentVersion(
            ComplianceForm CompForm)
        {
            if (CompForm.Reviews == null || 
                CompForm.Reviews.Count == 0)
            {
                CompForm.Reviews = new List<Review>()
                {
                    new Review(){
                        RecId = Guid.NewGuid(),
                        AssigendTo = CompForm.AssignedTo,
                        AssignedBy = CompForm.AssignedTo,
                        AssignedOn = CompForm.SearchStartedOn,
                        StartedOn = CompForm.SearchStartedOn,
                        ReviewerRole = ReviewerRoleEnum.Reviewer,
                        Status = CompForm.IsReviewCompleted ?
                        ReviewStatusEnum.ReviewCompleted : ReviewStatusEnum.ReviewInProgress
                    }
                };
            }

            //Patrick: 11Feb2018:
            //if (CompForm.QCGeneralComment == null) {
            //    var newComment = new Comment();
            //    newComment.CategoryEnum = CommentCategoryEnum.Minor;
            //    newComment.ReviewerCategoryEnum = CommentCategoryEnum.CorrectionCompleted;
            //    CompForm.QCGeneralComment = newComment;
            //}
            //if (CompForm.QCAttachmentComment == null) {
            //    var newComment = new Comment();
            //    newComment.CategoryEnum = CommentCategoryEnum.Minor;
            //    newComment.ReviewerCategoryEnum = CommentCategoryEnum.CorrectionCompleted;
            //    CompForm.QCAttachmentComment = newComment;
            //}

            //Pradeep 22Feb2018
            if(CompForm.QCGeneralComments == null || 
                CompForm.QCGeneralComments.Count == 0)
            {
                var newComment = new Comment();
                newComment.CategoryEnum = CommentCategoryEnum.Minor;
                newComment.ReviewerCategoryEnum = CommentCategoryEnum.Accepted;
                CompForm.QCGeneralComments.Add(newComment);
            }
            if(CompForm.QCAttachmentComments == null ||
                CompForm.QCAttachmentComments.Count == 0)
            {
                var newComment = new Comment();
                newComment.CategoryEnum = CommentCategoryEnum.Minor;
                newComment.ReviewerCategoryEnum = CommentCategoryEnum.Accepted;
                CompForm.QCAttachmentComments.Add(newComment);
            }
        }

        public static List<ComplianceForm> UpdateComplianceFormToCurrentVersion(
            List<ComplianceForm> CompForms)
        {
            foreach(ComplianceForm Form in CompForms)
            {
                UpdateComplianceFormToCurrentVersion(Form);
            }
            return CompForms;
        }
    }
}