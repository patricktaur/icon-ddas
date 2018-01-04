import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';


@Component({
    selector: '[finding-view-base]',
    moduleId: module.id,
    templateUrl: 'finding-view-base.component.html',
})
export class FindingViewBaseComponent  {
    @Input() Finding: Finding;
    @Input() Title: string;
    
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

    // dividerGeneration(indexVal: number) {
    //     if ((indexVal + 1) % 2 == 0) {
    //         return true;
    //     }
    //     else {
    //         return false;
    //     }
    // }

    
}