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
        //BaseClass properties do not get serialized, hence two ready only properties added:
        public DateTime ExtractedOn { get { return CreatedOn; } }
        public new DateTime? SiteLastUpdatedOn { get { return base.SiteLastUpdatedOn; } }

        public List<ClinicalInvestigator> ClinicalInvestigatorInspectionList { get; set; }
    }

    public class ClinicalInvestigator : SiteDataItemBase
    {
        public Guid? RecId { get; set; }
        public Guid? ParentId { get; set; }

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
        public string InspectionType { get; set; }
        public string ClassificationCode { get; set; }
        public string DeficiencyCode { get; set; }

        public override string FullName {
            get {
                return Name;
            }
        }

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
                    "Inspection Type: " + InspectionTypeExpanded + "~" +
                    "Classification Code: " + ClassificationCodeExpanded + "~" +
                    "Deficiency Code: " + DeficiencyCodeExpanded;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                if (InspectionDate == "" || InspectionDate == null)
                    return null;

                string[] Formats =
                    { "M/d/yy", "M-d-yyyy", "M/d/yyyy"};

                //10Jul2020 Patrick
                //return DateTime.ParseExact(InspectionDate.Trim(),
                //    Formats, null,
                //    System.Globalization.DateTimeStyles.None);

                DateTime dateTime;
                if (DateTime.TryParseExact(InspectionDate.Trim(), Formats, 
                    null, System.Globalization.DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }
                return null;


                

            }
        }
        
        #region ClassificationCodesDictionary
        Dictionary<string, string> ClassificationCodes = 
            new Dictionary<string, string>() {
                { "MTF", "MTF - Case closed with a Memo to File" },
                { "NAI", "NAI - No Action Indicated.  No objectionable conditions or practices were found during the inspection" },
                { "VAI", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily" },
                { "VAI1", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAI1 - Correction made on site" },
                { "VAI2", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAI2 - No response requested" },
                { "VAI2C", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAI2C - Consent problems found" },
                { "VAI3", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAI3 - Response requested" },
                { "VAI3C", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAI3C - Case closed" },
                { "VAI3F", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAI3F - Follow-up for cause inspection issued" },
                { "VAI3R", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAI3R - Response received and accepted" },
                { "VAIRC", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAIRC - 30-day response requested and case closed" },
                { "VAIRR", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAIRR - 30-day response requested, received and accepted" },
                { "VAIR", "VAI - Voluntary Action Indicated.  Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily and VAIR - 30-day response requested" },
                { "OAI", "OAI - Official Action Indicated. Objectionable conditions were found and regulatory and/or administrative sanctions by FDA are indicated" },
                { "OAIC", "OAI - Official Action Indicated. Objectionable conditions were found and regulatory and/or administrative sanctions by FDA are indicated and OAIC - Completed" },
                { "OAIR", "OAI - Official Action Indicated. Objectionable conditions were found and regulatory and/or administrative sanctions by FDA are indicated and OAIR - Response requested" },
                { "OAIRR", "OAI - Official Action Indicated. Objectionable conditions were found and regulatory and/or administrative sanctions by FDA are indicated and OAIRR - Response requested and accepted" },
                { "OAIW", "OAI - Official Action Indicated. Objectionable conditions were found and regulatory and/or administrative sanctions by FDA are indicated and OAIW - Warning letter issued" },
                { "CANC", "CANC - Cancelled. The inspection assignment was canceled before the inspection was started" },
                { "WASH", "WASH - Washout. An inspection was initiated but no meaningful information could be obtained" },
                { "REF", "REF - Reference" }
            };
        #endregion

        #region DeficiencyCodesDictionary

        Dictionary<string, string> DeficiencyCodes =
            new Dictionary<string, string>()
            {
                { "00", "00 - No deficiencies noted, n/a" },
                { "01", "01 - Records availability, 21 CFR 312.62" },
                { "02", "02 - Failure to obtain and/or document subject consent, 21 CFR 312.60, 50.20, 50.27" },
                { "03", "03 - Inadequate informed consent form, 21 CFR 50.25" },
                { "04", "04 - Inadequate drug accountability, 21 CFR 312.60, 312.62" },
                { "05", "05 - Failure to follow investigational plan, 21 CFR 312.60" },
                { "06", "06 - Inadequate and inaccurate records, 21 CFR 312.62" },
                { "07", "07 - Unapproved concomitant therapy, 21 CFR 312.60" },
                { "08", "08 - Inappropriate payment to volunteers, 21 CFR 50.20" },
                { "09", "09 - Unapproved use of drug before IND submission, 21 CFR 312.40(d)" },
                { "10", "10 - Inappropriate delegation of authority, 21 CFR 312.7, 312.61" },
                { "11", "11 - Inappropriate use/commercialization of IND, 21 CFR 312.7, 312.61" },
                { "12", "12 - Failure to list additional investigators on 1572, 21 CFR 312.60" },
                { "13", "13 - Subjects receiving simultaneous investigational drugs, 21 CFR 312.60" },
                { "14", "14 - Failure to obtain or document IRB approval, 21 CFR 312.60, 62, 66; 56.103" },
                { "15", "15 - Failure to notify IRB of changes, failure to submit progress reports, 21 CFR 312.66" },
                { "16", "16 - Failure to report adverse drug reactions, 21 CFR 312.64, 312.66" },
                { "17", "17 - Submission of false information, 21 CFR 312.70" },
                { "18", "18 - Other, n/a" },
                { "19", "19 - Failure to supervise or personally conduct the clinical investigation, 21 CFR 312.60" },
                { "20", "20 - Failure to protect the rights, safety, and welfare of subjects, 21 CFR 312.60" },
                { "21", "21 - Failure to permit FDA access to records, 21 CFR 312.68" }
            };
        #endregion

        #region InspectionTypeDictionary

        Dictionary<string, string> InspectionTypes = new Dictionary<string, string>()
        {
            { "DA", "DA  - Data Audit: An inspection in which the focus is on verification of study data" },
            { "FC", "FC - For Cause: An inspection in which the focus is on the conduct of the study by the Clinical Investigator" },
            { "IG", "Information Gathering" }
        };

        #endregion

        public string ClassificationCodeExpanded {
            get
            {
                foreach(KeyValuePair<string, string> pair in ClassificationCodes)
                {
                    if (ClassificationCode != null && 
                        ClassificationCode.ToLower() == pair.Key.ToLower())
                        ClassificationCode = pair.Value;
                }
                return ClassificationCode;
            }
        }

        public string DeficiencyCodeExpanded {
            get
            {
                var DefCode = "";
                foreach (KeyValuePair<string, string> pair in DeficiencyCodes)
                {
                    if(DeficiencyCode != null)
                    {
                        var Codes = DeficiencyCode.Split(',').ToList();
                        foreach(string Code in Codes)
                        {
                            if (Code.Trim().ToLower() == pair.Key.ToLower())
                            {
                                if (DefCode == "")
                                    DefCode = pair.Value;
                                else
                                    DefCode += ", " + pair.Value;
                            }
                        }
                    }
                }
                return DefCode;
            }
        }

        public string InspectionTypeExpanded {
            get
            {
                foreach(KeyValuePair<string, string> pair in InspectionTypes)
                {
                    if (InspectionType != null && 
                        InspectionType.ToLower() == pair.Key.ToLower())
                        InspectionType = pair.Value;
                }
                return InspectionType;
            }
        }
    }
}
