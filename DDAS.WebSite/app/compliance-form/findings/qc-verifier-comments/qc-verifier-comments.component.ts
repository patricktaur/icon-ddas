import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding, Review, CommentCategoryEnum } from '../../../search/search.classes';

@Component({
    selector: '[qc-verifier-comments]',
    moduleId: module.id,
    templateUrl: 'qc-verifier-comments.component.html',
})
export class QCVerifierCommentsComponent implements OnInit {
    @Input() Finding: Finding;
    @Input() QCVerifierRecId: string;

    private pageChanged: boolean = false;
    
    ngOnInit(){
        if(this.QCVerifierRecId){
            this.Finding.Comments[0].ReviewId = this.QCVerifierRecId;
        }
    }

    formValueChanged(){
        this.pageChanged = true;
    }

    get getQCVerifierComment(){
<<<<<<< HEAD
        
        return this.Finding.Comments[1];
        // return this.Finding.Comments.find(x => 
        //     x.ReviewId == this.QCVerifierReview.RecId);
=======
        return this.Finding.Comments[0];
>>>>>>> Development
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

    get diagnostic() { return JSON.stringify(this.Finding); }
    
}