import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
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
    private highlightFilter: string;
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private _location: Location,
        private service: SearchService,
         private sanitizer: DomSanitizer
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

    get SiteHasUrl(){
        if (this.Site != null){
            if (this.Site.SiteUrl != null ){
                if (this.Site.SiteUrl.length > 0 ){
                    return true;
                }
                else{
                    return false;
                }
             }
            else{
                return false;
            }
            
        }
        else{
            return false;
        }
    }
    
    get IsManualExtractionSite(){
        let retValue :boolean = false;
        if (this.Site.ExtractionMode.toLowerCase() == "manual"){
            retValue =  true;
        }
        return  retValue;
    }
    get Investigator(){
        let inv = new InvestigatorSearched;
        let inv1 = this.CompForm.InvestigatorDetails.find(x => x.Id == this.InvestigatorId);
        if (inv1 == undefined){
            inv.Name = "Not found";
            return inv;
        }
        else{
              //remove special characters
             let str = inv1.Name.replace(/[^a-zA-Z ]/g,'');
             //remove words with less than 2 characters
             this.highlightFilter = str.replace(/(\b(\w{1,2})\b(\W|$))/g,''); //.split(/\s+/);
               
            return inv1;
        }
     }

     private comps: string[];
     private retvalue: string = "";
     removeSingleComponent(name:string){
         this.comps = name.split(" ");
         
         let spc = "";
        
        for (var c in this.comps){
            this.retvalue.concat("XXXXXXXXXXXXX");
            // if (c.length > 1){
                
            //     this.retvalue.concat(c);
            //     spc = " ";
            // }
        }
        return this.retvalue;
         
     }
     get InvestigatorNameComponents(){
         return this.Investigator.NameComponentCount;
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
  
   
     get FullMatchRecords(){
        
        return this.Findings.filter(x => x.Selected == false && x.IsMatchedRecord == true && x.MatchCount && x.IsFullMatch == true).sort(s=> s.MatchCount).reverse();
    }

    get PartialMatchRecords(){
        
        return this.Findings.filter(x => x.Selected == false && x.IsMatchedRecord == true && x.MatchCount && x.IsFullMatch == false).sort(s=> s.MatchCount).reverse();
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
        //finding.DateOfInspection = new Date() ;
        finding.Selected = true;
        finding.InvestigatorName = this.Investigator.Name;
        finding.IsAnIssue = true;
        this.CompForm.Findings.push(finding);
        this.pageChanged = true;
    }
    
    AddSelectedToFindings(){
           for (let item of this.CompForm.Findings) {
                if (item.UISelected == true) {
                    item.Selected = true;
                    item.IsAnIssue = true;
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
    
    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }
    get diagnostic() { return JSON.stringify(this.highlightFilter); }
}