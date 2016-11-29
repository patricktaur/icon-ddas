using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    //Int64? required for mongodb go generate ids
    public class FDADebarPageSiteData  //: AuditEntity<long?>
    {
        public FDADebarPageSiteData()
        {
            DebarredPersons = new List<DebarredPerson>(); 
        }
        public Guid? RecId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        //public DateTime UpdatedOn { get; set; }
        //public string UpdatedBy { get; set; }

        public Guid? ReferenceId { get; set; }
        public bool DataExtractionRequired { get; set; }
        public bool DataExtractionSucceeded { get; set; }
        public string DataExtractionErrorMessage { get; set; }
        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public List<DebarredPerson> DebarredPersons { get; set; }
    }

    public class DebarredPerson : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string NameOfPerson { get; set; }
        public string EffectiveDate { get; set; }
        public string EndOfTermOfDebarment { get; set; }
        public string FrDateText { get; set; }
        public string VolumePage { get; set; }
        public string DocumentLink { get; set; }
        public string DocumentName { get; set; }

        public override string FullName {
            get {
                return NameOfPerson; 
            }
        }

        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return "FullName: " + FullName + "~" +
                    "NameOfPerson: " + FullName + "~" +
                    "EffectiveDate: " + EffectiveDate + "~" +
                    "EndOfTermOfDebarment: " + EndOfTermOfDebarment + "~" +
                    "FrDateText: " + FrDateText + "~" +
                    "VolumePage: " + VolumePage + "~" +
                    "DocumentLink: " + DocumentLink + "~" +
                    "DocumentName: " + DocumentName; 
            }
        }

    }

    //Patrick Q - why inherit DebarredPerson ?
    public class FDADebarPageMatchRecords : DebarredPerson
    {
        public List<DebarredPerson> FDADebarMatchedRecords { get; set; }
    }

}
