import { Component, OnDestroy, OnInit, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Comment } from '../../../search/search.classes';

@Component({
    selector: '[qc-verifier-general-comment]',
    moduleId: module.id,
    templateUrl: 'qc-verifier-general-comment.component.html',
})
export class QCVerifierGeneralCommentComponent implements OnInit {
    @Input() Comment: Comment;
    @Input() Title: string;
    @Input() CanRemoveComment: boolean;
    @Output() Remove = new EventEmitter();
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

    get QCCommentCategory(){
        if(this.Comment){
            return this.tempNumber = this.Comment.CategoryEnum;
        }
    }

    onChange(newValue:any){
        this.Comment.CategoryEnum = newValue;
        this.ValueChanged.emit();
    }

    formValueChanged(){
        this.pageChanged = true;
        this.Comment.FindingComment = this.tempText;
        // this.Comment.CategoryEnum = this.tempNumber;
        this.ValueChanged.emit();
    }

    RemoveClicked(){
        this.Remove.emit();
    }

    get diagnostic() { return JSON.stringify(this.Comment); }
    
}