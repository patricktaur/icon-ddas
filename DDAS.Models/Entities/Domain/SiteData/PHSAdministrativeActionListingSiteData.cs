using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class PHSAdministrativeActionListingSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public PHSAdministrativeActionListingSiteData()
        {
            PHSAdministrativeSiteData = new List<PHSAdministrativeAction>();
        }
        public Guid? RecId { get; set; }
        public List<PHSAdministrativeAction> PHSAdministrativeSiteData { get; set; }
    }

    public class PHSAdministrativeAction : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
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

        public override string RecordDetails
        {
            get
            {
                return
                    "First Name: " + FirstName + "~" +
                    "Last Name: " + LastName + "~" +
                    "Middle Name: " + MiddleName + "~" +
                    "Debarment Until: " + DebarmentUntil + "~" +
                    "No PHS Advisory Until: " + NoPHSAdvisoryUntil + "~" +
                    "Certification Of Work Until: " + CertificationOfWorkUntil + "~" +
                    "Supervision Until: " + SupervisionUntil + "~" +
                    "Retraction Of Article(s): " + RetractionOfArticle + "~" +
                    "Correction Of Article(s): " + CorrectionOfArticle + "~" +
                    "Memo: " + Memo;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                if (NoPHSAdvisoryUntil.Trim() == null || 
                    !IsValidDateFormat(NoPHSAdvisoryUntil))
                    return null;

                string[] Formats =
                    { "M/d/yyyy", "M-d-yyyy" };

                return DateTime.ParseExact(
                    NoPHSAdvisoryUntil.Trim(), Formats, null,
                    System.Globalization.DateTimeStyles.None);
            }
        }

        private bool IsValidDateFormat(string Format)
        {
            foreach(char c in Format)
            {
                if((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    return false;
            }
            return true;
        }
    }
}
