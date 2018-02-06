using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class SAMSiteData : SiteDataItemBase
    {
        public Guid? RecId { get; set; }
        public Guid? ParentId { get; set; }
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }
        //public string Name { get; set; }
        public string ExcludingAgency { get; set; }
        public string ExclusionType { get; set; }
        public string AdditionalComments { get; set; }
        public string ActiveDate { get; set; }
        public string RecordStatus { get; set; }

        public override string FullName
        {
            get
            {
                var fullName = "";
                if (First != null && First.Trim().Length > 0)
                    fullName += First.Trim();
                if (Middle != null && Middle.Trim().Length > 0)
                    fullName += " " + Middle.Trim();
                if (Last != null && Last.Trim().Length > 0)
                    fullName += " " + Last.Trim();
                return fullName;
            }
        }

        public override string RecordDetails
        {
            get
            {
                return
                    "First: " + First.Trim() + "~" +
                    "Middle: " + Middle.Trim() + "~" +
                    "Last: " + Last.Trim() + "~" +
                    //"Last: " + Name.Trim() + "~" +
                    "Excluding Agency: " + ExcludingAgency + "~" +
                    "Exclusion Type: " + ExclusionType + "~" +
                    "Additional Comments: " + AdditionalComments + "~" +
                    "Active Date: " + ActiveDate + "~" +
                "Record Status: " + RecordStatus;
            }
        }

        public override DateTime? DateOfInspection
        {
            get
            {
                if (ActiveDate == "" || ActiveDate == null ||
                    ActiveDate.Length < 3)
                    return null;

                return DateTime.ParseExact(ActiveDate.Trim(),
                    "M'/'d'/'yyyy", null,
                    System.Globalization.DateTimeStyles.None);
            }
        }
    }
}
