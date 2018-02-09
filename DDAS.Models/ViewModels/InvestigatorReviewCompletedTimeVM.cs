using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class InvestigatorReviewCompletedTimeVM
    {
        //Investigator	Role	Project Number	Search Started On	Review Completed On	
        //Assigned To	Full Matches	Patrial Matches	Single Matches	Issues Status	TimeTaken (in Minutes) to Complete Review

        public string InvestigatorName { get; set; }
        public string Role { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public DateTime SearchStartedOn { get; set; }
        public DateTime ReviewCompletedOn { get; set; }
        public string AssignedTo { get; set; }
        public int FullMatchCount { get; set; }
        public int PartialMatchCount { get; set; }
        public int SingleMatchCount { get; set; }
        public string IssuesIdentifiedStatus { get; set; }
        public int TimeTakenToCompleteReview
        {
            get
            {
                return (int)ReviewCompletedOn.Subtract(SearchStartedOn).TotalMinutes;
            }
        }
    }
}
