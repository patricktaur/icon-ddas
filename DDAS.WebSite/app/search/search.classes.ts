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
    RecordDetails: string;
    RecordNumber: number;
    Links: Link[];
    Selected:boolean;  //this prop is not declared on server side
}

 export class Link{
         Title: string;
         url: string;
 }
 

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
      Name: string="";
      SponsorProtocolNumber : string = "";
      Country : string = "";
      Address : string = "";
      ProjectNumber : string = "";
      SearchStartedOn : Date ;
      Status: string;
      AssignedTo: string;
      Active: boolean;
      StatusEnum: ComplianceFormStatusEnum;
      ReviewCompleted: boolean;
      ExtractionPendingInvestigatorCount: number;
      ExtractionErrorInvestigatorCount: number;
      EstimatedExtractionCompletionWithin: string;
      SubInvestigators:SubInvestigator[]= [] ;
    }

    export class SubInvestigator
    {
        Name: string;
        Status: string;
        StatusEnum: ComplianceFormStatusEnum;
    }

  export class ComplianceFormA{
    RecId : string = "";
    Active: boolean;
    SponsorProtocolNumber : string = "";
    Country : string = "";
    Address : string = "";
    ProjectNumber : string = "";
    Institute : string = "";
    AssignedTo: string = "";
    SearchStartedOn : Date ;
    ExtractionQueue: number;
    ExtractionQuePosition: number;
    ExtractionQueStart : Date ;
    ExtractionEstimatedCompletion : Date ;
    EstimatedExtractionCompletionWithin: string;
     
    ExtractedOn: Date;
    ExtractionErrorInvestigatorCount: number;

    FullMatchesFoundInvestigatorCount: number;
    PartialMatchesFoundInvestigatorCount : number;
    IssuesFoundInvestigatorCount: number;
    ReviewCompletedInvestigatorCount : number;

    InvestigatorDetails:InvestigatorSearched[] = [];
    SiteSources : SiteSourceToSearch[] = [];
    Findings: Finding[] = [];
    Status: string = "";
    StatusEnum: ComplianceFormStatusEnum = ComplianceFormStatusEnum.NotScanned;

    SearchPending: boolean = true;

    InstituteSearchSiteCount : number;
    }
 

  export class InvestigatorSearched{
      Id: number = 0;
      DisplayPosition: number = 0;
      Name: string = "";

        FirstName : string = "";
        MiddleName : string = "";
        LastName: string = "";
        SearchName: string = "";
         MemberId : string = "";


      Role: string = "";
      Qualification: string;
      MedicalLiceseNumber: string;
      
      ExtractedOn: Date;
      ExtractionErrorSiteCount: number;
      HasExtractionError : boolean;

      Sites_FullMatchCount: number = 0;
      Sites_PartialMatchCount: number = 0;
      IssuesFoundSiteCount: number =0;
      ReviewCompletedSiteCount: number = 0;

      TotalIssuesFound: number = 0;
      ReviewCompletedCount: number = 0;
      Deleted: boolean = false;
          
      SitesSearched: SiteSearchStatus[] = [];
      InvestigatorSiteStatus:string[];
     Status: string = "";
      StatusEnum: ComplianceFormStatusEnum = ComplianceFormStatusEnum.NotScanned;

     
        ExtractionPendingSiteCount:number = 0;
        NameComponentCount: number = 0;

        //---For client side Validations
      CanEdit:boolean = true;  //set to false when the name is searched.
      Saved: boolean = false;  //temp, to be replaced by form validation
      Help: string = "";

      
  }

   
  export class SiteSearchStatus{
        
        
        
        SiteSourceId: number;
        SiteId: string;
        siteEnum: number = 0;
        SiteName: string = "";
        SiteUrl: string = "";
        SiteDataId: string = "";
        DisplayPosition: number;
        
        ExtractedOn: Date;
        HasExtractionError : boolean;
        ExtractionErrorMessage : string;
        FullMatchCount : number = 0;
        PartialMatchCount : number = 0;
        SingleComponentMatchedValues : string;
        IssuesFound : number = 0;
        ReviewCompleted : boolean = false;
        SiteSourceUpdatedOn: Date;
        ExtractionMode: string;
        Exclude : boolean = false;
   }
  
  
    export class SiteSourceToSearch{
        Id: number = 0;
        SiteId: string;
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
        Id: string;
        SiteId: string;
        SiteEnum: number;
        SiteSourceId: number; //Refers to CompForm.SiteSources.Id
        DisplayPosition: number;
        SiteDisplayPosition: number;

        InvestigatorSearchedId: number;
        InvestigatorName: string;

        RowNumberInSource : number;

        IsFullMatch: boolean;
        MatchCount : number;

        IsMatchedRecord: boolean;

        Status : string;
        
        RecordDetails : string;
        DateOfInspection: Date;
        Observation: string;
        IsAnIssue: boolean = false;
        Links: Link[] = [];

        UISelected: boolean = false;
        Selected: boolean = false;
    }

    export class SiteSource{
        RecId: string;
        SiteName :string;
        SiteShortName : string;
        SiteEnum : number; 
        SiteUrl : string;
        Recomended: boolean = false;
        Selected: boolean = false;
        Included: boolean = false; //Included in SiteSource collectio n
        ExtractionMode: string;
        ExcludeSI : boolean = false;
        ExcludePI : boolean = false;

    }  


    export class ComplianceFormManage
    {
        AssignedTo : string;
        Active : Boolean;
    }
    
     export class UpdateFindigs
    {
        //Guid id,  SiteEnum siteEnum, int InvestigatorId, bool ReviewCompleted, List<Finding> Findings
        FormId :string;
        //SiteEnum : number;
        SiteSourceId: number;
        InvestigatorSearchedId : number;
        ReviewCompleted : boolean;
        //InvestigatorSearched: InvestigatorSearched;
        Findings: Finding[];
    }
 

    export class UpdateInstituteFindings
    {
        FormId : string;
        SiteSourceId : number;
        Findings: Finding[];
    }

   export enum ComplianceFormStatusEnum
    {
        NotScanned,
        ReviewCompletedIssuesIdentified,
        ReviewCompletedIssuesNotIdentified,
        FullMatchFoundReviewPending,
        PartialMatchFoundReviewPending,
        NoMatchFoundReviewPending,
         ManualSearchSiteReviewPending,
        IssuesIdentifiedReviewPending,
        HasExtractionErrors
    } 

    export class CompFormFilter
    {
        InvestigatorName: string;
        ProjectNumber: string;
        SponsorProtocolNumber: string;
        SearchedOnFrom: Date;
        SearchedOnTo: Date;
       
        Country: string;
        Status: ComplianceFormStatusEnum
    }
     
    export class CalenderDate
    {
         date: { year: number, month: number, day: number} 
    } 

   export class InstituteFindingsSummaryViewModel
   {
       SiteId: string;
       DisplayPosition : number;
       IsMandatory: Boolean; 
       SiteName : string; 
       SiteShortName : string; 
       SiteUrl : string; 
       IssuesFound : number; 
   }