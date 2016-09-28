using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ClinicalInvestigatorDisqualificationSiteData : AuditEntity<long?>
    {
        public ClinicalInvestigatorDisqualificationSiteData()
        {
            DisqualifiedInvestigatorList = new List<DisqualifiedInvestigator>();
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public ICollection<DisqualifiedInvestigator> DisqualifiedInvestigatorList { get; set; }
        public string Source { get; set; }
    }

    public class DisqualifiedInvestigator
    {
        public int RowNumber { get; set; }
        public string Name { get; set; }
        public string Center { get; set; }
        public string Status { get; set; }
        public string DateOfStatus { get; set; }
        public string DateNIDPOEIssued { get; set; }
        public string DateNOOHIssued { get; set; }
        public string LinkToNIDPOELetter { get; set; }
        public string LinkToNOOHLetter { get; set; }
    }
}
