using System;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public abstract class SiteDataItemBase
    {
        public int Matched { get; set; }
        public int RowNumber { get; set; }
        public abstract string FullName { get;  }
        public string Status { get; set; }
        public string Issues { get; set; }
        public DateTime VerifiedOn { get; set; }
        public int IssueNumber { get; set; }
        public int FullMatchCount { get; set; }
        public int PartialMatchCount { get; set; }
        public bool IssuesIdentified { get; set; }
        public Guid? ComplianceFormId { get; set; }
    }
}
