import { Component, OnInit, OnDestroy, Input, OnChanges, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: '[comp-form-mandatory-sites-edit]',
    moduleId: module.id,
    templateUrl: 'mandatory-sites-edit.component.html',
})
export class ComplianceFormMandatorySitesEditComponent  {
    @Input() CompForm: ComplianceFormA;
    @Output() ValueChanged = new EventEmitter();
    constructor(
        private sanitizer: DomSanitizer,
    ) { }

    
    
    get MandatorySites() {
        return this.CompForm.SiteSources.filter(x => x.Deleted == false && x.IsMandatory == true)
    }

    isUrl(url: string){
        if (url == null){
            return false;
        }
        else{
           if (url.toLowerCase().startsWith("http")){
               return true;
           }else{
               return false;
           }
        }
  
    }

    formValueChanged(){
        this.ValueChanged.emit(true) ;
    } 

    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }
}