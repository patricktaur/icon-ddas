//This component and the template is replaced by InvestigatorSummaryComponent
import { Component} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {SiteInfo,SearchSummaryItem,SearchSummary,NameSearch, MatchedRecordsPerSite} from './search.classes';
import {Location} from '@angular/common';
import {SearchService} from './search-service';


@Component({
  moduleId: module.id,
  templateUrl: 'search-result-summary.component.html',
  //styleUrls: ['./stylesTable.css']
})
export class SearchResultSummaryComponent { 
  
   public  NameToSearch:string;
   public FullMatchCount : number;
   public PartialMatchCount : number;
   public TotalIssuesFound: number;
   private SearchName :NameSearch;
   private ComplianceFormId: string;
   
   public SearchSummary : SearchSummary;
   public SearchSummaryItems : SearchSummaryItem[];
   
   public OpenClose :string = "Open";

   SiteName : string;
   SiteEnum : number;

    w3site:string;
   
   processing: boolean;


   constructor(private service: SearchService,
       private route: ActivatedRoute,
       private _location: Location,
       private router: Router
  ) {}
  

  ngOnInit() {
      
      this.route.params.forEach((params: Params) => {
        this.ComplianceFormId = params['formid'];
        this.NameToSearch = params['NameToSearch'];
        this.FullMatchCount = params['FullMatchCount'];
        this.PartialMatchCount = params['PartialMatchCount'];
        //this.loadMockSearchSummary();
        this.LoadSearchSummary();
      });
    
  }
  OpenCloseValueChange(value:string){
      if (value=="Open"){
          this.OpenClose="Close";
      }
      else{
          this.OpenClose="Open";
      }
      
  }

  LoadSearchSummary(){
        
        this.SearchName={'NameToSearch' : this.NameToSearch};
        this.processing = true;
        
        //this.loadMockSearchSummary();
        //required:
        this.service.getSearchSummary(this.ComplianceFormId)
        .subscribe((item) => {
            this.processing = false;
            this.SearchSummary = item;
            this.SearchSummaryItems=item.SearchSummaryItems;
            this.FullMatchCount = item.Sites_FullMatchCount;
            this.PartialMatchCount = item.Sites_PartialMatchCount;
            this.TotalIssuesFound = item.TotalIssuesFound;
            console.log('In Summery :  ' + this.SearchSummary);
             console.log('In Summery Data :  ' + item.SearchSummaryItems);
         },
        error => {
            this.processing = false;
            //this.slimLoader.complete();
            //this.notificationService.printErrorMessage('Failed to load users. ' + error);
        });
        
 }

 
 gotoSearch() {
   
    this.router.navigate(['/search']);
 }

 onSelect(summary: SiteInfo) {
 
        this.router.navigate([summary.Component, summary.SiteId], { relativeTo: this.route.parent });
 }
   
 onSelectedSite(item: SearchSummaryItem) {
       console.log(item.SiteName);
       this.router.navigate(["details", item.SiteEnum, this.ComplianceFormId,this.NameToSearch], { relativeTo: this.route.parent });
 }

 loadMockSearchSummary(){

  
     this.SearchSummaryItems = [
     {RecId: "1", SiteName: "Site 1", SiteUrl: "Site URL", MatchStatus : "No match", SiteEnum:1},
      {RecId: "1",SiteName: "Site 1", SiteUrl: "Site URL", MatchStatus : "No match", SiteEnum:1},
       {RecId: "1",SiteName: "Site 1", SiteUrl: "Site URL", MatchStatus : "No match", SiteEnum:1},
       ]
    ; 
 }

 MatchCountHighlight(MatchCount:number){
    if (MatchCount==0){
      return true;
    }
    else{
      return false;
    }
}

dividerGeneration(indexVal : number){
    if ((indexVal+1) % 3 == 0){
        return true;
    }
    else{
        return false;
    }
}

 get diagnostic() { return JSON.stringify(this.SearchSummary); }
}