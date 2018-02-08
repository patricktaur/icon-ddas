using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class SpeciallyDesignatedNationalsListSiteData : BaseSiteData
    {
        public SpeciallyDesignatedNationalsListSiteData()
        {
            SDNListSiteData = new List<SDNList>();
        }
        public Guid? RecId { get; set; }
        //BaseClass properties do not get serialized, hence two ready only properties added:
        public DateTime ExtractedOn { get { return CreatedOn; } }
        public new DateTime? SiteLastUpdatedOn { get { return base.SiteLastUpdatedOn; } }

        public List<SDNList> SDNListSiteData { get; set; }
    }

    public class SDNList : SiteDataItemBase
    {
        public Guid? RecId { get; set; }
        public Guid? ParentId { get; set; }

        //public int RowNumber { get; set; }
        public string Status { get; set; }
        //public int RecordNumber { get; set; }
        public string Name { get; set; }
        //public int PageNumber { get; set; }
        public string WordsMatched { get; set; }
        
        public override string FullName {
            get {
                return Name;
            }
        }

        public override string RecordDetails
        {
            get
            {
                return
                    "Name: " + Name + "~" +
                    //"Page Number: " + PageNumber + "~" +
                    "Record Number: " + RecordNumber;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                return null;
                //return DateTime.ParseExact(InspectionStartAndEndDate.Split('-')[0],
                //    "M'/'d'/'yyyy", null,
                //    System.Globalization.DateTimeStyles.None);
            }
        }
    }
}
