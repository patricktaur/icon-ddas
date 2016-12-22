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

        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return
                    "Full Name: " + FullName + "~" +
                    "Company: " + Company + "~" +
                    "Letter Issued: " + LetterIssued + "~" +
                    "Issuing Office: " + IssuingOffice + "~" +
                    "Subject: " + Subject + "~" +
                    "Response Letter Posted: " + ResponseLetterPosted + "~" +
                    "Closeout Date: " + CloseoutDate;
            }
        }
    }

}
