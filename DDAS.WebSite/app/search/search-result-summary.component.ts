import { Component} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {SiteInfo,SearchSummaryItem,SearchSummary,NameSearch} from './search.classes';
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
        this.NameToSearch = params['name'];
        this.LoadSearchSummary();
      });
    
  }

  LoadSearchSummary(){
        
        this.SearchName={'NameToSearch' : this.NameToSearch};
        this.processing = true;
        this.service.getSearchSummary(this.NameToSearch)
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
   
 onSelectedSite(summary: SearchSummaryItem) {
       //this.router.navigate([summary.SiteName,this.NameToSearch, summary.RecId], { relativeTo: this.route.parent });
       //'details/:siteEnum/:name/:id',
       this.router.navigate(["details", summary.SiteEnum,this.NameToSearch,  this.SearchSummary.ComplianceFormId], { relativeTo: this.route.parent });
 }

 get diagnostic() { return JSON.stringify(this.SearchSummary); }
}