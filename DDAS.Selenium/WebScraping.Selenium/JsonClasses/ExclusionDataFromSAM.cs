using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping.Selenium.JsonClasses
{
    

    public class ExclusionDetails
    {
        public string classificationType { get; set; }
        public string exclusionType { get; set; }
        public string exclusionProgram { get; set; }
        public string excludingAgencyCode { get; set; }
        public string excludingAgencyName { get; set; }
    }

    public class ExclusionIdentification
    {
        public object ueiSAM { get; set; }
        public object ueiDUNS { get; set; }
        public object cageCode { get; set; }
        public object npi { get; set; }
        public object prefix { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public object suffix { get; set; }
        public string entityName { get; set; }
        public object dnbOpenData { get; set; }
    }

    public class ListOfAction
    {
        public string createDate { get; set; }
        public string updateDate { get; set; }
        public string activateDate { get; set; }
        public string terminationDate { get; set; }
        public string terminationType { get; set; }
        public string recordStatus { get; set; }
    }

    public class ExclusionActions
    {
        public List<ListOfAction> listOfActions { get; set; }
    }

    public class ExclusionPrimaryAddress
    {
        public object addressLine1 { get; set; }
        public object addressLine2 { get; set; }
        public string city { get; set; }
        public string stateOrProvinceCode { get; set; }
        public string zipCode { get; set; }
        public object zipCodePlus4 { get; set; }
        public string countryCode { get; set; }
    }

    public class ReferencesList
    {
        public object exclusionName { get; set; }
        public object type { get; set; }
    }

    public class References
    {
        public List<ReferencesList> referencesList { get; set; }
    }

    public class PrimaryAddress
    {
        public object addressLine1 { get; set; }
        public object addressLine2 { get; set; }
        public object city { get; set; }
        public object stateOrProvinceCode { get; set; }
        public object zipCode { get; set; }
        public object zipCodePlus4 { get; set; }
        public object countryCode { get; set; }
    }

    public class SecondaryAddress
    {
        public object addressLine1 { get; set; }
        public object addressLine2 { get; set; }
        public object city { get; set; }
        public object stateOrProvinceCode { get; set; }
        public object zipCode { get; set; }
        public object zipCodePlus4 { get; set; }
        public object countryCode { get; set; }
    }

    public class MoreLocation
    {
        public object exclusionName { get; set; }
        public object duns { get; set; }
        public object cageCode { get; set; }
        public object npi { get; set; }
        public PrimaryAddress primaryAddress { get; set; }
        public List<SecondaryAddress> secondaryAddress { get; set; }
    }

    public class ExclusionOtherInformation
    {
        public object additionalComments { get; set; }
        public object ctCode { get; set; }
        public object evsInvestigationStatus { get; set; }
        public References references { get; set; }
        public List<MoreLocation> moreLocations { get; set; }
    }

    public class VesselDetails
    {
        public object callSign { get; set; }
        public object type { get; set; }
        public object tonnage { get; set; }
        public object grt { get; set; }
        public object flag { get; set; }
        public object owner { get; set; }
    }

    public class ExcludedEntity
    {
        public ExclusionDetails exclusionDetails { get; set; }
        public ExclusionIdentification exclusionIdentification { get; set; }
        public ExclusionActions exclusionActions { get; set; }
        public ExclusionPrimaryAddress exclusionPrimaryAddress { get; set; }
        public List<object> exclusionSecondaryAddress { get; set; }
        public ExclusionOtherInformation exclusionOtherInformation { get; set; }
        public VesselDetails vesselDetails { get; set; }
    }

    public class Links
    {
        public string selfLink { get; set; }
        public string nextLink { get; set; }
    }
   
    public class ExclusionDataFromSAM
    {
        public int totalRecords { get; set; }
        public List<ExcludedEntity> excludedEntity { get; set; }
        public Links links { get; set; }
    }
}
