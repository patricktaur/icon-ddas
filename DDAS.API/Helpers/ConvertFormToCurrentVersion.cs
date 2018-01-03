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
            if(CompForm.Comments == null || 
                CompForm.Comments.Count == 0)
            {
                CompForm.Comments = new List<Comment>();
            }

            if(CompForm.Reviews == null || 
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

            foreach(Finding finding in CompForm.Findings)
            {
                if(finding.Comments == null ||
                    finding.Comments.Count == 0)
                {
                    finding.Comments = new List<Comment>();
                }
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