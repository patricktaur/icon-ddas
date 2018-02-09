using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class AdminDashboardDrillDownViewModel
    {
        public string UserFullName { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public string PrincipalInvestigator { get; set; }
        public int InvestigatorCount { get; set; }
        public string SponsorProtocolNumber { get; set; }
        public string SponsorProtocolNumber2 { get; set; }
        public string ComplianceFormStatus { get; set; }
    }
}
