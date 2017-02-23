import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'data-extraction-history.component.html',
})
export class ExtractionHistoryComponent implements OnInit {
    public ExtractionDetails: any[];
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
        this.service.getExtractionHistory()
        .subscribe((item : any[]) => {
            this.ExtractionDetails = item;
            this.formLoading = false;
        },
        error => {
            this.formLoading = false;
        });
    }
}