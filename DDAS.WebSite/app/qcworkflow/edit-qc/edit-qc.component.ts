import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { QualityCheck, AuditObservation } from '../qc.classes';
import { ConfigService } from '../../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { QCService } from '../qc-service';
import {
    ComplianceFormA,
    SiteSource,
    Finding,
    Comment,
    CommentCategoryEnum,
    ReviewerRoleEnum,
    ReviewStatusEnum
} from '../../search/search.classes';
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
    public status: number = -1;
    public qcSummary: any[];
    public qcVerifierComment: Comment;

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
        this.listQCSummary();
    }

    loadComplianceForm() {
        this.auditService.getQC(this.complianceFormId, this.qcAssignedTo)
            .subscribe((item: any) => {
                this.complianceForm = item;
                this.isSubmitted = this.isQCPassedOrFailed;
            },
            error => {

            });
    }

    getQCVerifierComment() {
        var review = this.complianceForm.Reviews.find(x =>
            x.AssigendTo == this.authService.userName &&
            x.ReviewerRole == ReviewerRoleEnum.QCVerifier);
    }

    get isQCPassedOrFailed() {
        let review = this.complianceForm.Reviews.find(x =>
            x.Status == ReviewStatusEnum.QCFailed ||
            x.Status == ReviewStatusEnum.QCPassed);

        if(review != undefined && review.Status == ReviewStatusEnum.QCFailed)
            this.status = 0;
        else if(review != undefined && review.Status == ReviewStatusEnum.QCPassed)
            this.status = 1;

        if (review != undefined)
            return true;
        else
            return false;
        // if(this.complianceForm != null && 
        //     (this.complianceForm.CurrentReviewStatus == ReviewStatusEnum.QCPassed) ||
        //     this.complianceForm.CurrentReviewStatus == ReviewStatusEnum.QCFailed){
        //     return true;
        // }
        // else
        //     return false;
    }

    listQCSummary() {
        this.auditService.listQCSummary(this.complianceFormId)
            .subscribe((item: any) => {
                this.qcSummary = item;
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

    // get commentCategoryString(){
    //     if(this.complianceForm.Comments[0].CategoryEnum != null ||
    //     this.complianceForm.Comments[0].CategoryEnum != undefined){
    //         switch(this.complianceForm.Comments[0].CategoryEnum){
    //             case CommentCategoryEnum.Minor: return "Minor";
    //             case CommentCategoryEnum.Major: return "Major";
    //             case CommentCategoryEnum.Critical: return "Critical";
    //             case CommentCategoryEnum.Suggestion: return "Suggestion";
    //             case CommentCategoryEnum.Others: return "Others";
    //             case CommentCategoryEnum.CorrectionPending: return "Correction Pending";
    //             case CommentCategoryEnum.CorrectionCompleted: return "Correction Completed";
    //             case CommentCategoryEnum.Accepted: return "Accepted";
    //             default: "";
    //         }
    //     }
    //     else
    //         return null;
    // }

    openComplianceForm() {
        //this.router.navigate(['comp-form-edit', this.complianceForm.RecId, { rootPath: '', page: this.pageNumber }], { relativeTo: this.route });
        this.router.navigate(['comp-form-edit', this.complianceForm.RecId, { rootPath: 'qc', page: this.pageNumber }], { relativeTo: this.route.parent });
    }

    save() {
    }

    submit() {
        alert('You are about to submit QC. You will not be allowed to edit QC. Do you want to proceed ?');

        if (this.status == -1) {
            alert('Please select one of the options: QC passed or failed');
            return;
        }

        this.complianceForm.Reviews.forEach(review => {
            if (review.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase()) {
                review.Status =
                    this.status == 0 ? ReviewStatusEnum.QCFailed : ReviewStatusEnum.QCPassed;
            }
        });

        this.auditService.saveQC(this.complianceForm)
            .subscribe((item: any) => {
                this.isSubmitted = true;
                alert('QC has been submitted successfully');
            },
            error => {
            });
    }

    goBack() {
        this._location.back();
    }
}