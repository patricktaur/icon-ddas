import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';


@Component({
    selector: '[comp-form-searched-by]',
    moduleId: module.id,
    templateUrl: 'searched-by.component.html',
})
export class ComplianceFormSearchedByComponent  {
    @Input() CompForm: ComplianceFormA;

    
    
    
    

}