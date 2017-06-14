import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ConfigService } from '../shared/utils/config.service';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../auth/auth.service';
import {AppAdminService} from './app-admin.service';
//import { Http, Response, Headers , RequestOptions } from '@angular/http';

@Component({
    moduleId: module.id,
    templateUrl: 'cber-investigate.component.html',
})
export class CBERComponent implements OnInit {
    public basicOptions: Object;
    public progress: number = 0;
    public response: any = {};
    public Loading: boolean = false;
    public uploadUrl: string;
    private error: any;
    public cberRecords: any[];
    public downloadUrl: string;
    public downloadTemplateUrl: string;
    public PrincipalInvestigatorNameToDownload: string;

    public ComplianceFormGenerationError: string;

    public filterStatus: number = -1;
    public filterInvestigatorName: string = "";

    @ViewChild('UploadComplianceFormInputsModal') modal: ModalComponent;

    public makeActiveCompFormId: string;
    public makeActivePrincipalInvestigatorName: string

    public pageNumber: number;
    constructor(
        private service: AppAdminService,
        private route: ActivatedRoute,
        private router: Router,
        private configService: ConfigService,
        private authService: AuthService
    ) { }

    ngOnInit() {
        this.loadcberRecords();
    }

    loadcberRecords(){
        this.service.getcberRecords()
        .subscribe((item: any[]) => {
            this.cberRecords = item;
        })
    }

    get diagnostic() { return JSON.stringify(this.response); }
}