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

                string[] Formats =
                    { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                    "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy", "M/d/yy" };

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
