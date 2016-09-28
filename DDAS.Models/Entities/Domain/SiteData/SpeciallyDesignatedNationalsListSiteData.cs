using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class SpeciallyDesignatedNationalsListSiteData
    {
        public SpeciallyDesignatedNationalsListSiteData()
        {
            SDNListSiteData = new List<SDNList>();
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public ICollection<SDNList> SDNListSiteData { get; set; }
    }

    public class SDNList
    {
        public int RowNumber { get; set; }
        public string Names { get; set; }
        public int PageNumbers { get; set; }
        public string WordsMatched { get; set; }
    }
}
