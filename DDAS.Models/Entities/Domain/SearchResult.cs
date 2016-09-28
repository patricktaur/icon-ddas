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
        public Guid ID { get; set; }
        public string MatchName { get; set; }
        public string MatchLocation { get; set; }
    }

    #endregion

    #region Revised
    public class SearchSummary
    {
        public SearchSummary()
        {
            SearchSummaryItems = new List<SearchSummaryItem>();
        }
        public string NameToSearch { get; set; }
        public List<SearchSummaryItem> SearchSummaryItems { get; set; }
    }
    public class SearchSummaryItem
    {
        public string SiteName { get; set; }
        public SiteEnum SiteEnum { get; set; }
        public string SiteUrl { get; set; }
        public string MatchStatus { get; set; }
    }

    public class AdequateAssuranceListResult: AdequateAssuranceList
    {
        public int MatchWeightage { get; set; }
    }

    public class FDADebarPageResult: DebarredPerson
    {
        public int MatchWeightage { get; set; }
    }
    #endregion
}
