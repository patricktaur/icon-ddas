import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';


@Component({
    selector: '[comp-form-general-edit]',
    moduleId: module.id,
    templateUrl: 'general-edit.component.html',
})
export class ComplianceFormGeneralEditComponent implements OnInit, OnChanges {
    @Input() CompForm: ComplianceFormA;

    constructor(
       
    ) { }

    ngOnInit() {
        
    }


    ngOnChanges(){
      
    }
    
    

}