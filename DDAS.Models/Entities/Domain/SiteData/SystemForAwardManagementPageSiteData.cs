using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class SystemForAwardManagementPageSiteData //: AuditEntity<long?>
    {
        public SystemForAwardManagementPageSiteData()
        {
            SAMSiteData = new List<SystemForAwardManagement>();
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
        public ICollection<SystemForAwardManagement> SAMSiteData { get; set; }
    }

    public class SystemForAwardManagement : SiteDataItemBase
    {
        public int RowNumber { get; set; }
        public string Status { get; set; }
        public string Entity { get; set; }
        public string Duns { get; set; }
        public string HasActiveExclusion { get; set; }
        public string ExpirationDate { get; set; }
        public string PurposeOfRegistration { get; set; }
        public string CAGECode { get; set; }
        public string DoDAAC { get; set; }
        public string DelinquentFederalDebt { get; set; }
        public string Classification { get; set; }
        public string ActivationDate { get; set; }
        public string TerminationDate { get; set; }

        public override string FullName {
            get {
                return Entity;
            }
        }
    }
}
