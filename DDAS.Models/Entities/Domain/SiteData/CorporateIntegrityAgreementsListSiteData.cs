using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class CorporateIntegrityAgreementsListSiteData : AuditEntity<long?>
    {
        public CorporateIntegrityAgreementsListSiteData()
        {
            CIAListSiteData = new List<CIAList>();
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public List<CIAList> CIAListSiteData { get; set; }

    }
    public class CIAList
    {
        public int RowNumber { get; set; }
        public string Provider { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Effective { get; set; }
    }
}
