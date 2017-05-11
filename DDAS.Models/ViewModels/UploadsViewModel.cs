using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class UploadsViewModel
    {
        public string UploadedFileName { get; set; }
        public string AssignedTo { get; set; }
        public string GeneratedFileName { get; set; }
        public DateTime? UploadedOn { get; set; }
    }
}
