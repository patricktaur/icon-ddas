import { Component} from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import {SiteData} from '../search.classes';
import {SearchService} from '../search-service';
import {ExclusionDatabaseSearchPageSiteData, DebarredPerson} from '../detail-classes/ExclusionDatabaseSearchPageSiteData';
import { AuthService }      from '../../auth/auth.service';

@Component({
     moduleId: module.id,
     templateUrl:'fda-debar-page-detail.component.html',
     styleUrls: ['../stylesTable.css'],
})

export class ExclusionDatabaseSearchPageDetailComponent {
    private isChecked: boolean = false;
    private siteData: SiteData = new SiteData;
    private Token:string;
    private DebarredPersonsList: DebarredPerson[];

    constructor(private service: SearchService,
        private route: ActivatedRoute,
        private authservice: AuthService
        ) { }

    ngOnInit() {
        this.route.params.forEach((params: Params) => {
            this.siteData.RecId = params['id'];
            this.siteData.NameToSearch = params['name'];
            this.siteData.SiteEnum = 6;
            this.siteData.SiteName ="Exclusion Database Search";
            this.LoadSiteResultDetails();
        });
        this.Token=this.authservice.getToken();
        console.log('Token in Exclusion Database Search Site : ' + this.Token);
    }

    LoadSiteResultDetails() {
          this.service.getSearchSummaryDetails(this.siteData.NameToSearch, this.siteData.RecId, this.siteData.SiteEnum)
            .subscribe((item: ExclusionDatabaseSearchPageSiteData) => {
                this.siteData.SiteLastUpdatedOn = item.SiteLastUpdatedOn;
                this.siteData.Source = item.Source;
                this.siteData.CreatedOn = item.CreatedOn;
                this.DebarredPersonsList = item.DebarredPersons;
            },
            error => {
                console.log('Error in Exclusion Database Search Site');
            });
    }

    get diagnostic() { return JSON.stringify(this.DebarredPersonsList); }
}