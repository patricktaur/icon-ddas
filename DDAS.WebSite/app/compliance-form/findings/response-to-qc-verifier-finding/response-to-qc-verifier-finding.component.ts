import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';


@Component({
    selector: '[response-to-qc-verifier-finding]',
    moduleId: module.id,
    templateUrl: 'response-to-qc-verifier-finding.component.html',
})
export class ResponseToQCVerifierFindingComponent  {
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

    get getReviewerComment(){
        this.Finding.Comments[1].CategoryEnum = 5;
        return this.Finding.Comments[1];
    }

    get getQCVerifierComment(){
        return this.Finding.Comments[0];
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