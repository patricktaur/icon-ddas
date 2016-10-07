﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    //Int64? required for mongodb go generate ids
    public class FDADebarPageSiteData  //: AuditEntity<long?>
    {
        public FDADebarPageSiteData()
        {
            DebarredPersons = new List<DebarredPerson>(); 
        }
        public Guid? RecId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        
        public DateTime SiteLastUpdatedOn { get; set; }
        //Patrick-1 - 4 CHANGES
        //public ICollection<DebarredPerson> DebarredPersons  { get; set; }
        public List<DebarredPerson> DebarredPersons { get; set; }
        public string Source { get; set; }
    }

    public class DebarredPerson : SiteDataItemBase  ////Patrick-2
    {
        //Patrick-3
          /*
        public string Status { get; set; }
        public int RowNumber { get; set; }
        public int Matched { get; set; }
        */
        public string NameOfPerson { get; set; }
        public string EffectiveDate { get; set; }
        public string EndOfTermOfDebarment { get; set; }
        public string FrDateText { get; set; }
        public string VolumePage { get; set; }
        public string DocumentLink { get; set; }
        public string DocumentName { get; set; }

        public override string FullName {
            get {
                return NameOfPerson; 
            }
        }
    }
}