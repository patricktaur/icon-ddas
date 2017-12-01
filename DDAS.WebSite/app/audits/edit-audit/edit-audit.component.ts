import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Audit, AuditObservation} from '../audit.classes';
import { ConfigService } from '../../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { AuditService } from '../audit-service';
import { ComplianceFormA, SiteSource, Finding } from '../../search/search.classes';
import { Location } from '@angular/common';

@Component({
    moduleId: module.id,
    templateUrl: 'edit-audit.component.html',
})
export class EditAuditComponent implements OnInit {
    public Loading: boolean = false;
    private error: any;
    public auditId: string;
    public complianceFormId: string;
    public SelectedComplianceFormId: string;
    public audit: Audit = new Audit;
    public complianceForm: ComplianceFormA;
    public pageNumber: number;
    public observation: string;
    public siteId: number = 0;
    public isSubmitted: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private configService: ConfigService,
        private _location: Location,
        private authService: AuthService,
        private auditService: AuditService
    ) { }

    ngOnInit() {
        this.Loading = true;
        this.route.params.forEach((params: Params) => {
            this.auditId = params['AuditId'];
        });
        this.complianceForm = new ComplianceFormA;
        this.loadAudit();
    }

    loadAudit(){
        this.auditService.loadAudit(this.auditId)
        .subscribe((item: Audit) => {
            this.audit = item;
            this.isSubmitted = this.audit.IsSubmitted;
            this.loadComplianceForm();
        },
        error => {

        });
    }

    loadComplianceForm(){
        this.auditService.getComplianceForm(this.audit.ComplianceFormId)
        .subscribe((item: any) => {
            this.complianceForm = item;
        },
        error => {

        });
    }

    get Investigators(){
        if(this.complianceForm != undefined || this.complianceForm != null)
            return this.complianceForm.InvestigatorDetails;
        else
            return null;
    }

    get SiteSources(){
        if(this.complianceForm != undefined || this.complianceForm != null)
            return this.complianceForm.SiteSources.filter(x => x.IsMandatory == true);
        else
            return null;
    }

    get additionalSiteSources(){
        if(this.complianceForm != undefined || this.complianceForm != null)
            return this.complianceForm.SiteSources.filter(x => x.IsMandatory == false);
        else
            return null;
    }

    get Findings(){
        if(this.complianceForm != undefined || this.complianceForm != null)
            return this.complianceForm.Findings;
        else
            return null;
    }

    get Observations(){
        if(this.audit.Observations != null || this.audit.Observations != undefined)
            return this.audit.Observations.filter(x => x.Status != null);
        else
            return null;
    }

    clearValues(){
        this.siteId = 0;
        this.observation = null;
    }

    acceptObservation(){
        this.audit.Observations.forEach((observation: AuditObservation) => {
            if(this.siteId == observation.SiteId){
                observation.Comments = this.observation;
                observation.Status = "Accepted";
            }
        });
        this.clearValues();
    }

    rejectObservation(){
        if(this.observation == null || this.observation.length == 0){
            alert("Observation cannot be empty. Please provide your observations");
            return;
        }

        this.audit.Observations.forEach((observation: AuditObservation) => {
            if(this.siteId == observation.SiteId){
                observation.Comments = this.observation;
                observation.Status = "Rejected";
            }
        });
        this.clearValues();
    }

    deleteObservation(deleteObservationById: number){
        var observation = this.audit.Observations.find(x => x.SiteId == deleteObservationById);
        observation.Comments = null;
        observation.Status = null;
    }

    save(){
        this.audit.IsSubmitted = false;
        this.auditService.saveAudit(this.audit)
        .subscribe((item: any) => {

        },
        error => {

        });
    }

    submit(){
        var observation = 
        this.audit.Observations.find(x => x.Status == "accepted" || x.Status == "rejected");

        if(observation == null || observation == undefined)
        {
            alert("Please provide at least one observation to submit the audit");
            return;
        }

        this.audit.IsSubmitted = true;
        this.audit.AuditStatus = "Completed";
        this.audit.CompletedOn = new Date();

        this.auditService.saveAudit(this.audit)
        .subscribe((item: any) => {

        },
        error => {

        });        
    }

        goBack() {
            this._location.back();
    }    
}