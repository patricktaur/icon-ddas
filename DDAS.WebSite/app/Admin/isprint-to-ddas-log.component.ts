import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'isprint-to-ddas-log.component.html',
})

export class ISprintToDDASLogComponent implements OnInit {
    public iSprintToDDASLog: any[];
    public pageNumber: number = 1;
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
        this.loadISprintToDDASLog();
    }

    loadISprintToDDASLog(){
        this.formLoading = false;
        this.service.getISprintToDDASLog()
        .subscribe((item : any[]) => {
            this.iSprintToDDASLog = item;
            this.formLoading = true;
        },
        error => {
            // this.formLoading = false;
        });
    }
}