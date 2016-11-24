using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class CorporateIntegrityAgreementListSiteData //: AuditEntity<long?>
    {
        public CorporateIntegrityAgreementListSiteData()
        {
            CIAListSiteData = new List<CIAList>();
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
        public List<CIAList> CIAListSiteData { get; set; }

    }
    public class CIAList : SiteDataItemBase
    {
        public int RowNumber { get; set; }
        public string Status { get; set; }
        public string Provider { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Effective { get; set; }

        public override string FullName {
            get {
                return Provider;
            }
        }
    }
}
