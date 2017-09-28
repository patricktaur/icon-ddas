using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public class ExclusionDatabaseSearchPageSiteData : BaseSiteData //: AuditEntity<long?>
    {
        public ExclusionDatabaseSearchPageSiteData()
        {
            ExclusionSearchList = new List<ExclusionDatabaseSearchList>();
        }
        public Guid? RecId { get; set; }
        public List<ExclusionDatabaseSearchList> ExclusionSearchList { get; set; }
    }

    public class ExclusionDatabaseSearchList : SiteDataItemBase
    {
        public Guid? RecId { get; set; }
        public Guid? ParentId { get; set; }

        //public int RowNumber { get; set; }
        public string Status { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        //public string BusinessName { get; set; }
        public string General { get; set; }
        public string Specialty { get; set; }
        //public string UPIN { get; set; }
        //public string NPI { get; set; }
        //public string DOB { get; set; }
        //public string Address { get; set; }
        //public string City { get; set; }
        //public string State { get; set; }
        //public string Zip { get; set; }
        public string ExclusionType { get; set; }
        public string ExclusionDate { get; set; }
        //public string WaiverDate { get; set; }
        //public string WaiverState { get; set; }
        //public string SSNorEIN { get; set; }

        public override string FullName {
            get {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }

        //Patrick 28Nov2016
        public override string RecordDetails
        {
            get
            {
                return
                    "First Name: " + FirstName + "~" +
                    "Last Name: " + LastName + "~" +
                    "Middle Name: " + MiddleName + "~" +
                    "General: " + General + "~" +
                    "Specialty: " + Specialty + "~" +
                    "Exclusion Type: " + ExclusionType + "~" +
                    "Exclusion Date: " + DateOfAction;
            }
        }

        private string DateOfAction {
            get
            {
                if (ExclusionDate == "" || ExclusionDate == null)
                    return null;
                //try
                //{

                string[] Formats =
                    { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                    "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy", "yyyyMMdd" };

                return DateTime.ParseExact(ExclusionDate.Trim(), Formats, null,
                    System.Globalization.DateTimeStyles.None).ToShortDateString();
                //}
                //catch (FormatException)
                //{
                //    return null;
                //}
            }

        }

        public override DateTime? DateOfInspection {
            get {
                if (ExclusionDate == "" || ExclusionDate == null)
                    return null;

                string[] Formats =
                    { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                    "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy", "yyyyMMdd" };

                return DateTime.ParseExact(ExclusionDate.Trim(), Formats, null,
                    System.Globalization.DateTimeStyles.None); //yyyyMMdd
            }
        }
    }
}
