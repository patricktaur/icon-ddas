using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class ExtractionLogViewModel
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Step { get; set; }
        public string Status { get; set; }
        public string SiteEnumString { get; set; }
        public string Caption { get; set; }
        public string Message { get; set; }
    }
}
