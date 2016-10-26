export class SiteInfo
    {

        SiteName : string;
        SiteId : number;
        Component:string;
        
    }
export class NameSearch{
        NameToSearch:string;
    }

export class SearchSummary
    {
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



//tokenDetails
export class TokenDetails{
    access_token:string;
    token_type:string;
    expires_in: number;
    userName: string;
    
  }

export const enum SiteEnum{
    FDADebarPage,
    ClinicalInvestigatorInspectionPage,
    FDAWarningLettersPage,
    ERRProposalToDebarPage,
    AdequateAssuranceListPage,
    ClinicalInvestigatorDisqualificationPage,
    CBERClinicalInvestigatorInspectionPage,
    PHSAdministrativeActionListingPage,
    ExclusionDatabaseSearchPage,
    CorporateIntegrityAgreementsListPage,
    SystemForAwardManagementPage,
    SpeciallyDesignedNationalsListPage
}