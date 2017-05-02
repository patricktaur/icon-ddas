import { Component} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import {Location} from '@angular/common';
import {SearchService} from './search-service';
import { AuthService }      from '../auth/auth.service';


import { ComplianceFormA,    InstituteFindingsSummaryViewModel   } from './search.classes';

@Component({
  moduleId: module.id,
  templateUrl: 'institute-findings-summary.component.html',
 
  styles: [
  `
  .selected {
    border: 2px solid blue;  
   
  }
  `
  ]
})
export class InstituteFindingsSummaryComponent { 
  
   
   private ComplianceFormId: string;
   

   public loading: boolean;
   public CompForm: ComplianceFormA = new ComplianceFormA;
   
   
   public siteDisplayPos: string;

   private rootPath: string;
   public HideReviewCompletedSites: boolean;
   private ShowMatchesFoundSites: boolean;
   
   public InstituteSearchSummary : InstituteFindingsSummaryViewModel[] = [];

   
   constructor(private service: SearchService,
       private route: ActivatedRoute,
       private _location: Location,
       private router: Router,
        private sanitizer: DomSanitizer,
         private authService: AuthService
  ) {}
  

    ngOnInit() {
 
        this.route.params.forEach((params: Params) => {
        this.ComplianceFormId = params['formId'];
 
        this.siteDisplayPos =  params['siteDisplayPos'];
        
        this.rootPath =  params['rootPath'];

        this.LoadOpenComplainceForm();
        
        
    });

  }
  
    LoadOpenComplainceForm() {
        this.loading = true;
        this.service.getComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.CompForm = item;
                 this.LoadInstituteSiteSummary();
                this.loading = false;
               },
            error => {
                this.loading = false;
            });
    }
  
  LoadInstituteSiteSummary(){
      this.loading = true;
        this.service.getInstituteFindingsSummary(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.InstituteSearchSummary = item;
                this.loading = false;
            },
            error => {
                this.loading = false;
            });
  }

 
gotoSiteDetails(SiteSourceId: number){
   
    this.router.navigate(['institute-findings', this.ComplianceFormId,  SiteSourceId, {rootPath:this.rootPath}], 
    { relativeTo: this.route.parent});
}

goBack() {

    //this._location.back();
    this.router.navigate(['comp-form-edit', this.ComplianceFormId, {rootPath: this.rootPath, tab:"invTab"}], { relativeTo: this.route.parent});
}

BoolYesNo (value: boolean): string   {
    if (value == null){
        return "";
    }
    if (value == true){
        return "Yes"
    }
    else{
        return "No"
    }
}

dividerGeneration(indexVal : number){
    if ((indexVal+1) % 3 == 0){
        return true;
    }
    else{
        return false;
    }
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
 get diagnostic() { return JSON.stringify(null); }
}