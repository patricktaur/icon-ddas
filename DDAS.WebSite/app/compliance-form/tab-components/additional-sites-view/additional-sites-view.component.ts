import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource, ComplianceFormStatusEnum, Finding, InstituteFindingsSummaryViewModel } from '../../../search/search.classes';
//import { ModalComponent } from '../../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { DomSanitizer } from '@angular/platform-browser';
import { SearchService } from '../../../search/search-service';


@Component({
    selector: '[comp-form-additional-sites-view]',
    moduleId: module.id,
    templateUrl: 'additional-sites-view.component.html',
})
export class ComplianceFormAdditionalSitesViewComponent  {
    @Input() CompForm: ComplianceFormA;
    public SiteSource: SiteSource = new SiteSource;
    public SiteSources: any[];
    private pageChanged: boolean = false;
    public siteToRemove: SiteSourceToSearch = new SiteSourceToSearch;
    constructor(
        private sanitizer: DomSanitizer,
        private service: SearchService,
    ) { }

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