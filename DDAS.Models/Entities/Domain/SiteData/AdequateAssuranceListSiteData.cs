using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    //Int64? required for mongodb go generate ids
    public class AdequateAssuranceListSiteData : AuditEntity<Int64?>
    {
        public DateTime SiteLastUpdatedOn { get; set; }
        public AdequateAssurance[] AdequateAssurances { get; set; }
    }

    public class AdequateAssurance
    {
        public string NameAndAddress { get; set; }
        public string Center { get; set; }
        public string Type { get; set; }
        public string ActionDate { get; set; }
        public string Comments { get; set; }
    }
}
