
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
          //public List<MatchResult> Results { get; set; }
          Results: MatchResult[] = new Array<MatchResult>();
          //Result: ResultAtSite;
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
        Results: MatchResult[] = new Array<MatchResult>();
    }

export class MatchResult
    {
        MatchName: string;
        MatchLocation: string;
    }



 

    