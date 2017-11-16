using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class AuditListViewModel
    {
        public Guid RecId { get; set; }
        public Guid ComplianceFormId { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedOn { get; set; }
        public string Auditor { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string AuditStatus { get; set; }
        public string PrincipalInvestigator { get; set; }
        public string ProjectNumber { get; set; }
    }
}
