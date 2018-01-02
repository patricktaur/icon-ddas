public class SiteSource
    {
        public int Id { get; set; }
        public Guid? SiteId { get; set; } //RecId of SiteSourceRepository
        public int DisplayPosition { get; set; }
        public string SiteName { get; set; }
        public string SiteShortName { get; set; }
        public Guid? SiteDataId { get; set; }
        public DateTime DataExtractedOn { get; set; }
        public DateTime? SiteSourceUpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ExtractionMode { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsOptional { get; set; }
        public SiteEnum SiteEnum { get; set; }
        public string SiteUrl { get; set; }
        public bool IssuesIdentified { get; set; }
        public bool Deleted { get; set; } = false; //Patrick 30NOv2016

        //public bool ExcludeSI { get; set; }
        //public bool ExcludePI { get; set; }

        public SearchAppliesToEnum SearchAppliesTo { get; set; }
        public string SearchAppliesToText { get; set; }

    }