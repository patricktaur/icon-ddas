import { Component, OnDestroy, OnInit, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Comment, ComplianceFormA } from '../../../search/search.classes';

@Component({
    selector: '[reviewer-response-to-general-comment]',
    moduleId: module.id,
    templateUrl: 'reviewer-response-to-general-comment.component.html',
})
export class ReviewerResponseToGeneralCommentComponent implements OnInit {
    @Input() Comment: Comment;
    @Input() Title: string;
    @Output() ValueChanged = new EventEmitter();

    private pageChanged: boolean = false;

    public tempText: string;
    public tempNumber: number;
    // public generalComment: Comment;

    ngOnInit(){
        if(this.Comment){
            this.tempNumber = this.Comment.ReviewerCategoryEnum;
            this.tempText = this.Comment.ReviewerComment;
        }
        // if(this.ComplianceForm){
        //     this.generalComment = this.ComplianceForm.QCGeneralComment
        // }
    }

    get QCCommentCategory(){
        if(this.Comment){
            return this.tempNumber = this.Comment.ReviewerCategoryEnum;
        }
    }

    onChange(newValue: any){
        this.Comment.ReviewerCategoryEnum = newValue;
    }

    formValueChanged(){
        this.pageChanged = true;
        this.Comment.ReviewerComment = this.tempText;
        // this.Comment.ReviewerCategoryEnum = this.tempNumber;
        this.ValueChanged.emit();
    }

    get diagnostic() { return JSON.stringify(this.Comment); }
}