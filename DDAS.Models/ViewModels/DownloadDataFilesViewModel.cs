using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class DownloadDataFilesViewModel
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        //public string SiteName { get; set; }
        public DateTime DownloadedOn { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
    }
}
