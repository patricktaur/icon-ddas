using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain
{

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

        public string SiteName { get; set; }
        public List<MatchResult> Results { get; set; }
    }

    public class MatchResult
    {
        public string MatchName { get; set; }
        public string MatchLocation { get; set; }
    }

    
}
