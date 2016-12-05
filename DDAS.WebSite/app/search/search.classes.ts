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
        Sites_FullMatchCount :number;
        Sites_PartialMatchCount:number;
        TotalIssuesFound:number;
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
    Observation: string = "";
    IssueNumber :number = 0;
    RowNumber :number;
    RecordDetails :string = "";
    Status :string = ""; 
    Selected:boolean;  //this prop is not declared on server side
    HiddenStatus: string = "";
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
      Sites_MatchStatus:string="";
      SitesProcessed : string = "";
      Active: boolean;
      TotalIssuesFound:number=0;
      IssueStatus:string="";
      SiteDetails:string="";
     //List<SitesIncludedInSearch> SiteDetails : string = "";
    }

  
  
    export class PrincipalInvestigatorDetails{
      RecId : string = "";
      PrincipalInvestigator: string="";
      SponsorProtocolNumber : string = "";
      Country : string = "";
      Address : string = "";
      ProjectNumber : string = "";
      SearchStartedOn : Date ;
      
    }


  export class ComplianceFormA{
      RecId : string = "";
      SponsorProtocolNumber : string = "";
      Country : string = "";
      Address : string = "";
      ProjectNumber : string = "";
      SearchStartedOn : Date ;
      InvestigatorDetails:InvestigatorSearched[] = [];
      SiteSources : SiteSource[] = [];
      Findings: Finding[] = [];
    }

  export class InvestigatorSearched{
      Id: number = 0;
      DisplayPosition: number = 0;
      Name: string = "";
      Role: string = "";
      Sites_FullMatchCount: number = 0;
      Sites_PartialMatchCount: number = 0;
      AllSitesProcessed: boolean = false;
      TotalIssuesFound: number = 0;
      Deleted: boolean = false;
      SitesSearched: SiteSearchStatus[] = [];
  }

  export class SiteSearchStatus{
        siteEnum: number = 0;
        SiteName: string = "";
        SiteUrl: string = "";
        HasExtractionError : boolean;
        ExtractionErrorMessage : string;
        FullMatchCount : number = 0;
        PartialMatchCount : number = 0;
        IssuesFound : number = 0;
        ReviewCompleted : boolean = false;
  }
  
  
    export class SiteSource{
        Id: number = 0;
        DisplayPosition: number = 0;
        SiteName :string;
        SiteShortName : string;
        SiteDataId: string;
        DataExtractedOn : Date;
        SiteSourceUpdatedOn : Date;
        CreatedOn: Date;
        ExtractionMode : string;
        SiteEnum : number; 
        SiteUrl : string;
        IssuesIdentified : boolean = false;
        Deleted: boolean = false;
        IsMandatory: boolean = true;  //temp
    }  

    export class Finding{
        SiteEnum: number;
        InvestigatorSearchedId: number;
        MatchCount : number;
        RowNumberInSource : number;
        Observation: string;
        RecordDetails : string;
        Status : string;
        HiddenStatus : string = "";
        Selected: boolean = false;
        UISelected: boolean = false;
        IsMatchedRecord: boolean;
        SourceNumber: number;
        DateOfInspection: Date;
        InvestigatorName: string;
    }
  
     
    export class SiteToAdd{
        SiteName :string;
        SiteShortName : string;
        SiteEnum : number; 
        SiteUrl : string;
        Recomended: boolean = false;
        Selected: boolean = false;
        Included: boolean = false; //Included in SiteSource collectio n
    }  
    
    
    