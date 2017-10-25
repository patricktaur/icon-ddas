using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class OpenInvestigationsViewModel
    {
        public string AssignedTo { get; set; }
        public int Count { get; set; }
        public DateTime? Earliest { get; set; }
        public DateTime? Latest { get; set; }
    }
}
