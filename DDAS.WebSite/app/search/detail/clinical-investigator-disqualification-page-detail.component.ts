import { Component} from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import {SiteData} from '../search.classes';
import {SearchService} from '../search-service';
import {ClinicalInvestigatorDisqualificationPageSiteData, DebarredPerson} from '../detail-classes/ClinicalInvestigatorDisqualificationPageSiteData';
import { AuthService }      from '../../auth/auth.service';

@Component({
     moduleId: module.id,
     templateUrl:'fda-debar-page-detail.component.html',
     styleUrls: ['../stylesTable.css'],
})

export class ClinicalInvestigatorDisqualificationPageDetailComponent {
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
            this.siteData.SiteEnum = 3;
            this.siteData.SiteName ="Clinical Investigator Disqualification";
            this.LoadSiteResultDetails();
        });
        this.Token=this.authservice.getToken();
        console.log('Token in Clinical Investigator Disqualification Site : ' + this.Token);
    }

    LoadSiteResultDetails() {
          this.service.getSearchSummaryDetails(this.siteData.NameToSearch, this.siteData.RecId, this.siteData.SiteEnum)
            .subscribe((item: ClinicalInvestigatorDisqualificationPageSiteData) => {
                this.siteData.SiteLastUpdatedOn = item.SiteLastUpdatedOn;
                this.siteData.Source = item.Source;
                this.siteData.CreatedOn = item.CreatedOn;
                this.DebarredPersonsList = item.DebarredPersons;
            },
            error => {
                console.log('Error in Clinical Investigator Disqualification Site');
            });
    }

    get diagnostic() { return JSON.stringify(this.DebarredPersonsList); }
}