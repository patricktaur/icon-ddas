import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import { ConfigService } from '../shared/utils/config.service';

import {DefaultSite} from './appAdmin.classes';


@Component({
    moduleId: module.id,
    templateUrl: 'default-site-source.component.html',
})

export class DefaultSitesComponent implements OnInit {
    public defaultSite: DefaultSite = new DefaultSite;
     public defaultSites: any[];
    public SiteSources: any[];
   
    public pageNumber: number;
    public formLoading: boolean;
    public message: string;
    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.message = "";
        
        this.loadSiteSources();
    }

    loadSiteSources(){
        this.service.getSiteSources()
        .subscribe((item: any[]) =>{
            this.SiteSources = item;
            this.loadDefaultSites();
        },
        error => {

        });
    }

    loadDefaultSites(){
        // this.formLoading = false;
        this.defaultSite.OrderNo = 0;
        this.defaultSite.ExcludeSI = false; 
        this.defaultSite.IsMandatory = true;
        
        this.service.getDefaultSites()
        .subscribe((item : any[]) => {
            this.defaultSites = item;
            // this.formLoading = true;
        },
        error => {
            // this.formLoading = false;
        });
    }

 
    Edit(RecId: string){
        this.router.navigate(['default-site-edit', RecId], { relativeTo: this.route.parent});
    }

    Add(){
        this.router.navigate(['default-site-edit', ""], { relativeTo: this.route.parent});
    } 
   
   
    removeDefaultSite(RecId: string){
        this.service.removeDefaultSite(RecId)
        .subscribe((item: any) => {
            this.loadDefaultSites();
        },
        error => {
        
        });
    }
}
