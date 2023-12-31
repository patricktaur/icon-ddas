import { Component, OnInit, OnDestroy, NgZone } from '@angular/core';
import {DomSanitizer} from '@angular/platform-browser';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormArray} from '@angular/forms';

import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource } from './search.classes';
import { SearchService } from './search-service';
import { Location } from '@angular/common';

@Component({
    moduleId: module.id,
    //templateUrl: 'compliance-form.component.html',
    //templateUrl: 'compliance-form-reactive-style.component.html',
    templateUrl: 'compliance-form-template-style-component.html',

})
export class ComplianceFormComponent implements OnInit {
    compFormForm: FormGroup;
    public formLoading: boolean;

    public CompForm: ComplianceFormA = new ComplianceFormA;
    private ComplianceFormId: string;
    public SitesAvailable : SiteSource[] = [];
    public searchInProgress: boolean = false;
    public Selected: boolean = false;
     public InvestigatorToRemove: InvestigatorSearched = new InvestigatorSearched;
     public siteToRemove: SiteSourceToSearch = new SiteSourceToSearch;
    private pageChanged: boolean= false;
   
    private rootPath: string;
    private page: number;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private _location: Location,
        private service: SearchService,
        private fb: FormBuilder,
        private sanitizer:DomSanitizer
    ) { }

    ngOnInit() {
        
        //this.buildForm();
        // console.log("this.route.parent:" + this.route.parent);
        // console.log("this.route.parent:" + this.route.parent.url);
       
          this.route.params.forEach((params: Params) => {
            this.ComplianceFormId = params['formId'];
            this.rootPath = params['rootPath'];
            this.page =  +params['page'];
 
            this.LoadOpenComplainceForm();
        });
    
    }

    //for REactive Validation
    buildForm(): void {
         this.compFormForm = this.fb.group({
            'projNumber': [this.CompForm.ProjectNumber,
             [ 
            Validators.minLength(9), 
           Validators.maxLength(9) 
            ]
            ],
           
             'SponsorProtocolNumber': [this.CompForm.SponsorProtocolNumber],
              'Institute': [this.CompForm.Institute],
               'Country': [this.CompForm.Country],
                'Address': [this.CompForm.Address],
                
                'Investigators': this.fb.array([])
 
            });

            this.LoadInvestigatorControls();
        // this.compFormForm.valueChanges
        // .subscribe(data => this.onValueChanged(data));
        //  this.onValueChanged(); // (re)set validation messages now
    }

    InitInvestigatorControls(){
         return this.fb.group({
            Name: ['', Validators.required],
            CanEdit:['']
        });
    }
    
    LoadInvestigatorControls(){
        const control = <FormArray>this.compFormForm.controls['Investigators'];
        
       this.CompForm.InvestigatorDetails.forEach(inv => 
            control.push(this.getInvestigatorControl(inv))
       );
    }

    addInvestigatorControl(){
        const control = <FormArray>this.compFormForm.controls['Investigators'];
        let inv = new InvestigatorSearched;
        control.push(this.getInvestigatorControl(inv))
    }
    
    getInvestigatorControl(inv: InvestigatorSearched){
          return this.fb.group({
            Name: [inv.Name, Validators.required],
             CanEdit:[inv.CanEdit]
         });
    }

   onValueChanged(data?: any) {
         if (!this.compFormForm) { return; }
         const form = this.compFormForm;
        for (const field in this.formErrors) {
          // clear previous error message (if any)
          this.formErrors[field] = '';
          const control = form.get(field);
          if (control && control.dirty && !control.valid) {
            const messages = this.validationMessages[field];
            for (const key in control.errors) {
              this.formErrors[field] += messages[key] + ' ';
            }
          }
        }
    }

 formErrors = {
    'projNumber': '',
    'SponsorProtocolNumber': '',
    'Institute': '',
    'Country': '',
    'Address': '',

  };
  
  
  
  investigator = {

  }

  validationMessages = {
    'projNumber': {
      'required':      'Project Number is required.',
      'minlength':     'Project Number must be at 9 characters long.',
      'maxlength':     'Project Number must be at 9 characters long.'
      
    }
  };
    
    SaveReactiveForm() {
             

              (<FormGroup>this.compFormForm)
             .setValue(this.CompForm, { onlySelf: true });
             //this.CompForm = this.compFormForm.value;
             this.service.saveComplianceForm(this.CompForm)
            .subscribe((item: any) => {
                console.log("Save Successful");
                let newCompForm: ComplianceFormA = item;
                this.ComplianceFormId = newCompForm.RecId;
                this.LoadOpenComplainceForm();
               },
            error => {

            });
    }

    LoadOpenComplainceForm() {
        this.formLoading = true;
        this.service.getComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.CompForm = item;
                this.LoadSitesAvailable();
                this.Initialize();
                this.SetInvestigatorsSavedFlag();
                this.pageChanged = false;
                this.buildForm();
                this.formLoading = false;
                },
            error => {
                this.formLoading = false;
            });
    }

    Initialize(){
        
        let SearchPending = false;
        
        for (let inv of this.CompForm.InvestigatorDetails){
            let SitesSearchedCount : number = 0;
             for (let searchStatus of inv.SitesSearched){
                if (searchStatus.ExtractedOn == null){
                    SearchPending = true;
                 }
                else{
                     SitesSearchedCount +=1;
                }
                inv.CanEdit = true;
                inv.Saved = true;
                if (SitesSearchedCount > 0){
                    inv.CanEdit = false;
                }
            }
            
            this.CompForm.SearchPending = SearchPending;
        }
    }
   
   get PrincipalInvestigatorName(){
       let p =  this.CompForm.InvestigatorDetails.find(x=> x.Role == "PI");
       if (p != null)
       {
           return p.Name;
       }
       return null;
   }
   get CurrentComplianceForm(){
       if (this.CompForm == null){
           return null;
       }
       return this.CompForm;
   }
  
   get statusEnum():number{
      return this.CompForm.StatusEnum;
  }

   CloseForEdit(){
       this.service.CloseComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                    console.log("success");
                     this.router.navigate(['search']);
                },
            error => {
            });
   }
   //Investigators:
   //================
   get Investigators(){
       if (this.CompForm == undefined) {return null;}
       return this.CompForm.InvestigatorDetails.filter(x => x.Deleted == false);
   }
   
   InvestigatorAdd(){
        let inv = new InvestigatorSearched;
        let lastInvestigatorNumber: number = this.LastInvestigatorNumber;
        inv.Id = lastInvestigatorNumber + 1;
        inv.Saved = false;
        inv.Help = "Save pending"
       
        this.CompForm.InvestigatorDetails.push(inv);
        this.SetInvestigatorRole();
        this.Initialize();
        this.pageChanged = true;
   } 
     
 setInvestigatorRemove(inv: InvestigatorSearched){
     this.InvestigatorToRemove = inv
    //  this.ComplianceFormIdToDelete = inv.RecId;
    //  this.InvestigatorNameToDelete = inv.PrincipalInvestigator;
   }
   
   InvestigatorRemove(){
       // item.Deleted = true;
        this.InvestigatorToRemove.Deleted = true;

        //Remove from Findings:
        this.CompForm.Findings = this.CompForm.Findings.filter(x => x.InvestigatorSearchedId != this.InvestigatorToRemove.Id)

        var index = this.CompForm.InvestigatorDetails.indexOf(this.InvestigatorToRemove);
        if (index > -1) {
            this.CompForm.InvestigatorDetails.splice(index, 1);
        }

        this.SetInvestigatorRole();
        this.Initialize();
        this.pageChanged = true;
   } 
   
   //Better method?
   get LastInvestigatorNumber():number{
      let lastNumber:number = 0
      for (let item of this.CompForm.InvestigatorDetails){
          if (item.Id > lastNumber){
                lastNumber =item.Id
          }
      }
       return lastNumber;
   }
   
   SetInvestigatorRole(){
      let index:number = 0
      for (let item of this.Investigators){
          if (index == 0){
               item.Role = "PI" 
          }
          else{
            item.Role = "Sub I"
          }
          index +=1;
      }
   } 
    
   SetInvestigatorsSavedFlag(){
       for (let item of this.Investigators){
         item.Saved = true;
         item.Help = "Edit Investigator related findings"
      }
   } 
   
     //Does not work if an item is markedAsDeleted
   move(idx: number, step: number) {
    
    var tmp = this.CompForm.InvestigatorDetails[idx];

    
     this.CompForm.InvestigatorDetails[idx] = this.CompForm.InvestigatorDetails[idx - step];
     this.CompForm.InvestigatorDetails[idx - step] = tmp;
     this.SetInvestigatorRole();
    
    }
    

  
  moveA(inv: InvestigatorSearched, step: number) {
      var tmp = inv;
  }


 
 //SitesParticpatingInSearch  
 //================================   
    
    LoadSitesAvailable(){
            this.service.getSiteSources()
            .subscribe((item: any) => {
                 this.SitesAvailable = item;
                 this.InitializeSitesAvailableToAdd();
             },
            error => {
            });
    }
 
   LoadMockSitesAvailable(){
        this.SitesAvailable = [];
        
        let s1 = new SiteSource;
        s1.SiteEnum = 13;
        s1.SiteName = "Site - 1";
        s1.Included = false;
        this.SitesAvailable.push(s1);

        let s2 = new SiteSource;
        s2.SiteEnum = 14;
        s2.SiteName = "Site - 2";
         s2.Included = false;
        this.SitesAvailable.push(s2);

        let s3 = new SiteSource;
        s3.SiteEnum = 15;
        s3.SiteName = "Site - 3";
         s2.Included = false;
        this.SitesAvailable.push(s3);

}
    
    InitializeSitesAvailableToAdd(){
        //Mark sites that are already included Comp Form:
   
        for (let site of this.SitesAvailable){
            let siteInCompForm = this.CompForm.SiteSources.find(x => x.SiteEnum == site.SiteEnum);
            site.Selected = false;
            if (siteInCompForm == null){
                  site.Included = false;  
            }
            else{
                site.Included = true;  
            }
         }
    }

    get SitesParticpatingInSearch() {
         return this.CompForm.SiteSources.filter(x => x.Deleted == false)
    }

    
    
    get SitesAvalaibleToInclude(){
      return this.SitesAvailable.filter(x => x.Included == false);
   }
   
   AddSelectedSite(){
      var index = 0;

        for (index = 0; index <this.SitesAvailable.length; ++index) {
            if (this.SitesAvailable[index].Selected == true){
                   //Check if site is already included
                   let enumOfSiteToAdd = this.SitesAvailable[index].SiteEnum;
                   let check = this.CompForm.SiteSources.find(x => x.SiteEnum == enumOfSiteToAdd)
                   if (check){ //If found then it was possibly marked as deleted 
                        check.Deleted = false;
                   }
                   else{  //add it to the collection
                        let siteToAdd = new SiteSourceToSearch; 
                        siteToAdd.SiteName = this.SitesAvailable[index].SiteName;
                        siteToAdd.SiteEnum = this.SitesAvailable[index].SiteEnum;
                        siteToAdd.Id = this.LastSiteSourceId + 1;
                        siteToAdd.IsMandatory = false;
                        siteToAdd.ExtractionMode = this.SitesAvailable[index].ExtractionMode;
                        siteToAdd.SiteUrl = this.SitesAvailable[index].SiteUrl;
                        this.CompForm.SiteSources.push(siteToAdd);
                        this.SitesAvailable[index].Included = true;
                   }
                   //one or more sites are added.
                   this.pageChanged = true;
            }
            this.SitesAvailable[index].Selected = false;
        }
        this.SetSiteDisplayPosition();
}  
 
 setSiteToRemove(site: SiteSourceToSearch){
     this.siteToRemove = site;
   }
 
 RemoveSite(){
     //item.Deleted = true;
     this.siteToRemove.Deleted = true;
     let site = this.SitesAvailable.find(x => x.SiteEnum == this.siteToRemove.SiteEnum);
     if (site){
         site.Included = false;
         this.pageChanged = true;
     }
     this.SetSiteDisplayPosition();
}
  
  SetSiteDisplayPosition(){
       let pos:number = 1
      for (let item of this.CompForm.SiteSources){
          
          if (item.Deleted == false){
                item.DisplayPosition = pos;
                pos += 1;
          }
          else{
              item.DisplayPosition = 0;
          }
      }
  }
       //Better method?
   get LastSiteSourceId():number{
      let lastNumber:number = 0
      for (let item of this.CompForm.SiteSources){
          if (item.Id > lastNumber){
                lastNumber =item.Id
          }
      }
    return lastNumber;
   }  
    
   //Findgins
   get Findings(){
       if (this.CompForm == undefined){
           return null;
       }
       return this.CompForm.Findings.filter(x => x.Selected == true);
   }
  
      Save() {
             
             
             this.service.saveComplianceForm(this.CompForm)
            .subscribe((item: ComplianceFormA) => {
                //this.CompForm = item;
                this.ComplianceFormId = item.RecId;
                this.LoadOpenComplainceForm();
                // this.Initialize();
                // this.SetInvestigatorsSavedFlag();
                
              },
            error => {

            });
    } 

    ScanNSave() {
            this.searchInProgress = true;
            this.service.scanSaveComplianceForm(this.CompForm)
            .subscribe((item: any) => {
                
                this.searchInProgress = false;
                this.LoadOpenComplainceForm();
                
               },
            error => {
                this.searchInProgress = false;
            });
    }
    selectionChange(){
        this.SitesAvalaibleToInclude.forEach(i=>i.Selected=!this.Selected);
    }

