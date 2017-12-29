import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: '[comp-form-additional-sites-edit]',
    moduleId: module.id,
    templateUrl: 'additional-sites-edit.component.html',
})
export class ComplianceFormAdditionalSitesEditComponent implements OnInit, OnChanges {
    @Input() CompForm: ComplianceFormA;

    constructor(
        private sanitizer: DomSanitizer,
    ) { }

    ngOnInit() {
        
    }


    ngOnChanges(){
      
    }
    
    get OptionalSites() {
        return this.CompForm.SiteSources.filter(x => x.Deleted == false && x.IsMandatory == false)
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