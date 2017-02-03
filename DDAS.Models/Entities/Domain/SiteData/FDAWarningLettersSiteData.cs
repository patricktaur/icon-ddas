using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class FDAWarningLettersSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public FDAWarningLettersSiteData()
        {
            FDAWarningLetterList = new List<FDAWarningLetter>();
        }
        public Guid? RecId { get; set; }
        public List<FDAWarningLetter> FDAWarningLetterList { get; set; }
        
        public override List<SiteDataItemBase> Records { get { return new List<FDAWarningLetter>().Cast<SiteDataItemBase>().ToList(); } }
        //public override List<SiteDataItemBase> Records { get {return new FDAWarningLetterList<FDAWarningLetter>().Cast<SiteDataItemBase>().ToList() } ; }

    }

    public class FDAWarningLetter : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string Company { get; set; }
        public string LetterIssued { get; set; }
        public string IssuingOffice { get; set; }
        public string Subject { get; set; }
        public string ResponseLetterPosted { get; set; }
        public string CloseoutDate { get; set; }

        public override string FullName {
            get {
                return Company;
            }
        }

        public override string RecordDetails
        {
            get
            {
                return
                    "Company: " + Company + "~" +
                    "Letter Issued: " + LetterIssued + "~" +
                    "Issuing Office: " + IssuingOffice + "~" +
                    "Subject: " + Subject + "~" +
                    "Response Letter Posted: " + ResponseLetterPosted + "~" +
                    "Closeout Date: " + CloseoutDate;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                if (LetterIssued == "" || LetterIssued == null)
                    return null;

                return DateTime.ParseExact(LetterIssued.Trim(),
                    "M'/'d'/'yyyy", null,
                    System.Globalization.DateTimeStyles.None);
            }
        }
    }
}