gotoInvestigatorSummaryResult(inv: InvestigatorSearched){

    this.router.navigate(['investigator-summary', this.CompForm.RecId, inv.Id, {rootPath: this.rootPath}], 
    { relativeTo: this.route.parent});
 
}

    goBack() {
  
        if (this.rootPath == null){
            this._location.back();
        }
        else{
            this.router.navigate([this.rootPath, { id: this.ComplianceFormId, page: this.page }]);
        }
       
    }
    
Split = (RecordDetails: string) => {
    if (RecordDetails == undefined){
        return null;
    }
    var middleNames : string[] = RecordDetails.split("~");

    return middleNames;
}

  canDeactivate(): Promise<boolean> | boolean {
              
        if (this.pageChanged == false){
            return true;
        }
        // Otherwise ask the user with the dialog service and return its
        // promise which resolves to true or false when the user decides
        //this.IgnoreChangesConfirmModal.open();
        //return this.canDeactivateValue;
        return window.confirm("Changes not saved. Ignore changes?");//this.dialogService.confirm('Discard changes?');
    }

    sanitize(url:string){
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }
    
// BoolYesNo (value: boolean): string   {
//     if (value == null){
//         return "";
//     }
//     if (value == true){
//         return "Yes"
//     }
//     else{
//         return "No"
//     }
// }
    get diagnostic() { return JSON.stringify(this.SitesAvailable); }

 
}

