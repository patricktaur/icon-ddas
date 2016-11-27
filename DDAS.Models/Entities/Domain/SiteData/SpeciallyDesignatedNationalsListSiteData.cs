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
        public Guid? RecId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public bool DataExtractionStatus { get; set; }
        public string DataExtractionMessage { get; set; }
        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public List<SDNList> SDNListSiteData { get; set; }
    }

    public class SDNList : SiteDataItemBase
    {
        public int RowNumber { get; set; }
        public string Status { get; set; }
        public int RecordNumber { get; set; }
        public string Name { get; set; }
        public int PageNumber { get; set; }
        public string WordsMatched { get; set; }
        
        public override string FullName {
            get {
                return Name;
            }
        }
    }
}
