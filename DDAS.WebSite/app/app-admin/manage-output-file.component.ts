import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import { AppAdminService } from './app-admin.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'manage-output-file.component.html',
})
export class ManageOutputFileComponent implements OnInit {
    public outputFiles: any[];
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
        this.service.getOutputFilePath()
            .subscribe((item: any[]) => {
                this.ApiHost = this.configService.getApiHost() + item;
                this.LoadOutputFiles();
            });
    }

    LoadOutputFiles() {
        this.service.getOutputFiles()
            .subscribe((item: any[]) => {
                console.log(item);
                this.outputFiles = item;
            });
    }

    setSelectedRecordDetails(rec: any) {
        this.selectedRecordName = "";
        this.selectedRecordName = rec.GeneratedFileName;
        console.log(this.selectedRecordName);
    }

    Delete(outputFileName: string) {
        console.log(outputFileName);
        this.service.deleteUploadedFile(outputFileName)
            .subscribe((item: any[]) => {
                this.LoadOutputFiles();
            });
    }

    deleteOutputFiles() {
        this.service.deleteAllOutputFiles()
            .subscribe((item: any[]) => {
                this.LoadOutputFiles();
            });
    }
}