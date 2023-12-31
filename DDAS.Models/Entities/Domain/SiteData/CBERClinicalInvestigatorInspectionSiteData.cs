﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class CBERClinicalInvestigatorInspectionSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public CBERClinicalInvestigatorInspectionSiteData()
        {
            ClinicalInvestigator = new List<CBERClinicalInvestigator>();
        }
        public Guid? RecId { get; set; }
        //BaseClass properties do not get serialized, hence two ready only properties added:
        public DateTime ExtractedOn { get  { return CreatedOn;} }
        public new DateTime?  SiteLastUpdatedOn { get { return base.SiteLastUpdatedOn; } }
        public List<CBERClinicalInvestigator> ClinicalInvestigator { get; set; }
    }

    public class CBERClinicalInvestigator : SiteDataItemBase
    {
        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string InstituteAndAddress { get; set; }
        public string InspectionStartAndEndDate { get; set; }
        public string Classification { get; set; }

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
                    "Title: " + Title + "~" +
                    "Institute/Address: " + InstituteAndAddress + "~" +
                    "Inspection Start/End Date: " + InspectionStartAndEndDate + "~" +
                    "Classification: " + ClassificationCodeExpanded;
            }
        }

        public override DateTime? DateOfInspection {
            get {
                string[] Formats = {
                    "M/d/yyyy",
                    "yyyy-MM-dd", "M-d-yyyy"
                };
                var dateOfInspection = new DateTime();
                var IsDateParsed = false;
                if (InspectionStartAndEndDate.Contains("-"))
                {
                    var InspectionStartDate = InspectionStartAndEndDate.Split('-')[0].Trim();
                    IsDateParsed = DateTime.TryParseExact(InspectionStartDate,
                        Formats, null,
                        System.Globalization.DateTimeStyles.None, out dateOfInspection);
                    if (IsDateParsed)
                        return dateOfInspection;
                    else
                        return null;
                }
                else if (InspectionStartAndEndDate.Trim() != "")
                    IsDateParsed = DateTime.TryParseExact(InspectionStartAndEndDate,
                        Formats, null,
                        System.Globalization.DateTimeStyles.None, out dateOfInspection);
                if (IsDateParsed)
                    return dateOfInspection;
                else
                    return null;
            }
        }

        Dictionary<string, string> ClassificationCode = new Dictionary<string, string>()
        {
            {"NAI", "NAI - No Action Indicated. No objectionable conditions or practices were found during the inspection"},
            {"VAI", "VAI - Voluntary Action Indicated. Objectionable conditions were found but the problems do not justify further regulatory action. Any corrective action is left to the investigator to take voluntarily"},
            {"OAI", "OAI - Official Action Indicated. Objectionable conditions were found and regulatory and/or administrative sanctions by FDA are indicated"}
        };

        public string ClassificationCodeExpanded {
            get
            {
                foreach(KeyValuePair<string, string> pair in ClassificationCode)
                {
                    if (Classification.ToLower() == pair.Key.ToLower())
                        Classification = pair.Value;
                }
                return Classification;
            }
        }
    }
}

