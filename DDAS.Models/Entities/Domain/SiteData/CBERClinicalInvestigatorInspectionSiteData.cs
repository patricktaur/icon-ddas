using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class CBERClinicalInvestigatorInspectionSiteData : AuditEntity<long?>
    {

        public CBERClinicalInvestigatorInspectionSiteData()
        {
            ClinicalInvestigator = new List<CBERClinicalInvestigator>();
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public ICollection<CBERClinicalInvestigator> ClinicalInvestigator { get; set; }
        public string Source { get; set; }
    }

    public class CBERClinicalInvestigator
    {
        public int RowNumber { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string InstituteAndAddress { get; set; }
        public string InspectionStartAndEndDate { get; set; }
        public string Classification { get; set; }
    }
}

