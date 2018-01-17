import { Component, OnInit, OnDestroy, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding, CommentCategoryEnum } from '../../../search/search.classes';

@Component({
    selector: '[finding-edit-base]',
    moduleId: module.id,
    templateUrl: 'finding-edit-base.component.html',
})
export class FindingEditBaseComponent implements OnInit {
    @Input() Finding: Finding;
    @Output() Remove = new EventEmitter();
    @Input() Title: string;
    
    private pageChanged: boolean = false;
    
    ngOnInit(){
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

    // dividerGeneration(indexVal: number) {
    //     if ((indexVal + 1) % 2 == 0) {
    //         return true;
    //     }
    //     else {
    //         return false;
    //     }
    // }

    RemoveClicked(){
        console.log("Inside: SelectedFindingEditComponent");
        this.Remove.emit();
    }

    get diagnostic(){
        return JSON.stringify(this.Finding);
    }
}