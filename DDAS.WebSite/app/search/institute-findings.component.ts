import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource, Finding, SiteSearchStatus, UpdateInstituteFindings } from './search.classes';
import { SearchService } from './search-service';
import { Location } from '@angular/common';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';

@Component({
    moduleId: module.id,
    templateUrl: 'institute-findings.component.html',
   
    
})
export class InstituteFindingsComponent implements OnInit {
    public CompForm: ComplianceFormA = new ComplianceFormA;
    private ComplianceFormId: string;
   
    private SiteSourceId: number;

    public SitesAvailable : SiteSourceToSearch[] = [];
    public searchInProgress: boolean = false;

    private pageChanged: boolean= false;
     private rootPath: string;
     public loading: boolean;
     public singleMatchRecordsLoading: boolean;
     public minMatchCount: number;
     private singleMatchRecords: Finding[]=[];
     private recordToDelete: Finding = new Finding;
     public pageNumber: number;
     public filterRecordDetails: string = "";
    
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
 
            this.SiteSourceId = +params['siteSourceId']
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

  
    get Site(){
        let site = new SiteSourceToSearch;
        
       
        let site1 = this.CompForm.SiteSources.find(x => x.Id == this.SiteSourceId);
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
                if (this.Site.SiteUrl.toLowerCase().startsWith("http") ){
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
    

 
    get Findings(){
         
           return this.CompForm.Findings.filter(x => x.SiteSourceId == this.SiteSourceId);
           
    }

   

 
    Add(){
        let finding = new Finding;
        finding.IsMatchedRecord = false;
        // finding.InvestigatorSearchedId = this.InvestigatorId;
        finding.SiteSourceId = this.SiteSourceId
       
        finding.SiteDisplayPosition = this.Site.DisplayPosition;
        finding.SiteId = this.Site.SiteId;
        finding.SiteEnum = this.Site.SiteEnum; //this.SiteEnum;
        
        //finding.DateOfInspection = new Date() ;
        finding.Selected = true;
        finding.InvestigatorName = this.CompForm.Institute;
        finding.IsAnIssue = true;
        this.CompForm.Findings.push(finding);
        this.pageChanged = true;
    }
 
    SetFindingToRemove(selectedRecord: Finding){
        this.recordToDelete = selectedRecord;
    }

    get RecordToDeleteText(){
        if (this.recordToDelete == null){
            return "";
        }else{
            if (this.recordToDelete.RecordDetails == null){
                return "";
            }else{
                return this.recordToDelete.RecordDetails.substr(0, 100) + " ...";
            }
         }
    }

    RemoveFinding(){
        this.pageChanged = true;
         var index = this.CompForm.Findings.indexOf(this.recordToDelete, 0);
            if (index > -1) {
                this.CompForm.Findings.splice(index, 1);
            }
    } 
 
    SaveAndClose(){
            
            let updateFindings = new UpdateInstituteFindings;
              
              updateFindings.FormId= this.ComplianceFormId;
              //SiteSourceId
              updateFindings.SiteSourceId = this.SiteSourceId;
              updateFindings.Findings = this.Findings;
               
            this.service.updateInstituteFindings(updateFindings)
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
 
        this.router.navigate(['institute-findings-summary', this.ComplianceFormId, {siteDisplayPos:this.SiteSourceId, rootPath: this.rootPath}], { relativeTo: this.route.parent});
        
      
    }
    
    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }
    get diagnostic() { return JSON.stringify(this.highlightFilter); }
}