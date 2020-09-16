import { Location } from '@angular/common';
import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {LogsService} from '../logs-service';

@Component({
    moduleId: module.id,
    templateUrl: 'logs-main.component.html',
    
})
export class LogsMainComponent implements OnInit {
    public loading: boolean = false;
    public error: any;
    public logStatus: boolean;
    

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private logsService : LogsService
        
    ) { }

    ngOnInit() {
        
    }

    logStop(){
        this.logsService.stopLog()
        .subscribe((res : any) => {
            this.loading = false;
        },
        error => {
            console.log(JSON.stringify(error));
            this.loading = false;
        });
    }

    logResume(){
        this.logsService.resumeLog()
        .subscribe((res : any) => {
            this.loading = false;
        },
        error => {
            console.log(JSON.stringify(error));
            this.loading = false;
        });
    }

    getLogStatus(){
        this.logsService.logStatus()
        .subscribe((res : any) => {
            this.loading = false;
            this.logStatus = res;
        },
        error => {
            console.log(JSON.stringify(error));
            this.loading = false;
        });
    }    

}