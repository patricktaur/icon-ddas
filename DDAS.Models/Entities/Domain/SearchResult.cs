using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
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
        public List<SearchQuerySite> SearchSites { get; set; }
    }

    //For multiple sites request
    public class SearchQuerySite
    {
        public string SiteName { get; set; }
        //public bool IsOptional { get; set; }
        public string ExtractionMode { get; set; }
        public bool Mandatory { get; set; }
        public string SiteShortName { get; set; }
        //public bool Selected { get; set; }
        public SiteEnum SiteEnum { get; set; }
        public string SiteUrl { get; set; }
        public string SearchTimeTakenInMs { get; set; }
        public bool HasErrors { get; set; } = false;
        public string ErrorDescription { get; set; }
        public List<MatchResult> Results { get; set; }

    }

    public class SiteScan : SearchQuerySite
    {
        public Guid? DataId { get; set; }
        public DateTime DataExtractedOn { get; set; }
        public DateTime? SiteLastUpdatedOn { get; set; }
        public int FullMatch { get; set; }
        public int PartialMatch { get; set; }
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
        }
        
        public Guid? RecId { get; set; }
        public string AssignedTo { get; set; }
        public bool Active { get; set; } = true;
        public string SponsorProtocolNumber { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string ProjectNumber { get; set; }
        public string Institute { get; set; }
        public DateTime SearchStartedOn { get; set; }

        public DateTime? ExtractedOn { get; set; } //null indicates 'Not extracted' 
        public int ExtractionErrorInvestigatorCount { get; set; }
        public int FullMatchesFoundInvestigatorCount { get; set; }
        public int PartialMatchesFoundInvestigatorCount { get; set; }
        public int IssuesFoundInvestigatorCount { get; set; }
        public int ReviewCompletedInvestigatorCount { get; set; }

        public List<InvestigatorSearched> InvestigatorDetails { get; set; }
        //Patrick 27NOvb2016
        public List<SiteSource> SiteSources { get; set; }
        public List<Finding> Findings { get; set; }
        //patrick 31Dec2016
        public string Status { get {
                if (_Status == null)
                {
                    setStatusNStatusEnum();
                }
                return _Status;
            } }
        public ComplianceFormStatusEnum StatusEnum { get {
                if (_Status == null)
                {
                    setStatusNStatusEnum();
                }
                return _StatusEnum;
            } }
        private string _Status;
        private ComplianceFormStatusEnum _StatusEnum;
        private void setStatusNStatusEnum()
        {
            var ReviewCompleted = false;
            if (ReviewCompletedInvestigatorCount == InvestigatorDetails.Count)
            {
                ReviewCompleted = true;
            }
            if (ReviewCompleted == true)
            {
                _Status = "Review completed, Issues Not Identified";
                _StatusEnum = ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified;
                if (IssuesFoundInvestigatorCount > 0)
                {
                    _Status = "Review completed, Issues Identified";
                    _StatusEnum = ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified;
                }
            }
            else if (ExtractedOn == null)
            {
                _Status = "Data not extracted";
                _StatusEnum = ComplianceFormStatusEnum.NotScanned;
            }
            else
            {
                if (FullMatchesFoundInvestigatorCount > 0)
                {
                    _Status = "Full Match Found, Review Pending";
                    _StatusEnum = ComplianceFormStatusEnum.FullMatchFoundReviewPending;
                }
                else if (FullMatchesFoundInvestigatorCount > 0)
                {
                    _Status = "Partial Match Found, Review Pending";
                    _StatusEnum = ComplianceFormStatusEnum.PartialMatchFoundReviewPending;
                }
                else
                {
                    _Status = "No Match Found, Review Pending";
                    _StatusEnum = ComplianceFormStatusEnum.NoMatchFoundReviewPending;
                }
            }
        }

        
    }

    public class PrincipalInvestigator
    {
        public Guid? RecId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string SponsorProtocolNumber { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string ProjectNumber { get; set; }
        public DateTime SearchStartedOn { get; set; }
        public string Status { get; set; }
        public ComplianceFormStatusEnum StatusEnum { get; set; }
        public string AssignedTo { get; set; }
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
        public string Role { get; set; }
        public string Qualification { get; set; }
        public string MedicalLiceseNumber { get; set; }
        public string InvestigatorId { get; set; }

        public DateTime? ExtractedOn { get; set; } //null indicates 'Not extracted' 
        public bool HasExtractionError { get; set; }
        public int ExtractionErrorSiteCount { get; set; }
        //public int MatchesFoundSiteCount { get; set; }

        public int Sites_FullMatchCount { get; set; }
        public int Sites_PartialMatchCount { get; set; }
        public int IssuesFoundSiteCount { get; set; }
        public int ReviewCompletedSiteCount { get; set; }

        public int TotalIssuesFound { get; set; }
        public int ReviewCompletedCount { get; set; }

        //Patrick 27NOvb2016: - to be removed:
        public List<SitesIncludedInSearch> SiteDetails { get; set; }

        public bool Deleted { get; set; }
        public List<SiteSearchStatus> SitesSearched { get; set; }
  
    }

  
    //Patrick 01Dec2016 - new props added:
    public class SiteSearchStatus
    {
        public int DisplayPosition { get; set; }
        public SiteEnum siteEnum { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public DateTime? ExtractedOn { get; set; } //null indicates 'Not extracted' or has errors.
        public bool HasExtractionError { get; set; }
        public string ExtractionErrorMessage { get; set; }
        public int FullMatchCount { get; set; }
        public int PartialMatchCount { get; set; }
        public int IssuesFound { get; set; }
        public bool ReviewCompleted { get; set; }

        public DateTime? SiteSourceUpdatedOn { get; set; }
        public string ExtractionMode { get; set; }
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
    
    #endregion

    #region ByPatrick
    //Patrick 28Nov2016
    public class MatchedRecord
    {
        public int MatchCount { get; set; }
        public int RowNumber { get; set; }
        public string RecordDetails { get; set; }
        public List<Link> Links { get; set; }
    }

    public class SiteSource
    {
        public int Id { get; set; }
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
    }

    //Patrick 27Nov2016
    public class Finding
    {
        //Patrick 04Dec2016:
        //public int SiteSourceId { get; set; }
        
        //Pradeep 2Dec2016
        public SiteEnum SiteEnum { get; set; }

        public int InvestigatorSearchedId { get; set; }
        public int MatchCount { get; set; }
        public int RowNumberInSource { get; set; }
        public string Observation { get; set; }
        public string RecordDetails { get; set; }
        //???:
        public string Status { get; set; }
        ////Patrick 04Dec2016: no longer required, can be deleted.  Comp Form collection in MOngodB has to be dropped.
        public string HiddenStatus { get; set; }
        
        //Patrick 04Dec2016 added:
        public bool Selected { get; set; }
        public bool IsMatchedRecord { get; set; }
        public int SourceNumber { get; set; }
        public DateTime? DateOfInspection { get; set; }
        public string InvestigatorName { get; set; }
        public bool IsAnIssue { get;  set;}

        public List<Link> Links { get; set; } = new List<Link>();
    }

    public class ComplianceFormManage
    {
        public string AssignedTo { get; set; }
        public bool Active { get; set; }
    }

        #endregion

        #region Save Results

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

        public Role(){}
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

}
