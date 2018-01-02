 public class SiteSearchStatus
    {
        public int SiteSourceId { get; set; } //CompForm.SiteSources.Id
        public Guid? SiteId { get; set; } //RecId of SiteSourceRepository
        public SiteEnum siteEnum { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public Guid? SiteDataId { get; set; }
        public int DisplayPosition { get; set; }

        // Remove?
        public bool ExtractionPending { get; set; }
        public DateTime? ExtractedOn { get; set; } //null indicates 'Not extracted' or has errors.
        public bool HasExtractionError { get; set; }
        public string ExtractionErrorMessage { get; set; }
        
        public int FullMatchCount { get; set; }
        public int PartialMatchCount { get; set; }
        public int SingleMatchCount { get; set; }

        public int IssuesFound { get; set; }
        
        
        public bool Exclude { get; set; }
        public bool ReviewCompleted { get; set; }

        public int[] SingleComponentMatchCount { get; set; }
        //public int SingleComponentCount { get; set; }

        public DateTime? SiteSourceUpdatedOn { get; set; }
        public string ExtractionMode { get; set; }

        private string _Status;
        private ComplianceFormStatusEnum _StatusEnum;
        public string Status
        {
            get
            {
                if (_Status == null)
                {
                    setStatusNStatusEnum();
                }
                return _Status;
            }
        }
        public ComplianceFormStatusEnum StatusEnum
        {
            get
            {
                if (_Status == null)
                {
                    setStatusNStatusEnum();
                }
                return _StatusEnum;
            }
        }

        private void setStatusNStatusEnum()
        {
            string plural = "";

            if (ReviewCompleted == true)
            {
                _Status = "Review completed, Issues Not Identified";
                _StatusEnum = ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified;
                if (IssuesFound > 0)
                {
                    if (IssuesFound > 1)
                    {
                        plural = "s";
                    }
                     _Status = string.Format("Review completed, {0} Issue{1} Identified.", IssuesFound, plural);
                    _StatusEnum = ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified;
                }
            }
            else if (IssuesFound > 0)
            {
                if (IssuesFound > 1)
                {
                    plural = "s";
                }
                _Status = string.Format("{0} Issue{1} Identified, Review Pending ", IssuesFound, plural);
                _StatusEnum = ComplianceFormStatusEnum.IssuesIdentifiedReviewPending;
            }
            else if (ExtractionMode == "Manual")
            {
                _Status = "Issues Not Identified (Manual Search), Review Pending";
                _StatusEnum = ComplianceFormStatusEnum.ManualSearchSiteReviewPending;
            }
            else if (FullMatchCount > 0)
            {
                plural = "es";
                if (FullMatchCount > 0)
                {
                    _Status = string.Format("{0} Full Match{1} Found, Review Pending", FullMatchCount, plural);
                    _StatusEnum = ComplianceFormStatusEnum.FullMatchFoundReviewPending;
                }
                //else if (PartialMatchCount > 0)
                //{
                //    _Status = string.Format("Review Pending, {0} Partial Match{1} Found", PartialMatchCount, plural);
                //    _StatusEnum = ComplianceFormStatusEnum.PartialMatchFoundReviewPending;
                //}
            }
            else if(PartialMatchCount > 0)
            {
                plural = "es";
                _Status = string.Format("Review Pending, {0} Partial Match{1} Found", PartialMatchCount, plural);
                _StatusEnum = ComplianceFormStatusEnum.PartialMatchFoundReviewPending;
            }
            else if(SingleMatchCount > 0)
            {
                plural = "es";
                _Status = string.Format("Review Pending, {0} Single Match{1} Found", SingleMatchCount, plural);
                _StatusEnum = ComplianceFormStatusEnum.SingleMatchFoundReviewPending;
            }
            else if (HasExtractionError == true)
            {
                _Status = ExtractionErrorMessage;
                _StatusEnum = ComplianceFormStatusEnum.HasExtractionErrors;
            }
            else if (ExtractedOn == null)
            {
                _Status = "Data not extracted";
                _StatusEnum = ComplianceFormStatusEnum.NotScanned;
            }
            else
            {
                _Status = "No Match Found, Review Pending";
                _StatusEnum = ComplianceFormStatusEnum.NoMatchFoundReviewPending;
            }
        }
    }