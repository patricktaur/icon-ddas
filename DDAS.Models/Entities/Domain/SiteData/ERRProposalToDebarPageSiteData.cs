using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ERRProposalToDebarPageSiteData : AuditEntity<long?>
    {
        public ERRProposalToDebarPageSiteData()
        {
            ProposalToDebar = new List<ProposalToDebar>();
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public ICollection<ProposalToDebar> ProposalToDebar { get; set; }
    }
    public class ProposalToDebar
    {
        public int RowNumber { get; set; }
        public string name { get; set; }
        public string center { get; set; }
        public string date { get; set; }
        public string IssuingOffice { get; set; }
    }
}
