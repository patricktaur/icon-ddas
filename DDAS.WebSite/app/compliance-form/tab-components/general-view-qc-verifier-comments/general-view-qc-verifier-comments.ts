import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA} from '../../../search/search.classes';


@Component({
    selector: '[comp-form-general-view-qc-verifier-comment]',
    moduleId: module.id,
    templateUrl: 'general-view-qc-verifier-comments.html',
})
export class ComplianceFormGeneralViewQCVerifierCommentsComponent  {
    @Input() CompForm: ComplianceFormA;
    private pageChanged: boolean = false;
    
    formValueChanged(){
        this.pageChanged = true;
    }
}