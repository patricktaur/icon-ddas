import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../../shared/utils/config.service';
import {Country} from './appAdmin.classes';
import {SiteSourceViewModel} from './appAdmin.classes';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'add-country.component.html',
})

export class AddCountryComponent implements OnInit {
    public countryList: Country = new Country;
    public SiteSources: SiteSourceViewModel = new SiteSourceViewModel;
    public pageNumber: number;
    public formLoading: boolean;
    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router, 
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.LoadSiteSources();
    }

    LoadSiteSources(){
        // this.formLoading = false;
        this.service.getSiteSources()
        .subscribe((item : any) => {
            this.SiteSources = item;
            // this.formLoading = true;
        },
        error => {
            // this.formLoading = false;
        });
    }
}
