import { Component, Input, Output, EventEmitter, ViewChild} from '@angular/core';

import {Location} from '@angular/common';
import { ActivatedRoute, Params } from '@angular/router';

import { SiteDataItemBase, SearchResultSaveData, SiteData, saveSearchDetails,SitesIncludedInSearch,  MatchedRecordsPerSite} from './search.classes';
import {SearchService} from './search-service';



@Component({
    moduleId: module.id,
    selector: 'search-detail',
    templateUrl: 'search-detail.component.html',
    //styleUrls: ['./stylesTable.css']
     
})
export class SearchDetailComponent {

    private displayTitle:string;
    private displayName:string;
    private detailItems:SiteDataItemBase[];
    private _isChecked: boolean = false;
    public ObservationCount :number = 0;
    public MatchFoundCount : number = 0;
    public _isObservationCount :boolean = false;
    public _isMatchFoundCount : boolean = false;
    public _SiteData: SiteData = new SiteData;

    private siteDetails: SitesIncludedInSearch;
    
    public matchedRecords: MatchedRecordsPerSite[];

    constructor(private service: SearchService, private _location: Location, private route: ActivatedRoute) { }

   ngOnInit() {
        this.route.params.forEach((params: Params) => {
  
            this._SiteData.RecId = params['formid'];  //RecId = compid
            this._SiteData.SiteEnum = params['siteEnum']; 
            this._SiteData.NameToSearch = params['NameToSearch'];
            
            
            this.LoadMatchedRecords();
            //this.LoadMockMatchedRecords();
        });
    }

  LoadMatchedRecords() {
          
          
          this.service.getSearchSummaryDetails(this._SiteData.NameToSearch, this._SiteData.RecId, this._SiteData.SiteEnum)
            .subscribe((item: any) => {
                this.siteDetails = item;
                this._SiteData.SiteLastUpdatedOn = item.SiteLastUpdatedOn;
                this._SiteData.Source = item.Source;
                this._SiteData.SiteName = item.SiteName;
                this._SiteData.CreatedOn = item.CreatedOn;
                this.matchedRecords = item.MatchedRecords;
                this.GetObservation_MatchFoundCount();
            },
            error => {

            });
    
}
    
  GetObservation_MatchFoundCount(){
      for (let item of this.matchedRecords) {
               // item.RecordDetails=item.RecordDetails.replace('~','<br/>');
                //item.RecordDetails=item.RecordDetails.split('~');
                if (item.HiddenStatus=="selected"){
                    this.ObservationCount += 1;
                }
               else{
                    this.MatchFoundCount += 1;
                }
        }
        //Adder because == will not work in server
        if (this.ObservationCount==0){
            this._isObservationCount=true;
        }
        else{
            this._isObservationCount=false;
        }
        if (this.MatchFoundCount==0){
            this._isMatchFoundCount=true;
        }
        else{
            this._isMatchFoundCount=false;
        }
  }

saveContact = (RecordDetails: string) => {
    var middleNames : string[] = RecordDetails.split("~");
    console.log(middleNames);
    return middleNames;
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

    LoadMockMatchedRecords(){
        // this.matchedRecords = [
        // { Issues: "Site 1", IssueNumber: 1, RecordDetails: "Site URL",  RowNumber:1, Status:"", Selected : true, HiddenStatus:""},
        // {Issues: "Site 2",IssueNumber: 2, RecordDetails: "Site URL",  RowNumber:1, Status:"", Selected : true, HiddenStatus:""},
        // { Issues: "Site 3", IssueNumber: 3, RecordDetails: "Site URL",  RowNumber:1, Status:"", Selected : true, HiddenStatus:""},
        // ]
        // ; 
    }
    
    get SelectedRecords(){
        if (this.matchedRecords==undefined){
            return null;    
        }
        else
        {
            //return this.matchedRecords;
            
            return this.matchedRecords.filter((a) =>
                    a.HiddenStatus == "selected"
                    //a.Selected == true //is not working, hence a workaround
                    //a.IssueNumber > 0
            );
        }
    }

     get NotSelectedRecords(){
        if (this.matchedRecords==undefined){
            return null;    
        }
        else
        {
           // return this.matchedRecords;
            return this.matchedRecords.filter((a) =>
                      //a.Selected != true 
                      a.HiddenStatus != "selected"
            );
        }
    }

    MoveToSelected(matchedRecord: MatchedRecordsPerSite){
        matchedRecord.HiddenStatus = "selected";
        this.ObservationCount=0;
        this.MatchFoundCount = 0;
        this.GetObservation_MatchFoundCount();
    }
    
     RemoveFromSelected(matchedRecord: MatchedRecordsPerSite){
        matchedRecord.HiddenStatus = "";
        this.ObservationCount=0;
        this.MatchFoundCount = 0;
        this.GetObservation_MatchFoundCount();
    }
    goBack() {
        this._location.back();
    }


dividerGeneration(indexVal : number){
    if ((indexVal+1) % 2 == 0){
        return true;
    }
    else{
        return false;
    }
}


    
    get diagnostic() { return JSON.stringify(this.matchedRecords); }

}