using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain
{
    #region SearchQuery

    public class SearchQuery
    {
        public string NameToSearch { get; set; }
        public List<SitesToSearch> SearchSites { get; set; }
    }

    //For multiple sites request
    public class SitesToSearch
    {
        public Guid? RecId { get; set; }
        public string SiteName { get; set; }
        //public bool IsOptional { get; set; }
        public string ExtractionMode { get; set; }
        //public bool Mandatory { get; set; }
        public string SiteShortName { get; set; }
        //public bool Selected { get; set; }
        public SiteEnum SiteEnum { get; set; }
        public string SiteUrl { get; set; }
        //public bool ExcludeSI { get; set; }
        //public bool ExcludePI { get; set; }
        //public string SearchTimeTakenInMs { get; set; }
        //public bool HasErrors { get; set; } = false;
        //public string ErrorDescription { get; set; }
        //public List<MatchResult> Results { get; set; }
    }

    public class SiteScan : SitesToSearch
    {
        public Guid? DataId { get; set; }
        public DateTime DataExtractedOn { get; set; }
        public DateTime? SiteLastUpdatedOn { get; set; }
        public int FullMatch { get; set; }
        public int PartialMatch { get; set; }
        public bool HasErrors { get; set; } = false;
        public string ErrorDescription { get; set; }
    }

    //for Single site query:
    public class SearchQueryAtSite
    {
        public string NameToSearch { get; set; }
        public SiteEnum SiteEnum { get; set; }

    }

    #endregion

    #region SearchResult


    public class SearchResult
    {

        public SearchResult()
        {
            resultAtSites = new List<ResultAtSite>();
        }

        public string NameToSearch { get; set; }
        public string SearchedBy { get; set; }
        public string SearchedOn { get; set; }
        public List<ResultAtSite> resultAtSites { get; set; }
    }

    public class ResultAtSite
    {
        public ResultAtSite()
        {
            Results = new List<MatchResult>();
        }
        public SiteEnum SiteEnum { get; set; }
        public string SiteName { get; set; }
        public string TimeTakenInMs { get; set; }
        public List<MatchResult> Results { get; set; }
        public bool HasErrors { get; set; } = false;
        public string ErrorDescription { get; set; }
    }

    public class MatchResult
    {
        public string MatchName { get; set; }
        public string MatchLocation { get; set; }
    }

    public class NameToSearchQuery
    {
        public string NameToSearch { get; set; }
        public Guid? ComplianceFormId { get; set; }
    }

    public class SearchDetailsQuery
    {
        public Guid? RecId { get; set; }
        public string NameToSearch { get; set; }
        public SiteEnum siteEnum { get; set; }
    }

    #endregion

    #region Revised
    public class SearchSummary
    {
        public SearchSummary()
        {
            SearchSummaryItems = new List<SearchSummaryItem>();
        }
        public Guid? ComplianceFormId { get; set; }
        public string NameToSearch { get; set; }
        public int Sites_FullMatchCount { get; set; }
        public int Sites_PartialMatchCount { get; set; }
        public int TotalIssuesFound { get; set; }
        public List<SearchSummaryItem> SearchSummaryItems { get; set; }
    }

    public class SearchSummaryItem
    {
        public Guid? RecId { get; set; }
        public DateTime DataExtractedOn { get; set; }
        public DateTime SiteLastUpdatedOn { get; set; }
        public string SiteName { get; set; }
        public SiteEnum SiteEnum { get; set; }
        public string SiteUrl { get; set; }
        public string MatchStatus { get; set; }
        public int FullMatch { get; set; }
        public int PartialMatch { get; set; }
        public int IssuesFound { get; set; }
        public string IssuesFoundStatus { get; set; }
    }
    #endregion

    #region ComplianceFormDetails

    public class ComplianceForm
    {
        public ComplianceForm()
        {
            InvestigatorDetails = new List<InvestigatorSearched>();
            SiteSources = new List<SiteSource>();
            Findings = new List<Finding>();
            Reviews = new List<Review>();
            Comments = new List<Comment>();
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
        //Patrick 19Feb2017:
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
        public List<Review> Reviews { get; set; }
        public List<Comment> Comments
        {
            get
            {
                if (Comments == null)
                {
                    return new List<Comment>();
                }
                else
                    return Comments;
            }
            set { Comments = value; }
        }
        public ReviewStatusEnum CurrentReviewStatus {
            get
            {
                var Review = Reviews.LastOrDefault();
                if (Review != null)
                    return Review.Status;
                else
                    return ReviewStatusEnum.SearchCompleted;
            }
        }

        public bool IsReviewCompleted {
            get {
                return 
                    (ReviewCompletedInvestigatorCount == 
                    InvestigatorDetails.Count() ?
                    true : false);
            }
        }

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

    public class PrincipalInvestigator
    {
        public Guid? RecId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string SponsorProtocolNumber { get; set; }
        public string SponsorProtocolNumber2 { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public DateTime SearchStartedOn { get; set; }
        public string Status { get; set; }
        public ComplianceFormStatusEnum StatusEnum { get; set; }
        public string AssignedTo { get; set; }
        public bool ReviewCompleted {
            get {
                if (StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified 
                    || StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                {
                    return true;
                }
                else
                {
                    return false;
                }     
            }
        }
        public List<SubInvestigator> SubInvestigators { get; set; } =
            new List<SubInvestigator>();
        public int ExtractionPendingInvestigatorCount { get; set; }
        public int ExtractionErrorInvestigatorCount { get; set; }
        public string EstimatedExtractionCompletionWithin { get; set; }
    }

    //Pradeep30Jan2017
    public class SubInvestigator
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public ComplianceFormStatusEnum StatusEnum { get; set; }
    }

    //Patrick 03Dec2016 modified.
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

    //Patrick 01Dec2016 - new props added:
    public class SiteSearchStatus
    {
        public int SiteSourceId { get; set; } //CompForm.SiteSources.Id
        public Guid? SiteId { get; set; } //RecId of SiteSourceRepository
        public SiteEnum siteEnum { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public Guid? SiteDataId { get; set; }
        public int DisplayPosition { get; set; }

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

    public class SitesIncludedInSearch
    {
        public int SourceNumber { get; set; }
        public string SiteName { get; set; }
        public DateTime DataExtractedOn { get; set; }
        public DateTime SiteSourceUpdatedOn { get; set; }
        //public DateTime SiteLastUpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ExtractionMode { get; set; }
        public SiteEnum SiteEnum { get; set; }
        public string SiteUrl { get; set; }
        public DateTime UpdatedOn { get; set; }
        //public DateTime ScannedOn { get; set; }
        public int FullMatchCount { get; set; }
        public int PartialMatchCount { get; set; }
        public string MatchStatus { get; set; }
        public bool IssuesIdentified { get; set; }
        public int IssuesFound { get; set; }
        public string IssuesFoundStatus { get; set; }
        public bool HasExtractionError { get; set; }
        public string ExtractionErrorMessage { get; set; }
        public bool ReviewCompleted { get; set; }
        //Patrick 27Nov2016:
        //remove MatchedRecords -- to be replaced by Findings in CompForm
        public List<MatchedRecordsPerSite> MatchedRecords { get; set; }
    }

    public class MatchedRecordsPerSite
    {
        public int Matched { get; set; }
        public int IssueNumber { get; set; }
        public int RowNumber { get; set; }
        public string Observation { get; set; }
        public string RecordDetails { get; set; }
        public string Status { get; set; }
        public string HiddenStatus { get; set; }
    }

    //pradeep 27Dec2017
    public class Review
    {
        public Guid? RecId { get; set; }
        public Guid? PreviousReviewId { get; set; }
        public string AssigendTo { get; set; }
        public DateTime AssignedOn { get; set; }
        public string AssignedBy { get; set; }
        public ReviewStatusEnum Status { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public ReviewerRoleEnum ReviewerRole { get; set; }
        public string Comment { get; set; }
    }
    #endregion

    #region ByPatrick

    public class MatchedRecord
    {
        public bool IsFullMatch { get; set; }
        public int MatchCount { get; set; }
        public int RowNumber { get; set; }
        public string RecordDetails { get; set; }
        public DateTime? DateOfInspection { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();
    }

    public class SiteSource
    {
        public int Id { get; set; }
        public Guid? SiteId { get; set; } //RecId of SiteSourceRepository
        public int DisplayPosition { get; set; }
        public string SiteName { get; set; }
        public string SiteShortName { get; set; }
        public Guid? SiteDataId { get; set; }
        public DateTime DataExtractedOn { get; set; }
        public DateTime? SiteSourceUpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ExtractionMode { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsOptional { get; set; }
        public SiteEnum SiteEnum { get; set; }
        public string SiteUrl { get; set; }
        public bool IssuesIdentified { get; set; }
        public bool Deleted { get; set; } = false; //Patrick 30NOv2016

        //public bool ExcludeSI { get; set; }
        //public bool ExcludePI { get; set; }

        public SearchAppliesToEnum SearchAppliesTo { get; set; }
        public string SearchAppliesToText { get; set; }

    }

    public class Finding
    {
        //10Feb2017-todo: 
        public Guid? Id { get; set; }
        public Guid? SiteId { get; set; } //RecId of SiteSourceRepository
        public SiteEnum? SiteEnum { get; set; }
        public int? SiteSourceId { get; set; } // refers to the CompForm.SiteSource.Id  //SourceNumber renamed to SiteSourceId
        public int SiteDisplayPosition { get; set; } // CompForm.SiteSource.DisplayPosition -- display Site No 
        public int DisplayPosition { get; set; } //to facilitate ordering on client side.

        public int? InvestigatorSearchedId { get; set; }
        public string InvestigatorName { get; set; }

        //Remove ??
        public int RowNumberInSource { get; set; }

        public bool IsFullMatch { get; set; }
        public int MatchCount { get; set; }

        //Remove?
        public bool Selected { get; set; }  
        public bool IsMatchedRecord { get; set; }

        //Remove ???:
        public string Status { get; set; }
        ////Patrick 04Dec2016: no longer required, can be deleted.  Comp Form collection in MOngodB has to be dropped.
        //public string HiddenStatus { get; set; }

        //Findings:
        public string RecordDetails { get; set; }  //content copied from matched record.
        public DateTime? DateOfInspection { get; set; } //for DB site: Date extracted, for Manual Site: editable.
        public string Observation { get; set; }
        public bool IsAnIssue { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();//for DB sites only, not used for Manual sites
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();//not yet used by client

        public List<Comment> Comments { get; set; } = new List<Comment>();
        public Guid ReviewId { get; set; }
        //New fields: 11Apr2017
        //guid SiteDataId
        //guid SiteRecordId
        //DateTime AddedOn
    }

    public class Comment
    {
        public Guid ReviewId { get; set; }
        public string FindingComment { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime CorrectedOn { get; set; }
        public CommentCategoryEnum CategoryEnum { get; set; }
    }

    public class UpdateFindigs
    {
        //Guid id,  SiteEnum siteEnum, int InvestigatorId, bool ReviewCompleted, List<Finding> Findings
        public Guid FormId { get; set; }
        //public SiteEnum SiteEnum { get; set; }
        public int SiteSourceId { get; set; }
        public int InvestigatorSearchedId { get; set; }
        public bool ReviewCompleted { get; set; }
        //public InvestigatorSearched InvestigatorSearched { get; set; }
        public List<Finding> Findings { get; set; }
    }

    public class UpdateInstituteFindings
    {
         public Guid FormId { get; set; }
        public int SiteSourceId { get; set; }
        public List<Finding> Findings { get; set; }
    }

    public class ComplianceFormManage
    {
        public string AssignedTo { get; set; }
        public bool Active { get; set; }
    }

    public class Attachment
    {
        public string Title { get; set; }
        public string FileName { get; set; }
        public string GeneratedFileName { get; set; }
    }
    #endregion

    #region AppAdmin
    
    public class ErrorScreenCapture
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime Created { get; set; }
    }
    #endregion

    #region DataExtractionView
    public class DataExtractionHistory
    {
        //public string SiteName { get; set; }
        public Guid? RecId { get; set; }
        public int SiteNumber { get; set; }
        public string SiteName { get; set; }
        public SiteEnum Enum { get; set; }
        public DateTime ExtractionDate { get; set; }
        public DateTime? SiteLastUpdatedOn { get; set; }
        public string ErrorDescription { get; set; }
        public string ExtractionMessage { get; set; }
    }
    #endregion

    #region Save Results

    #region ComplianceFormFilter

    public class ComplianceFormFilter
    {
        public string InvestigatorName { get; set; }
        public string ProjectNumber { get; set; }
        public string SponsorProtocolNumber { get; set; }
        public DateTime? SearchedOnFrom { get; set; }
        public DateTime? SearchedOnTo { get; set; }
        public string AssignedTo { get; set; }
        public string Country { get; set; }
        public ComplianceFormStatusEnum Status { get; set; }
    }

    #endregion



    public class SaveSearchResult
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string NameToSearch { get; set; }
        public SiteEnum siteEnum { get; set; }
        public Guid? RecId { get; set; }
        public Guid? DataId { get; set; }
        public DateTime HistoryOn { get; set; }
        public List<SaveSearchDetails> saveSearchDetails { get; set; }
    }

    public class SaveSearchDetails
    {
        public int RowNumber { get; set; }
        public string Status { get; set; }
    }
    #endregion

    #region Indentity
    //Pradeep 16Jan2017
    public class LoginDetails
    {
        public Guid? RecId { get; set; }
        public string UserName { get; set; }
        //public string Role { get; set; }
        public bool IsLoginSuccessful { get; set; }
        public DateTime LoginAttemptTime { get; set; }
        public string LocalIPAddress { get; set; }
        public string HostIPAddress { get; set; }
        public string PortNumber { get; set; }
        public string ServerProtocol { get; set; }
        public string ServerSoftware { get; set; }
        public string HttpHost { get; set; }
        public string ServerName { get; set; }
        public string GatewayInterface { get; set; }
        public string Https { get; set; }
    }

    public class User
    {
        private ICollection<Role> _roles;
        #region Scalar Properties
        //[BsonId]
        //public MongoDB.Bson.ObjectId _id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public string EmailId { get; set; }
        public string UserFullName { get; set; }
        public bool Active { get; set; }
        public bool Enterprise { get; set; }
        
        #endregion

        public virtual ICollection<Role> Roles
        {
            get { return _roles ?? (_roles = new List<Role>()); }
            set { _roles = value; }
        }
    }

    public class Role
    {
        private ICollection<User> _users;

        #region Scalar Properties

        public Guid RoleId { get; set; }
        public string Name { get; set; }
        #endregion

        public Role() { }
        public Role(string roleName)
        {
            Name = roleName;
        }

        public ICollection<User> Users
        {
            get { return _users ?? (_users = new List<User>()); }
            set { _users = value; }
        }
    }


    public class UserRole
    {
        public Guid Id { get; set; } //for Mongo mapping
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
    }
    #endregion

    #region BaseSites
    public class ComplianceFormBaseSite
    {
        public string Name { get; set; }
        public Guid? SiteId { get; set; }
        public bool IsMandatory { get; set; }
        //public string SiteName { get; set; }
        public SearchAppliesToEnum SearchAppliesTo { get; set; }
    }
    #endregion

    #region DefaultSites
        public class DefaultSite: ComplianceFormBaseSite
    {
        public Guid? RecId { get; set; }
        //public Guid? SiteId { get; set; }
        public int OrderNo { get; set; }
        //public bool IsMandatory { get; set; }
        //public bool ExcludeSI { get; set; }

        //public string SiteName { get; set; }
        //public string ExtractionMode { get; set; }
        //public string SiteShortName { get; set; }
        //public SiteEnum SiteEnum { get; set; }
        //public string SiteUrl { get; set; }

        //public SearchAppliesToEnum SearchAppliesTo { get; set; }
    }
    #endregion

    #region Country
    public class Country: ComplianceFormBaseSite
    {
        public Guid? RecId { get; set; }
        public string CountryName { get; set; }
        //public Guid? SiteId { get; set; }

        //---

        //public int OrderNo { get; set; }
        //public bool IsMandatory { get; set; }
        //public string SiteName { get; set; }
        //public string ExtractionMode { get; set; }
        
        //public SearchAppliesToEnum SearchAppliesTo { get; set; }
    }
    #endregion

    #region Sponsor
    public class SponsorProtocol : ComplianceFormBaseSite
    {
        public Guid? RecId { get; set; }
        public string SponsorProtocolNumber { get; set; }
        //public Guid? SiteId { get; set; }

        //---

        //public int OrderNo { get; set; }
        //public bool IsMandatory { get; set; }
        //public string SiteName { get; set; }
       
        //public SearchAppliesToEnum SearchAppliesTo { get; set; }
    }
    #endregion

    #region ReportFilters
    
    public class ReportFilters
    {
        public string AssignedTo { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public ReportPeriodEnum ReportPeriodEnum { get; set; }
    }
    
    public class InvestigationsReport
    {
        public List<ReportByUser> ReportByUsers { get; set; }
        public string DatesAdjustedTo;
    }

    public class ReportByUser
    {
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public List<ReportItem> ReportItems { get; set; } 
            = new List<ReportItem>();
    }

    public class ReportItem
    {
        public string ReportPeriod { get; set; }
        public double Value { get; set; }
    }

    public static class DateTimeExtensions
    {
        public static DateTime LastDayOfMonth(DateTime Value)
        {
            return new DateTime(
                Value.Year,
                Value.Month,
                DateTime.DaysInMonth(Value.Year, Value.Month));
        }

        public static DateTime FirstDayOfMonth(DateTime Value)
        {
            return new DateTime(Value.Year, Value.Month, 1);
        }

        public static DateTime StartOfWeek(DateTime Value, DayOfWeek startOfWeek)
        {
            int diff = Value.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return Value.AddDays(-1 * diff).Date;
        }

        public static int QuarterDifference(DateTime first, DateTime second)
        {
            int firstQuarter = getQuarter(first);
            int secondQuarter = getQuarter(second);
            return 1 + Math.Abs(secondQuarter - firstQuarter);
        }

        private static int getQuarter(DateTime date)
        {
            return (date.Year * 4) + ((date.Month - 1) / 3);
        }

        public static DateTime FirstDayOfQuarter(DateTime Value)
        {
            int quarterNumber = (Value.Month - 1) / 3 + 1;

            return new DateTime(Value.Year, 
                (quarterNumber - 1) * 3 + 1, 1);
        }

        public static DateTime LastDayOfQuarter(DateTime firstDayOfQuarter)
        {
            return firstDayOfQuarter.AddMonths(3).AddDays(-1);
        }

        public static DateTime FirstDayOfYear(DateTime Value)
        {
            return new DateTime(Value.Year, 1, 1);
        }

        public static DateTime LastDayOfYear(DateTime Value)
        {
            return new DateTime(Value.Year, 12, 31);
        }
    }
    #endregion

    public class ValidationError
    {
        public string ErrorMessage { get; set; }
    }

    public class Config : IConfig
    {
        public string AppDataDownloadsFolder { get; set; }
        public string ErrorScreenCaptureFolder { get; set; }
        public string DataExtractionLogFile { get; set; }
        public string UploadsFolder { get; set; }
        public string ComplianceFormFolder { get; set; }
        public string ExcelTempateFolder { get; set; }
        public string AttachmentsFolder { get; set; }
        public string WordTemplateFolder { get; set; }
        public string CIILFolder { get; set; }
        public string OutputFileFolder { get; set; }

        public string ExclusionDatabaseFolder { get;  set; }

        public string FDAWarningLettersFolder { get; set;}

        public string SAMFolder { get; set; }

        public string SDNFolder{ get; set; }
    }

    #region ReadExcelInput
    public class ExcelInput
    {
        public List<ExcelInputRow> ExcelInputRows { get; set; } 
            = new List<ExcelInputRow>();
    }

    public class ExcelInputRow
    {
        public string Role { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public string SponsorProtocolNumber { get; set; }
        public string SponsorProtocolNumber2 { get; set; }
        public string DisplayName { get; set; }
        public string InvestigatorID { get; set; }
        public string MemberID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string InstituteName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string MedicalLicenseNumber { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public string FullName {
            get {
                var fullName = "";
                fullName = FirstName.Trim() + " " + MiddleName.Trim() + " " + LastName.Trim();
                return fullName.Trim();
            }
        }

        public string Address {
            get {
                var address = "";
                if (AddressLine1.Trim().Length > 0)
                    address += AddressLine1 + " ";
                if(AddressLine2.Trim().Length > 0)
                    address += AddressLine2 + " ";
                if(City.Trim().Length > 0)
                    address += City + " ";
                if(State.Trim().Length > 0)
                    address += State + " ";
                if(PostalCode.Trim().Length > 0)
                    address += PostalCode;

                return address.Trim();
            }
        }
    }
    #endregion

    #region Audit
    
    public class Audit
    {
        public Guid? RecId { get; set; }
        public Guid ComplianceFormId { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedOn { get; set; }
        public string Auditor { get; set; }
        public bool IsSubmitted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string AuditorComments { get; set; }
        public string RequestorComments { get; set; }
        public string AuditStatus { get; set; }
        public List<AuditObservation> Observations { get; set; }
    }
    
    public class AuditObservation
    {
        public int SiteId { get; set; }
        public string SiteShortName { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
    #endregion

    #region Assignment History

    public class AssignmentHistory
    {
        public Guid RecId { get; set; }
        public Guid ComplianceFormId { get; set; }
        public DateTime AssignedOn { get; set; }
        //public DateTime? RemovedOn { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedBy { get; set; }
        public string PreviouslyAssignedTo { get; set; }
    }

    #endregion
}