
import { Injectable } from '@angular/core';
import { AuthService } from '../../../auth/auth.service';
import {
    Finding, CurrentReviewStatusViewModel, ReviewStatusEnum
} from '../../search.classes';

@Injectable()
export class CompFormLogicService {
    
    constructor(
        private authService: AuthService
    ) { }


    CanDisplayFindingComponent(selectedFinding: Finding, componentName: string, reviewStatus: CurrentReviewStatusViewModel) {
        switch (componentName) {
            case "findingEdit":
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    (reviewStatus.CurrentReview.Status == ReviewStatusEnum.ReviewInProgress ||
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
                    selectedFinding.ReviewId == reviewStatus.ReviewerRecId)
                    return true;
                else
                    return false;
            case "responseToQCVerifierFinding":
                if (reviewStatus != undefined &&
                    reviewStatus.CurrentReview.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                    reviewStatus.CurrentReview.Status == ReviewStatusEnum.QCCorrectionInProgress &&
                    selectedFinding.ReviewId != reviewStatus.ReviewerRecId)
                    return true;
                else
                    return false;
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

}