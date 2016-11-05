using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ERRProposalToDebarPageSiteData //: AuditEntity<long?>
    {
        public ERRProposalToDebarPageSiteData()
        {
            ProposalToDebar = new List<ProposalToDebar>();
        }
        public Guid? RecId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public List<ProposalToDebar> ProposalToDebar { get; set; }
    }

    public class ProposalToDebar : SiteDataItemBase
    {
        public int RowNumber { get; set; }
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
    }
}
