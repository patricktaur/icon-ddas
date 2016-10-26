using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ClinicalInvestigatorDisqualificationSiteData //: AuditEntity<long?>
    {
        public ClinicalInvestigatorDisqualificationSiteData()
        {
            DisqualifiedInvestigatorList = new List<DisqualifiedInvestigator>();
        }

        public Guid? RecId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime SiteLastUpdatedOn { get; set; }
        public List<DisqualifiedInvestigator> 
            DisqualifiedInvestigatorList { get; set; }
        public string Source { get; set; }
    }

    public class DisqualifiedInvestigator : SiteDataItemBase
    {
        public string Name { get; set; }
        public string Center { get; set; }
        public string DateOfStatus { get; set; }
        public string DateNIDPOEIssued { get; set; }
        public string DateNOOHIssued { get; set; }
        public string LinkToNIDPOELetter { get; set; }
        public string LinkToNOOHLetter { get; set; }

        public override string FullName {
            get {
                return Name;
            }
        }
    }
}
