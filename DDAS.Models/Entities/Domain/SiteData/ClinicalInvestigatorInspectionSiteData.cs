using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ClinicalInvestigatorInspectionSiteData : AuditEntity<long?>
    {
        public ClinicalInvestigatorInspectionSiteData()
        {
            ClinicalInvestigatorInspectionList = new List<ClinicalInvestigator>();
        }
        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public List<ClinicalInvestigator> ClinicalInvestigatorInspectionList { get; set; }
    }

    public class ClinicalInvestigator
    {
        public string SiteQuery { get; set; }
        public int RowNumber { get; set; }
        public string IdNumber { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public string InspectionDate { get; set; }
        public string Type { get; set; }
        public string Class { get; set; }
        public string DefTypes { get; set; }
    }
}
