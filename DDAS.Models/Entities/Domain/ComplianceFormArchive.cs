using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain
{
    public class ComplianceFormArchive 
    {
        public DateTime ArchivedOn { get; set; }

       
        public string SponsorProtocolNumber { get; set; }
        public string SponsorProtocolNumber2 { get; set; }
        
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public DateTime SearchStartedOn { get; set; }
        public string Status { get; set; }
        public string AssignedToFullName { get; set; }
        public string Reviewer { get; set; }
        public bool ReviewCompleted { get; set; }
        
        public DateTime? ReviewCompletedOn { get; set; }

        public ComplianceForm ComplianceForm { get; set; }
    }
}
