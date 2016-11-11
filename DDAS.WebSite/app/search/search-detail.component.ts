import { Component, Input, Output, EventEmitter, ViewChild} from '@angular/core';

import {Location} from '@angular/common';
import { ActivatedRoute, Params } from '@angular/router';

import { SiteDataItemBase, SearchResultSaveData, SiteData, saveSearchDetails,SitesIncludedInSearch,  MatchedRecordsPerSite} from './search.classes';
import {SearchService} from './search-service';
import {  Ng2PopupComponent } from 'ng2-popup';


@Component({
    moduleId: module.id,
    selector: 'search-detail',
    templateUrl: 'search-detail.component.html',
    styleUrls: ['./stylesTable.css']
     
})
export class SearchDetailComponent {
    
    @ViewChild(Ng2PopupComponent) popup: Ng2PopupComponent;


    private displayTitle:string;
    private displayName:string;
    private detailItems:SiteDataItemBase[];
    private _isChecked: boolean = false;

    public _SiteData: SiteData = new SiteData;

    private siteDetails: SitesIncludedInSearch;
    
    public matchedRecords: MatchedRecordsPerSite[];

    constructor(private service: SearchService, private _location: Location, private route: ActivatedRoute) { }

   ngOnInit() {
        this.route.params.forEach((params: Params) => {
  
            this._SiteData.RecId = params['formid'];  //RecId = compid
            this._SiteData.SiteEnum = params['siteEnum']; 
            this._SiteData.SiteName ="FDA Debarred Person List";
            
            //this.LoadSiteResultDetails();
            this.LoadMatchedRecords();
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

    LoadMatchedRecords(){
   
   
    this.matchedRecords = [
     { Issues: "Site 1", IssueNumber: 1, RecordDetails: "Site URL",  RowNumber:1, Status:"", Selected : true},
      {Issues: "Site 2",IssueNumber: 2, RecordDetails: "Site URL",  RowNumber:1, Status:"", Selected : true},
       { Issues: "Site 3", IssueNumber: 3, RecordDetails: "Site URL",  RowNumber:1, Status:"", Selected : true},
       ]
    ; 
    
  

    }
    
    

    goBack() {
        this._location.back();
    }
    get diagnostic() { return JSON.stringify(this.siteDetails); }

}