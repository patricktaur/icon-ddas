using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class StudySpecificInvestigatorVM
    {
        public string InvestigatorName { get; set; }
        public DateTime ReviewCompletedOn { get; set; }
        public string FindingStatus { get; set; }
        public string AssignedTo { get; set; }
    }
}
