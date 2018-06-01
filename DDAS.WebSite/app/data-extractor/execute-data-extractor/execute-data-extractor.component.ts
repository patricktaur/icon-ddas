import { Component, OnInit } from '@angular/core';
import {DataExtractorService} from "../data-extractor-service"
import {StatusActivatorService} from "../shared/service/status.activator.service"
import {SiteEnum} from "../data-extractor.classes"

@Component({
    moduleId: module.id,
    selector: 'data-extractor',
    templateUrl: 'execute-data-extractor.component.html',
})
export class ExecuteDataExtractorComponent implements OnInit {
    public ExtractionDetails: any[];
    public pageNumber: number;
    public extractionInProgress: Boolean;
    public extractionProgressMessage: string;
    public extractionSuccessError: string;
    constructor(
        private service: DataExtractorService,
        private statusActivator: StatusActivatorService
    ) { }

    ngOnInit(){
        this.LoadExtractionHistory();
    }

    LoadExtractionHistory(){
        this.extractionInProgress = true;
        this.service.getLatestExtractionStatus()
        .subscribe((item : any[]) => {
            this.ExtractionDetails = item;
            this.extractionInProgress = false;
        },
        error => {
            this.extractionInProgress = false;
        });
    }

    ExecuteDataExtractor(siteEnum: SiteEnum){
        this.extractionSuccessError = "";
        this.extractionProgressMessage = "Data Extraction in progress for :" + SiteEnum[siteEnum] + " ...";
        this.extractionInProgress = true;
        this.service.executeDataExtractor(siteEnum)
        .subscribe((item : any[]) => {
            this.extractionProgressMessage = "";
            this.extractionSuccessError = "Extraction successful for " + SiteEnum[siteEnum] + "."
            this.extractionInProgress = false;
            this.LoadExtractionHistory();
            this.statusActivator.reloadEvent('clicked');
        },
        error => {
            //var twoAsString = Numbers[myNumber]; // twoAsString == "two"
            this.extractionProgressMessage = "";
            this.extractionSuccessError = "Extraction did not succeed for " + SiteEnum[siteEnum] + ".  The error is logged in Logs/Extraction Log";
            this.extractionInProgress = false;
        });
    }
}