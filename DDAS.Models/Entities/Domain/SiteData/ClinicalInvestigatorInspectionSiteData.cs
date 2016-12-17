using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ClinicalInvestigatorInspectionSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public ClinicalInvestigatorInspectionSiteData()
        {
            ClinicalInvestigatorInspectionList = new List<ClinicalInvestigator>();
        }
        public Guid? RecId { get; set; }
        public List<ClinicalInvestigator> ClinicalInvestigatorInspectionList { get; set; }
    }

    public class ClinicalInvestigator : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string IdNumber { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string InspectionDate { get; set; }
        public string ClassificationType { get; set; }
        public string ClassificationCode { get; set; }
        public string DeficiencyCode { get; set; }

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
                    "Full Name: " + FullName + "~" +
                    "Name: " + Name + "~" +
                    "Id Number: " + IdNumber + "~" +
                    "Location: " + Location + "~" +
                    "Address: " + Address + "~" +
                    "City: " + City + "~" +
                    "State: " + State + "~" +
                    "Country: " + Country + "~" +
                    "Zip: " + Zip + "~" +
                    "Inspection Date: " + InspectionDate + "~" +
                    "Classification Type: " + ClassificationType + "~" +
                    "Classification Code: " + ClassificationCode + "~" +
                    "Deficiency Code: " + DeficiencyCode; ;
            }
        }
    }
}
