using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class SystemForAwardManagementEntitySiteData : BaseSiteData
    {
        public Guid? RecId { get; set; }
        //BaseClass properties do not get serialized, hence two ready only properties added:
        public DateTime ExtractedOn { get { return CreatedOn; } }
        public new DateTime? SiteLastUpdatedOn { get { return base.SiteLastUpdatedOn; } }

        public ICollection<SystemForAwardManagementEntity> SAMEntitySiteData { get; set; }
        = new List<SystemForAwardManagementEntity>();
    }

    public class SystemForAwardManagementEntity : SiteDataItemBase
    {
        public string Duns { get; set; }
        public string CAGECode { get; set; }
        public string ExpirationDate { get; set; }
        public string ActivationDate { get; set; }
        public string LegalBusinessName { get; set; }
        public string DBAName { get; set; }
        public string DelinquentFederalDebtFlag { get; set; }


        public override DateTime? DateOfInspection
        {
            get
            {
                if (ActivationDate == "" || ActivationDate == null ||
                    ActivationDate.Length < 3)
                    return null;

                return DateTime.ParseExact(ActivationDate.Trim(),
                    "M'/'d'/'yyyy", null,
                    System.Globalization.DateTimeStyles.None);
            }
        }

        public override string FullName
        {
            get
            {
                return LegalBusinessName;
            }
        }

        public override string RecordDetails
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
