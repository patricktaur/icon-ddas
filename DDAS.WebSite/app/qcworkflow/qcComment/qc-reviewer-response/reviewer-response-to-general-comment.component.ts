import { Component, OnDestroy, OnInit, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Comment } from '../../../search/search.classes';

@Component({
    selector: '[reviewer-response-to-general-comment]',
    moduleId: module.id,
    templateUrl: 'reviewer-response-to-general-comment.component.html',
})
export class ReviewerResponseToGeneralCommentComponent implements OnInit {
    @Input() Comment: Comment;
    @Output() ValueChanged = new EventEmitter();

    private pageChanged: boolean = false;

    public tempText: string;
    public tempNumber: number;

    ngOnInit(){
        if(this.Comment){
            this.tempNumber = this.Comment.ReviewerCategoryEnum;
            this.tempText = this.Comment.ReviewerComment;
        }
    }    
    
    formValueChanged(){
        this.pageChanged = true;
        this.Comment.ReviewerComment = this.tempText;
        this.Comment.ReviewerCategoryEnum = this.tempNumber;        
        this.ValueChanged.emit();
    }

    get diagnostic() { return JSON.stringify(this.Comment); }
    
}