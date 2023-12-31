﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    //Int64? required for mongodb go generate ids
    public class FDADebarPageSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public FDADebarPageSiteData()
        {
            DebarredPersons = new List<DebarredPerson>();
        }
        public Guid? RecId { get; set; }
        //BaseClass properties do not get serialized, hence two ready only properties are added:
        public DateTime ExtractedOn { get { return CreatedOn; } }
        public new DateTime? SiteLastUpdatedOn { get { return base.SiteLastUpdatedOn; } }
        public List<DebarredPerson> DebarredPersons { get; set; }
    }

    public class DebarredPerson : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string NameOfPerson { get; set; }
        public string EffectiveDate { get; set; }
        public string EndOfTermOfDebarment { get; set; }
        public string FrDateText { get; set; }
        public string VolumePage { get; set; }
        public string DocumentLink { get; set; }
        public string DocumentName { get; set; }

        public override string FullName {
            get {
                return NameOfPerson; 
            }
        }

        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return
                    "Name Of Person: " + FullName + "~" +
                    "Effective Date: " + EffectiveDate + "~" +
                    "End/Term Of Debarment: " + EndOfTermOfDebarment + "~" +
                    "FR Date.txt: " + FrDateText + "~" +
                    "VOLUMEPAGE.pdf: " + VolumePage + "~" +
                    "Document Link: " + DocumentLink + "~" +
                    "Document Name: " + DocumentName;
            }
        }

        public override DateTime? DateOfInspection {
            get
            {
                if (EffectiveDate == null || EffectiveDate == "")
                    return null;

                string[] Formats =
                    { "M/d/yyyy", "M-d-yyyy" };
                DateTime DateValue;
                if (DateTime.TryParseExact(EffectiveDate.Trim(), Formats, null, System.Globalization.DateTimeStyles.None, out DateValue))
                {
                    return DateValue;
                    //return DateTime.ParseExact(
                    //EffectiveDate.Trim(), Formats, null,
                    //System.Globalization.DateTimeStyles.None);
                }else
                {
                    return null;
                }
            }
                
            
        }
    }
}
