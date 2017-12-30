import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource, ComplianceFormStatusEnum, Finding, InstituteFindingsSummaryViewModel } from '../../../search/search.classes';
import { SearchService } from '../../../search/search-service';

@Component({
    selector: '[comp-form-institute-view]',
    moduleId: module.id,
    templateUrl: 'institute-view.component.html',
})
export class ComplianceFormInstituteViewComponent implements OnInit, OnChanges {
    @Input() CompForm: ComplianceFormA;
    @Input() RootPath: string;
    public InstituteSearchSummary : InstituteFindingsSummaryViewModel[] = [];

    constructor(
        private service: SearchService,
        private route: ActivatedRoute,
        private router: Router,
        private _location: Location,
    ) { }

    ngOnInit() {
        this.LoadInstituteSiteSummary();
    }


    ngOnChanges(){
        if (this.ComplianceFormId){
            this.LoadInstituteSiteSummary();
        }
    }
    
    get ComplianceFormId(){
        return this.CompForm.RecId;
    }
    
    LoadInstituteSiteSummary(){
        console.log("Inside LoadInstituteSiteSummary");
        if (this.ComplianceFormId != null && this.ComplianceFormId.length > 0){  //ComplianceFormId is null when the form is created manually
              //this.formLoading = true;
              this.service.getInstituteFindingsSummary(this.ComplianceFormId)
                  .subscribe((item: any) => {
                      this.InstituteSearchSummary = item;
                      //this.formLoading = false;
                  },
                  error => {
                      //this.formLoading = false;
                  });
        }
        
    }

    private Todate = new Date(); 
    
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

         gotoSiteDetails(SiteSourceId: number){
            this.router.navigate(['institute-findings', this.ComplianceFormId,  SiteSourceId, {rootPath:this.RootPath}], 
            { relativeTo: this.route.parent});
        }

}