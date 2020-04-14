import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import {DataExtractorService} from '../../../data-extractor-service';


@Component({
    moduleId: module.id,
    templateUrl: 'fda-warning-letter-site-data.component.html',
})
export class FDAWarningLetterSiteDataComponent implements OnInit {
    public pageNumber: number;
    public SiteData: any;
    
    constructor(
        private service: DataExtractorService,
        private _location: Location,
    ) { }

    ngOnInit() {
        this.loadData();
    }

    loadData(){
        this.service.getFDAWarningLetterSiteData()
        .subscribe((item: any[]) => {
            this.SiteData = item;
        })
    }

    get SiteDataList(){
        if (this.SiteData == null){
            return null;
        }else{
            return this.SiteData.DebarredPersons;
        }
        
    }
    
    goBack() {
        this._location.back();
    }
    
    get diagnostic() { return JSON.stringify(this.SiteData); }
}