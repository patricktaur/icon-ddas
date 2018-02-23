import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
//import { Router, ActivatedRoute, Params } from '@angular/router';
//import { ConfigService } from '../shared/utils/config.service';
//import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
//import { AuthService } from '../auth/auth.service';
//import {AppAdminService} from './app-admin.service';
//import {DataExtractorService} from '../data-extractor/data-extractor-service';
import {DataExtractorService} from '../../../data-extractor-service';



@Component({
    moduleId: module.id,
    templateUrl: 'cber-investigate.component.html',
})
export class CBERClinicalInvestigatorInspectionSiteDataComponentXXX implements OnInit {
    public pageNumber: number;
    public CBERInvestigator: any[];
    
    // public basicOptions: Object;
    // public progress: number = 0;
    // public response: any = {};
    // public Loading: boolean = false;
    // public uploadUrl: string;
    // private error: any;
    // public downloadUrl: string;
    // public downloadTemplateUrl: string;
    // public PrincipalInvestigatorNameToDownload: string;

    // public ComplianceFormGenerationError: string;

    // public filterStatus: number = -1;
    // public filterInvestigatorName: string = "";

    //@ViewChild('UploadComplianceFormInputsModal') modal: ModalComponent;

    // public makeActiveCompFormId: string;
    // public makeActivePrincipalInvestigatorName: string

    
    constructor(
        private service: DataExtractorService,
        private _location: Location,
        //private route: ActivatedRoute,
        //private router: Router,
        //private configService: ConfigService,
        //private authService: AuthService
    ) { }

    ngOnInit() {
        this.loadcberRecords();
    }

    loadcberRecords(){
        this.service.getCBERClinicalInvestigatorInspectionSiteData()
        .subscribe((item: any[]) => {
            this.CBERInvestigator = item;
        })
    }

    goBack() {
        this._location.back();
    }
    
    get diagnostic() { return JSON.stringify(this.CBERInvestigator); }
}