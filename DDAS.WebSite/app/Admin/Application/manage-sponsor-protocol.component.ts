import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../../shared/utils/config.service';
import {Country} from './appAdmin.classes';
import {SiteSourceViewModel} from './appAdmin.classes';
import {SponsorProtocol} from './appAdmin.classes';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'manage-sponsor-protocol.component.html',
})

export class ManageSponsorProtocolComponent implements OnInit {
    public sponsorProtocol: SponsorProtocol = new SponsorProtocol;
    //public countryViewModel: CountryViewModel[] = [];
    public SiteSource: any[];
    public sponsorProtocolsAdded: any[];
    public pageNumber: number;
    public formLoading: boolean;
    public message: string;
    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.message = "";
        this.loadSponsorProtocol();
        this.loadSiteSources();
    }

    loadSiteSources(){
        this.service.getSiteSources()
        .subscribe((item: any[]) =>{
            this.SiteSource = item;
        },
        error => {

        });
    }

    loadSponsorProtocol(){
        // this.formLoading = false;
        this.sponsorProtocol.SponsorProtocolNumber = "";
        this.sponsorProtocol.SiteId = "";
        
        this.service.getSponsorProtocol()
        .subscribe((item : any[]) => {
            this.sponsorProtocolsAdded = item;
            // this.formLoading = true;
        },
        error => {
            // this.formLoading = false;
        });
    }

    addSponsorProtocol(){
        this.service.addSponsorProtocol(this.sponsorProtocol)
        .subscribe((item: any) => {
            this.message = item;
            this.loadSponsorProtocol();
        },
        error => {
        
        });
    }

    removeSponsorProtocol(RecId: string){
        this.service.removeSponsorProtocol(RecId)
        .subscribe((item: any) => {
            this.loadSponsorProtocol();
        },
        error => {
        
        });
    }
}
