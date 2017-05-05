import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { LoginHistoryService } from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import { SiteSourceViewModel } from './appAdmin.classes';
import { SponsorProtocol } from './appAdmin.classes';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'sponsor-protocol-edit.component.html',
})

export class SponsorSpecificSiteEditComponent implements OnInit {
    public SponsorSpecificSite: SponsorProtocol = new SponsorProtocol;
    public pageNumber: number;
    public formLoading: boolean;
    
    private RecId: string;

     private processing: boolean;
     public isNew: boolean = false;
    public isNewText: string = "";    
    public SiteSources: any[];
    
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
            let newSiteSource = new SponsorProtocol();
            //newSiteSource.ExtractionMode = "Manual";
            this.SponsorSpecificSite = newSiteSource;
        }
        else{        
        this.service.getSponsorProtocol(this.RecId)
        .subscribe((item: any) => {
            this.SponsorSpecificSite = item;
            });        
        }
    }

    test: any;
   onSiteSourceChange(value:any){
      var site = this.SiteSources.find(x => x.RecId == value);
      this.SponsorSpecificSite.Name = site.SiteName;
   }
   
   Save() {
        this.service.saveSponsorProtocol(this.SponsorSpecificSite)
            .subscribe((item: any) => {
                this.router.navigate(["/manage-sponsor-protocol"]);
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
    
    CancelSave() {
        this.router.navigate(["/manage-sponsor-protocol"]);
    }

    get diagnostic() { return JSON.stringify(this.test); }
}
