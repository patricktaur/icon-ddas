using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ClinicalInvestigatorInspectionSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public ClinicalInvestigatorInspectionSiteData()
        {
            ClinicalInvestigatorInspectionList = new List<ClinicalInvestigator>();
        }
        public Guid? RecId { get; set; }
        public List<ClinicalInvestigator> ClinicalInvestigatorInspectionList { get; set; }
    }

    public class ClinicalInvestigator : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string IdNumber { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string InspectionDate { get; set; }
        public string ClassificationType { get; set; }
        public string ClassificationCode { get; set; }
        public string DeficiencyCode { get; set; }

        public override string FullName {
            get {
                return Name;
            }
        }

        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return
                    "Name: " + Name + "~" +
                    "Id Number: " + IdNumber + "~" +
                    "Location: " + Location + "~" +
                    "Address: " + Address + "~" +
                    "City: " + City + "~" +
                    "State: " + State + "~" +
                    "Country: " + Country + "~" +
                    "Zip: " + Zip + "~" +
                    "Inspection Date: " + InspectionDate + "~" +
                    "Classification Type: " + ClassificationType + "~" +
                    "Classification Code: " + ClassificationCodeExpanded + "~" +
                    "Deficiency Code: " + DeficiencyCode;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                if (InspectionDate == "" || InspectionDate == null)
                    return null;

                return DateTime.ParseExact(InspectionDate.Trim(),
                    "M'/'d'/'yyyy", null,
                    System.Globalization.DateTimeStyles.None);
            }
        }
        
        #region ClassificationCodesDictionary
        Dictionary<string, string> ClassificationCodes = 
            new Dictionary<string, string>() {
                { "MTF", "MTF - Case closed with a Memo to File" },
                { "NAI", "NAI - No Action Indicated.  No objectionable conditions or practices were found during the inspection" },
                { "VAI", "Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily" },
                { "VAI1", "Correction made on site" },
                { "VAI2", "No response requested" },
                { "VAI2C", "Consent problems found" },
                { "VAI3", "Response requested" },
                { "VAI3C", "Case closed" },
                { "VAI3F", "Follow-up for cause inspection issued" },
                { "VAI3R", "Response received and accepted" },
                { "VAIRC", "30-day response requested and case closed" },
                { "VAIRR", "30-day response requested, received and accepted" },
                { "VAIR", "30-day response requested" },
                { "OAI", "Official Action Indicated. Objectionable conditions were found and regulatory and/or administrative sanctions by FDA are indicated" },
                { "OAIC", "Completed" },
                { "OAIR", "Response requested" },
                { "OAIRR", "Response requested and accepted" },
                { "OAIW", "Warning letter issued" },
                { "CANC", "Cancelled. The inspection assignment was canceled before the inspection was started" },
                { "WASH", "Washout. An inspection was initiated but no meaningful information could be obtained" },
                { "REF", "REF - Reference" }
            };
        #endregion

        public string ClassificationCodeExpanded {
            get
            {
                foreach(KeyValuePair<string, string> pair in ClassificationCodes)
                {
                    if (ClassificationCode.ToLower() == pair.Key.ToLower())
                    {
                        ClassificationCode = pair.Value;
                    }
                }
                return ClassificationCode;
            }
        }
    }
}
