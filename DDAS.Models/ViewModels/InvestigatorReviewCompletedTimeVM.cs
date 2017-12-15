using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class InvestigatorReviewCompletedTimeVM
    {
        public string InvestigatorName { get; set; }
        public string Role { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public DateTime SearchStartedOn { get; set; }
        public DateTime ReviewCompletedOn { get; set; }
        public string AssignedTo { get; set; }
        public int TimeTakenToCompleteReview {
            get
            {
                return (int)ReviewCompletedOn.Subtract(SearchStartedOn).TotalMinutes;
            }
        }
        public int FullMatchCount { get; set; }
        public int PartialMatchCount { get; set; }
        public int SingleMatchCount { get; set; }
        public string IssuesIdentifiedStatus { get; set; }
    }
}
