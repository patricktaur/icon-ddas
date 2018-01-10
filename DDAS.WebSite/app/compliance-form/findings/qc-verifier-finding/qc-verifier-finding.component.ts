import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding, Review } from '../../../search/search.classes';

@Component({
    selector: '[qc-verifier-finding]',
    moduleId: module.id,
    templateUrl: 'qc-verifier-finding.component.html',
})
export class QCVerifierFindingComponent  {
    @Input() Finding: Finding;
    @Input() QCVerifierReview: Review;
    @Output() Remove = new EventEmitter();
    
    RemoveClicked(){
        this.Remove.emit();
    }
    
    private pageChanged: boolean = false;
    
    get getQCVerifierComment(){
        return this.Finding.Comments.find(x => 
            x.ReviewId == this.QCVerifierReview.RecId);
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

    
}