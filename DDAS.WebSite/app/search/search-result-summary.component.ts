import { Component} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {SiteInfo,SearchSummaryItem,SearchSummary,NameSearch, MatchedRecordsPerSite} from './search.classes';
import {Location} from '@angular/common';
import {SearchService} from './search-service';


@Component({
  moduleId: module.id,
  templateUrl: 'search-result-summary.component.html',
  //styleUrls: ['./app/search/styles.css'],  
})
export class SearchResultSummaryComponent { 
  
   public  NameToSearch:string;
   private SearchName :NameSearch;
   private ComplianceFormId: string;
  
   public SearchSummary : SearchSummary;
   public SearchSummaryItems : SearchSummaryItem[];
   
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
        
        this.LoadSearchSummary();
      });
    
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
            this.SearchSummaryItems=item.SearchSummaryItems
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
       this.router.navigate(["details", item.SiteEnum, this.ComplianceFormId], { relativeTo: this.route.parent });
 }

 loadMockSearchSummary(){

  
     this.SearchSummaryItems = [
     {RecId: "1", SiteName: "Site 1", SiteUrl: "Site URL", MatchStatus : "No match", SiteEnum:1},
      {RecId: "1",SiteName: "Site 1", SiteUrl: "Site URL", MatchStatus : "No match", SiteEnum:1},
       {RecId: "1",SiteName: "Site 1", SiteUrl: "Site URL", MatchStatus : "No match", SiteEnum:1},
       ]
    ; 
 }

 get diagnostic() { return JSON.stringify(this.SearchSummary); }
}