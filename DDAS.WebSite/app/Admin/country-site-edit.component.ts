import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { LoginHistoryService } from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import { SiteSourceViewModel } from './appAdmin.classes';
import { Country } from './appAdmin.classes';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'country-site-edit.component.html',
})

export class CountrySiteEditComponent implements OnInit {
    public CountrySite: Country = new Country;
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
            let newSiteSource = new Country();
            //newSiteSource.ExtractionMode = "Manual";
            this.CountrySite = newSiteSource;
        }
        else{        
        this.service.getCountry(this.RecId)
        .subscribe((item: any) => {
            this.CountrySite = item;
            });        
        }
    }

    test: any;
   onSiteSourceChange(value:any){
      var site = this.SiteSources.find(x => x.RecId == value);
      this.CountrySite.Name = site.SiteName;
   }
   
   Save() {
        this.service.saveCountry(this.CountrySite)
            .subscribe((item: any) => {
                this.router.navigate(["/country-site"]);
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
        this.router.navigate(["/country-site"]);
    }

    get diagnostic() { return JSON.stringify(this.test); }
}
