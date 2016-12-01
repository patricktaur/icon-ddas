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

        public Guid? RecId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        //public DateTime UpdatedOn { get; set; }
        //public string UpdatedBy { get; set; }

        public Guid? ReferenceId { get; set; }
        public bool DataExtractionRequired { get; set; }
        public bool DataExtractionSucceeded { get; set; }
        public string DataExtractionErrorMessage { get; set; }
        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public List<AdequateAssuranceList> AdequateAssurances { get; set; }
    }

    public class AdequateAssuranceList : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string NameAndAddress { get; set; }
        public string Center { get; set; }
        public string Type { get; set; }
        public string ActionDate { get; set; }
        public string Comments { get; set; }

        public override string FullName {
            get {
                return NameAndAddress;
            }
        }

        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return 
                    "FullName: " + FullName + "~" +
                    "NameAndAddress: " + NameAndAddress + "~" +
                    "Center: " + Center + "~" +
                    "Type: " + Type + "~" +
                    "ActionDate: " + ActionDate + "~" +
                    "Comments: " + Comments;
            }
        }
    }


}
