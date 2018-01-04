import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';


@Component({
    selector: '[response-to-qc-verifier-comment]',
    moduleId: module.id,
    templateUrl: 'response-to-qc-verifier-comment.component.html',
})
export class ResponseToQCVerifierCommentComponent  {
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

    dividerGeneration(indexVal: number) {
        if ((indexVal + 1) % 2 == 0) {
            return true;
        }
        else {
            return false;
        }
    }

    
}