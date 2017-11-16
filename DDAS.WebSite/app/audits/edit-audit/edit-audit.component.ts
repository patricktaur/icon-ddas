import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Audit, AuditObservation} from '../audit.classes';
import { ConfigService } from '../../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { AuditService } from '../audit-service';

@Component({
    moduleId: module.id,
    templateUrl: 'edit-audit.component.html',
})
export class EditAuditComponent implements OnInit {
    public Loading: boolean = false;
    private error: any;
    public complianceFormId: string;
    public SelectedComplianceFormId: string;
    public audit: Audit = new Audit;
    public pageNumber: number;
    public observation: string;
    public siteId: number;
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private configService: ConfigService,
        private authService: AuthService,
        private auditService: AuditService
    ) { }

    ngOnInit() {
        this.Loading = true;
        this.route.params.forEach((params: Params) => {
            this.complianceFormId = params['AuditId'];
        });
        this.loadAudit();
    }

    loadAudit(){
        this.auditService.loadAudit(this.complianceFormId)
        .subscribe((item: Audit) => {
            this.audit = item;
            console.log('audit: ', this.audit);
        },
        error => {

        });
    }

    get Observations(){
        if(this.audit.Observations != null || this.audit.Observations != undefined)
            return this.audit.Observations.filter(x => x.Status != null);
        else
            return null;
    }

    clearValues(){
        this.siteId = -1;
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
        this.audit.Observations.forEach((observation: AuditObservation) => {
            if(this.siteId == observation.SiteId){
                observation.Comments = this.observation;
                observation.Status = "Rejected";
            }
        });
        this.clearValues();
    }

    deleteObservation(deleteObservationById: number){
        this.audit.Observations.forEach((observation: AuditObservation) => {
            if(observation.SiteId == deleteObservationById){
                var index = this.audit.Observations.indexOf(observation);
                if(index !== -1)
                    this.audit.Observations.splice(index, 1);
            }
        });
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
        this.audit.IsSubmitted = true;
        this.audit.AuditStatus = "Completed";
        this.audit.CompletedOn = new Date();
        console.log(this.audit);
        this.auditService.saveAudit(this.audit)
        .subscribe((item: any) => {

        },
        error => {

        });        
    }
}