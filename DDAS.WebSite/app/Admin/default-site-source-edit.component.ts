import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import {SiteSourceViewModel} from './appAdmin.classes';
import {DefaultSite} from './appAdmin.classes';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'default-site-source-edit.component.html',
})

export class DefaultSiteSourceEditComponent implements OnInit {
    public DefaultSite: DefaultSite = new DefaultSite;
    public pageNumber: number;
    public formLoading: boolean;
    private RecId: string;
    private processing: boolean;
    public isNew: boolean = false;
    public isNewText: string = "";    
    public SiteSources: any[];
    public error: string = "";

    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router, 
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.route.params.forEach((params: Params) => {
            this.RecId = params['RecId'];
            this.isNew = false;
            this.isNewText = "Edit";
            if (this.RecId == ""){
                this.isNew = true;
                this.isNewText = "New";
                //this.isNewText.toLowerCase
            }
            this.loadSiteSources();
            
        });
    }

    loadSiteSources(){
        this.service.getSiteSources()
        .subscribe((item: any[]) =>{
            this.SiteSources = item;
            this.LoadRecord();
        },
        error => {

        });
    }
    
    LoadRecord(){
        if (this.RecId == ""){
            let newSiteSource = new DefaultSite();
            //newSiteSource.ExtractionMode = "Manual";
            this.DefaultSite = newSiteSource;
        }
        else{
            this.service.getDefaultSite(this.RecId)
        .subscribe((item: any) => {
            this.DefaultSite = item;
        });
        }
    }

    test: any;
   onSiteSourceChange(value:any){
      var site = this.SiteSources.find(x => x.RecId == value);
      this.DefaultSite.Name = site.SiteName;
   }
   
   Save() {
        this.service.saveDefaultSite(this.DefaultSite)
            .subscribe((item: boolean) => {
                if(!item){
                    this.error = "Default site with same settings already exists";
                }
                else
                    this.router.navigate(["/default-sites"]);
            },
            error => {

            });
    }

    get AppliesToItems(){
        var items: { id: number, name: string }[] = [
        { "id": 0, "name": "PIs and SIs" },
        { "id": 1, "name": "PIs" },
        { "id": 2, "name": "Institute" }];
        return items;
    }
    
    get siteTypes(){
        var items: { id: number, name: string } [] = [
            { "id":0, "name":"Normal" },
            { "id":1, "name":"World Check" }];
            // { "id":2, "name": "DMC Exclusion"}];
        return items;
    }

    CancelSave() {
        this.router.navigate(["/default-sites"]);
    }

    get canShowError(){
        if(this.error.length > 0)
            return true;
        else
            return false;
    }

    get diagnostic() { return JSON.stringify(this.test); }
}