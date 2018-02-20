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

            //if (CompForm.Comments == null ||
            //    CompForm.Comments.Count == 0)
            //{
            //    CompForm.Comments = new List<Comment>();
            //    CompForm.Comments.Add(new Comment()
            //    {
            //        ReviewId = CompForm.Reviews.First().RecId
            //    });
            //}

            //foreach(Finding finding in CompForm.Findings)
            //{
            //    if(finding.Comments == null ||
            //        finding.Comments.Count == 0)
            //    {
            //        finding.Comments = new List<Comment>() {
            //            new Comment()
            //            {

            //            }
            //        };
            //    }
            //}

            //Patrick: 11Feb2018:
            if (CompForm.QCGeneralComment == null) {
                var newComment = new Comment();
                newComment.CategoryEnum = CommentCategoryEnum.Minor;
                newComment.ReviewerCategoryEnum = CommentCategoryEnum.CorrectionCompleted;
                CompForm.QCGeneralComment = newComment;
            }
            if (CompForm.QCAttachmentComment == null) {
                var newComment = new Comment();
                newComment.CategoryEnum = CommentCategoryEnum.Minor;
                newComment.ReviewerCategoryEnum = CommentCategoryEnum.CorrectionCompleted;
                CompForm.QCAttachmentComment = newComment;
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