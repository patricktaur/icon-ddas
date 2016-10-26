import { Component, Input, Output, EventEmitter} from '@angular/core';

import {Location} from '@angular/common';
import { SiteDataItemBase, SearchResultSaveData, SiteData, saveSearchDetails} from '../search.classes';
import {SearchService} from '../search-service';
@Component({
    moduleId: module.id,
    selector: 'search-detail',
    templateUrl: 'search-detail.component.html',
    styleUrls: ['../stylesTable.css']
     
})
export class SearchDetailComponent {
    private displayTitle:string;
    private displayName:string;
    private detailItems:SiteDataItemBase[];
    private _isChecked: boolean = false;

    private _SiteData: SiteData ;
    constructor(private service: SearchService, private _location: Location) { }
    
      @Input()
    set ListItems(value: SiteDataItemBase[]) {
        this.detailItems=value;
    }
   
    @Input() set SiteData(sd: SiteData){
        this._SiteData = sd;
    }
  
   @Input()
  get isChecked() {
    return this._isChecked;
  }

   @Output() isCheckedChange = new EventEmitter();

    set isChecked(val: boolean) {
    
    this._isChecked = val;
    this.isCheckedChange.emit(this._isChecked);

    this.checkAll();
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
        this.isChecked = false;
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
    get diagnostic() { return JSON.stringify(this.detailItems); }

}