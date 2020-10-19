import { Location } from '@angular/common';
import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {LogsService} from '../logs-service';
import {ConfigService} from '../../shared/utils/config.service'

// import {DownloadDataFilesViewModel} from '../../data-extractor/data-extractor.classes';

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
    public ApiHost: string;
    // public archivedLogs : any;
    public archivedLogs: any; //DownloadDataFilesViewModel[] = [];
    public archivedFileCount: any;
    public deleteFilesOlderThan : number = 30;
    public deletedResponse : string = "";

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private logsService : LogsService,
        private configService: ConfigService,
        
        
    ) { 
        this._baseUrl = configService.getApiURI() ;
        //remove api/
        this._downloadUrl = this._baseUrl.replace("api/", "");
        this.ApiHost = this.configService.getApiHost();
    }

    ngOnInit() {
        this.refreshStatus();
    }

    refreshStatus(){
        this.logArchivedFiles();
        this.getLogStatus();
        this.getArchivedFileCount();
    }
    
    logArchivedFiles(){
        this.logsService.getArchivedLogs()
        .subscribe((res : any) => {
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

    getArchivedFileCount(){
        this.logsService.archivedFileCount()
        .subscribe((res : any) => {
            this.loading = false;
            this.archivedFileCount = res;
        },
        error => {
            console.log(JSON.stringify(error));
            this.loading = false;
        });
    }

    deleteArchivedFilesOlderThan(){
        this.deletedResponse = "";
        this.logsService.deleteOlderThan(this.deleteFilesOlderThan)
        .subscribe((res : any) => {
            this.loading = false;
            this.deletedResponse = res;
            this.refreshStatus();
        },
        error => {
            console.log(JSON.stringify(error));
            this.loading = false;
        });
    }

}