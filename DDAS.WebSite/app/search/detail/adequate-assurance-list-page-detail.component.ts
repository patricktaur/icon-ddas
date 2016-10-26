import { Component} from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import {SiteData} from '../search.classes';
import {SearchService} from '../search-service';
import {AdequateAssuranceListPageSiteData, DebarredPerson} from '../detail-classes/AdequateAssuranceListPageSiteData';
import { AuthService }      from '../../auth/auth.service';

@Component({
     moduleId: module.id,
     templateUrl:'adequate-assurance-list-page-detail.component.html',
     styleUrls: ['../stylesTable.css'],
})

export class AdequateAssuranceListPageDetailComponent {
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
            this.siteData.SiteEnum = 2;
            this.siteData.SiteName ="Adequate Assurance List";
            this.LoadSiteResultDetails();
        });
        this.Token=this.authservice.getToken();
        console.log('Token in Adequate Assurance List Site : ' + this.Token);
    }

    LoadSiteResultDetails() {
          this.service.getSearchSummaryDetails(this.siteData.NameToSearch, this.siteData.RecId, this.siteData.SiteEnum)
            .subscribe((item: AdequateAssuranceListPageSiteData) => {
                this.siteData.SiteLastUpdatedOn = item.SiteLastUpdatedOn;
                this.siteData.Source = item.Source;
                this.siteData.CreatedOn = item.CreatedOn;
                this.DebarredPersonsList = item.DebarredPersons;
            },
            error => {
                console.log('Error in Adequate Assurance List Site');
            });
    }

    get diagnostic() { return JSON.stringify(this.DebarredPersonsList); }
}