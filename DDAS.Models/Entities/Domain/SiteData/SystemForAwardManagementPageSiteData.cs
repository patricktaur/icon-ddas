using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class SystemForAwardManagementPageSiteData : AuditEntity<long?>
    {
        public SystemForAwardManagementPageSiteData()
        {
            SAMSiteData = new List<SystemForAwardManagement>();
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public ICollection<SystemForAwardManagement> SAMSiteData { get; set; }
    }

    public class SystemForAwardManagement
    {
        public string SiteQuery { get; set; }
        public int RowNumber { get; set; }
        public string Entity { get; set; }
        public string Status { get; set; }
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
    }
}
