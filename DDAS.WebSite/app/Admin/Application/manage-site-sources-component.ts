import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../../shared/utils/config.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'manage-site-sources.component.html',
})

export class ManageSiteSourcesComponent implements OnInit {
    public SiteSources: any[];
    public ApiHost: string;
    public pageNumber: number;
    public formLoading: boolean;
    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService
    ) { }

    ngOnInit(){
        this.formLoading = false;
        this.LoadSiteSources();
    }

    LoadSiteSources()
    {
        this.service.getSiteSources()
        .subscribe((item : any[]) => {
            this.SiteSources = item;
            this.formLoading = true;
        });
    }
}