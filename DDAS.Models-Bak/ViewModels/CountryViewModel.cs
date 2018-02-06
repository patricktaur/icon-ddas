using DDAS.Models.Enums;
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
        public string CountryName { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public Guid? RecId { get; set; }

        public string ExtractionMode { get; set; }
        public string SearchAppliesToText { get; set; }
        public SearchAppliesToEnum SearchAppliesTo { get; set; }

        public bool IsMandatory { get; set; }
    }
}
