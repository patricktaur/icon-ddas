using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ExclusionDatabaseSearchPageSiteData : AuditEntity<long?>
    {
        public ExclusionDatabaseSearchPageSiteData()
        {
            ExclusionSearchList = new List<ExclusionDatabaseSearchList>();
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public ICollection<ExclusionDatabaseSearchList> ExclusionSearchList { get; set; }
    }

    public class ExclusionDatabaseSearchList
    {
        public string LocatedAt { get; set; }
        public int RowNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string General { get; set; }
        public string Specialty { get; set; }
        public string Exclusion { get; set; }
        public string Waiver { get; set; }
        public string SSNorEIN { get; set; }
    }
}
