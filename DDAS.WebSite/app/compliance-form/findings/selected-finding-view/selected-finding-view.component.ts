import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding, CommentCategoryEnum } from '../../../search/search.classes';


@Component({
    selector: '[selected-finding-view]',
    moduleId: module.id,
    templateUrl: 'selected-finding-view.component.html',
})
export class SelectedFindingViewComponent  {
    @Input() Finding: Finding;
    
    private pageChanged: boolean = false;
    
    formValueChanged(){
        this.pageChanged = true;
    } 

    Split = (RecordDetails: string) => {
        if (RecordDetails == undefined) {
            return null;
        }
        var middleNames: string[] = RecordDetails.split("~");

        return middleNames;
    }

    get getReviewerComment(){
        //this.Finding.Comments[0].CategoryEnum = 5;
        var reviewerComment = this.Finding.Comments.find(x => 
            x.CategoryEnum == CommentCategoryEnum.CorrectionPending ||
            x.CategoryEnum == CommentCategoryEnum.CorrectionCompleted ||
            x.CategoryEnum == CommentCategoryEnum.Accepted);

        if(reviewerComment)
            return reviewerComment;
        else
            return null;
    }

    get getQCVerifierComment(){
        var qcComment = this.Finding.Comments.find(x => x.FindingComment != null);

        if(qcComment)
            return qcComment;
        else
            return null;
        // return this.Finding.Comments.find(x => 
        //     x.CategoryEnum == CommentCategoryEnum.Minor ||
        //     x.CategoryEnum == CommentCategoryEnum.Major ||
        //     x.CategoryEnum == CommentCategoryEnum.Critical ||
        //     x.CategoryEnum == CommentCategoryEnum.Suggestion ||
        //     x.CategoryEnum == CommentCategoryEnum.Others);
    }

    isQCCommentAdded(){
        if(this.getQCVerifierComment != null || this.getQCVerifierComment != undefined){
            return true;
        }
        else
            false;
    }

    get qcCommentCategory(){
        if(this.getQCVerifierComment != null || this.getQCVerifierComment != undefined)
            return CommentCategoryEnum[this.getQCVerifierComment.CategoryEnum]; //this.getQCVerifierComment.CategoryEnum.toString();
        else
            return null;
    }

    get qcComment(){
        if(this.getQCVerifierComment != null || this.getQCVerifierComment != undefined)
            return this.getQCVerifierComment.FindingComment;
        else
            return null;        
    }

    get reviewerCommentCategory(){
        if(this.getReviewerComment != null || this.getReviewerComment != undefined)
            return this.getReviewerComment.CategoryEnum.toString();
        else
            return null;
    }

    dividerGeneration(indexVal: number) {
        if ((indexVal + 1) % 2 == 0) {
            return true;
        }
        else {
            return false;
        }
    }

    
}