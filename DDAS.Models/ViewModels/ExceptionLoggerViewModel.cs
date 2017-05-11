using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class ExceptionLoggerViewModel
    {
        public string Address { get; set; }
        public string UserId { get; set; }
        public string Request { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime AddedOn { get; set; }
        public long Id { get; set; }
    }
}
