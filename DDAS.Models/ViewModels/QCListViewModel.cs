using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class QCListViewModel
    {
        public Guid? RecId { get; set; }
        public Guid ComplianceFormId { get; set; }
        public string Requester { get; set; }
        public string RequesterFullName { get; set; }
        public DateTime RequestedOn { get; set; }
        public string QCVerifier { get; set; }
        public string QCVerifierFullName { get; set; }
        public DateTime? CompletedOn { get; set; }
        public ReviewStatusEnum Status { get; set; }
        public string PrincipalInvestigator { get; set; }
        public List<string> SubInvestigators { get; set; } = new List<string>();
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
    }
}
