import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportService } from './report-service';
import { InvestigatorFindingViewModel } from './report.classes';

@Component({
    moduleId: module.id,
    templateUrl: 'investigator-findings.component.html'
})

export class InvestigatorFindingsComponent {
    public generating: boolean = false;

    public investigatorByFinding: InvestigatorFindingViewModel[];
    public filterValue: number;
    public pageNumber: number;
    public formLoading: boolean;

    constructor(
        private service: ReportService,
        private route: ActivatedRoute) {

    }

    ngOnInit() {
        this.formLoading = true;
        this.service.getInvestigatorsByFinding()
            .subscribe((item: InvestigatorFindingViewModel[]) => {
                this.investigatorByFinding = item;
                this.formLoading = false;
                this.filterValue = -1;
            },
            error => {
                this.formLoading = false;
            });
    }

    get filterRecords(){
    if(this.filterValue == -1)
        return this.investigatorByFinding;
    else if(this.filterValue == 0)
        return this.investigatorByFinding.filter(x => x.SiteName == null);
    else if(this.filterValue == 1)
        return this.investigatorByFinding.filter(x => x.SiteName != null);

    }

    diagnostic(){
        return JSON.stringify(this.investigatorByFinding);
    }
}