using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class CorporateIntegrityAgreementListSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public CorporateIntegrityAgreementListSiteData()
        {
            CIAListSiteData = new List<CIAList>();
        }
        public Guid? RecId { get; set; }
        public List<CIAList> CIAListSiteData { get; set; }

    }
    public class CIAList : SiteDataItemBase
    {
       // public int RowNumber { get; set; }
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

        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return
                    "Provider: " + Provider + "~" +
                    "Status: " + Status + "~" +
                    "City: " + City + "~" +
                    "State: " + State + "~" +
                    "Effective: " + Effective;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                if (Effective == "" || Effective == null)
                    return null;

                return DateTime.ParseExact(Effective.Trim(), "M-d-yyyy", null,
                    System.Globalization.DateTimeStyles.None);
            }
        }
    }
}
