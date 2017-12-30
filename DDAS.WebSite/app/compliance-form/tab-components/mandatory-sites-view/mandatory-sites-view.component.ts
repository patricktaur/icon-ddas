import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: '[comp-form-mandatory-sites-view]',
    moduleId: module.id,
    templateUrl: 'mandatory-sites-view.component.html',
})
export class ComplianceFormMandatorySitesViewComponent  {
    @Input() CompForm: ComplianceFormA;

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

    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }
}