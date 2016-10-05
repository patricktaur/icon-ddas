using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public abstract class SiteDataItemBase
    {
        public int Matched { get; set; }
        public int RowNumber { get; set; }
        public abstract string FullName { get;  }
    }

    public interface ISiteDataItemBase
    {
         int Matched { get; set; }
         int RowNumber { get; set; }
         string FullName { get; }
    }
}
