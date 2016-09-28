using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    //Int64? required for mongodb go generate ids
    public class FDADebarPageSiteData  : AuditEntity<long?>
    {
        public FDADebarPageSiteData()
        {
            DebarredPersons = new List<DebarredPerson>(); 
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public ICollection<DebarredPerson> DebarredPersons  { get; set; }
        public string Source { get; set; }
    }

    public class DebarredPerson
    {
        public int RowNumber { get; set; }
        public string NameOfPerson { get; set; }
        public string EffectiveDate { get; set; }
        public string EndOfTermOfDebarment { get; set; }
        public string FrDateText { get; set; }
        public string VolumePage { get; set; }
        public string DocumentLink { get; set; }
    }
}
