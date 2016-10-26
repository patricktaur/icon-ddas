import { Component} from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import {SiteData, SiteEnum} from '../search.classes';
import {SearchService} from '../search-service';
import {ClinicalInvestigatorInspectionPageData, DebarredPerson} from '../detail-classes/ClinicalInvestigatorInspectionPageData';
import { AuthService }      from '../../auth/auth.service';

@Component({
     moduleId: module.id,
     templateUrl:  'clinical-Investigator-inspection-page-detail.component.html',
     styleUrls: ['../stylesTable.css'],
})

export class ClinicalInvestigatorInspectionPageDetailComponent {
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
            this.siteData.SiteEnum = SiteEnum.ClinicalInvestigatorInspectionPage; //1;
            this.siteData.SiteName ="Clinical Investigator Inspection List (CLIL)(CDER";
            this.LoadSiteResultDetails();
        });
        this.Token=this.authservice.getToken();
        console.log('Token in Clinical Investigator Inspection List (CLIL)(CDER) Site : ' + this.Token);
    }

    LoadSiteResultDetails() {
          this.service.getSearchSummaryDetails(this.siteData.NameToSearch, this.siteData.RecId, this.siteData.SiteEnum)
            .subscribe((item: ClinicalInvestigatorInspectionPageData) => {
                this.siteData.SiteLastUpdatedOn = item.SiteLastUpdatedOn;
                this.siteData.Source = item.Source;
                this.siteData.CreatedOn = item.CreatedOn;
                this.DebarredPersonsList = item.ClinicalInvestigatorInspectionList;
            },
            error => {
                console.log('Error in Clinical Investigator Inspection List (CLIL)(CDER) Site');
            });
    }

    get diagnostic() { return JSON.stringify(this.DebarredPersonsList); }
}