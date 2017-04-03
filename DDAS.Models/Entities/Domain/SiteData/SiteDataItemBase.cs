using System;
using System.Collections.Generic;

namespace DDAS.Models.Entities.Domain.SiteData
{
    public abstract class SiteDataItemBase
    {
        public int Matched { get; set; }
        public abstract string FullName { get; }
        public abstract string RecordDetails { get; }
        public int RowNumber { get; set; }
        public int RecordNumber { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();
       
        public abstract DateTime? DateOfInspection { get; }
        
    }

    public class Link
    {
        public string Title;
        public string url;
    }
}
