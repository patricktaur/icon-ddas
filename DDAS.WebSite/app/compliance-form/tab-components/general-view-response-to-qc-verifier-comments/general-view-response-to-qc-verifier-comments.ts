import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA} from '../../../search/search.classes';


@Component({
    selector: '[comp-form-general-view-response-to-qc-verifier-comment]',
    moduleId: module.id,
    templateUrl: 'general-view-response-to-qc-verifier-comments.html',
})
export class ComplianceFormGeneralViewResponseToQCVerifierCommentsComponent  {
    @Input() CompForm: ComplianceFormA;
    private pageChanged: boolean = false;
    
    formValueChanged(){
        this.pageChanged = true;
    }
}