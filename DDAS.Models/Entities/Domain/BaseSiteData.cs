using DDAS.Models.Entities.Domain.SiteData;
using System;
using System.Collections.Generic;

namespace DDAS.Models.Entities.Domain
{
    public class BaseSiteData
    {
        //public Guid? RecId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        //public DateTime UpdatedOn { get; set; }
        //public string UpdatedBy { get; set; }

        public Guid? ReferenceId { get; set; }
        public bool DataExtractionRequired { get; set; }
        public bool DataExtractionSucceeded { get; set; }
        public string DataExtractionErrorMessage { get; set; }
        public DateTime? SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public virtual List<SiteDataItemBase> Records { get; } 
    }
}
