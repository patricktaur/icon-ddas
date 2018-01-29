import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'ddas-to-isprint-log.component.html',
})

export class DDAStoiSprintLogComponent implements OnInit {
    public ddasToiSprintLog: any[];
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
        this.loadDDASToiSprintLog();
    }

    loadDDASToiSprintLog(){
        this.formLoading = false;
        this.service.getDDAStoiSprintLog()
        .subscribe((item : any[]) => {
            this.ddasToiSprintLog = item;
            this.formLoading = true;
        },
        error => {
            // this.formLoading = false;
        });
    }
}