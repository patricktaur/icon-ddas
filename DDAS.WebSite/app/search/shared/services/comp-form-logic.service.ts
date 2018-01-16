
import { Injectable } from '@angular/core';
import { AuthService } from '../../../auth/auth.service';
//import { SearchService } from '../../search-service';
import {
    ComplianceFormA, Finding, CurrentReviewStatusViewModel, 
    ReviewStatusEnum, Review, ReviewerRoleEnum, ComplianceForm,
    CommentCategoryEnum
} from '../../search.classes';
import { reverse } from 'dns';

@Injectable()
export class CompFormLogicService {
    public currentReviewStatus: CurrentReviewStatusViewModel;
    constructor(
        private authService: AuthService,
        //private service: SearchService,
    ) { }

    CanDisplayFindingComponent(selectedFinding: Finding, componentName: string, reviewStatus: CurrentReviewStatusViewModel) {
        switch (componentName) {
            case "findingEdit":
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    (reviewStatus.CurrentReview.Status == ReviewStatusEnum.ReviewInProgress ||
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.ReviewCompleted || 
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.SearchCompleted))
                    return true;
                else
                    return false;
            case "qcVerifierComments":
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCInProgress &&
                    selectedFinding.ReviewId != reviewStatus.QCVerifierRecId)
                    return true;
                else
                    return false;
            case "qcVerifierFinding":
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCInProgress &&
                    selectedFinding.ReviewId == reviewStatus.QCVerifierRecId)
                    return true;
                else
                    return false;
            case "responseToQCVerifierComments":
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCorrectionInProgress &&
<<<<<<< HEAD
                    selectedFinding.ReviewId == reviewStatus.ReviewerRecId)
                    {
                        return true;
                    }
=======
                    selectedFinding.ReviewId == reviewStatus.ReviewerRecId &&
                    selectedFinding.Comments[0].CategoryEnum != CommentCategoryEnum.NotApplicable)
                    return true;
>>>>>>> Development
                else
                    {
                       
                        return false;
                    }
            case "responseToQCVerifierFinding":
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCorrectionInProgress &&
                    selectedFinding.ReviewId != reviewStatus.ReviewerRecId)
                    //selectedFinding.ReviewId == reviewStatus.QCVerifierRecId)
                    {
                        return true;
                    }
                else
                    {
                        return false;
                    }
            case "findingView":
                // return true;
                if (reviewStatus != undefined &&
                    (reviewStatus.CurrentReview.Status == ReviewStatusEnum.Completed ||
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCFailed ||
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCRequested ||
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() != this.authService.userName.toLowerCase()))
                    return true;
                else
                    return false;
            default: return false;
        }
    }

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

    getCurrentLoggedInUserReview(CompForm: ComplianceFormA) {
        return CompForm.Reviews.find(x =>
            x.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase());
    }

    getReviewStatus(currentReviewStatus: number) {
        switch(currentReviewStatus) {
            case ReviewStatusEnum.SearchCompleted: return "Search Completed";
            case ReviewStatusEnum.ReviewInProgress: return "Review In Progress";
            case ReviewStatusEnum.ReviewCompleted: return "Review Completed";
            case ReviewStatusEnum.Completed: return "Completed";
            case ReviewStatusEnum.QCRequested: return "QC Requested";
            case ReviewStatusEnum.QCInProgress: return "QC In Progress";
            case ReviewStatusEnum.QCFailed: return "QC Failed";
            case ReviewStatusEnum.QCPassed: return "QC Passed";
            case ReviewStatusEnum.QCCorrectionInProgress: return "QC Correction In Progress";
            default: "";
        }
    }

    isLoggedInUserReviewerAndCanSaveForm(CompForm: ComplianceFormA){
        if (CompForm.Reviews.length > 0) {
            var review = CompForm.Reviews.find(x =>
                x.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                x.ReviewerRole == ReviewerRoleEnum.Reviewer && 
                (x.Status == ReviewStatusEnum.SearchCompleted ||
                 x.Status == ReviewStatusEnum.ReviewInProgress ||
                 x.Status == ReviewStatusEnum.ReviewCompleted));

            if (!review)
                return false;
            else
                return true;
        }
    }

     canDisplayComponent(CompForm: ComplianceFormA, componentName: string){
        // return true;
        // console.log(this.CompForm.CurrentReviewStatus);
        switch(componentName){
            case "generalEdit":
                if((CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewCompleted) &&
                    CompForm.AssignedTo.toLowerCase() == this.authService.userName.toLowerCase())
                    return true;
                else
                    return false;
            // case "generalEditQC":
            //     if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress)
            //         return true;
            //     else
            //         return false;
            // case "generalEditResponseToQC":
            //     if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress &&
            //         this.isGeneralQCCommentAdded)
            //         return true;
            //     else
            //         return false;
            case "generalView":
                if((CompForm.CurrentReviewStatus == ReviewStatusEnum.QCRequested ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCFailed ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.Completed))
                    return true;
                else
                    return false;
            case "instituteEdit":
                if((CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewCompleted) &&
                    CompForm.AssignedTo.toLowerCase() == this.authService.userName.toLowerCase())
                    return true;
                else
                    return false;
            case "instituteView":
                if((CompForm.CurrentReviewStatus == ReviewStatusEnum.QCRequested ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCFailed ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.Completed))
                    return true;
                else
                    return false;
            case "investigatorEdit":
                if((CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewCompleted) &&
                    CompForm.AssignedTo.toLowerCase() == this.authService.userName.toLowerCase())                    
                    return true;
                else
                    return false;
            case "investigatorView":
                if((CompForm.CurrentReviewStatus == ReviewStatusEnum.QCRequested ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCFailed ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.Completed))
                    return true;
                else
                    return false;
            case "mandatorySitesEdit":
                if((CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewCompleted) &&
                    CompForm.AssignedTo.toLowerCase() == this.authService.userName.toLowerCase())                    
                    return true;
                else
                    return false;
            case "mandatorySitesView":
                if((CompForm.CurrentReviewStatus == ReviewStatusEnum.QCRequested ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCFailed ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.Completed))
                    return true;
                else
                    return false;
            case "additionalSitesEdit":
                if((CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress ||
                    CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewCompleted) &&
                    CompForm.AssignedTo.toLowerCase() == this.authService.userName.toLowerCase())                    
                    return true;
                else
                    return false;
            case "additionalSitesView":
                return true;
            case "findingsEdit":
                // if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress)
                    return true;
                // else
                //     return false;
            case "searchedByView": return true;
            case "summaryView": return true;

            default: return true;
        }
    }

    unHideReviewCompletedSiteCheckBox(CompForm: ComplianceFormA){
        if(CompForm && 
            (CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
            CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress)){
            return true;
        }
        else{
            return false;
        }
    }    
}