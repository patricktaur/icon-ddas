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
        //BaseClass properties do not get serialized, hence two ready only properties added:
        public DateTime ExtractedOn { get { return CreatedOn; } }
        public new DateTime? SiteLastUpdatedOn { get { return base.SiteLastUpdatedOn; } }

        public List<FDAWarningLetter> FDAWarningLetterList { get; set; }
        
        public override List<SiteDataItemBase> Records { get { return new List<FDAWarningLetter>().Cast<SiteDataItemBase>().ToList(); } }
        //public override List<SiteDataItemBase> Records { get {return new FDAWarningLetterList<FDAWarningLetter>().Cast<SiteDataItemBase>().ToList() } ; }

    }

    public class FDAWarningLetter : SiteDataItemBase
    {
        public Guid? RecId { get; set; }
        public Guid? ParentId { get; set; }

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

                string[] Formats =
                    { "yyyy-M-d", "M-d-yyyy", "M/d/yyyy"};

                return DateTime.ParseExact(
                    LetterIssued.Trim(), Formats, null,
                    System.Globalization.DateTimeStyles.None);
            }
        }
    }

    public class FDAWarningLetterFile
    {
        public string path { get; set; }
        public string field_change_date_2 { get; set; } // Posted Date
        public string field_letter_issue_datetime { get; set; } // Letter Issue Date
        public string field_company_name_warning_lette { get; set; } //Company
        public string field_building { get; set; } // Issuing Office
        public string field_detailed_description_2 { get; set; } // Subject
        public string field_associated_for_response_le { get; set; } // Response Letter
        public string field_associated_for_closeout_le { get; set; } // Closeout Letter
    }
}
