import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding, CommentCategoryEnum } from '../../../search/search.classes';

@Component({
    selector: '[response-to-qc-verifier-comment]',
    moduleId: module.id,
    templateUrl: 'response-to-qc-verifier-comment.component.html'
})
export class ResponseToQCVerifierCommentComponent implements OnInit {
    @Input() Finding: Finding;
    
    private pageChanged: boolean = false;
    
    ngOnInit(){
        // if(this.Finding.Comments[0].CategoryEnum == 0)
        //     this.Finding.Comments[0].ReviewerCategoryEnum = 5; //CorrectionPgending
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

    get getQCVerifierComment(){
        return this.Finding.Comments[0];
    }

    get qcVerifierCommentCategoryEnum(){
        return CommentCategoryEnum[this.Finding.Comments[0].CategoryEnum];
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