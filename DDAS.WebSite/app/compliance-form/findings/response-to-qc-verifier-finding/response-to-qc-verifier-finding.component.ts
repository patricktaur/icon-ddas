import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding, CommentCategoryEnum } from '../../../search/search.classes';
import {CompFormLogicService} from "../../../search/shared/services/comp-form-logic.service"

@Component({
    selector: '[response-to-qc-verifier-finding]',
    moduleId: module.id,
    templateUrl: 'response-to-qc-verifier-finding.component.html',
})
export class ResponseToQCVerifierFindingComponent implements OnInit {
    @Input() Finding: Finding;
    
    private pageChanged: boolean = false;
    
    ngOnInit(){
        if( this.Finding.Comments[0].CategoryEnum == 0)
            this.Finding.Comments[0].CategoryEnum = 5; //CorrectionPending
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

    get diagnostic() { return JSON.stringify(this.Finding); } 
}