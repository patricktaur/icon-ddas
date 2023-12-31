﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class SystemForAwardManagementPageSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public SystemForAwardManagementPageSiteData()
        {
            SAMSiteData = new List<SystemForAwardManagement>();
        }
        public Guid? RecId { get; set; }
        //BaseClass properties do not get serialized, hence two ready only properties added:
        public DateTime ExtractedOn { get { return CreatedOn; } }
        public new DateTime? SiteLastUpdatedOn { get { return base.SiteLastUpdatedOn; } }

        public ICollection<SystemForAwardManagement> SAMSiteData { get; set; }
    }

    public class SystemForAwardManagement : SiteDataItemBase
    {
        //commenting below code to reduce doc size in mongo
        //public int RowNumber { get; set; }
        //public string Status { get; set; }
        //public string Entity { get; set; }
        //public string Duns { get; set; }
        //public string HasActiveExclusion { get; set; }
        //public string ExpirationDate { get; set; }
        //public string PurposeOfRegistration { get; set; }
        //public string CAGECode { get; set; }
        //public string DoDAAC { get; set; }
        //public string DelinquentFederalDebt { get; set; }
        //public string Classification { get; set; }
        //public string ActivationDate { get; set; } = "";
        //public string TerminationDate { get; set; }

        //As per the SAM_Exclusions_Public_Extract file
        public Guid? RecId { get; set; }
        public Guid? ParentId { get; set; }

        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }
        public string Name { get; set; }
        public string ExcludingAgency { get; set; }
        public string ExclusionType { get; set; }
        public string AdditionalComments { get; set; }
        public string ActiveDate { get; set; }
        public string RecordStatus { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        //return Entity for live search
        public override string FullName {
            get {
                //First = First + "";
                //Middle = Middle + "";
                //Last = Last + "";
                //return First.Trim() + " " + Middle.Trim() + " " + Last.Trim();
                //return Entity;
                var FullName = "";
                if (First != null && First.Trim().Length > 0)
                    FullName += First.Trim();
                if (Middle != null && Middle.Trim().Length > 0)
                    FullName += " " + Middle.Trim();
                if (Last != null && Last.Trim().Length > 0)
                    FullName += " " + Last.Trim();
                return FullName;
            }
        }

        public override string RecordDetails
        {
            get
            {
                //Uncomment beow section for live search
                //return
                //    "Name: " + Entity + "~" +
                //    "Duns: " + Duns + "~" +
                //    "Has Active Exclusion?: " + HasActiveExclusion + "~" +
                //    "Expiration Date: " + ExpirationDate + "~" +
                //    "Purpose Of Registration: " + HasActiveExclusion + "~" +
                //    "CAGE Code: " + HasActiveExclusion + "~" +
                //    "DoDAAC: " + HasActiveExclusion + "~" +
                //    "Delinquent Federal Debt?: " + HasActiveExclusion + "~" +
                //    "Classification: " + HasActiveExclusion + "~" +
                //    "Activation Date: " + HasActiveExclusion + "~" +
                //    "Termination Date: " + HasActiveExclusion;

                return
                    "First: " + First.Trim() + "~" +
                    "Middle: " + Middle.Trim() + "~" +
                    "Last: " + Last.Trim() + "~" +
                    "City: " + City.Trim() + "~" +
                    "State: " + State.Trim() + "~" +
                    "Country: " + Country.Trim() + "~" +
                    "Excluding Agency: " + ExcludingAgency + "~" +
                    "Exclusion Type: " + ExclusionType + "~" +
                    "Additional Comments: " + AdditionalComments + "~" +
                    "Active Date: " + ActiveDate + "~" +
                    "Record Status: " + RecordStatus;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                //Uncomment below code for live search
                //if (ActivationDate == "" || ActivationDate == null ||
                //    ActivationDate.Length < 3)
                //    return null;

                //return DateTime.ParseExact(ActivationDate.Trim(),
                //    "M'/'d'/'yyyy", null,
                //    System.Globalization.DateTimeStyles.None);

                if (ActiveDate == "" || ActiveDate == null ||
                    ActiveDate.Length < 3)
                    return null;

                string[] Formats =
                    { "M-d-yyyy", "M/d/yyyy"};

                return DateTime.ParseExact(
                    ActiveDate.Trim(), Formats, null,
                    System.Globalization.DateTimeStyles.None);
            }
        }
    }
}
