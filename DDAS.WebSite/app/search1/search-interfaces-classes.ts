
export interface ISearchHistory {
    searchedOn: string;
    searchedBy: string;
    SearchName:string;
    SearchCount:number;
}

export interface IStudyNumber{
    StudyNumber: string;
}

export class SearchQuery{
    NameToSearch: string;
    SearchSites: SearhQuerySite[] = new Array<SearhQuerySite>();
}

export class SearhQuerySite
    {
          SiteName:string;
          SiteShortName:string;
          Selected : boolean;
          SiteEnum : number ;
          SiteUrl:string;
          HasErrors: boolean;
          ErrorDescription: string;

          Results: MatchResult[] = new Array<MatchResult>();

          
           Processing: boolean;
           Processed: boolean;

    }

export class SearchQueryAtSite{
    NameToSearch: string;
    SiteEnum : number ;
}


export class SearchResult
    {

        NameToSearch : string;
        SearchedBy : string;
        SearchedOn :string;
        ResultAtSites : ResultAtSite[] = new Array<ResultAtSite>();
    }

export class ResultAtSite
    {
        SiteName:string;
        SiteEnum : number ;
        TimeTakenInMs:string;
         HasErrors: boolean;
          ErrorDescription: string;
        Results: MatchResult[] = new Array<MatchResult>();
    }

export class MatchResult
    {
        MatchName: string;
        MatchLocation: string;
    }


 

    