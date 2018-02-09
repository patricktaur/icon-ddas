import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {DataExtractorService} from '../data-extractor-service'
//import {LoginHistoryService} from '../../Admin/all-loginhistory.service';

//import {  IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';

@Component({
    moduleId: module.id,
    selector: 'data-extraction-status',
    templateUrl: 'data-extraction-status.component.html',
})
export class DataExtractionStatusComponent implements OnInit {
    public ExtractionDetails: any[];
    public pageNumber: number;
    public formLoading: Boolean;
    constructor(
        private service: DataExtractorService
    ) { }

    ngOnInit(){
        this.LoadExtractionHistory();
    }

    LoadExtractionHistory(){
        this.formLoading = true;
        this.service.getLatestExtractionStatus()
        .subscribe((item : any[]) => {
            this.ExtractionDetails = item;
            this.formLoading = false;
        },
        error => {
            this.formLoading = false;
        });
    }
}