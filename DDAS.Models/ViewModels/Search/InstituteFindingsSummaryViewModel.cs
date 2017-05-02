using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class InstituteFindingsSummaryViewModel
    {
        public int SiteSourceId { get; set; }
        public Guid? SiteId { get; set; }
        public int DisplayPosition { get; set; }

        public bool IsMandatory { get; set; }
        public string SiteName { get; set; }
        public string SiteShortName { get; set; }
        public string SiteUrl { get; set; }
        public int IssuesFound { get; set; }

    }
}
