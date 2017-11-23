import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { LoginHistoryService } from './all-loginhistory.service';
import { IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'data-file.component.html',
})
export class DataFileComponent implements OnInit {
    public dataFiles: any[];
    public ApiHost: string;
    public pageNumber: number;
    public formLoading: boolean;
    private selectedRecId: string;
    public selectedRecordName: string;

    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService
    ) { }

    ngOnInit() {
        // this.service.getErrorImageFolderPath()
        //     .subscribe((item: any) => {
        //         this.ApiHost = this.configService.getApiHost() + item;
        //     });
        this.loadDataFiles();
    }

    loadDataFiles() {
        this.service.getAllDataFiles()
            .subscribe((item: any[]) => {
                this.dataFiles = item;
                this.ApiHost = this.configService.getApiHost();
            });
    }

    setSelectedRecordDetails(rec: any) {
        //    this.selectedRecId = rec.RecId;
        this.selectedRecordName = "";
        this.selectedRecordName = rec.FileName;
    }

    Delete() {
        this.service.deleteErrorImage(this.selectedRecordName)
            .subscribe((item: any[]) => {
                this.loadDataFiles();
            });
    }

    deleteAllErrorImages() {
        this.service.deleteAllErrorImages()
            .subscribe((item: any[]) => {
                //this.ErrorImages = item;
                this.loadDataFiles();
            });
    }
}