using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class CountryViewModel
    {
        public Guid? SiteId { get; set; }
        public string Name { get; set; }
        public string SiteName { get; set; }
        public Guid? RecId { get; set; }
    }
}
