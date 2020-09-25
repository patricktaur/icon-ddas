import { Location } from '@angular/common';
import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {LogsService} from '../logs-service';
import {ConfigService} from '../../shared/utils/config.service'

import {DownloadDataFilesViewModel} from '../../data-extractor/data-extractor.classes';

@Component({
    moduleId: module.id,
    templateUrl: 'logs-main.component.html',
    
})
export class LogsMainComponent implements OnInit {
    public loading: boolean = false;
    public error: any;
    public logStatus: boolean;
    _baseUrl: string = '';
    _downloadUrl : string = '';
    public pageNumber: number = 1;
    // public archivedLogs : any;
    public archivedLogs: DownloadDataFilesViewModel[] = [];

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private logsService : LogsService,
        private configService: ConfigService,
        
        
    ) { 
        this._baseUrl = configService.getApiURI() ;
        //remove api/
        this._downloadUrl = this._baseUrl.replace("api/", "");
    }

    ngOnInit() {
        this.logArchivedFiles();
    }

    logArchivedFiles(){
        this.logsService.getArchivedLogs()
        .subscribe((res : DownloadDataFilesViewModel[]) => {
            this.loading = false;
            this.archivedLogs = res;
        },
        error => {
            console.log(JSON.stringify(error));
            this.loading = false;
        });
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