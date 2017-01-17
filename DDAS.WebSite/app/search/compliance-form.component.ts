import { Component, OnInit, OnDestroy, NgZone } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource } from './search.classes';
import { SearchService } from './search-service';
import { Location } from '@angular/common';

@Component({
    moduleId: module.id,
    templateUrl: 'compliance-form.component.html',

})
export class ComplianceFormComponent implements OnInit {
    public CompForm: ComplianceFormA = new ComplianceFormA;
    private ComplianceFormId: string;
    public SitesAvailable : SiteSource[] = [];
    public searchInProgress: boolean = false;
    public Selected: boolean = false;
    public InvestigatorToRemove: InvestigatorSearched = new InvestigatorSearched;
    public siteToRemove: SiteSourceToSearch = new SiteSourceToSearch;

  
    
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private _location: Location,
        private service: SearchService

        
       
       

    ) { }

    ngOnInit() {
        
        
        this.route.params.forEach((params: Params) => {
            this.ComplianceFormId = params['formid'];
            this.LoadOpenComplainceForm();
        });
    
    }

    LoadOpenComplainceForm() {
        this.service.getComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.CompForm = item;
                this.LoadSitesAvailable();
                this.Initialize();
                this.SetInvestigatorsSavedFlag();
                },
            error => {
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
                if (SitesSearchedCount > 0){
                    inv.CanEdit = false;
                }
            }

            this.CompForm.SearchPending = SearchPending;
        }
    }
   
   get PrincipalInvestigatorName(){
       let p =  this.CompForm.InvestigatorDetails.find(x=> x.Role == "Principal");
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
   } 
     
 setInvestigatorRemove(inv: InvestigatorSearched){
     this.InvestigatorToRemove = inv
    //  this.ComplianceFormIdToDelete = inv.RecId;
    //  this.InvestigatorNameToDelete = inv.PrincipalInvestigator;
   }
   
   InvestigatorRemove(){
       // item.Deleted = true;
        this.InvestigatorToRemove.Deleted = true;
        this.SetInvestigatorRole();
        this.Initialize();
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
               item.Role = "Principal" 
          }
          else{
            item.Role = "Sub" 
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
     //var tmp = this.Investigators[idx];
    
    //find non deleted item and then swap:
    let index:number = 0
  
    
    console.log("idx:" + idx);
    console.log("step:" + step);
    console.log("len:" + this.CompForm.InvestigatorDetails.length);
    let deletedCount: number = 0;
    for (let  _i = idx - step; _i < this.CompForm.InvestigatorDetails.length && _i >= 0; _i += -step){
          console.log( " _i:" + _i);
           console.log( " Deleted:" + this.CompForm.InvestigatorDetails[_i].Deleted);
          if (this.CompForm.InvestigatorDetails[_i].Deleted == false){
                //  this.CompForm.InvestigatorDetails[idx] = this.CompForm.InvestigatorDetails[_i];
                // this.CompForm.InvestigatorDetails[_i ] = tmp;
                 
                  this.CompForm.InvestigatorDetails[idx + 1] = this.CompForm.InvestigatorDetails[_i + deletedCount];
                this.CompForm.InvestigatorDetails[_i + deletedCount ] = tmp;
                 
                 console.log( " swapped idx, _i" );
                break;
          }
          else{
              deletedCount +=1;
          }
     }
    
 

    //  this.Investigators[idx] = this.Investigators[idx - step];
    // this.Investigators[idx - step] = tmp;
    
    // this.CompForm.InvestigatorDetails[idx] = this.CompForm.InvestigatorDetails[idx - step];
    // this.CompForm.InvestigatorDetails[idx - step] = tmp;
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
                        this.CompForm.SiteSources.push(siteToAdd);
                        this.SitesAvailable[index].Included = true;
                   }
                   
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
            .subscribe((item: any) => {
                this.CompForm = item;
                this.Initialize();
                this.SetInvestigatorsSavedFlag();
                
              },
            error => {

            });
    }

    ScanNSave() {
            this.searchInProgress = true;
            this.service.scanSaveComplianceForm(this.CompForm)
            .subscribe((item: any) => {
                this.CompForm = item;
                this.Initialize();
                this.SetInvestigatorsSavedFlag();
                this.searchInProgress = false;
               },
            error => {
                this.searchInProgress = false;
            });
    }
    selectionChange(){
        this.SitesAvalaibleToInclude.forEach(i=>i.Selected=!this.Selected);
    }

gotoInvestigatorSummaryResult(inv: InvestigatorSearched){

    this.router.navigate(['investigator-summary', this.CompForm.RecId, inv.Id], 
    { relativeTo: this.route.parent});
 
}

    goBack() {
        this._location.back();
    }
    
Split = (RecordDetails: string) => {
    if (RecordDetails == undefined){
        return null;
    }
    var middleNames : string[] = RecordDetails.split("~");

    return middleNames;
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
    get diagnostic() { return JSON.stringify(this.SitesAvailable); }
}