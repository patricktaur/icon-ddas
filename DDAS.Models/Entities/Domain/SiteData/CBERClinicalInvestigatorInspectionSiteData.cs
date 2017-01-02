using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class CBERClinicalInvestigatorInspectionSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public CBERClinicalInvestigatorInspectionSiteData()
        {
            ClinicalInvestigator = new List<CBERClinicalInvestigator>();
        }
        public Guid? RecId { get; set; }
        public List<CBERClinicalInvestigator> ClinicalInvestigator { get; set; }
    }

    public class CBERClinicalInvestigator : SiteDataItemBase
    {

        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string InstituteAndAddress { get; set; }
        public string InspectionStartAndEndDate { get; set; }
        public string Classification { get; set; }
        public override string FullName {
            get {
                return Name;
            }
        }
        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return
                    "Name: " + Name + "~" +
                    "Title: " + Title + "~" +
                    "Institute/Address: " + InstituteAndAddress + "~" +
                    "Inspection Start/End Date: " + InspectionStartAndEndDate + "~" +
                    "Classification: " + Classification;
            }
        }
        public override List<Link> Links
        {
            get
            {
                return new List<Link>();
            }
        }
    }
}

