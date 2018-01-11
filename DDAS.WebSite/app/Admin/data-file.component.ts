import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { LoginHistoryService } from './all-loginhistory.service';
import { IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import { DownloadDataFilesViewModel } from './appAdmin.classes';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'data-file.component.html',
})
export class DataFileComponent implements OnInit {
    //public dataFiles: DownloadDataFilesViewModel[];
    public ApiHost: string;
    public pageNumber: number;
    public formLoading: boolean;
    private selectedRecId: string;
    public selectedRecordName: string;
    public filterBySite: number;
    public filterDataFilesByText: string;

    public files: DownloadDataFilesViewModel[] = [];

    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService
    ) { }

    ngOnInit() {
        this.filterBySite = 1;
        this.ApiHost = this.configService.getApiHost();
        this.loadDataFiles();
    }

    loadDataFiles() {
        this.service.getAllDataFiles(this.filterBySite)
            .subscribe((item: DownloadDataFilesViewModel[]) => {
                this.files = item;
            });
    }

    get filteredDataFiles() {
        if (this.files == null || this.files == undefined || this.files.length == 0)
            return null;
        else if(this.filterDataFilesByText != null){
            return this.files.filter(x => x.FileName.toLowerCase()
                .includes(this.filterDataFilesByText.toLowerCase()));
        }
        else
            return this.files;
    }    
}