using DDAS.Models.Entities.Domain.SiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Services.Search
{
    public class Test : SiteDataItemBase
    {
        public override string FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string NameOfPerson { get; set; }
        public string EffectiveDate { get; set; }
        public string EndOfTermOfDebarment { get; set; }
        public string FrDateText { get; set; }
        public string VolumePage { get; set; }
        public string DocumentLink { get; set; }
    }
}
