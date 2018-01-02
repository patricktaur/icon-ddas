import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { QualityCheck, AuditObservation } from '../qc.classes';
import { ConfigService } from '../../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { QCService } from '../qc-service';
import { ComplianceFormA, 
    SiteSource, 
    Finding,
    Comment, 
    CommentCategoryEnum } from '../../search/search.classes';
import { Location } from '@angular/common';

@Component({
    moduleId: module.id,
    templateUrl: 'edit-qc.component.html',
})
export class EditQCComponent implements OnInit {
    public Loading: boolean = false;
    private error: any;
    public qcId: string;
    public complianceFormId: string;
    public SelectedComplianceFormId: string;
    public audit: QualityCheck = new QualityCheck;
    public complianceForm: ComplianceFormA = new ComplianceFormA;
    public pageNumber: number = 1;
    public observation: string;
    public siteId: number = 0;
    public isSubmitted: boolean;
    public qcAssignedTo: string;
    public commentCategory: string;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private configService: ConfigService,
        private _location: Location,
        private authService: AuthService,
        private auditService: QCService
    ) { }

    ngOnInit() {
        this.isSubmitted = false;
        this.Loading = true;
        this.route.params.forEach((params: Params) => {
            this.complianceFormId = params['complianceFormId'];
            this.qcAssignedTo = params['qcAssignedTo'];
        });
        this.complianceForm = new ComplianceFormA;
        this.loadComplianceForm();
    }

    loadComplianceForm() {
        this.auditService.getQC(this.complianceFormId, this.qcAssignedTo)
            .subscribe((item: any) => {
                this.complianceForm = item;
                this.commentCategory = this.commentCategoryString(this.complianceForm.Comments[0].CategoryEnum);
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
            return this.complianceForm.Findings.filter(x => x.IsAnIssue);
        else
            return null;
    }

    commentCategoryString(categoryEnum: number){
        switch(categoryEnum){
            case CommentCategoryEnum.Minor: return "Minor";
            case CommentCategoryEnum.Major: return "Major";
            case CommentCategoryEnum.Critical: return "Critical";
            case CommentCategoryEnum.Suggestion: return "Suggestion";
            case CommentCategoryEnum.Others: return "Others";
            default: "";
        }
    }

    openComplianceForm(){
        //this.router.navigate(['comp-form-edit', this.complianceForm.RecId, { rootPath: '', page: this.pageNumber }], { relativeTo: this.route });
        this.router.navigate(['comp-form-edit', this.complianceForm.RecId, {rootPath:'qc', page:this.pageNumber}], { relativeTo: this.route.parent });
    
    }

    save() {
    }

    submit() {

    }

    goBack() {
        this._location.back();
    }
}