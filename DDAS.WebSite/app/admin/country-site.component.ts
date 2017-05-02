import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import {Country} from './appAdmin.classes';
import {SiteSourceViewModel} from './appAdmin.classes';
//import {CountryViewModel} from './appAdmin.classes';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'country-site.component.html',
})

export class AddCountryComponent implements OnInit {
    public countryList: Country = new Country;
    //public countryViewModel: CountryViewModel[] = [];
    public SiteSource: any[];
    public countriesAdded: any[];
    public pageNumber: number;
    public formLoading: boolean;
    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.loadCountries();
        this.loadSiteSources();
    }

    loadSiteSources(){
        this.service.getSiteSources()
        .subscribe((item: any[]) =>{
            this.SiteSource = item;
        },
        error => {

        });
    }

    loadCountries(){
        // this.formLoading = false;
        this.countryList.Name = "";
        this.countryList.SiteId = "";
        
        this.service.getCountries()
        .subscribe((item : any[]) => {
            this.countriesAdded = item;
            // this.formLoading = true;
        },
        error => {
            // this.formLoading = false;
        });
    }

    addCountry(){
        this.service.addCountry(this.countryList)
        .subscribe((item: any) => {
            this.loadCountries();
        },
        error => {
        
        });
    }
    removeCountry(RecId: string){
        this.service.removeCountry(RecId)
        .subscribe((item: any) => {
            this.loadCountries();
        },
        error => {
        
        });        
    }
}
