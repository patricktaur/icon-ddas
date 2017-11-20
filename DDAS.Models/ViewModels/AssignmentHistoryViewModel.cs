using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class AssignmentHistoryViewModel
    {
        public string PrincipalInvestigator { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectNumber2 { get; set; }
        public string AssignedBy { get; set; }
        public DateTime AssignedOn { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? RemovedOn { get; set; }
    }
}
