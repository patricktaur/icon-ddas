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

    get getQCVerifierComment(){
        return this.Finding.Comments[0];
    }

    isQCCommentAdded(){
        if((this.getQCVerifierComment != null || this.getQCVerifierComment != undefined) &&
            this.getQCVerifierComment.CategoryEnum != CommentCategoryEnum.NotApplicable){
            return true;
        }
        else
            false;
    }

    get qcCommentCategory(){
        if(this.getQCVerifierComment != null || this.getQCVerifierComment != undefined)
            return CommentCategoryEnum[this.getQCVerifierComment.CategoryEnum];
        else
            return null;
    }

    get reviewerCommentCategory(){
        if(this.getQCVerifierComment != null || this.getQCVerifierComment != undefined)
            return CommentCategoryEnum[this.getQCVerifierComment.ReviewerCategoryEnum];
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