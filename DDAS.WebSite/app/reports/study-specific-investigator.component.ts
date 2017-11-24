import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportService } from './report-service';

@Component({
    moduleId: module.id,
    templateUrl: 'study-specific-investigator.component.html'
})

export class StudySpecificInvestigatorComponent {
    public generating: boolean = false;

    public studySpecificInvestigators: any[];
    public pageNumber: number;
    public formLoading: boolean;
    public projectNumber: string;

    constructor(
        private service: ReportService,
        private route: ActivatedRoute) {

    }

    ngOnInit() {
        
    }
    
    getInvestigators(){
        this.formLoading = true;
        this.service.getStudySpecificInvestigators(this.projectNumber)
            .subscribe((item: any[]) => {
                this.studySpecificInvestigators = item;
                this.formLoading = false;
            },
            error => {
                this.formLoading = false;
            });
    }

    diagnostic(){
        return JSON.stringify(this.studySpecificInvestigators);
    }
}