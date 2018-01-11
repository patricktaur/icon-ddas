import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportService } from './report-service';

@Component({
    moduleId: module.id,
    templateUrl: 'open-investigations.component.html'
})

export class OpenInvestigationsComponent {
    public generating: boolean = false;
    public openInvestigations: any[];
    public pageNumber: number;
    public formLoading: boolean;

    constructor(
        private service: ReportService,
        private route: ActivatedRoute,
    ) {
    }

    ngOnInit() {
        this.formLoading = true;
        this.service.getOpenInvestigations()
            .subscribe((item: any[]) => {
                this.openInvestigations = item;
                this.formLoading = false;
            },
            error => {
                this.formLoading = false;
            });
    }
    
    diagnostic(){
        return JSON.stringify(this.openInvestigations);
    }
}