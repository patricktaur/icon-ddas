import { Component, OnDestroy, OnInit, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Comment } from '../../../search/search.classes';

@Component({
    selector: '[qc-verifier-general-comment-view]',
    moduleId: module.id,
    templateUrl: 'qc-verifier-general-comment-view.component.html',
})
export class QCVerifierGeneralCommentViewComponent implements OnInit {
    @Input() Comment: Comment;
    @Input() Title: string;
    @Output() ValueChanged = new EventEmitter();

    private pageChanged: boolean = false;

    public tempText: string;
    public tempNumber: number;

    ngOnInit(){
        if(this.Comment){
            this.tempNumber = this.Comment.CategoryEnum;
            this.tempText = this.Comment.FindingComment;
        }
    }

    formValueChanged(){
        this.pageChanged = true;
        this.Comment.FindingComment = this.tempText;
        this.Comment.CategoryEnum = this.tempNumber;
        this.ValueChanged.emit();
    }

    get diagnostic() { return JSON.stringify(this.Comment); }
    
}