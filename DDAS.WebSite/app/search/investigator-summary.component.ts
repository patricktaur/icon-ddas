import { Component} from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';

import {Location} from '@angular/common';
import {SearchService} from './search-service';

//import {SiteInfo,SearchSummaryItem,SearchSummary,NameSearch, MatchedRecordsPerSite} from './search.classes';

import {InvestigatorSearched, ComplianceFormA, SiteSource, 
     SiteSearchStatus } from './search.classes';

@Component({
  moduleId: module.id,
  templateUrl: 'investigator-summary.component.html',
 
  styles: [
  `
  .selected {
    background-color: yellow;
  }
  `
  ]
})
export class InvestigatorSummaryComponent { 
  
   public  InvestigatorId:number;
   private ComplianceFormId: string;
   public InvestigatorSummary : InvestigatorSearched = new InvestigatorSearched;

   processing: boolean;
   public CompForm: ComplianceFormA = new ComplianceFormA;
   public retSiteEnum: number;

   constructor(private service: SearchService,
       private route: ActivatedRoute,
       private _location: Location,
       private router: Router
  ) {}
  

    ngOnInit() {
        
        this.route.params.forEach((params: Params) => {
        this.ComplianceFormId = params['formId'];
        this.InvestigatorId = +params['investigatorId'];
        this.retSiteEnum =  +params['siteEnum'];

        console.log('this.ComplianceFormId' + this.ComplianceFormId);
         console.log('this.InvestigatorId' + this.InvestigatorId);
          console.log('this.retSiteEnum' + this.retSiteEnum);

        this.LoadOpenComplainceForm();

        
    });

  }
  
    LoadOpenComplainceForm() {
        this.service.getComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.CompForm = item;
                //this.IntiliazeRecords();
                
              },
            error => {
            });
    }
  

 loadMockInvestigatorSummary(){

    //    this.InvestigatorSummary =   { 
    //         Id: 0,
    //         DisplayPosition: 0,
    //         //SourceNumber: 0,
    //         Name:  "Inv - 1",
    //         Role: "Principal",
    //         Sites_FullMatchCount:  0,
    //         Sites_PartialMatchCount:  0,
    //         AllSitesProcessed:  false,
    //         TotalIssuesFound:  0,
    //         Deleted:  false,
    //         SitesSearched: [
    //                {
    //                     siteEnum:  0,
    //                     SiteName : "Site - 1",
    //                     SiteUrl : "",    
    //                     HasExtractionError : false,
    //                     ExtractionErrorMessage : "",
    //                     FullMatchCount :  0,
    //                     PartialMatchCount :  0,
    //                     IssuesFound :  0,
    //                     ReviewCompleted :  false
    //                } ,
    //                {
    //                     siteEnum:  0,
    //                     SiteName : "Site - 2",
    //                     SiteUrl : "",    
    //                     HasExtractionError : false,
    //                     ExtractionErrorMessage : "",
    //                     FullMatchCount :  2,
    //                     PartialMatchCount :  4,
    //                     IssuesFound :  6,
    //                     ReviewCompleted :  true
    //                },
    //                {
    //                     siteEnum:  0,
    //                     SiteName : "Site - 3",
    //                     SiteUrl : "",    
    //                     HasExtractionError : false,
    //                     ExtractionErrorMessage : "",
    //                     FullMatchCount :  3,
    //                     PartialMatchCount :  9,
    //                     IssuesFound :  18,
    //                     ReviewCompleted :  false
    //                } ,
    //                {
    //                     siteEnum:  0,
    //                     SiteName : "Site - 4",
    //                     SiteUrl : "",    
    //                     HasExtractionError : false,
    //                     ExtractionErrorMessage : "",
    //                     FullMatchCount :  4,
    //                     PartialMatchCount :  16,
    //                     IssuesFound :  32,
    //                     ReviewCompleted :  true
    //                } 
    //         ]
    //     }
       
       
      
 }
 

get InvestigatorSiteSummary(){
 
    let sitesSearched:  SiteSearchStatus[];
    if (this.Investigator == undefined){
        return sitesSearched;
    }
    else{
        return this.Investigator.SitesSearched;
    }
   
}

get Investigator(): InvestigatorSearched{
  
    let inv:  InvestigatorSearched = new InvestigatorSearched;
    let inv1 = this.CompForm.InvestigatorDetails.find(x => x.Id == this.InvestigatorId);
    if (inv1 == undefined){
        return inv;
    }
    else{
        return inv1;
    }
}

get Summary(){
    if (this.CompForm == null) return null;

    let retSummary: string[] ;
    retSummary.push("Use 'Search' option to find matching records" );
    retSummary.push("Use 'Search' option to find matching records aaaa" );
    if (this.Investigator.ExtractionErrorSiteCount > 0){
        retSummary.push("Extraction of matching records at" + this.Investigator.ExtractionErrorSiteCount + " site(s) was not successfull." );
        retSummary.push("Use 'Search' option on the Compliance Form re-extract matching records" );

    }
    if (this.Investigator.ExtractionErrorSiteCount == 0){
        if (this.Investigator.ExtractedOn == null){
            retSummary.push("Use 'Search' option to find matching records" );
        }
        else{
            retSummary.push("Matching records were extracted on: " + this.Investigator.ExtractedOn);
            if (this.Investigator.Sites_FullMatchCount > 0){
                retSummary.push(this.Investigator.Sites_FullMatchCount + " sites have full match records." );
            }
            else{
                retSummary.push("No site found with Full Match records." );
            }
			if (this.Investigator.Sites_PartialMatchCount > 0){
                retSummary.push(this.Investigator.Sites_PartialMatchCount + " sites have partially matching records." );
            }
            else{
                retSummary.push("No site found with partially matching records." );
            }
        }
        
        if (this.Investigator.ReviewCompletedSiteCount == this.Investigator.SitesSearched.length){
            retSummary.push("Review completed for all sites" );
        }
        else{
            retSummary.push("Review completed for " + this.Investigator.ReviewCompletedSiteCount + " of " + "");
        }


    }

    return retSummary;
}
 
gotoSiteDetails(siteEnum: number){
    this.router.navigate(['findings', this.ComplianceFormId, this.InvestigatorId, siteEnum], 
    { relativeTo: this.route.parent});
}
 
goBack() {

    //this._location.back();
    this.router.navigate(['complianceform', this.ComplianceFormId], { relativeTo: this.route.parent});
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

 get diagnostic() { return JSON.stringify(this.Investigator); }
}