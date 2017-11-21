import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportService } from './report-service';
import { AssignmentHistoryViewModel } from './report.classes';

@Component({
    moduleId: module.id,
    templateUrl: 'investigator-reviewcompletedtime.component.html'
})

export class InvestigatorReviewCompletedTimeComponent {
    public generating: boolean = false;

    public reviewCompletedInvestigators: any[];
    public pageNumber: number;
    public formLoading: boolean;

    constructor(
        private service: ReportService,
        private route: ActivatedRoute) {

    }

    ngOnInit() {
        this.formLoading = true;
        this.service.getInvestigatorReviewCompletedTime()
            .subscribe((item: any[]) => {
                this.reviewCompletedInvestigators = item;
                this.formLoading = false;
            },
            error => {
                this.formLoading = false;
            });
    }
    
    diagnostic(){
        return JSON.stringify(this.reviewCompletedInvestigators);
    }
}