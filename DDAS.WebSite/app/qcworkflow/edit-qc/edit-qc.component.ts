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
    CurrentReviewStatusViewModel,
    UndoEnum,
    QCCompletedStatusEnum
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
    public qcVerifierFullName: string;
    public defaultTab: boolean = true;
    public defaultTabInActive: string = " in active";
    public currentReviewStatus: CurrentReviewStatusViewModel;
    public findingRecordToEdit: Finding;
    private pageChanged: boolean = false;
    private recordToDelete: Finding = new Finding;
    public Attachments: string[];
    public modalTitle: string;
    public modalText: string;
    public showGenralComment: boolean = false;
    

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
        this.loadAttachments();
        //this.listQCSummary();
    }

    loadComplianceForm() {
        this.auditService.getQC(this.complianceFormId, this.qcAssignedTo)
            .subscribe((item: any) => {
                this.complianceForm = item;
                this.isSubmitted = this.isQCPassedOrFailed;
                this.getCurrentReviewStatus();
                this.loadQCVerifierFullName(this.complianceForm.QCVerifier);
            },
            error => {
                
            });
    }

    loadAttachments(){
        this.auditService.getAttachmentsList(this.complianceFormId)
            .subscribe((item: any) => {
                this.Attachments = item;
            },
            error => {
                
            });
    }

    loadQCVerifierFullName(qcVerifier: string){
        this.service.getUserFullName(qcVerifier)
        .subscribe((item: string) => {
            this.qcVerifierFullName = item;
            },
        error => {

        });
    }

    getCurrentReviewStatus() {
        this.service.getCurrentReviewStatus(this.complianceFormId)
            .subscribe((item: CurrentReviewStatusViewModel) => {
                this.currentReviewStatus = item;
                this.Loading = false;
                //console.log('current review status: ', this.currentReviewStatus);
            },
            error => {

            });
    }

    getAttachmentDownloadURL(fileName: string){
        return "Search/DownloadAttachmentFile?formId=" + this.complianceFormId + "&fileName=" + fileName;
        // return "Search/DownloadAttachmentFile?formId=" + this.complianceFormId
        // + "&fileName=" + fileName;
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
            x.QCStatus == QCCompletedStatusEnum.NoIssues ||
            x.QCStatus == QCCompletedStatusEnum.IssuesNoted);

        if(review != undefined && review.QCStatus == QCCompletedStatusEnum.NoIssues)
            this.status = 2;
        else if(review != undefined && review.QCStatus == QCCompletedStatusEnum.IssuesNoted)
            this.status = 3;

        if (review != undefined)
            return true;
        else
            return false;
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
                x.Selected == true);
                //x.Comments[0].CategoryEnum != CommentCategoryEnum.NotApplicable);
            
            //this.compFormLogic.getQCVerifiedFindings(this.complianceForm, this.QCVerifierReviewId)
            //.filter(x => x.Comments[0].CategoryEnum != CommentCategoryEnum.NotApplicable);
        }
    }

    get reviewStatus(){
        if(this.complianceForm){
            return this.compFormLogic.getReviewStatus(this.complianceForm.CurrentReviewStatus);
        }
    }

    enumValue(value: CommentCategoryEnum){
        return this.compFormLogic.getCommentCategoryEnumValue(value);
    }

    get QCGeneralComment(){
        if(this.complianceForm && this.complianceForm.QCGeneralComment){
            if(this.complianceForm.QCGeneralComment.ReviewerCategoryEnum == CommentCategoryEnum.Minor){
                this.complianceForm.QCGeneralComment.CategoryEnum = CommentCategoryEnum.NotApplicable;
                this.complianceForm.QCGeneralComment.ReviewerCategoryEnum = CommentCategoryEnum.Accepted;
            }
            return this.complianceForm.QCGeneralComment;
        }
        else if(this.complianceForm) {
            this.complianceForm.QCGeneralComment = new Comment();
            this.complianceForm.QCGeneralComment.CategoryEnum = CommentCategoryEnum.NotApplicable;
            this.complianceForm.QCGeneralComment.ReviewerCategoryEnum = CommentCategoryEnum.Accepted;
            return this.complianceForm.QCGeneralComment;
        }
    }

    get QCAttachmentComment(){
        if(this.complianceForm && this.complianceForm.QCAttachmentComment){
            if(this.complianceForm.QCAttachmentComment.ReviewerCategoryEnum == CommentCategoryEnum.Minor){
                this.complianceForm.QCAttachmentComment.CategoryEnum = CommentCategoryEnum.NotApplicable;
                this.complianceForm.QCAttachmentComment.ReviewerCategoryEnum = CommentCategoryEnum.Accepted;
            }
            return this.complianceForm.QCAttachmentComment;
        }
        if(this.complianceForm) {
            this.complianceForm.QCAttachmentComment = new Comment();
            this.complianceForm.QCAttachmentComment.CategoryEnum = CommentCategoryEnum.NotApplicable;
            this.complianceForm.QCAttachmentComment.ReviewerCategoryEnum = CommentCategoryEnum.Accepted;
            return this.complianceForm.QCAttachmentComment;
        }
    }

    get canDisplayComments(){
        let value = false;
        if(this.complianceForm && this.complianceForm.QCGeneralComment){
            if(this.complianceForm.QCGeneralComment.CategoryEnum != CommentCategoryEnum.NotApplicable)
                value = true;
        }
        
        if(this.complianceForm && this.complianceForm.QCAttachmentComment){
            if(this.complianceForm.QCAttachmentComment.CategoryEnum != CommentCategoryEnum.NotApplicable)
                value = true;
        }
        else
            value = false;

        return value;
    }

    get canDisplayGeneralComment(){
        // return false;
        if(this.complianceForm && this.complianceForm.QCGeneralComment &&
            this.complianceForm.QCGeneralComment.CategoryEnum != CommentCategoryEnum.NotApplicable){
            return true;
        }else{
            return false;
        }
    }

    get canDisplayAttachmentComment(){
        if(this.complianceForm && this.complianceForm.QCAttachmentComment){
            if(this.complianceForm.QCAttachmentComment.CategoryEnum != CommentCategoryEnum.NotApplicable)
                return true;
        }
        else
            return false;
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
        let findingToEdit = this.complianceForm.Findings.find(x => x.Id == qcSummary.FindingId);
        this.findingRecordToEdit =  findingToEdit;
        this.FindingResponseModal.open();
    }
    
    openFindingDialogA(finding: Finding) {
        this.findingRecordToEdit =  finding;
        this.FindingResponseModal.open();
    }

    getcommentCategory(categoryEnum: CommentCategoryEnum){
        return CommentCategoryEnum[categoryEnum];
    }
    
    getReviewerCategory(categoryEnum: CommentCategoryEnum){
        return CommentCategoryEnum[categoryEnum];
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
        this.excludeOrIncludeFinding();
        this.Loading = true;
        this.service.saveReviewCompletedComplianceForm(this.complianceForm)            
        .subscribe((item: boolean) => {
            // this.goBack();
            this.pageChanged = false;
            this.Loading = false;
        },
        error => {
            this.Loading = false;
        });
    }

    get canSubmitQC(){
        if(this.complianceForm && this.compFormLogic.isLoggedInUserQCVerifier(this.complianceForm) && 
            this.complianceForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress)
            return true;
        else
            return false;
    }

    get disableQCResponse(){
        if(this.complianceForm){
            return this.compFormLogic.canDisableQCResponse(this.complianceForm);
        }
        else
            return true; //disable the QC response dropdown
    }

    get isVerifierFindingNA(){
        return this.QCVerifiedFindings.forEach(finding =>{
            if(finding.Comments[0].CategoryEnum == CommentCategoryEnum.NotApplicable)
                return finding;
            });
    }

    submit() {
        let review = this.complianceForm.Reviews.find(x => 
            x.Status == ReviewStatusEnum.QCInProgress);
        review.Status = ReviewStatusEnum.QCCompleted;
        review.QCStatus = this.status;

        this.auditService.submitQC(this.complianceForm)
            .subscribe((item: any) => {
                this.isSubmitted = true;
                this.pageChanged = false;
                this.goBack();
            },
            error => {
            });
    }

    get canUndoQCSubmit(){
        if(this.complianceForm){
            return this.compFormLogic.canUndoQCSubmit(this.complianceForm);
        }
        else
            return false;
    }

    undoQCSubmit(){
        this.service.undo(this.complianceForm.RecId, UndoEnum.UndoQCSubmit, '')
        .subscribe((item: any) => {
            this.goBack();
        }, 
        error => {

        });
    }

    excludeOrIncludeFinding(){
        let excludeFindings = this.QCVerifiedFindings.filter(x => 
            x.Comments[0].ReviewerCategoryEnum == CommentCategoryEnum.ExcludeFinding ||
            x.Comments[0].ReviewerCategoryEnum == CommentCategoryEnum.NotAccepted);

        if(excludeFindings){
            excludeFindings.forEach(record => {
                record.IsAnIssue = false;
            });
        }

        let includeFindings = this.QCVerifiedFindings.filter(x =>
            x.Comments[0].ReviewerCategoryEnum == CommentCategoryEnum.Accepted ||
            x.Comments[0].ReviewerCategoryEnum == CommentCategoryEnum.CorrectionCompleted);

        if(includeFindings){
            includeFindings.forEach(record => {
                record.IsAnIssue = true;
            });
        }
    }

    submitQCByReviewer(){
        this.excludeOrIncludeFinding();

        this.auditService.submitQC(this.complianceForm)
            .subscribe((item: ComplianceFormA) => {
                this.isSubmitted = true;
                this.complianceForm = item;
                this.Save(); //for rollup.. in case of excluding a finding
                this.goBack();
            },
            error => {
            });
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
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }

    // get canReviewerSubmitQC(){
    //     if (this.complianceForm){
    //         return this.complianceForm.Findings.filter(x => x.Comments != undefined && 
    //             x.Comments.length > 0 && 
    //             x.Comments[0].CategoryEnum != CommentCategoryEnum.NotApplicable &&
    //             x.Comments[0].ReviewerCategoryEnum != CommentCategoryEnum.NotApplicable);
    //     }
    // }

    setValues(actionType: string){
        switch(actionType){
            case "submitQC": this.modalTitle = "Confirm QC Submit";
                this.modalText = "are you sure you want to submit QC?";
            case "submitQCCorrection": this.modalTitle = "Confirm Submit QC Correction";
                this.modalText = "are you sure you want to submit QC Correction?";
        }
    }

    formValueChanged() {
        this.pageChanged = true;
    }

    canDeactivate(): Promise<boolean> | boolean {
        
                if (this.pageChanged == false) {
                    return true;
                }
                // Otherwise ask the user with the dialog service and return its
                // promise which resolves to true or false when the user decides
                //this.IgnoreChangesConfirmModal.open();
                //return this.canDeactivateValue;
                return window.confirm("Changes not saved. Ignore changes?");//this.dialogService.confirm('Discard changes?');
    }

    SetFindingToRemove(selectedRecord: Finding) {
        this.recordToDelete = selectedRecord;
    }

    get RecordToDeleteText() {
        if (this.recordToDelete == null) {
            return "";
        } else {
            if (this.recordToDelete.RecordDetails == null) {
                return "";
            } else {
                return this.recordToDelete.RecordDetails.substr(0, 100) + " ...";
            }
        }
    }

    RemoveFinding() {
        this.pageChanged = true;
        if (this.recordToDelete.IsMatchedRecord) {
            this.recordToDelete.IsAnIssue = false;
            this.recordToDelete.Observation = "";
            this.recordToDelete.Selected = false;
        }
        else {
            var index = this.complianceForm.Findings.indexOf(this.recordToDelete, 0);
            if (index > -1) {
                this.complianceForm.Findings.splice(index, 1);
            }

        }

    }
           
    canDisableQCSave(){
        if(this.currentReviewStatus &&
            this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase()
            != this.authService.userName.toLowerCase())
            return true;
        else if(this.currentReviewStatus && 
            this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCompleted)
            return true;
        else
            return false;
    }

    goBack() {
        //this._location.back();
        this.router.navigate(["qc"]);
    }

    get diagnostic() { return JSON.stringify(this.findingRecordToEdit) }
    
    

}