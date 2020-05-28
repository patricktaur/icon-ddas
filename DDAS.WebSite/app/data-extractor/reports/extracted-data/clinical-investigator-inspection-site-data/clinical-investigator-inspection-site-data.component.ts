import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import {DataExtractorService} from '../../../data-extractor-service';


@Component({
    moduleId: module.id,
    templateUrl: 'clinical-investigator-inspection-site-data.component.html',
})
export class ClinicalInvestigatorInspectionCDERSiteDataComponent implements OnInit {
    public pageNumber: number;
    public SiteData: any;
    public loading: any;
    
    constructor(
        private service: DataExtractorService,
        private _location: Location,
    ) { }

    ngOnInit() {
        this.loadData();
    }

    loadData(){
        this.loading = true;
        this.service.getClinicalInvestigatorInspectionCDERSiteData()
        .subscribe((item: any) => {
            this.SiteData = item;
            this.loading = false;
        })
    }

    get SiteDataList(){
        if (this.SiteData == null){
            return null;
        }else{
            return this.SiteData.ClinicalInvestigatorInspectionList;
        }
        
    }

    goBack() {
        this._location.back();
    }
    
    get diagnostic() { return JSON.stringify(this.SiteData); }
}