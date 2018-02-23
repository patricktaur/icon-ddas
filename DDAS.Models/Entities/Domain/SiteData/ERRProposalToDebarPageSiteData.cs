using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ERRProposalToDebarPageSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public ERRProposalToDebarPageSiteData()
        {
            ProposalToDebar = new List<ProposalToDebar>();
        }
        public Guid? RecId { get; set; }
        //BaseClass properties do not get serialized, hence two ready only properties added:
        public DateTime ExtractedOn { get { return CreatedOn; } }
        public new DateTime? SiteLastUpdatedOn { get { return base.SiteLastUpdatedOn; } }

        public List<ProposalToDebar> ProposalToDebar { get; set; }
    }

    public class ProposalToDebar : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string center { get; set; }
        public string date { get; set; }
        public string IssuingOffice { get; set; }

        public override string FullName {
            get {
                return Name;
            }
        }

        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return
                    "Name: " + Name + "~" +
                    "Center: " + center + "~" +
                    "Date: " + date + "~" +
                    "Issuing Office: " + IssuingOffice;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                if (date == "" || date == null)
                    return null;

                string[] Formats = {
                    "M-d-yy", "M-d-yyyy",
                    "M/d/yyyy", "M/d/yy" };

                return DateTime.ParseExact(
                    date.Trim(), Formats, null,
                    System.Globalization.DateTimeStyles.None);
                //return DateTime.ParseExact(date.Trim(),
                //    "M'/'d'/'yy", null,
                //    System.Globalization.DateTimeStyles.None);
            }
        }
    }
}
