import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { AuthService } from '../auth/auth.service';
//import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource, Finding, SiteSearchStatus, UpdateInstituteFindings } from './search.classes';
import {
    ComplianceFormA, InvestigatorSearched, SiteSourceToSearch,
    SiteSource, Finding, SiteSearchStatus, UpdateFindigs,
    ReviewerRoleEnum, ReviewStatusEnum, Comment, Review, CurrentReviewStatusViewModel,
    UpdateInstituteFindings
} from './search.classes';

import { SearchService } from './search-service';
import { Location } from '@angular/common';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';



@Component({
    moduleId: module.id,
    templateUrl: 'institute-findings.component.html',


})
export class InstituteFindingsComponent implements OnInit {
    public CompForm: ComplianceFormA = new ComplianceFormA;
    private ComplianceFormId: string;

    private SiteSourceId: number;

    public SitesAvailable: SiteSourceToSearch[] = [];
    public searchInProgress: boolean = false;

    private pageChanged: boolean = false;
    private rootPath: string;
    public loading: boolean;
    public singleMatchRecordsLoading: boolean;
    public minMatchCount: number;
    private singleMatchRecords: Finding[] = [];
    private recordToDelete: Finding = new Finding;
    public pageNumber: number;
    public filterRecordDetails: string = "";
    public isQCVerifier: boolean;
    public qcReview: Review;
    public currentReviewStatus: CurrentReviewStatusViewModel;

