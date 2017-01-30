using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class SystemForAwardManagementPageSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public SystemForAwardManagementPageSiteData()
        {
            SAMSiteData = new List<SystemForAwardManagement>();
        }
        public Guid? RecId { get; set; }
        public ICollection<SystemForAwardManagement> SAMSiteData { get; set; }
    }

    public class SystemForAwardManagement : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
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
        public string ActivationDate { get; set; } = "";
        public string TerminationDate { get; set; }

        public override string FullName {
            get {
                return Entity;
            }
        }

        public override string RecordDetails
        {
            get
            {
                return
                    "Name: " + Entity + "~" +
                    "Duns: " + Duns + "~" +
                    "Has Active Exclusion?: " + HasActiveExclusion + "~" +
                    "Expiration Date: " + ExpirationDate + "~" +
                    "Purpose Of Registration: " + HasActiveExclusion + "~" +
                    "CAGE Code: " + HasActiveExclusion + "~" +
                    "DoDAAC: " + HasActiveExclusion + "~" +
                    "Delinquent Federal Debt?: " + HasActiveExclusion + "~" +
                    "Classification: " + HasActiveExclusion + "~" +
                    "Activation Date: " + HasActiveExclusion + "~" +
                    "Termination Date: " + HasActiveExclusion;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                if (ActivationDate == "")
                    return null;

                return DateTime.ParseExact(ActivationDate,
                    "M'/'d'/'yyyy", null,
                    System.Globalization.DateTimeStyles.None);
            }
        }
    }
}
