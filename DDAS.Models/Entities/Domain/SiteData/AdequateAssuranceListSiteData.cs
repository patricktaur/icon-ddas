﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    //Int64? required for mongodb go generate ids
    public class AdequateAssuranceListSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public AdequateAssuranceListSiteData()
        {
            AdequateAssurances = new List<AdequateAssuranceList>();
        }
        public Guid? RecId { get; set; }
        public List<AdequateAssuranceList> AdequateAssurances { get; set; }
    }

    public class AdequateAssuranceList : SiteDataItemBase
    {
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
                    "Full Name: " + FullName + "~" +
                    "Name And Address: " + NameAndAddress + "~" +
                    "Center: " + Center + "~" +
                    "Type: " + Type + "~" +
                    "Action Date: " + ActionDate + "~" +
                    "Comments: " + Comments;
            }
        }
    }


}
