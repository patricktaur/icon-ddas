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
        public string SiteShortName { get; set; }
        public bool Selected { get; set; }
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
        public DateTime SiteLastUpdatedOn { get; set; }
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
        public List<SearchSummaryItem> SearchSummaryItems { get; set; }
    }
    public class SearchSummaryItem
    {
        public Guid? RecId { get; set; }
        public string SiteName { get; set; }
        public SiteEnum SiteEnum { get; set; }
        public string SiteUrl { get; set; }
        public string MatchStatus { get; set; }
        public int FullMatch { get; set; }
        public int PartialMatch { get; set; }
    }
    #endregion

    #region ComplianceFormDetails
    
    public class ComplianceForm
    {
        public Guid? RecId { get; set; }
        public string SponsorProtocolNumber { get; set; }
        public string Country { get; set; }
        public string NameToSearch { get; set; }
        public string Address { get; set; }
        public string ProjectNumber { get; set; }
        public DateTime SearchStartedOn { get; set; }
        public DateTime SearchClosedOn { get; set; }
        public List<SitesIncludedInSearch> SiteDetails { get; set; }
    }

    public class SitesIncludedInSearch
    {
        public string SiteName { get; set; }
        public SiteEnum SiteEnum { get; set; }
        public string SiteUrl { get; set; }
        public DateTime ScannedOn { get; set; }
        public int FullMatchCount { get; set; }
        public int PartialMatchCount { get; set; }
        public bool IssuesIdentified { get; set; }
        public string Findings { get; set; }
        //public string Issues { get; set; }
        public List<MatchedRecordsPerSite> MatchedRecords { get; set; }
    }

    public class MatchedRecordsPerSite
    {
        public string Issues { get; set; }
        public int IssueNumber { get; set; }
        public int RowNumber { get; set; }
        public string RecordDetails { get; set; }
        public string Status { get; set; }
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

        //public MongoDB.Bson.ObjectId _id { get; set; }

        //[BsonId]
        public Guid RoleId { get; set; }
        public string Name { get; set; }
        #endregion

        public ICollection<User> Users
        {
            get { return _users ?? (_users = new List<User>()); }
            set { _users = value; }
        }
    }

    
    public class UserRole
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
    }
    #endregion

}
