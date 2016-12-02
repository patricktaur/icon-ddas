﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class FDAWarningLettersSiteData //: AuditEntity<long?>
    {
        public FDAWarningLettersSiteData()
        {
            FDAWarningLetterList = new List<FDAWarningLetter>();
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
        public string CloseOutDate { get; set; }

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
                    "FullName: " + FullName + "~" +
                    "Company: " + Company + "~" +
                    "LetterIssued: " + LetterIssued + "~" +
                    "IssuingOffice: " + IssuingOffice + "~" +
                    "Subject: " + Subject + "~" +
                    "ResponseLetterPosted: " + ResponseLetterPosted + "~" +
                    "CloseOutDate: " + CloseOutDate;
            }
        }
    }

}
