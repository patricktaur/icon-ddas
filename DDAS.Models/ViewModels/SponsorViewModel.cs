using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class SponsorProtocolViewModel
    {
        public string SponsorProtocolNumber { get; set; }
        public string SiteName { get; set; }
        public Guid? SiteId { get; set; }
        public Guid? RecId { get; set; }
    }
}
