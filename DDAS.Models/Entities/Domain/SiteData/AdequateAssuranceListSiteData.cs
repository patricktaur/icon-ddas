﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    //Int64? required for mongodb go generate ids
    public class AdequateAssuranceListSiteData //: AuditEntity<long?>
    {
        public AdequateAssuranceListSiteData()
        {
            AdequateAssurances = new List<AdequateAssuranceList>();
        }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Guid RecId { get; set; }

        public DateTime SiteLastUpdatedOn { get; set; }
        public ICollection<AdequateAssuranceList> AdequateAssurances { get; set; }
    }

    public class AdequateAssuranceList
    {
        public int Matched { get; set; }
        public int RowNumber { get; set; }
        public string NameAndAddress { get; set; }
        public string Center { get; set; }
        public string Type { get; set; }
        public string ActionDate { get; set; }
        public string Comments { get; set; }
    }


}