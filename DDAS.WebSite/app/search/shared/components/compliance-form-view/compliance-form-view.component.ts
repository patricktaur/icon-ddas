import { Component, OnInit, OnDestroy, Input, OnChanges, SimpleChanges } from '@angular/core';
//import { Router, ActivatedRoute, Params } from '@angular/router';
//import { Audit, AuditObservation } from '../audit.classes';
//import { ConfigService } from '../../shared/utils/config.service';
//import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../../../auth/auth.service';
//import { AuditService } from '../audit-service';
import {SearchService} from '../../../search-service'
import { ComplianceFormA, SiteSource, Finding } from '../../../../search/search.classes';
//import { Location } from '@angular/common';

@Component({
    selector: '[compliance-form-view]',
    moduleId: module.id,
    templateUrl: 'compliance-form-view.component.html',
})
export class ComplianceFormViewComponent implements OnInit, OnChanges {
    @Input() ComplianceFormId: string;
    public Loading: boolean = false;
    private error: any;
    
    public SelectedComplianceFormId: string;
    //public audit: Audit = new Audit;
    public complianceForm: ComplianceFormA;
    public pageNumber: number;
    //public observation: string;
    public siteId: number = 0;
    public isSubmitted: boolean;

    constructor(
        //private route: ActivatedRoute,
        //private router: Router,
        //private configService: ConfigService,
        //private _location: Location,
        private authService: AuthService,
        //private auditService: AuditService
        private searchService: SearchService
    ) { }

    ngOnInit() {
        this.isSubmitted = false;
        this.Loading = true;
        // this.route.params.forEach((params: Params) => {
        //     this.auditId = params['AuditId'];
        // });
        // this.complianceForm = new ComplianceFormA;
        //this.loadAudit();
        //this.loadComplianceForm();
    }

    // loadAudit() {
    //     this.auditService.loadAudit(this.auditId)
    //         .subscribe((item: Audit) => {
    //             this.audit = item;
    //             this.isSubmitted = this.audit.IsSubmitted;
    //             this.loadComplianceForm();
    //         },
    //         error => {

    //         });
    // }

    ngOnChanges(changes: SimpleChanges){
        if (this.ComplianceFormId){
            this.loadComplianceForm();
        }
    }
    
    
    loadComplianceForm() {
        console.log("Inside loadComplianceForm:" + this.ComplianceFormId);
        this.searchService.getComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.complianceForm = item;
            },
            error => {

            });
    }

    get Investigators() {
        if (this.complianceForm != undefined || this.complianceForm != null)
            return this.complianceForm.InvestigatorDetails;
        else
            return null;
    }

    get SiteSources() {
        if (this.complianceForm != undefined || this.complianceForm != null)
            return this.complianceForm.SiteSources.filter(x => x.IsMandatory == true);
        else
            return null;
    }

    get additionalSiteSources() {
        if (this.complianceForm != undefined || this.complianceForm != null)
            return this.complianceForm.SiteSources.filter(x => x.IsMandatory == false);
        else
            return null;
    }

    get Findings() {
        if (this.complianceForm != undefined || this.complianceForm != null)
            return this.complianceForm.Findings;
        else
            return null;
    }

    

    // clearValues() {
    //     this.siteId = 0;
    //     this.observation = null;
    // }

    // acceptObservation() {
    //     this.audit.Observations.forEach((observation: AuditObservation) => {
    //         if (this.siteId == observation.SiteId) {
    //             observation.Comments = this.observation;
    //             observation.Status = "Accepted";
    //         }
    //     });
    //     this.clearValues();
    // }

    // rejectObservation() {
    //     if (this.observation == null || this.observation.length == 0) {
    //         alert("Observation cannot be empty. Please provide your observations");
    //         return;
    //     }

    //     this.audit.Observations.forEach((observation: AuditObservation) => {
    //         if (this.siteId == observation.SiteId) {
    //             observation.Comments = this.observation;
    //             observation.Status = "Rejected";
    //         }
    //     });
    //     this.clearValues();
    // }

    // deleteObservation(deleteObservationById: number) {
    //     var observation = this.audit.Observations.find(x => x.SiteId == deleteObservationById);
    //     observation.Comments = null;
    //     observation.Status = null;
    // }

    // save() {
    //     this.audit.IsSubmitted = false;
    //     this.auditService.saveAudit(this.audit)
    //         .subscribe((item: any) => {

    //         },
    //         error => {

    //         });
    // }

    // submit() {
    //     var observation =
    //         this.audit.Observations.find(x => x.Status != null &&
    //             (x.Status.toLowerCase() == "accepted" ||
    //             x.Status.toLowerCase() == "rejected"));
        
    //     console.log('observation: ', observation);
        
    //     if (observation == null || observation == undefined) {
    //         alert("Please provide at least one observation to submit the audit");
    //         return;
    //     }

    //     this.audit.IsSubmitted = true;
    //     this.audit.AuditStatus = "Completed";
    //     this.audit.CompletedOn = new Date();

    //     this.auditService.saveAudit(this.audit)
    //         .subscribe((item: any) => {
    //             this.isSubmitted = true;
    //         },
    //         error => {

    //         });
    // }

    // goBack() {
    //     this._location.back();
    // }
}