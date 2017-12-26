using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class InvestigatorFindingViewModel
    {
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public string InvestigatorName { get; set; }
        public string Role { get; set; }
        public string ReviewCompletedBy { get; set; }
        public DateTime ReviewCompletedOn { get; set; }
        public string SiteShortName { get; set; }
        public string FindingObservation { get; set; }
    }
}
