import { Component} from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import {SiteData, SiteEnum} from '../search.classes';
import {SearchService} from '../search-service';
import {SpeciallyDesignedNationalsPageSiteData, DebarredPerson} from '../detail-classes/SpeciallyDesignedNationalsPageSiteData';
import { AuthService }      from '../../auth/auth.service';

@Component({
     moduleId: module.id,
     templateUrl:'specially-designed-nationals-page-detail.component.html',
     styleUrls: ['../stylesTable.css'],
})

export class SpeciallyDesignedNationalsPageDetailComponent {
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
            this.siteData.SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage; // 7;
            this.siteData.SiteName ="Specially Designed Nationals List";
            this.LoadSiteResultDetails();
        });
        this.Token=this.authservice.getToken();
        console.log('token in Specially Designed Nationals List Site : ' + this.Token);
    }

    LoadSiteResultDetails() {
          this.service.getSearchSummaryDetails(this.siteData.NameToSearch, this.siteData.RecId, this.siteData.SiteEnum)
            .subscribe((item: SpeciallyDesignedNationalsPageSiteData) => {
                this.siteData.SiteLastUpdatedOn = item.SiteLastUpdatedOn;
                this.siteData.Source = item.Source;
                this.siteData.CreatedOn = item.CreatedOn;
                this.DebarredPersonsList = item.SDNListSiteData;
            },
            error => {
                console.log('Error in Specially Designed Nationals List Site');
            });
    }

    get diagnostic() { return JSON.stringify(this.DebarredPersonsList); }
}