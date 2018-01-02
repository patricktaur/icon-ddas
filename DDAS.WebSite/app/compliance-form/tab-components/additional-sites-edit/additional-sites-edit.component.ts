import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource, ComplianceFormStatusEnum, Finding, InstituteFindingsSummaryViewModel } from '../../../search/search.classes';
import { DomSanitizer } from '@angular/platform-browser';
import { SearchService } from '../../../search/search-service';


@Component({
    selector: '[comp-form-additional-sites-edit]',
    moduleId: module.id,
    templateUrl: 'additional-sites-edit.component.html',
})
export class ComplianceFormAdditionalSitesEditComponent implements OnInit {
    @Input() CompForm: ComplianceFormA;
    public SiteSource: SiteSource = new SiteSource;
    public SiteSources: any[];
    private pageChanged: boolean = false;
    public siteToRemove: SiteSourceToSearch = new SiteSourceToSearch;
    constructor(
        private sanitizer: DomSanitizer,
        private service: SearchService,
    ) { }

    ngOnInit() {
        this.loadSiteSources();
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

    onSearchAppliesToChange(value:any){
        this.SiteSource.SearchAppliesToText = value;
        console.log(value);
    }
    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }

    loadSiteSources(){
        this.service.getSiteSources()
        .subscribe((item: any[]) =>{
            this.SiteSources = item;
        },
        error => {

        });
    }

    get AppliesToItems(){       
        var items: { id: number, name: string }[] = [
        { "id": 0, "name": "PIs and SIs" },
        { "id": 1, "name": "PIs" },
        { "id": 2, "name": "Institute" }];
        return items;
    }
    
    onSiteSourceChange(value:any){
        var site = this.SiteSources.find(x => x.RecId == value);
        
        this.SiteSource.RecId = site.RecId;
        this.SiteSource.SiteName = site.SiteName;
        this.SiteSource.SiteShortName = site.SiteShortName;
        this.SiteSource.SiteUrl = site.SiteUrl;
        this.SiteSource.ExtractionMode = site.ExtractionMode;
     }

    clearSelectedSite(){
        
        this.SiteSource = new SiteSource;
    }

    get LastSiteSourceId(): number {
        let lastNumber: number = 0
        for (let item of this.CompForm.SiteSources) {
            if (item.Id > lastNumber) {
                lastNumber = item.Id
            }
        }
        return lastNumber;
    }
    AddSelectedSite() {
  
      
        let siteToAdd = new SiteSourceToSearch;
        siteToAdd.SiteId = this.SiteSource.RecId;
        siteToAdd.SiteName = this.SiteSource.SiteName;
        //siteToAdd.SiteEnum = this.SitesAvailable[index].SiteEnum;
        siteToAdd.SiteUrl = this.SiteSource.SiteUrl;
        siteToAdd.SiteShortName = this.SiteSource.SiteShortName;
        siteToAdd.Id = this.LastSiteSourceId + 1;
        siteToAdd.IsMandatory = false;
        siteToAdd.SearchAppliesTo = this.SiteSource.SearchAppliesTo;
        siteToAdd.SearchAppliesToText = this.SiteSource.SearchAppliesToText;

        console.log("SearchAppliesTo : " + this.SiteSource.SearchAppliesTo);

        let extractionMode: string = "Manual";
        if (this.SiteSource.ExtractionMode != null){
            extractionMode = this.SiteSource.ExtractionMode;
        }
 
        if (this.SiteSource.SearchAppliesTo == 2)  //Applies to Institute
        {
            extractionMode = "Manual"
        }
        siteToAdd.ExtractionMode = extractionMode; 

        
        this.CompForm.SiteSources.push(siteToAdd);
        this.pageChanged = true;
        
        // var index = 0;

        // for (index = 0; index < this.SitesAvailable.length; ++index) {
        //     if (this.SitesAvailable[index].Selected == true) {
        //         //Check if site is already included
        //         let enumOfSiteToAdd = this.SitesAvailable[index].SiteEnum;
        //         let siteIdToAdd = this.SitesAvailable[index].RecId;
        //         //let check = this.CompForm.SiteSources.find(x => x.SiteEnum == enumOfSiteToAdd)
        //         let check = this.CompForm.SiteSources.find(x => x.SiteId == siteIdToAdd)
        //         if (check) { //If found then it was possibly marked as deleted 
        //             check.Deleted = false;
        //         }
        //         else {  //add it to the collection
        //             let siteToAdd = new SiteSourceToSearch;
        //             siteToAdd.SiteId = this.SitesAvailable[index].RecId;
        //             siteToAdd.SiteName = this.SitesAvailable[index].SiteName;
        //             siteToAdd.SiteEnum = this.SitesAvailable[index].SiteEnum;
        //             siteToAdd.SiteUrl = this.SitesAvailable[index].SiteUrl;
        //             siteToAdd.Id = this.LastSiteSourceId + 1;
        //             siteToAdd.IsMandatory = false;
        //             siteToAdd.ExtractionMode = this.SitesAvailable[index].ExtractionMode;
        //             this.CompForm.SiteSources.push(siteToAdd);
        //             this.SitesAvailable[index].Included = true;
        //         }
        //         //one or more sites are added.
        //         this.pageChanged = true;
        //     }
        //     this.SitesAvailable[index].Selected = false;
        // }
        
        
        this.SetSiteDisplayPosition();
    }

