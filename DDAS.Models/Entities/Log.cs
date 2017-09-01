using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Repository
{
    public class Log
    {
        //public Guid? RecId { get; set; }
        public Guid Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Caption { get; set; }
        public string Message { get; set; }
    }
}
