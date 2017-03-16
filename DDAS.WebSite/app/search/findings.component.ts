import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource, Finding, SiteSearchStatus, UpdateFindigs } from './search.classes';
import { SearchService } from './search-service';
import { Location } from '@angular/common';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';

@Component({
    moduleId: module.id,
    templateUrl: 'findings.component.html',
   
    
})
export class FindingsComponent implements OnInit {
    public CompForm: ComplianceFormA = new ComplianceFormA;
    private ComplianceFormId: string;
    private InvestigatorId: number;
    private SiteEnum: number;

    public SitesAvailable : SiteSourceToSearch[] = [];
    public searchInProgress: boolean = false;

    private pageChanged: boolean= false;
     private rootPath: string;
     public loading: boolean;
     public minMatchCount: number;
    
    @ViewChild('IgnoreChangesConfirmModal') IgnoreChangesConfirmModal: ModalComponent;
    private canDeactivateValue: boolean;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private _location: Location,
        private service: SearchService,
    ) { }

    ngOnInit() {
 
        this.route.params.forEach((params: Params) => {
            this.ComplianceFormId = params['formId'];
            this.InvestigatorId = +params['investigatorId'];
            this.SiteEnum = +params['siteEnum'];
            this.rootPath =  params['rootPath'];
            this.LoadOpenComplainceForm();

            
            
        });
     
    }

    LoadOpenComplainceForm() {
        this.loading = true;
        this.service.getComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.CompForm = item;
                //this.IntiliazeRecords();
                 this.loading = false;
              },
            error => {
                 this.loading = false;
            });
    }

    IntiliazeRecords(){
        for (let item of this.CompForm.Findings) {
                if (item.Selected == null) {
                    item.Selected = false;
                 }
              
        }
    }

    get Site(){
        let site = new SiteSourceToSearch;
        let site1 = this.CompForm.SiteSources.find(x => x.SiteEnum == this.SiteEnum);
        if (site1 == undefined){
            site.SiteName = "Not found";
            return site;
         }
         else{
            return site1;
         }
        
    }
    
    get Investigator(){
        let inv = new InvestigatorSearched;
        let inv1 = this.CompForm.InvestigatorDetails.find(x => x.Id == this.InvestigatorId);
        if (inv1 == undefined){
            inv.Name = "Not found";
            return inv;
        }
        else{
            return inv1;
        }
     }
    
    get Findings(){
         
         return this.CompForm.Findings.filter(x => x.InvestigatorSearchedId == this.InvestigatorId 
         && x.SiteEnum == this.SiteEnum);
    }

    get SelectedFindings(){
        return this.Findings.filter(x => x.Selected == true);
    }
    
    get MatchedSiteRecords(){
       
        return this.Findings.filter(x => x.Selected == false && x.IsMatchedRecord == true).sort(s=> s.MatchCount).reverse();
    }
  
     get MatchedSiteRecordsForMatchCount(){
        //return this.Findings;
        return this.Findings.filter(x => x.Selected == false && x.IsMatchedRecord == true  ).sort(s=> s.MatchCount).reverse();
    }

    get  SiteSearchStatus(){

        let siteSearched = new SiteSearchStatus;
        let siteSearched1 = this.Investigator.SitesSearched.find(x => x.siteEnum == this.SiteEnum);
        if (siteSearched1 == undefined){
             //siteSearched.siteEnum = -1;
            return siteSearched;
        }
        else{
            return siteSearched1;
        }
 
    }

    
    // AddNewSearchStatusItem(){
        
    //     if (this.SiteSearchStatus.siteEnum = -1){
    //         this.SiteSearchStatus.siteEnum = this.SiteEnum;
    //         this.Investigator.SitesSearched.push(this.SiteSearchStatus);
    //     }
    // }

    Add(){
        let finding = new Finding;
        finding.IsMatchedRecord = false;
        finding.InvestigatorSearchedId = this.InvestigatorId;
        finding.SiteEnum = this.SiteEnum;
        finding.SourceNumber = this.Site.DisplayPosition;
        finding.DateOfInspection = new Date(Date.now()) ;
        finding.Selected = true;
        finding.InvestigatorName = this.Investigator.Name;
        this.CompForm.Findings.push(finding);
        this.pageChanged = true;
    }
    
    AddSelectedToFindings(){
           for (let item of this.CompForm.Findings) {
                if (item.UISelected == true) {
                    item.Selected = true;
                    item.UISelected = false;
                 }
            }
            this.pageChanged = true;
    }
    
    RemoveFromSelected(selectedRecord: Finding){
        this.pageChanged = true;
        selectedRecord.IsAnIssue = false;
        selectedRecord.Observation = "";
        selectedRecord.Selected = false;
    } 
    
    get ExtractionModeIsManual(){
        if (this.SiteSearchStatus.ExtractionMode == "Manual"){
             return true;
        }
        else
        {
            return false;
        }
       
    }
    
  
    
    
    // Save() {
    //         //this.AddNewSearchStatusItem();
    //         this.service.saveComplianceForm(this.CompForm)
    //         .subscribe((item: any) => {
    //             this.pageChanged = false;
    //             this.CompForm = item;
    //             this.IntiliazeRecords();
    //           },
    //         error => {

    //         });
    // }

    // SaveAndClose(){
    //         //this.AddNewSearchStatusItem();
    //            this.service.saveComplianceForm(this.CompForm)
    //         .subscribe((item: any) => {
    //             this.pageChanged = false;
    //             this.goBack()
    //             //this._location.back();
    //              },
    //         error => {

    //         });
    //  }
    
    SaveAndClose(){
            //formId : string, siteEnum:number, InvestigatorId:number, ReviewCompleted : boolean,  Findings:Finding[]
            let updateFindings = new UpdateFindigs;
              
              updateFindings.FormId= this.ComplianceFormId;
              updateFindings.SiteEnum = this.SiteEnum;
              updateFindings.InvestigatorSearchedId = this.InvestigatorId;
              updateFindings.ReviewCompleted = this.SiteSearchStatus.ReviewCompleted; 
              updateFindings.Findings = this.Findings;
               
            this.service.saveFindingsAndObservations(updateFindings)
            .subscribe((item: any) => {
                this.pageChanged = false;
                this.goBack()
                
                 },
            error => {

            });
     }
    
    Split = (RecordDetails: string) => {
        if (RecordDetails == undefined){
            return null;
        }
        var middleNames : string[] = RecordDetails.split("~");
    
        return middleNames;
    }
    
    dividerGeneration(indexVal : number){
        if ((indexVal+1) % 2 == 0){
            return true;
        }
        else{
            return false;
        }
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
    
    setDeactivateValue(){
        this.canDeactivateValue = true;
    }
    
    goBack() {
 
        this.router.navigate(['investigator-summary', this.ComplianceFormId, this.InvestigatorId,  {siteEnum:this.SiteEnum, rootPath: this.rootPath}], { relativeTo: this.route.parent});
        //this.router.navigate(['comp-form-edit', this.ComplianceFormId, this.InvestigatorId,  {siteEnum:this.SiteEnum, rootPath: this.rootPath}], { relativeTo: this.route.parent});
      
    }
    
    get diagnostic() { return JSON.stringify(this.CompForm.Findings.length); }
}