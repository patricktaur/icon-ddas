import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportService } from './report-service';
import { AssignmentHistoryViewModel } from './report.classes';
import {FileService} from '../shared/utils/file.service'

@Component({
    moduleId: module.id,
    templateUrl: 'assignment-history.component.html'
})

export class AssignmentHistoryComponent {
    public generating: boolean = false;

    public assignmentHisotryList: AssignmentHistoryViewModel[];
    public pageNumber: number;
    public formLoading: boolean;

    constructor(
        private service: ReportService,
        private route: ActivatedRoute,
        private fileService: FileService
    ) {

    }

    ngOnInit() {
        this.formLoading = true;
        this.service.getAssignmentHistoryList()
            .subscribe((item: AssignmentHistoryViewModel[]) => {
                this.assignmentHisotryList = item;
                this.formLoading = false;
            },
            error => {
                this.formLoading = false;
            });
    }
    
    public Download(){
        this.fileService.download("Reports/AssignmentHistory1");
     }
     
    diagnostic(){
        return JSON.stringify(this.assignmentHisotryList);
    }
}