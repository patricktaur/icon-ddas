import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: '[comp-form-findings-edit]',
    moduleId: module.id,
    templateUrl: 'findings.component.html',
})
export class ComplianceFormFindingsComponent  {
    @Input() CompForm: ComplianceFormA;

    constructor(
        private sanitizer: DomSanitizer,
    ) { }

    
    
    get Findings() {
        if (this.CompForm == undefined) {
            return null;
        }
        //return this.CompForm.Findings.filter(x => x.Selected == true);
         return this.CompForm.Findings.filter(x => x.Selected == true).sort(
            function(a,b){
                if (a.DisplayPosition < b.DisplayPosition) return -1;
                else if (a.DisplayPosition > b.DisplayPosition) return 1;
                else return 0;
            }
         );
    }

    
}