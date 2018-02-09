import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportService } from './report-service';
import { AdminDashboardViewModel } from './report.classes';

@Component({
    moduleId: module.id,
    templateUrl: 'admin-dashboard.component.html'
})

export class AdminDashboardComponent {
    public generating: boolean = false;

    public adminDashboardList: AdminDashboardViewModel[];
    public pageNumber: number;
    public formLoading: boolean;

    constructor(
        private service: ReportService,
        private router: Router,
        private route: ActivatedRoute) {

    }

    ngOnInit() {
        this.loadAdminDashboardList();
    }
    
    loadAdminDashboardList(){
        this.formLoading = true;
        this.service.getAdminDashboardList()
            .subscribe((item: AdminDashboardViewModel[]) => {
                this.adminDashboardList = item;
                this.formLoading = false;
            },
            error => {
                this.formLoading = false;
            });
    }

    loadDrillDownDetails(assignedTo: string, reportType: number, value: number){
        if(value != 0)
            this.router.navigate(['admin-dashboard-drilldown', assignedTo, reportType], { relativeTo: this.route.parent});
        // this.router.navigate(['admin-dashboard-drilldown'], { relativeTo: this.route.parent});
    }

    isGreaterThanZero(value: number){
        console.log(value);
        if(value == 0)    
            return true;
        else
            return false;
    }

    diagnostic(){
        return JSON.stringify(this.adminDashboardList);
    }
}