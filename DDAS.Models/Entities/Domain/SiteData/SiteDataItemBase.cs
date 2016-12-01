using System;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public abstract class SiteDataItemBase
    {
        public int Matched { get; set; }
        public abstract string FullName { get; }
        //Patrick 28Nov2016
        public abstract string RecordDetails { get; }
        public int RowNumber { get; set; }
        public int RecordNumber { get; set; }

    }
}
