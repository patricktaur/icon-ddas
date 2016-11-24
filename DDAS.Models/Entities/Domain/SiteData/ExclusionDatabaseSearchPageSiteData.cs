using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ExclusionDatabaseSearchPageSiteData //: AuditEntity<long?>
    {
        public ExclusionDatabaseSearchPageSiteData()
        {
            ExclusionSearchList = new List<ExclusionDatabaseSearchList>();
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
        public List<ExclusionDatabaseSearchList> ExclusionSearchList { get; set; }
    }

    public class ExclusionDatabaseSearchList : SiteDataItemBase
    {
        public int RowNumber { get; set; }
        public string Status { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        //public string BusinessName { get; set; }
        public string General { get; set; }
        public string Specialty { get; set; }
        //public string UPIN { get; set; }
        //public string NPI { get; set; }
        //public string DOB { get; set; }
        //public string Address { get; set; }
        //public string City { get; set; }
        //public string State { get; set; }
        //public string Zip { get; set; }
        public string ExclusionType { get; set; }
        //public string ExclusionDate { get; set; }
        //public string WaiverDate { get; set; }
        //public string WaiverState { get; set; }
        //public string SSNorEIN { get; set; }

        public override string FullName {
            get {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }
    }
}
