import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import { AppAdminService } from './app-admin.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'manage-uploads.component.html',
})
export class ManageUploadsComponent implements OnInit {
    public AllUploadedFiles: any[];
    public ApiHost: string;
    public pageNumber: number;
    public formLoading: boolean;
    //private selectedRecId: string;
    public selectedRecordName: string;

    constructor(
        private service: AppAdminService,
        private configService: ConfigService
    ) { }

    ngOnInit() {
        this.service.getUploadsFolderPath()
            .subscribe((item: any[]) => {
                this.ApiHost = this.configService.getApiHost() + item;
                this.LoadUploadedFiles();
            });
    }

    LoadUploadedFiles() {
        this.service.getUploadedFiles()
            .subscribe((item: any[]) => {
                console.log(item);
                this.AllUploadedFiles = item;
            });
    }

    setSelectedRecordDetails(rec: any) {
        this.selectedRecordName = "";
        this.selectedRecordName = rec.GeneratedFileName;
        console.log(this.selectedRecordName);
    }

    Delete(generatedFileName: string) {
        console.log(generatedFileName);
        this.service.deleteUploadedFile(generatedFileName)
            .subscribe((item: any[]) => {
                this.LoadUploadedFiles();
            });
    }

    deleteAllUploadedFiles() {
        this.service.deleteAllUploadedFiles()
            .subscribe((item: any[]) => {
                this.LoadUploadedFiles();
            });
    }
}