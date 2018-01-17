import { Location } from '@angular/common';
import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { QualityCheck, AuditObservation } from '../qc.classes';
import { ConfigService } from '../../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { SearchService } from '../../search/search-service';
import { UpdateFindigs } from '../../search/search.classes';
import { QCService } from '../qc-service';
import {
    ComplianceFormA,
    SiteSource,
    Finding,
    Comment,
    CommentCategoryEnum,
    ReviewerRoleEnum,
    ReviewStatusEnum,
    CurrentReviewStatusViewModel
} from '../../search/search.classes';
import {CompFormLogicService} from "../../search/shared/services/comp-form-logic.service";

@Component({
    moduleId: module.id,
    templateUrl: 'edit-qc.component.html',
    styles: [`
    body {
  padding : 10px ;
  
}

#exTab1 .tab-content {
  color : white;
  background-color: #428bca;
  padding : 5px 15px;
}

#exTab2 h3 {
  color : white;
  background-color: #428bca;
  padding : 5px 15px;
}

/* remove border radius for the tab */

#exTab1 .nav-pills > li > a {
  border-radius: 0;
}

/* change border radius for the tab , apply corners on top*/

#exTab3 .nav-pills > li > a {
  border-radius: 4px 4px 0 0 ;
}

#exTab3 .tab-content {
  color : white;
  background-color: #428bca;
  padding : 5px 15px;
}

    `]
})
export class EditQCComponent implements OnInit {
    @ViewChild('FindingResponseModal') FindingResponseModal: ModalComponent;
    public Loading: boolean = false;
    private error: any;
    public qcId: string;
    public complianceFormId: string;
    public SelectedComplianceFormId: string;
    public audit: QualityCheck = new QualityCheck;
    public complianceForm: ComplianceFormA; //= new ComplianceFormA;
    public pageNumber: number = 1;
    public observation: string;
    public siteId: number = 0;
    public isSubmitted: boolean;
    public qcAssignedTo: string;
    public commentCategory: string;
    public status: number = -1;
    public qcSummary: any[];
    public qcVerifierComment: Comment;
    public defaultTab: boolean = true;
    public defaultTabInActive: string = " in active";
    public currentReviewStatus: CurrentReviewStatusViewModel;
    public findingRecordToEdit: Finding;
    
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private configService: ConfigService,
        private _location: Location,
        private authService: AuthService,
        private auditService: QCService,
        private service: SearchService,
        private compFormLogic: CompFormLogicService
    ) { }

    ngOnInit() {
        this.isSubmitted = false;
        this.Loading = true;
        this.route.params.forEach((params: Params) => {
            this.complianceFormId = params['complianceFormId'];
            this.qcAssignedTo = params['qcAssignedTo'];
        });
        //this.complianceForm = new ComplianceFormA;
        this.loadComplianceForm();
        //this.listQCSummary();

        //this.compFormLogic.CanDisplayFindingComponent
    }

    loadComplianceForm() {
        this.auditService.getQC(this.complianceFormId, this.qcAssignedTo)
            .subscribe((item: any) => {
                this.complianceForm = item;
                this.isSubmitted = this.isQCPassedOrFailed;
                this.getCurrentReviewStatus();
            },
            error => {
                
            });
    }

    getCurrentReviewStatus() {
        this.service.getCurrentReviewStatus(this.complianceFormId)
            .subscribe((item: CurrentReviewStatusViewModel) => {
                this.currentReviewStatus = item;
                //console.log('current review status: ', this.currentReviewStatus);
            },
            error => {

            });
    }

    // get QCVerifiedFindings(){
    //     return this.complianceForm.Findings.find(
    //         x => x.Comments[1].CategoryEnum == QCVer
    //     )
    // }

    get principalInvestigatorName(){
        if(this.complianceForm){
            return this.complianceForm.InvestigatorDetails[0].Name;
        }
    }

    getQCVerifierComment() {
        var review = this.complianceForm.Reviews.find(x =>
            x.AssigendTo == this.authService.userName &&
            x.ReviewerRole == ReviewerRoleEnum.QCVerifier);
    }

    get isQCPassedOrFailed() {
        let review = this.complianceForm.Reviews.find(x =>
            x.Status == ReviewStatusEnum.QCFailed ||
            x.Status == ReviewStatusEnum.QCPassed ||
            x.Status == ReviewStatusEnum.Completed);

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

    //Not used:
    listQCSummary() {
        this.auditService.listQCSummary(this.complianceFormId)
            .subscribe((item: any) => {
                this.qcSummary = item;
            },
            error => {
            });
    }

    // get Investigators() {
    //     if (this.complianceForm != undefined || this.complianceForm != null)
    //         return this.complianceForm.InvestigatorDetails;
    //     else
    //         return null;
    // }

    // get SiteSources() {
    //     if (this.complianceForm != undefined || this.complianceForm != null)
    //         return this.complianceForm.SiteSources.filter(x => x.IsMandatory == true);
    //     else
    //         return null;
    // }

    // get additionalSiteSources() {
    //     if (this.complianceForm != undefined || this.complianceForm != null)
    //         return this.complianceForm.SiteSources.filter(x => x.IsMandatory == false);
    //     else
    //         return null;
    // }

    // get Findings() {
    //     if (this.complianceForm != undefined || this.complianceForm != null)
    //         return this.complianceForm.Findings.filter(x => x.IsAnIssue);
    //     else
    //         return null;
    // }

    //Patrick:
    get QCVerifierReview(){
        if(this.complianceForm){
            return this.compFormLogic.getQCVerifierReview(this.complianceForm.Reviews);
        }
    }

    get qcRequestorComment(){
        if(this.QCVerifierReview)
            return this.QCVerifierReview.Comment;
        else
            return null;
    }

    get QCVerifierReviewId(): string{
        if (this.complianceForm){
            return this.compFormLogic.getQCVerifierReviewId(this.complianceForm.Reviews);
        }else{
            return null;
        }
    }

    get QCVerifiedFindings(){
        if (this.complianceForm){
            return this.complianceForm.Findings.filter(x => x.Comments != undefined && 
                x.Comments.length > 0 && 
                x.Comments[0].CategoryEnum != CommentCategoryEnum.NotApplicable);
            
            //this.compFormLogic.getQCVerifiedFindings(this.complianceForm, this.QCVerifierReviewId)
            //.filter(x => x.Comments[0].CategoryEnum != CommentCategoryEnum.NotApplicable);
        }
    }

    get reviewerSubmitQC(){
        if(this.complianceForm && 
            this.complianceForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress &&
            this.currentReviewStatus && 
            this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() ==
            this.authService.userName.toLowerCase()){
            return true;
        }
        else
            return false;
    }

    get CanSubmitCorrectionsCompleted(){
        
        if(this.QCVerifiedFindings) {

            let count = this.QCVerifiedFindings.filter(
                x => x.Comments[0].ReviewerCategoryEnum == CommentCategoryEnum.NotApplicable
                || x.Comments[0].ReviewerCategoryEnum == CommentCategoryEnum.CorrectionPending
                 
            ).length;
            if (count > 0){
                return true
            }else{
                return false;
            }

        }else{
            return false;
        }        
    }


    get canReviewerSubmitQC(){
        if (this.complianceForm){
            return this.complianceForm.Findings.filter(x => x.Comments != undefined && 
                x.Comments.length > 0 && 
                x.Comments[0].CategoryEnum != CommentCategoryEnum.NotApplicable &&
                x.Comments[0].ReviewerCategoryEnum != CommentCategoryEnum.NotApplicable);
        }
    }


    get reviewStatus(){
        if(this.complianceForm){
            return this.compFormLogic.getReviewStatus(this.complianceForm.CurrentReviewStatus);
        }
    }

    //Patrick:    
    selectFindingComponentToDisplay(selectedFinding: Finding, componentName: string) {
        if (selectedFinding){
            //console.log("selected finding:" + JSON.stringify(selectedFinding));
            //console.log("this.currentReviewStatus:" + JSON.stringify(this.currentReviewStatus));
            return this.compFormLogic.CanDisplayFindingComponent(selectedFinding, componentName, this.currentReviewStatus)
        }else{
            return false;
        }
    }

    openFindingDialog(qcSummary: any){
        console.log("FindingId" + qcSummary.FindingId);
        let findingToEdit = this.complianceForm.Findings.find(x => x.Id == qcSummary.FindingId);
        this.findingRecordToEdit =  findingToEdit;
        this.FindingResponseModal.open();
    }
    
    openFindingDialogA(finding: Finding){
       
        this.findingRecordToEdit =  finding;
        this.FindingResponseModal.open();
    }

    getcommentCategory(categoryEnum: CommentCategoryEnum){
        return CommentCategoryEnum[categoryEnum];
    }
    
    getReviewerCategory(categoryEnum: CommentCategoryEnum){
        return CommentCategoryEnum[categoryEnum];
    }

    getSourceName(finding: Finding){
        if (finding){
            return this.complianceForm.SiteSources.find( x=> x.Id == finding.SiteSourceId).SiteShortName;
        }
        //return "";
    }

    getType(finding: Finding){
        if (finding ){
            if (finding.ReviewId == this.QCVerifierReview.RecId){
                return "Finding";
            }else{
                return "Comment";
            }
        }else{
            return "--";
        }
    }
    openComplianceForm() {
        //this.router.navigate(['comp-form-edit', this.complianceForm.RecId, { rootPath: '', page: this.pageNumber }], { relativeTo: this.route });
        //this.qcAssignedTo
        this.router.navigate(['comp-form-edit', this.complianceForm.RecId, 
        {rootPath:'edit-qc', qcAssignedTo:this.qcAssignedTo}], 
        { relativeTo: this.route.parent });
    }

    Save() {

        //this.service.saveReviewCompletedComplianceForm(this.complianceForm);

        this.service.saveReviewCompletedComplianceForm(this.complianceForm)
        .subscribe((item: ComplianceFormA) => {
            console.log("Save Called");
        },
        error => {
        });
    }

    get canSubmitQC(){
        if(this.complianceForm && this.compFormLogic.isLoggedInUserQCVerifier && 
            this.complianceForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress)
            return true;
        else
            return false;
    }

    get isVerifierFindingNA(){
        return this.QCVerifiedFindings.forEach(finding =>{
            if(finding.Comments[0].CategoryEnum == CommentCategoryEnum.NotApplicable)
                return finding;
            });
    }



    submit() {
        alert('You are about to submit QC. You will not be allowed to edit QC. Do you want to proceed ?');

        if(this.compFormLogic.isLoggedInUserQCVerifier){
            if (this.status == -1) {
                alert('Please select one of the options: QC passed or failed');
                return;
            }
            else if(this.isVerifierFindingNA) {
                alert("finding added by verifier cannot be 'Not Applicable'" +
                "Request you to update the finding - " +
                this.isVerifierFindingNA);
                return;
            }
            this.complianceForm.Reviews.forEach(review => {
                if (review.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase()) {
                    review.Status =
                        this.status == 0 ? ReviewStatusEnum.QCFailed : ReviewStatusEnum.QCPassed;
                }
            });
        }

        this.auditService.saveQC(this.complianceForm)
            .subscribe((item: any) => {
                this.isSubmitted = true;
                this.goBack();
            },
            error => {
            });
    }

    submitQCByReviewer(){
        alert('You are about to submit QC. You will not be allowed to edit QC. Do you want to proceed ?');

        this.auditService.saveQC(this.complianceForm)
            .subscribe((item: any) => {
                this.isSubmitted = true;
                this.goBack();
            },
            error => {
            });
    }

    goBack() {
        //this._location.back();
        this.router.navigate(["qc"]);
    }

    get diagnostic() { return JSON.stringify(this.findingRecordToEdit) }
    
    

}