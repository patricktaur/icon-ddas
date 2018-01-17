public class ComplianceForm
    {
        public ComplianceForm()
        {
            InvestigatorDetails = new List<InvestigatorSearched>();
            SiteSources = new List<SiteSource>();
            Findings = new List<Finding>();
        }

        public Guid? RecId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string AssignedTo { get; set; }
        public bool Active { get; set; } = true;
        public string SponsorProtocolNumber { get; set; }
        public string SponsorProtocolNumber2 { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public string Institute { get; set; }
        public DateTime SearchStartedOn { get; set; }
        public string UploadedFileName { get; set; }
        public string GeneratedFileName { get; set; }
        
        //Remove ?
        //Patrick 19Feb2017:?????
        public int ExtractionQueue { get; set; }
        public int ExtractionQuePosition { get; set; }
        public int ExtractionAttempt { get; set; }
        public DateTime? ExtractionQueStart { get; set; }
        public DateTime? ExtractionEstimatedCompletion { get; set; }
        public DateTime? ExtractedOn { get; set; } //null indicates 'Not extracted' // extraction end
        public int ExtractionPendingInvestigatorCount { get; set; }
        public int ExtractionErrorInvestigatorCount { get; set; }
        public int ExtractedInvestigatorCount { get; set; }
        
        
        public int FullMatchesFoundInvestigatorCount { get; set; }
        public int PartialMatchesFoundInvestigatorCount { get; set; }
        public int SingleMatchFoundInvestigatorCount { get; set; }
        public int IssuesFoundInvestigatorCount { get; set; }
        public int ReviewCompletedInvestigatorCount { get; set; }
        public List<InvestigatorSearched> InvestigatorDetails { get; set; }
        public List<SiteSource> SiteSources { get; set; }
        public List<Finding> Findings { get; set; }

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
            string plural1 = "";
            //var InvIssuesIdentifiedCount = InvestigatorDetails.Where(s => s.StatusEnum == ComplianceFormStatusEnum.IssuesIdentifiedReviewPending).ToList().Count;
            var InvIssuesIdentifiedCount = InvestigatorDetails.Where(s => (s.StatusEnum == ComplianceFormStatusEnum.IssuesIdentifiedReviewPending 
            || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified)
            ).ToList().Count;

            var InvFullMatchCount = InvestigatorDetails.Where(s => s.StatusEnum == ComplianceFormStatusEnum.FullMatchFoundReviewPending).ToList().Count;

            var InvPartialMatchCount = InvestigatorDetails.Where(s => s.StatusEnum == ComplianceFormStatusEnum.PartialMatchFoundReviewPending).ToList().Count;

            var InvSingleMatchCount = InvestigatorDetails.Where(s => s.StatusEnum == ComplianceFormStatusEnum.SingleMatchFoundReviewPending).ToList().Count;

            var InvExtractionErrorsCount = InvestigatorDetails.Where(s => s.StatusEnum == ComplianceFormStatusEnum.HasExtractionErrors).ToList().Count;
            var InvNotScannedCount = InvestigatorDetails.Where(s => s.StatusEnum == ComplianceFormStatusEnum.NotScanned).ToList().Count;

            if (InvestigatorDetails.Count == 0)
            {
                _Status = "Investigator not added";
                _StatusEnum = ComplianceFormStatusEnum.NotScanned;
            }

            else if (ReviewCompletedInvestigatorCount == InvestigatorDetails.Count) 
            {
                _Status = "Review completed, Issues Not Identified";
                _StatusEnum = ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified;
                if (IssuesFoundInvestigatorCount > 0)
                {
                    if (IssuesFoundInvestigatorCount > 1)
                    {
                        plural = "s";
                    }
                    _Status = string.Format("Review completed, Issue{1} identified for {0} Investigator{1}", IssuesFoundInvestigatorCount, plural);
                    _StatusEnum = ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified;
                }
            }
            else if (InvIssuesIdentifiedCount > 0)
            {
                if (InvIssuesIdentifiedCount > 1)
                {
                    plural = "s";
                }
                _Status = string.Format("Issue{1} Identified for {0} Investigator{1}, Review Pending", InvIssuesIdentifiedCount, plural);
                _StatusEnum = ComplianceFormStatusEnum.IssuesIdentifiedReviewPending;
            }
            else if (InvFullMatchCount > 0)
            {
                if (InvFullMatchCount > 1)
                {
                    plural = "es";
                    plural1 = "s";
                }
                _Status = string.Format("Full Match{1} Found for {0} Investigator{2}, Review Pending", InvFullMatchCount, plural, plural1);
                _StatusEnum = ComplianceFormStatusEnum.FullMatchFoundReviewPending;
            }
            else if (InvPartialMatchCount > 0)
            {
                if (InvPartialMatchCount > 1)
                {
                    plural = "es";
                    plural1 = "s";
                }
                _Status = string.Format("Partial Match{1} Found for {0} Investigator{2}, Review Pending", InvPartialMatchCount, plural, plural1);
                _StatusEnum = ComplianceFormStatusEnum.PartialMatchFoundReviewPending;
            }
            else if(InvSingleMatchCount > 0)
            {
                if(InvSingleMatchCount > 1)
                {
                    plural = "es";
                    plural1 = "s";
                }
                _Status = string.Format("Partial Match{1} Found for {0} Investigator{2}, Review Pending", InvSingleMatchCount, plural, plural1);
                _StatusEnum = ComplianceFormStatusEnum.SingleMatchFoundReviewPending;
            }
            //Remove?
            else if (InvExtractionErrorsCount > 0)
            {
                if (InvExtractionErrorsCount > 1)
                {
                    plural = "s";
                }
                _Status = string.Format("Data Extraction Error for {0} Investigator{1}, Review Pending", InvExtractionErrorsCount, plural);
                _StatusEnum = ComplianceFormStatusEnum.HasExtractionErrors;
            }
            else if (InvNotScannedCount > 0)
            {
                if (InvNotScannedCount > 1)
                {
                    plural = "s";
                }

                _Status = string.Format("Data Extraction Pending at {0} Investigator{1}, Review Pending", InvNotScannedCount, plural);
                _StatusEnum = ComplianceFormStatusEnum.NotScanned;
            }
            else
            {
                _Status = "No Match Found, Review Pending";
                _StatusEnum = ComplianceFormStatusEnum.NoMatchFoundReviewPending;
            }
        }
        //Remove ?
        public string EstimatedExtractionCompletionWithin
        {
            get
            {
                if (ExtractionErrorInvestigatorCount > 0)
                {
                    return string.Format("Extraction Errors for {0} investigators. Scanning will be rescheduled.", ExtractionErrorInvestigatorCount);
                }
                else if (ExtractionPendingInvestigatorCount > 0)
                {
                    if (ExtractionEstimatedCompletion.HasValue)
                    {
                        if (ExtractionEstimatedCompletion > DateTime.Now)
                        {
                            var InSeconds = (ExtractionEstimatedCompletion.Value - DateTime.Now).TotalSeconds;

                            return getTimeValue(InSeconds);
                            
                        }
                        else
                        {
                            return "Taking longer than estimated.";
                        }
                    }
                    else
                    {
                        return "";
                    }


                    }
                else
                {
                    return "";
                }
  

            }
        }

        //Where Used ?  remove?
        private string getTimeValue(double ValueInSeconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(ValueInSeconds);

            //string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
            //                t.Hours,
            //                t.Minutes,
            //                t.Seconds);

            string hrs = "";
            if (t.Hours > 0)
            {
                hrs = t.Hours + " Hour";
            }
            if (t.Hours > 1)
            {
                hrs += "s";
            }
            if (hrs.Length > 0)
            {
                hrs += " ";
            }

            string mins = "";
            if (t.Minutes > 0)
            {
                mins = t.Minutes + " Minute";
            }
            if (t.Minutes > 1)
            {
                mins += "s";
            }
            if (mins.Length > 0)
            {
                mins += " ";
            }

            string secs = "";
            if (t.Seconds > 0)
            {
                secs = t.Seconds + " Second";
            }
            if (t.Seconds > 1)
            {
                secs += "s";
            }
            if (secs.Length > 0)
            {
                secs += " ";
            }

            return (hrs + mins + secs) ;
        }

        public int InstituteSearchSiteCount {
            get {
                return SiteSources.Where(x => x.SearchAppliesTo == SearchAppliesToEnum.Institute).Count();
            }
        }
    }