public class InvestigatorSearched
    {
        public InvestigatorSearched()
        {
            SitesSearched = new List<SiteSearchStatus>();
        }

        public int Id { get; set; }
        public int DisplayPosition { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string SearchName
        {
            get
            {
                var searchName = "";
                if (FirstName != null && FirstName.Trim().Length > 0)
                    searchName = FirstName.Trim();
                if (MiddleName != null && MiddleName.Trim().Length > 0)
                    searchName += " " + MiddleName.Trim();
                if (LastName != null && LastName.Trim().Length > 0)
                    searchName += " " + LastName.Trim();
                return searchName.Trim();
            }
        }

        public string Role { get; set; }
        public string Qualification { get; set; }
        public string MedicalLiceseNumber { get; set; }
        public string InvestigatorId { get; set; } = "";

        //changed ExtractedOn to AddedOn
        public DateTime? AddedOn { get; set; } //null indicates 'Not extracted'
        public DateTime SearchCompletedOn { get; set; } //10Oct2017 Pradeep
        public bool HasExtractionError { get; set; }
        public int ExtractionErrorSiteCount { get; set; }
        public int ExtractionPendingSiteCount { get; set; }
        //public int MatchesFoundSiteCount { get; set; }

        public int Sites_FullMatchCount { get; set; }
        public int Sites_PartialMatchCount { get; set; }
        public int Sites_SingleMatchCount { get; set; }
        public int IssuesFoundSiteCount { get; set; }
        public int ReviewCompletedSiteCount { get; set; }

        public DateTime? ReviewCompletedOn { get; set; } = null;

        public int TotalIssuesFound { get; set; }
        public int ReviewCompletedCount { get; set; }

        public string MemberId { get; set; }

        //Patrick 27NOvb2016: - to be removed:
        public List<SitesIncludedInSearch> SiteDetails { get; set; }

        public bool Deleted { get; set; }
        public List<SiteSearchStatus> SitesSearched { get; set; }

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
            var ReviewCompleted = false;
            //var searchStatusIssuesIdentifiedCount = SitesSearched.Where(s => s.StatusEnum == ComplianceFormStatusEnum.IssuesIdentifiedReviewPending).ToList().Count;
            var searchStatusIssuesIdentifiedCount = SitesSearched.Where(
                s => s.StatusEnum == ComplianceFormStatusEnum.IssuesIdentifiedReviewPending
                || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified
                ).ToList().Count;

            var searchStatusFullMatchCount = SitesSearched.Where(s => s.StatusEnum == ComplianceFormStatusEnum.FullMatchFoundReviewPending).ToList().Count;
            var searchStatusPartialMatchCount = SitesSearched.Where(s => s.StatusEnum == ComplianceFormStatusEnum.PartialMatchFoundReviewPending).ToList().Count;
            var searchStatusSingleMatchCount = SitesSearched.Where(s => s.StatusEnum == ComplianceFormStatusEnum.SingleMatchFoundReviewPending).ToList().Count;
            var searchStatusExtractionErrorsCount = SitesSearched.Where(s => s.StatusEnum == ComplianceFormStatusEnum.HasExtractionErrors).ToList().Count;
            var searchStatusNotScannedCount = SitesSearched.Where(s => s.StatusEnum == ComplianceFormStatusEnum.NotScanned).ToList().Count;
            var sitesSearchedCount = SitesSearched.Where(x => x.Exclude == false).Count();
            //ExtractionPendingSiteCount
            if (ReviewCompletedSiteCount == sitesSearchedCount)
            {
                ReviewCompleted = true;
            }
            if (ReviewCompleted == true)
            {
                _Status = "Review completed, Issues Not Identified";
                _StatusEnum = ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified;
                if (IssuesFoundSiteCount > 0)
                {
                    if (IssuesFoundSiteCount > 1)
                    {
                        plural = "s";
                    }
                     _Status = string.Format("Review completed, Issue{1} at {0} Source{1} identified.", IssuesFoundSiteCount, plural);
                    _StatusEnum = ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified;
                }
            }
            else if (searchStatusIssuesIdentifiedCount > 0)
            {
                if (searchStatusIssuesIdentifiedCount > 1)
                {
                    plural = "s";
                }
                _Status = string.Format("Issue{1} Identified at {0} Source{1}, Review Pending", searchStatusIssuesIdentifiedCount, plural);
                _StatusEnum = ComplianceFormStatusEnum.IssuesIdentifiedReviewPending;
            }
            else if (searchStatusFullMatchCount > 0)
            {
                if (searchStatusFullMatchCount > 1)
                {
                    plural = "es";
                    plural1 = "s";
                }
                _Status = string.Format("Full Match{1} Found at {0} Source{2}, Review Pending", searchStatusFullMatchCount, plural, plural1);
                _StatusEnum = ComplianceFormStatusEnum.FullMatchFoundReviewPending;
            }
            else if (searchStatusPartialMatchCount > 0)
            {
                if (searchStatusPartialMatchCount > 1)
                {
                    plural = "es";
                    plural1 = "s";
                }
                _Status = string.Format("Partial Match Found{1} at {0} Source{2}, Review Pending", searchStatusPartialMatchCount, plural, plural1);
                _StatusEnum = ComplianceFormStatusEnum.PartialMatchFoundReviewPending;
            }
            else if(searchStatusSingleMatchCount > 0)
            {
                if(searchStatusSingleMatchCount > 1)
                {
                    plural = "es";
                    plural1 = "s";
                }
                _Status = string.Format("Partial Match Found{1} at {0} Source{2}, Review Pending", searchStatusSingleMatchCount, plural, plural1);
                _StatusEnum = ComplianceFormStatusEnum.SingleMatchFoundReviewPending;
            }
            //searchStatusHasExtractionErrors
            else if (searchStatusExtractionErrorsCount > 0)
            {
                if (searchStatusExtractionErrorsCount > 1)
                {
                    plural = "s";
                }
                _Status = string.Format("Data Extraction Error at {0} Source{1}, Review Pending", searchStatusExtractionErrorsCount, plural);
                _StatusEnum = ComplianceFormStatusEnum.HasExtractionErrors;
            } 
            else if (searchStatusNotScannedCount > 0)
            {
                if (searchStatusNotScannedCount > 1)
                {
                    plural = "s";
                }

                _Status = string.Format("Data Extraction Pending at {0} Source{1}, Review Pending", searchStatusNotScannedCount, plural);
                _StatusEnum = ComplianceFormStatusEnum.NotScanned;
            }
            else
            {
                _Status = "No Match Found, Review Pending";
                _StatusEnum = ComplianceFormStatusEnum.NoMatchFoundReviewPending;
            }
            
            //else if (IssuesFoundSiteCount > 0)
            //{
            //    if (IssuesFoundSiteCount > 1)
            //    {
            //        plural = "s";
            //    }
            //    _Status = string.Format("Issue{1} Identified at {0} Source{1}, Review Pending", IssuesFoundSiteCount, plural);
            //    _StatusEnum = ComplianceFormStatusEnum.IssuesIdentifiedReviewPending;
            //}

            //else if (ExtractionPendingSiteCount> 0)
            //{
            //    if (ExtractionErrorSiteCount > 0)
            //    {
            //        if (ExtractionErrorSiteCount > 1)
            //        {
            //            plural = "s";
            //        }
            //        _Status = string.Format("Data Extraction Error at {0} Source{1}, Review Pending", ExtractionErrorSiteCount, plural);
            //        _StatusEnum = ComplianceFormStatusEnum.NotScanned;
            //    }
            //    else
            //    {
            //        if (ExtractionPendingSiteCount > 1)
            //        {
            //            plural = "s";
            //        }
         
            //        _Status = string.Format("Data Extraction Pending at {0} Source{1}, Review Pending", ExtractionPendingSiteCount, plural);
            //        _StatusEnum = ComplianceFormStatusEnum.NotScanned;
            //    }
            // }
            //else
            //{
            //    if (Sites_FullMatchCount > 0)
            //    {
            //        if (Sites_FullMatchCount > 1)
            //        {
            //            plural = "es";
            //            plural1 = "s";
            //        }
            //        _Status = string.Format("Full Match{1} Found at {0} Source{2}, Review Pending", Sites_FullMatchCount, plural, plural1);
            //        _StatusEnum = ComplianceFormStatusEnum.FullMatchFoundReviewPending;
            //    }
            //    else if (Sites_PartialMatchCount > 0)
            //    {
            //        if (Sites_PartialMatchCount > 1)
            //        {
            //            plural = "es";
            //            plural1 = "s";

            //        }
            //        _Status = string.Format("Partial Match Found{1} at {0} Source{2}, Review Pending", Sites_PartialMatchCount, plural, plural1);
            //        _StatusEnum = ComplianceFormStatusEnum.PartialMatchFoundReviewPending;
            //    }
            //    else
            //    {
            //        _Status = "No Match Found, Review Pending";
            //        _StatusEnum = ComplianceFormStatusEnum.NoMatchFoundReviewPending;
            //    }
        }
    }