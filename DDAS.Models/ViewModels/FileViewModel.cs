using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class FileViewModel
    {

        public string FileName { get; set; }
        public string Path { get; set; }
        public long FileSize { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
