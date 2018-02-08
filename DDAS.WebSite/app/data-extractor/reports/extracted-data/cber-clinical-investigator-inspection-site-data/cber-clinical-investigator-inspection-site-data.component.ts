import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import {DataExtractorService} from '../../../data-extractor-service';


@Component({
    moduleId: module.id,
    templateUrl: 'cber-clinical-investigator-inspection-site-data.component.html',
})
export class CBERClinicalInvestigatorInspectionSiteDataComponent implements OnInit {
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
        this.service.getCBERClinicalInvestigatorInspectionSiteData()
        .subscribe((item: any[]) => {
            this.SiteData = item;
        })
    }

    get SiteDataList(){
        if (this.SiteData == null){
            return null;
        }else{
            return this.SiteData.ClinicalInvestigator;
        }
        
    }

    goBack() {
        this._location.back();
    }
    
    get diagnostic() { return JSON.stringify(this.SiteData); }
}