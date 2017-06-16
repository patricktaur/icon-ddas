import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'extraction-log.component.html',
})

export class ExtractionLogComponent implements OnInit {
    public extractionLog: any[];
    public pageNumber: number;
    public formLoading: boolean;
    public message: string;

    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.message = "";
        this.loadExtractionLog();
    }

    loadExtractionLog(){
        this.formLoading = false;
        this.service.getExtractionLog()
        .subscribe((item : any[]) => {
            this.extractionLog = item;
            this.formLoading = true;
            console.log("done");
        },
        error => {
            // this.formLoading = false;
        });
    }
}