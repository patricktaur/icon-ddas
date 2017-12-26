import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportService } from './report-service';
import { AdminDashboardReportType } from './report.classes';
import { IMyDate, IMyDateModel, IMyInputFieldChanged } from '../shared/utils/my-date-picker/interfaces';

@Component({
    moduleId: module.id,
    templateUrl: 'admin-dashboard-drilldown.component.html'
})

export class AdminDashboardDrillDownComponent {
    // public generating: boolean = false;
    public pageNumber: number;
    public formLoading: boolean;
    public drillDownDetails: any[];
    public assignedTo: string;
    public reportType: number;
    public investigatorNameAndCount: string;

    constructor(
        private router: Router,
        private service: ReportService,
        private route: ActivatedRoute) {

    }

    ngOnInit() {
        this.route.params.forEach((params: Params) => {
            this.assignedTo = params['assignedTo'];
            this.reportType = params['reportType'];
            this.getDrillDownDetails();
        });
    }

    get reportName(){
        if(this.reportType == 0)
            return "Opening Balance";
        else if(this.reportType == 1)
            return "Compliance Forms Uploaded";
        else if(this.reportType == 2)
            return "Compliance Forms Completed";
        else if(this.reportType == 3)
            return "Closing Balance";
    }

    getDrillDownDetails(){
        this.formLoading = true;
        this.service.getAdminDashboardDrillDownDetails(this.assignedTo, this.reportType)
            .subscribe((item: any[]) => {
                this.drillDownDetails = this.drillDownList(item);
                this.formLoading = false;
            },
            error => {
                this.formLoading = false;
            });
    }

    drillDownList(list: any[]){
        list.forEach(record =>{
            if(record.InvestigatorCount > 1){
                record.PrincipalInvestigator += " + " + (record.InvestigatorCount - 1);
            }
        });
        return list;
    }

    back(){
        this.router.navigate(['/admin-dashboard']);
    }

    diagnostic(){
        return JSON.stringify(this.drillDownDetails);
    }
}