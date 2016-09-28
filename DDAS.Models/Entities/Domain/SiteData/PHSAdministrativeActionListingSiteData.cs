using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class PHSAdministrativeActionListingSiteData : AuditEntity<long?>
    {
        public PHSAdministrativeActionListingSiteData()
        {
            PHSAdministrativeSiteData = new List<PHSAdministrativeAction>();
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public List<PHSAdministrativeAction> PHSAdministrativeSiteData { get; set; }
    }

    public class PHSAdministrativeAction
    {
        public int RowNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string DebarmentUntil { get; set; }
        public string NoPHSAdvisoryUntil { get; set; }
        public string CertificationOfWorkUntil { get; set; }
        public string SupervisionUntil { get; set; }
        public string RetractionOfArticle { get; set; }
        public string CorrectionOfArticle { get; set; }
        public string Memo { get; set; }
    }
}
