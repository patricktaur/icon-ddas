import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
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
    public selectedRecId: string;
    public selectedRecordName: string;

    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.message = "";
        this.loadSponsorProtocols();
    }

    loadSponsorProtocols(){
        // this.formLoading = false;
        this.sponsorProtocol.SponsorProtocolNumber = "";
        this.sponsorProtocol.SiteId = "";
        
        this.service.getSponsorProtocols()
        .subscribe((item : any[]) => {
            this.sponsorProtocolsAdded = item;
            // this.formLoading = true;
        },
        error => {
            // this.formLoading = false;
        });
    }

    // addSponsorProtocol(){
    //     this.service.addSponsorProtocol(this.sponsorProtocol)
    //     .subscribe((item: any) => {
    //         this.message = item;
    //         this.loadSponsorProtocol();
    //     },
    //     error => {
        
    //     });
    // }

    extractionModeIsManual(extractionMode: string){
        return (extractionMode.toLowerCase() == "manual");
    }

  setSelectedRecordDetails(rec: any) {
       this.selectedRecId = rec.RecId;
       this.selectedRecordName = rec.SiteName;   
   }

    Edit(RecId: string){
        this.router.navigate(['sponsor-protocol-edit', RecId], { relativeTo: this.route.parent});
    }
    
    Add(){
        this.router.navigate(['sponsor-protocol-edit', ""], { relativeTo: this.route.parent});
    }

    Delete(){
        this.service.removeSponsorProtocol(this.selectedRecId)
        .subscribe((item: any) => {
            this.loadSponsorProtocols();
        },
        error => {
        
        });
    }
}
