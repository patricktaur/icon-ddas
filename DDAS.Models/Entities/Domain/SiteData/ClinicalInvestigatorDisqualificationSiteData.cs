using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ClinicalInvestigatorDisqualificationSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public ClinicalInvestigatorDisqualificationSiteData()
        {
            DisqualifiedInvestigatorList = new List<DisqualifiedInvestigator>();
        }
        public Guid? RecId { get; set; }
        public List<DisqualifiedInvestigator> 
            DisqualifiedInvestigatorList { get; set; }
    }

    public class DisqualifiedInvestigator : SiteDataItemBase
    {
       // public int RowNumber { get; set; }
        public string Status { get; set; }
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

        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return
                    "Name: " + Name + "~" +
                    "Center: " + Center + "~" +
                    "Date Of Status: " + DateOfStatus + "~" +
                    "Date NIDPOE Issued: " + DateNIDPOEIssued + "~" +
                    "Date NOOH Issued: " + DateNOOHIssued + "~" +
                    "Link To NIDPOE Letter: " + LinkToNIDPOELetter + "~" +
                    "Link To NOOH Letter: " + LinkToNOOHLetter;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                if (DateOfStatus == "")
                    return null;

                return DateTime.ParseExact(DateOfStatus,
                    "M'/'d'/'yyyy", null,
                    System.Globalization.DateTimeStyles.None);
            }
        }

        //public override List<Link> Links
        //{
        //    get
        //    {
        //        return new List<Link> {
        //            new Link { Title = "NIDPOE Letter", url = LinkToNIDPOELetter },
        //            new Link { Title = "NOOH Letter", url = LinkToNOOHLetter }
        //        };
        //    }
        //}
    }
}
