import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { LoginHistoryService } from './all-loginhistory.service';
import { IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import { SiteSourceViewModel } from './appAdmin.classes';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'edit-site-source.component.html',
})

export class EditSiteSourceComponent implements OnInit {
    public SiteSource: SiteSourceViewModel = new SiteSourceViewModel;
    public pageNumber: number;
    public formLoading: boolean;

    private RecId: string;

    private processing: boolean;
    public isNew: boolean = false;
    public isNewText: string = "";

    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.params.forEach((params: Params) => {
            this.RecId = params['RecId'];
            this.isNew = false;
            this.isNewText = "Edit";
            if (this.RecId == "") {
                this.isNew = true;
                this.isNewText = "New";
                //this.isNewText.toLowerCase
            }
            this.LoadSiteSource();
        });
    }

    LoadSiteSource() {
        if (this.RecId == "") {
            let newSiteSource = new SiteSourceViewModel();
            newSiteSource.ExtractionMode = "Manual";
            this.SiteSource = newSiteSource;
        }
        else {
            this.service.getSiteSource(this.RecId)
                .subscribe((item: any) => {
                    this.SiteSource = item;
                });
        }

    }

    Save() {
        this.service.saveSiteSource(this.SiteSource)
            .subscribe((item: any) => {
                this.router.navigate(["/manage-site-sources"]);
            },
            error => {

            });
    }

    CancelSave() {
        this.router.navigate(["/manage-site-sources"]);
    }

    get diagnostic() { return JSON.stringify(this.RecId); }
}