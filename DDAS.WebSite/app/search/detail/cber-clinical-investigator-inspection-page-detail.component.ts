import { Component} from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import {SiteData} from '../search.classes';
import {SearchService} from '../search-service';
import {CBERClinicalInvestigatorInspectionPageSiteData, DebarredPerson} from '../detail-classes/CBERClinicalInvestigatorInspectionPageSiteData';
import { AuthService }      from '../../auth/auth.service';

@Component({
     moduleId: module.id,
     templateUrl:'fda-debar-page-detail.component.html',
     styleUrls: ['../stylesTable.css'],
})

export class CBERClinicalInvestigatorInspectionPageDetailComponent {
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
            this.siteData.SiteEnum = 5;
            this.siteData.SiteName ="CBER Clinical Investigator Inspection";
            this.LoadSiteResultDetails();
        });
        this.Token=this.authservice.getToken();
        console.log('Token in CBER Clinical Investigator Inspection Site : ' + this.Token);
    }

    LoadSiteResultDetails() {
          this.service.getSearchSummaryDetails(this.siteData.NameToSearch, this.siteData.RecId, this.siteData.SiteEnum)
            .subscribe((item: CBERClinicalInvestigatorInspectionPageSiteData) => {
                this.siteData.SiteLastUpdatedOn = item.SiteLastUpdatedOn;
                this.siteData.Source = item.Source;
                this.siteData.CreatedOn = item.CreatedOn;
                this.DebarredPersonsList = item.DebarredPersons;
            },
            error => {
                console.log('Error in CBER Clinical Investigator Inspection Site');
            });
    }

    get diagnostic() { return JSON.stringify(this.DebarredPersonsList); }
}