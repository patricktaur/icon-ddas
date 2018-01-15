
import { Injectable } from '@angular/core';
import { AuthService } from '../../../auth/auth.service';
//import { SearchService } from '../../search-service';
import {
<<<<<<< HEAD
    ComplianceFormA, Finding, CurrentReviewStatusViewModel, 
    ReviewStatusEnum, Review, ReviewerRoleEnum, ComplianceForm,
    CommentCategoryEnum
=======
    Finding,CurrentReviewStatusViewModel
>>>>>>> comp-form-tabs-corrections
} from '../../search.classes';
import { reverse } from 'dns';

@Injectable()
export class CompFormLogicService {
    public currentReviewStatus: CurrentReviewStatusViewModel;
    constructor(
<<<<<<< HEAD
        private authService: AuthService,
        //private service: SearchService,
    ) { }

   

    CanDisplayFindingComponent(selectedFinding: Finding, componentName: string, reviewStatus: CurrentReviewStatusViewModel) {
        switch (componentName) {
            case "findingEdit":
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    (reviewStatus.CurrentReview.Status == ReviewStatusEnum.ReviewInProgress ||
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.SearchCompleted))
=======
        private authService: AuthService, ReviewStatusEnum
    ) { }

    selectFindingComponentToDisplay(selectedFinding: Finding, componentName: string) {
        switch (componentName) {
            case "findingEdit":
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    (this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.ReviewInProgress ||
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.SearchCompleted))
>>>>>>> comp-form-tabs-corrections
                    return true;
                else
                    return false;
            case "qcVerifierComments":
<<<<<<< HEAD
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCInProgress &&
                    selectedFinding.ReviewId != reviewStatus.QCVerifierRecId)
=======
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCInProgress &&
                    selectedFinding.ReviewId != this.currentReviewStatus.QCVerifierRecId)
>>>>>>> comp-form-tabs-corrections
                    return true;
                else
                    return false;
            case "qcVerifierFinding":
<<<<<<< HEAD
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCInProgress &&
                    selectedFinding.ReviewId == reviewStatus.QCVerifierRecId)
=======
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCInProgress &&
                    selectedFinding.ReviewId == this.currentReviewStatus.QCVerifierRecId)
>>>>>>> comp-form-tabs-corrections
                    return true;
                else
                    return false;
            case "responseToQCVerifierComments":
<<<<<<< HEAD
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCorrectionInProgress &&
                    selectedFinding.ReviewId == reviewStatus.ReviewerRecId)
=======
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCorrectionInProgress &&
                    selectedFinding.ReviewId == this.currentReviewStatus.ReviewerRecId)
>>>>>>> comp-form-tabs-corrections
                    return true;
                else
                    return false;
            case "responseToQCVerifierFinding":
<<<<<<< HEAD
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCorrectionInProgress &&
                    selectedFinding.ReviewId != reviewStatus.ReviewerRecId)
=======
                if (this.currentReviewStatus != undefined &&
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCorrectionInProgress &&
                    selectedFinding.ReviewId != this.currentReviewStatus.ReviewerRecId)
>>>>>>> comp-form-tabs-corrections
                    return true;
                else
                    return false;
            case "findingView":
                // return true;
<<<<<<< HEAD
                if (reviewStatus != undefined &&
                    (reviewStatus.CurrentReview.Status == ReviewStatusEnum.Completed ||
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCFailed ||
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCRequested ||
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() != this.authService.userName.toLowerCase()))
=======
                if (this.currentReviewStatus != undefined &&
                    (this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.Completed ||
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCFailed ||
                    this.currentReviewStatus.CurrentReview.Status == ReviewStatusEnum.QCRequested ||
                    this.currentReviewStatus.CurrentReview.AssigendTo.toLowerCase() != this.authService.userName.toLowerCase()))
>>>>>>> comp-form-tabs-corrections
                    return true;
                else
                    return false;
            default: return false;
        }
    }
<<<<<<< HEAD

    //Review
    getQCVerifierReview(reviews: Review[]) : Review{
        //Only one QCVerifier Review assumed.
        let retValue:  Review;
        let qcVerifierReviews = reviews.filter(x => x.ReviewerRole == ReviewerRoleEnum.QCVerifier);
        if (qcVerifierReviews){
            retValue = qcVerifierReviews[0]; //First Available
            return retValue;
        }else{
            return retValue;
        }
    }
    
    
    //Not working:
    getQCVerifierReviewId(reviews: Review[]) : string{
        let qcVerifierReivew = this.getQCVerifierReview(reviews);
        if (qcVerifierReivew){
            qcVerifierReivew.RecId;
        }else{
            return  null;
        }
        
    }

    //Not used yet? :
    getQCVerifiedFindings(compForm: ComplianceFormA, reviewId: string): Finding[]{
        //let qcVerReviewId = this.getQCVerifierReviewId(compForm.Reviews);
        return compForm.Findings.filter(x => x.Comments.filter(y => y.ReviewId == reviewId ));
    }

//    getFindingById(findings: Finding[], findingId: string){
//       return findings.find(x => x.Id == findingId );
//    }

    //Finding Comment:
    getReviewerComment(finding: Finding){
        //this.Finding.Comments[0].CategoryEnum = 5;
        var reviewerComment = finding.Comments.find(x => 
            x.CategoryEnum == CommentCategoryEnum.CorrectionPending ||
            x.CategoryEnum == CommentCategoryEnum.CorrectionCompleted ||
            x.CategoryEnum == CommentCategoryEnum.Accepted);

        if(reviewerComment)
            return reviewerComment;
        else
            return null;
    }

    getQCVerifierComment(finding: Finding){
        var qcComment = finding.Comments.find(x => x.FindingComment != null);

        if(qcComment)
            return qcComment;
        else
            return null;
    }

    isQCCommentAdded(){
        if(this.getQCVerifierComment != null || this.getQCVerifierComment != undefined){
            return true;
        }
        else
            false;
    }

    qcCommentCategory(finding: Finding){
        let qcVerifierComment = this.getQCVerifierComment(finding);
        if(qcVerifierComment)
            return CommentCategoryEnum[qcVerifierComment.CategoryEnum];
        else
            return null;
    }

    qcComment(finding: Finding){
        let qcVerifiedComment = this.getQCVerifierComment(finding);
        if(qcVerifiedComment)
            return qcVerifiedComment.FindingComment;
        else
            return null;        
    }

    reviewerCommentCategory(finding: Finding){
        let reviewerComment = this.getReviewerComment(finding);
        if(reviewerComment)
            return CommentCategoryEnum[reviewerComment.CategoryEnum];
        else
            return null;
    }

=======
>>>>>>> comp-form-tabs-corrections

}