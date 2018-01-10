using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class QCSummaryViewModel
    {
        public string Investigator { get; set; }
        public string SourceName { get; set; }
        public string CategoryEnumString { get; set; }
        public string Comment { get; set; }
        public string ResponseToQC { get; set; }
    }
}
