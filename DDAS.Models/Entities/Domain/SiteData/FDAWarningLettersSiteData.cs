using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class FDAWarningLettersSiteData : AuditEntity<long?>
    {
        public FDAWarningLettersSiteData()
        {
            FDAWarningLetterList = new List<FDAWarningLetter>();
        }

        public DateTime SiteLastUpdatedOn { get; set; }
        public string Source { get; set; }
        public List<FDAWarningLetter> FDAWarningLetterList { get; set; }
    }

    public class FDAWarningLetter
    {
        public string SiteQuery { get; set; }
        public int RowNumber { get; set; }
        public string Company { get; set; }
        public string LetterIssued { get; set; }
        public string IssuingOffice { get; set; }
        public string Subject { get; set; }
        public string ResponseLetterPosted { get; set; }
        public string CloseoutDate { get; set; }
    }

}
