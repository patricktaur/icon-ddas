
import { Injectable } from '@angular/core';
import { AuthService } from '../../../auth/auth.service';
import {
    Finding,
} from '../../search.classes';

@Injectable()
export class CompFormLogicService {
    constructor(
        private authService: AuthService
    ) { }

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

}