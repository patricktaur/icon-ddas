import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';


@Component({
    selector: '[comp-form-summary]',
    moduleId: module.id,
    templateUrl: 'summary.component.html',
})
export class ComplianceFormSummaryComponent implements OnInit, OnChanges {
    @Input() CompForm: ComplianceFormA;

    constructor(
       
    ) { }

    ngOnInit() {
        
    }


    ngOnChanges(){
      
    }
    
    

}