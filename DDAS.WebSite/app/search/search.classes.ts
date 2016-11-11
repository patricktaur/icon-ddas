export class SiteInfo
    {

        SiteName : string;
        SiteId : number;
        Component:string;
        
    }
export class NameSearch{
        NameToSearch:string;
    }

//revised: 05Nov2016:
export class SearchSummary
    {
        
        ComplianceFormId:string;
        NameToSearch:string;
        SearchSummaryItems : SearchSummaryItem[];
    }  
export class SearchSummaryItem
    {
        RecId:string;
        SiteName : string;
        SiteEnum : number;
        SiteUrl:string;
        MatchStatus: string;
        
    }  


export class StudyNumbers
    {
        StudyNumber : string;
        //StudyNumber : StudyNumberDetail;
    }
export class StudyNumberDetail{
        Name:string;
        Id:number;
    }

export class SiteResultDetails{
        RecId:string;
        NameToSearch:string;
        SiteEnum:number;
    }


export class SearchResultSaveData{
    NameToSearch:string;
    SiteEnum:number;
    DataId:string;
    saveSearchDetails:saveSearchDetails[] = [] ;
}
export class saveSearchDetails{
    RowNumber:number;
    Status:string;
}

//for extending FDADebarPageSiteData and 11 other classes

export class SiteData{
     RecId:string;
      CreatedOn : string;
      CreatedBy : number;
      UpdatedOn:string;
      UpdatedBy: string;
      SiteLastUpdatedOn : string;
      Source :string;
      NameToSearch:string;
      SiteEnum:number;
      SiteName:string;
}
//for extending items of SiteData in the child class
//example: export class DebarredPerson extends SiteDataItemBase
export class SiteDataItemBase{
    Matched :number = 0;
    RowNumber :number;
    FullName :string = "";
    Status :string = ""; 
    Selected:boolean;  //this prop is not declared on server side
}


//added 05Nov2016:
//to be converted to typescript classes
//  public class ComplianceForm
//     {
//         public Guid? RecId { get; set; }
//         public string SponsorProtocolNumber { get; set; }
//         public string Country { get; set; }
//         public string NameToSearch { get; set; }
//         public string Address { get; set; }
//         public string ProjectNumber { get; set; }
//         public DateTime SearchStartedOn { get; set; }
//         public DateTime SearchClosedOn { get; set; }
//         public List<SitesIncludedInSearch> SiteDetails { get; set; }
//     }

//     public class SitesIncludedInSearch
//     {
//         public string SiteName { get; set; }
//         public SiteEnum SiteEnum { get; set; }
//         public string SiteUrl { get; set; }
//         public DateTime ScannedOn { get; set; }
//         public int FullMatchCount { get; set; }
//         public int PartialMatchCount { get; set; }
//         public bool IssuesIdentified { get; set; }
//         public string Findings { get; set; }
//         //public string Issues { get; set; }
//         public List<MatchedRecordsPerSite> MatchedRecords { get; set; }
//     }

//     public class MatchedRecordsPerSite
//     {
//         public string Issues { get; set; }
//         public int IssueNumber { get; set; }
//         public int RowNumber { get; set; }
//         public string RecordDetails { get; set; }
//         public string Status { get; set; }
//     }

export class SitesIncludedInSearch{
  
    SiteName: string = "";
    SiteEnum :number = 0;
    SiteUrl :number = 0;
    ScannedOn: Date;
    FullMatchCount :number = 0;
    PartialMatchCount :number = 0;
    IssuesIdentified:string;
    Findings:string;
    Issues:string;
    MatchedRecords:MatchedRecordsPerSite[] = [] ;
   }


export class MatchedRecordsPerSite{
    Issues: string = "";
    IssueNumber :number = 0;
    RowNumber :number;
    RecordDetails :string = "";
    Status :string = ""; 
    Selected:boolean;  //this prop is not declared on server side
}


 
export class SearchList{
    NameToSearch: string = "";
     SearchDate: string = "";
      MatchSummary: string = "";
    ProcessedSummary: string = "";
    }

    export class ComplianceForm{
      RecId : string = "";
      SponsorProtocolNumber : string = "";
      Country : string = "";
      NameToSearch : string = "";
      Address : string = "";
      ProjectNumber : string = "";
      SearchStartedOn : Date ;
      SearchClosedOn : Date ;
      Sites_FullMatchCount : number = 0;
      Sites_PartialMatchCount : number = 0;
      SitesProcessed : string = "";
     //List<SitesIncludedInSearch> SiteDetails : string = "";
    }
    
    