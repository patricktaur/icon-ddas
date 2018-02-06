using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain
{
    public class ScanHistory
    {
        public SiteEnum siteEnum { get; set; }
        public DateTime scannedAt { get; set; }
    }
}