    @ViewChild('IgnoreChangesConfirmModal') IgnoreChangesConfirmModal: ModalComponent;
    private canDeactivateValue: boolean;
    private highlightFilter: string;
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private _location: Location,
        private service: SearchService,
        private sanitizer: DomSanitizer,
        private authService: AuthService
    ) { }

    ngOnInit() {

        this.route.params.forEach((params: Params) => {
            this.ComplianceFormId = params['formId'];

            this.SiteSourceId = +params['siteSourceId']
            this.rootPath = params['rootPath'];
            this.LoadOpenComplainceForm();

        });

    }

    LoadOpenComplainceForm() {
        this.loading = true;
        this.service.getComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.CompForm = item;
                //this.IntiliazeRecords();
                this.loading = false;
                this.isQCVerifier = this.isReviewerOrQCVerifier;
                this.getCurrentReviewStatus();               

            },
            error => {
                this.loading = false;
            });
    }

    get isReviewerOrQCVerifier() {
        if (this.CompForm.Reviews.length > 0) {
            var review = this.CompForm.Reviews.find(x =>
                x.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                x.ReviewerRole == ReviewerRoleEnum.QCVerifier);

            if (!review)
                return false;
            else {
                // this.qcReview = this.qcVerifierReview;
                return true;
            }
        }
    }

    getCurrentReviewStatus() {
        this.service.getCurrentReviewStatus(this.ComplianceFormId)
            .subscribe((item: CurrentReviewStatusViewModel) => {
                this.currentReviewStatus = item;
                console.log('current review status: ', this.currentReviewStatus);
            },
            error => {

            });
    }

    get currentLoggedInUserReview() {
        if (this.CompForm) {
            return this.CompForm.Reviews.find(x =>
                x.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase());
        }
        else
            return null;
    }
    get Site() {
        let site = new SiteSourceToSearch;


        let site1 = this.CompForm.SiteSources.find(x => x.Id == this.SiteSourceId);
        if (site1 == undefined) {
            site.SiteName = "Not found";
            return site;
        }
        else {
            return site1;
        }

    }

    get SiteHasUrl() {
        if (this.Site != null) {
            if (this.Site.SiteUrl != null) {
                if (this.Site.SiteUrl.toLowerCase().startsWith("http")) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }

        }
        else {
            return false;
        }
    }

    get IsManualExtractionSite() {
        let retValue: boolean = false;
        if (this.Site.ExtractionMode.toLowerCase() == "manual") {
            retValue = true;
        }
        return retValue;
    }



    get Findings() {

        return this.CompForm.Findings.filter(x => x.SiteSourceId == this.SiteSourceId);

    }

    selectFindingComponentToDisplay(selectedFinding: Finding, componentName: string) {
        switch (componentName) {
            case "findingEdit":
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    (this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.ReviewInProgress ||
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.SearchCompleted))
                    return true;
                else
                    return false;
            case "qcVerifierComments":
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCInProgress &&
                    selectedFinding.ReviewId != this.currentReviewStatus.QCVerifierRecId)
                    return true;
                else
                    return false;
            case "qcVerifierFinding":
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCInProgress &&
                    selectedFinding.ReviewId == this.currentReviewStatus.QCVerifierRecId)
                    return true;
                else
                    return false;
            case "responseToQCVerifierComments":
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCorrectionInProgress &&
                    selectedFinding.ReviewId == this.currentReviewStatus.ReviewerRecId)
                    return true;
                else
                    return false;
            case "responseToQCVerifierFinding":
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCorrectionInProgress &&
                    selectedFinding.ReviewId != this.currentReviewStatus.ReviewerRecId)
                    return true;
                else
                    return false;
            case "findingView":
                // return true;
                if (this.currentReviewStatus != undefined &&
                    (this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.Completed ||
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCFailed ||
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCRequested ||
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() != this.authService.userName.toLowerCase()))
                    return true;
                else
                    return false;
            default: return false;
        }
    }


    Add() {
        let finding = new Finding;
        finding.IsMatchedRecord = false;
        // finding.InvestigatorSearchedId = this.InvestigatorId;
        finding.SiteSourceId = this.SiteSourceId

        finding.SiteDisplayPosition = this.Site.DisplayPosition;
        finding.SiteId = this.Site.SiteId;
        finding.SiteEnum = this.Site.SiteEnum; //this.SiteEnum;

        //finding.DateOfInspection = new Date() ;
        finding.Selected = true;
        finding.InvestigatorName = this.CompForm.Institute;
        finding.IsAnIssue = true;

        this.addCommentCollectionToSelectedFinding(finding);
        this.CompForm.Findings.push(finding);
        this.pageChanged = true;
    }

    addCommentCollectionToSelectedFinding(selectedFinding: Finding) {
        let review = this.currentLoggedInUserReview;
        if (review != null) {
            selectedFinding.ReviewId = review.RecId;

            if (selectedFinding.Comments == null ||
                selectedFinding.Comments == undefined ||
                selectedFinding.Comments.length == 0) {
                console.log('adding comment collection');
                let comments = new Array<Comment>();
                let comment = new Comment();
                comment.ReviewId = review.RecId;
                comment.CategoryEnum = 0;
                comments.push(comment);
                let emptyComment = new Comment();
                emptyComment.CategoryEnum = 0;
                emptyComment.ReviewId = null;
                comments.push(emptyComment);
                selectedFinding.Comments.push(comment);
                selectedFinding.Comments.push(emptyComment);
            }
        }
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
        var index = this.CompForm.Findings.indexOf(this.recordToDelete, 0);
        if (index > -1) {
            this.CompForm.Findings.splice(index, 1);
        }
    }

    SaveAndClose() {

        let updateFindings = new UpdateInstituteFindings;

        updateFindings.FormId = this.ComplianceFormId;
        //SiteSourceId
        updateFindings.SiteSourceId = this.SiteSourceId;
        updateFindings.Findings = this.Findings;

        this.service.updateInstituteFindings(updateFindings)
            .subscribe((item: any) => {
                this.pageChanged = false;
                this.goBack()
            },
            error => {

            });
    }

    Split = (RecordDetails: string) => {
        if (RecordDetails == undefined) {
            return null;
        }
        var middleNames: string[] = RecordDetails.split("~");

        return middleNames;
    }

    dividerGeneration(indexVal: number) {
        if ((indexVal + 1) % 2 == 0) {
            return true;
        }
        else {
            return false;
        }
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

    setDeactivateValue() {
        this.canDeactivateValue = true;
    }

    // goBack() {
    //     this.router.navigate(['institute-findings-summary', this.ComplianceFormId, {siteDisplayPos:this.SiteSourceId, rootPath: this.rootPath}], { relativeTo: this.route.parent});
    // }

    goBack() {
        this.router.navigate(['comp-form-edit', this.ComplianceFormId, { rootPath: this.rootPath }], { relativeTo: this.route.parent });
    }

    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }
    get diagnostic() { return JSON.stringify(this.highlightFilter); }
}