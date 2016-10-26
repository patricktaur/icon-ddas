import { Component} from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import {SiteData} from '../search.classes';
import {SearchService} from '../search-service';
import {ErrorProposalToDebarPageSiteData, DebarredPerson} from '../detail-classes/ErrorProposalToDebarPageSiteData';
import { AuthService }      from '../../auth/auth.service';

@Component({
     moduleId: module.id,
     templateUrl:'Error-proposal-to-debar-page-detail.component.html',
     styleUrls: ['../stylesTable.css'],
})

export class ErrorProposalToDebarPageDetailComponent {
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
            this.siteData.SiteEnum = 1;
            this.siteData.SiteName ="ERR Proposal To Debar";
            this.LoadSiteResultDetails();
        });
        this.Token=this.authservice.getToken();
        console.log('Token in ERROR Proposal To Debar Site : ' + this.Token);
    }

    LoadSiteResultDetails() {
          this.service.getSearchSummaryDetails(this.siteData.NameToSearch, this.siteData.RecId, this.siteData.SiteEnum)
            .subscribe((item: ErrorProposalToDebarPageSiteData) => {
                this.siteData.SiteLastUpdatedOn = item.SiteLastUpdatedOn;
                this.siteData.Source = item.Source;
                this.siteData.CreatedOn = item.CreatedOn;
                this.DebarredPersonsList = item.DebarredPersons;
                console.log('item in ERROR Proposal To Debar' + item);
            },
            error => {
                console.log('Error in ERROR Proposal To Debar Site');
            });
    }

    get diagnostic() { return JSON.stringify(this.DebarredPersonsList); }
}