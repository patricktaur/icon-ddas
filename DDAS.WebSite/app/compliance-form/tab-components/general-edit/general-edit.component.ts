import { Component, OnInit, OnDestroy, Input, OnChanges, Output, EventEmitter, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, Form } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';


@Component({
    selector: '[comp-form-general-edit]',
    moduleId: module.id,
    templateUrl: 'general-edit.component.html',
})
export class ComplianceFormGeneralEditComponent  {
    @Input() CompForm: ComplianceFormA;
    @Output() ValueChanged = new EventEmitter();

    formValueChanged(){
        this.ValueChanged.emit(true) ;
    } 

   

   
}