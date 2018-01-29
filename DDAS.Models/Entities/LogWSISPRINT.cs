using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities
{
    public class LogWSISPRINT
    {
        public Guid? RecId { get; set; }
        public DateTime CreatedOn { get; set; }

        public string RequestPayload { get; set; }
        public string Response { get; set; }
        public string Status { get; set; }
    }
}
