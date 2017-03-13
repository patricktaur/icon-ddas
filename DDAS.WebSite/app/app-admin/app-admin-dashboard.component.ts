import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {AppAdminService} from './app-admin.service';
import {LiveSiteScannerProcessModel} from './app-admin.classes';
@Component({
    moduleId: module.id,
    templateUrl: 'app-admin-dashboard.component.html',
})
export class AppAdminDashboardComponent implements OnInit {

   private LiveSiteScannerInfo: LiveSiteScannerProcessModel[] = [];
   constructor(
        private service: AppAdminService
    ) { }
    
    ngOnInit(){
       this.getLiveScannerInfo();
    }

    LaunchLiveScan(){
        this.service.LaunchLiveScanner()
        .subscribe((item: any[]) => {
            
             this.getLiveScannerInfo();
           
        },
        error => {
            //this.formLoading = false;
        });

    }

    getLiveScannerInfo(){
        this.service.getLiveScannerInfo()
        .subscribe((item: LiveSiteScannerProcessModel) => {
            
            this.LiveSiteScannerInfo = item;
            
            //this.formLoading = true;
        },
        error => {
            //this.formLoading = false;
        });
    }

     KillLiveScanner(){
        this.service.KillLiveScanner()
        .subscribe((item: any[]) => {
            console.log("item :" + item);
            this.getLiveScannerInfo();
            //this.formLoading = true;
        },
        error => {
            //this.formLoading = false;
        });
    }

    get diagnostic() { return JSON.stringify(this.LiveSiteScannerInfo); }
}