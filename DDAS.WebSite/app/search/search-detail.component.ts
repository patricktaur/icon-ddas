import { Component, Input, Output, EventEmitter} from '@angular/core';

import {Location} from '@angular/common';
import { ActivatedRoute, Params } from '@angular/router';

import { SiteDataItemBase, SearchResultSaveData, SiteData, saveSearchDetails,SitesIncludedInSearch,  MatchedRecordsPerSite} from './search.classes';
import {SearchService} from './search-service';
@Component({
    moduleId: module.id,
    selector: 'search-detail',
    templateUrl: 'search-detail.component.html',
    styleUrls: ['./stylesTable.css']
     
})
export class SearchDetailComponent {
    private displayTitle:string;
    private displayName:string;
    private detailItems:SiteDataItemBase[];
    private _isChecked: boolean = false;

    public _SiteData: SiteData = new SiteData;

    private siteDetails: SitesIncludedInSearch;
    
    public matchedRecords: MatchedRecordsPerSite;

    constructor(private service: SearchService, private _location: Location, private route: ActivatedRoute) { }

   ngOnInit() {
        this.route.params.forEach((params: Params) => {
  
            this._SiteData.RecId = params['id'];  //RecId = compid
            this._SiteData.NameToSearch = params['name'];
            this._SiteData.SiteEnum = params['siteEnum']; 
            this._SiteData.SiteName ="FDA Debarred Person List";
            
            this.LoadSiteResultDetails();
        });
    }

  LoadSiteResultDetails() {
          this.service.getSearchSummaryDetails(this._SiteData.NameToSearch, this._SiteData.RecId, this._SiteData.SiteEnum)
            .subscribe((item: any) => {
                this.siteDetails = item;
                this._SiteData.SiteLastUpdatedOn = item.SiteLastUpdatedOn;
                this._SiteData.Source = item.Source;
                this._SiteData.CreatedOn = item.CreatedOn;
                this.matchedRecords = item.MatchedRecords;
            },
            error => {

            });
    }
    
  
   markApproved() {
        
        for (let item of this.detailItems) {
            if (item.Selected == true) {
                item.Status = "Approved"
            }
        }
        this.clearAllSelection();
    }
    
      markRejected() {
        for (let item of this.detailItems) {
            if (item.Selected == true) {
                item.Status = "Rejected"
            }
        }
        this.clearAllSelection();
    }
    clearSelected() {
       
        for (let item of this.detailItems) {
            if (item.Selected == true) {
                item.Status = ""
            }
        }
        this.clearAllSelection();
    }    
  
   checkAll(): void {
        
        if (this.detailItems != undefined){
            this.detailItems.forEach(item => item.Selected = this._isChecked);
         }
    }

    clearAllSelection(): void {
    
        this.detailItems.forEach(item => item.Selected = false);
        //this.isChecked = false;
    }
 
    saveSiteDetails(){
           this.service.saveSiteDetails(this.siteDetails, this._SiteData.RecId)
            .subscribe(
            error => {

            }); 

    }
    
    saveMarkedResults() {
        
        var searchResult = new SearchResultSaveData();
        searchResult.DataId = this._SiteData.RecId;
        searchResult.NameToSearch = this._SiteData.NameToSearch;
        searchResult.SiteEnum = this._SiteData.SiteEnum;

        for (let item of this.detailItems) {
            if ( item.Status) {
                if (item.Status.length > 0 ){
                    var itemToSave = new saveSearchDetails();
                    itemToSave.RowNumber = item.RowNumber;
                    itemToSave.Status = item.Status;
                    searchResult.saveSearchDetails.push(itemToSave);
                }
            }
        }
       
        this.service.saveCheckedResults(searchResult)
            .subscribe(
            error => {

            });
     }

    goBack() {
        this._location.back();
    }
    get diagnostic() { return JSON.stringify(this.siteDetails); }

}