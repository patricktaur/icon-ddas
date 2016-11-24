using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class PHSAdministrativeActionListingSiteData //: AuditEntity<long?>
    {
        public PHSAdministrativeActionListingSiteData()
        {
            PHSAdministrativeSiteData = new List<PHSAdministrativeAction>();
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
        public List<PHSAdministrativeAction> PHSAdministrativeSiteData { get; set; }
    }

    public class PHSAdministrativeAction : SiteDataItemBase
    {
        public int RowNumber { get; set; }
        public string Status { get; set; }
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

        public override string FullName {
            get {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }
    }
}
