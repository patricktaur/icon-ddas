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

    diagnostic(){
        return JSON.stringify(this.adminDashboardList);
    }
}