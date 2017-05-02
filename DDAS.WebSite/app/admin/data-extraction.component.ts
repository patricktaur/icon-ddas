import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'data-extraction.component.html',
})
export class DataExtractionComponent implements OnInit {
    public ExtractionDetails: any[];
    public SiteEnum: number = 0;
    public pageNumber: number;
    public formLoading: Boolean;
    constructor(
        private service: LoginHistoryService
    ) { }

    ngOnInit(){
        this.LoadExtractionHistory();
    }

    LoadExtractionHistory(){
        this.formLoading = true;
        this.service.getDataExtractionPerSite(this.SiteEnum)
        .subscribe((item : any[]) => {
            this.ExtractionDetails = item;
            this.formLoading = false;
        },
        error => {
            this.formLoading = false;
        });
    }

    deleteExtractionData(RecId: string){
        this.formLoading = true;
        this.service.deleteExtractionData(RecId, this.SiteEnum)
        .subscribe((item : any) => {
            //this.ExtractionDetails = item;
            console.log("SiteEnum: " + this.SiteEnum)
            this.LoadExtractionHistory();
            this.formLoading = false;
        },
        error => {
            this.formLoading = false;
        });
    }
}