    setSiteToRemove(site: SiteSourceToSearch) {
        this.siteToRemove = site;
    }

//     RemoveSite() {
        
//         this.siteToRemove.Deleted = true;
//         //this.siteToRemove.SiteEnum
//         let site = this.SitesAvailable.find(x => x.SiteEnum == this.siteToRemove.SiteEnum);
//         if (site) {
//             site.Included = false;
//             this.pageChanged = true;
//         }
//         this.SetSiteDisplayPosition();
    
// }

    RemoveSite(){
        
        let siteIdToRemove = this.siteToRemove.SiteId;
       
       
         
       //Remove Findings:
        var i:number;
        for (i = this.CompForm.Findings.length - 1; i >= 0; i -= 1) {
            if (this.CompForm.Findings[i].SiteSourceId == this.siteToRemove.Id ) {
                 console.log("Finding removed: " + this.CompForm.Findings[i].SiteId  );
                this.CompForm.Findings.splice(i, 1);
            }
        }
        
        //remove siteSearchStatus for all Investigators.
        var inv:number;
        for (inv = this.CompForm.InvestigatorDetails.length - 1; inv >= 0; inv -= 1) {
            var s: number;
            for (s = this.CompForm.InvestigatorDetails[inv].SitesSearched.length - 1; s >= 0; s -= 1) {
                if (this.CompForm.InvestigatorDetails[inv].SitesSearched[s].SiteSourceId == this.siteToRemove.Id ) {
                   var invName = this.CompForm.InvestigatorDetails[inv].SearchName;
                   console.log( "siteSearchStatus removed: inv: " + invName + "-" + this.CompForm.InvestigatorDetails[inv].SitesSearched[s].DisplayPosition);
                   
                    this.CompForm.InvestigatorDetails[inv].SitesSearched.splice(s, 1);
                }
            
            }
            
           
        }

        var index = this.CompForm.SiteSources.indexOf(this.siteToRemove, 0);
        if (index > -1) {
            this.CompForm.SiteSources.splice(index, 1);
        }

        this.SetSiteDisplayPosition();
        this.SetSiteDisplayPositionInFindings();
        this.pageChanged = true;
    }
    
    SetSiteDisplayPositionInFindings() {
        for (let finding of this.CompForm.Findings) {
            let pos: number = 0
            pos = this.CompForm.SiteSources.find(x => x.Id == finding.SiteSourceId).DisplayPosition;
            finding.SiteDisplayPosition = pos;
        }
    }
    
    SetSiteDisplayPosition() {
        let pos: number = 1
        for (let item of this.CompForm.SiteSources) {

            if (item.Deleted == false) {
                item.DisplayPosition = pos;
                pos += 1;
            }
            else {
                item.DisplayPosition = 0;
            }
        }
    }
}