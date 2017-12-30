import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';


@Component({
    selector: '[comp-form-general-view]',
    moduleId: module.id,
    templateUrl: 'general-view.component.html',
})
export class ComplianceFormGeneralViewComponent  {
    @Input() CompForm: ComplianceFormA;
    private pageChanged: boolean = false;
    
    formValueChanged(){
        this.pageChanged = true;
    } 
 
}