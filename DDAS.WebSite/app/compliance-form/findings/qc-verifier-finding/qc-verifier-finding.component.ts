import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding, Review, CommentCategoryEnum } from '../../../search/search.classes';

@Component({
    selector: '[qc-verifier-finding]',
    moduleId: module.id,
    templateUrl: 'qc-verifier-finding.component.html',
})
export class QCVerifierFindingComponent implements OnInit  {
    @Input() Finding: Finding;
    @Input() QCVerifierReview: Review;
    @Input() ReviewerRecId: string;
    @Output() Remove = new EventEmitter();
    
    private commentEnum: CommentCategoryEnum;

    ngOnInit(){
        this.Finding.Comments[1].ReviewId = this.ReviewerRecId;
    }

    RemoveClicked(){
        this.Remove.emit();
    }

    private pageChanged: boolean = false;
    
    get getQCVerifierComment(){
        return this.Finding.Comments[0];
        // if(this.QCVerifierReview){
        //     return this.Finding.Comments.find(x => 
        //     x.ReviewId == this.QCVerifierReview.RecId);
        // }
        // else {
        //     return CommentCategoryEnum.Minor;
        // }
    }

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

    dividerGeneration(indexVal: number) {
        if ((indexVal + 1) % 2 == 0) {
            return true;
        }
        else {
            return false;
        }
    }

    get diagnostic() { return JSON.stringify(this.QCVerifierReview); }
}