import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';
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
        // if( this.Finding.Comments[1].CategoryEnum == 0)
        //     this.Finding.Comments[1].CategoryEnum = 5;
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

    get getReviewerComment(){
        //this.Finding.Comments[1].CategoryEnum = 5;
        // if (this.Finding){
        //     return this.Finding.Comments[1];
        // }else{
        //     return null;
        // }
        if (this.Finding){
            if (this.Finding.Comments){
                return this.Finding.Comments[1].FindingComment;
            }else{
                return null;
            }
        }else{
            return null;
        }
        
    }


    get getQCVerifierComment(){
        if (this.Finding){
            if (this.Finding.Comments){
                return this.Finding.Comments[0];
            }else{
                return null;
            }
            
        }else{
            return null;
        }

    }

get ReviwerCategoryEnum(){
    if (this.Finding){
        if (this.Finding.Comments){
            return this.Finding.Comments[1].CategoryEnum;
        }else{
            return null;
        }
    }else{
        return null;
    }
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