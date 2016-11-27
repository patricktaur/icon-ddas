import { Component} from '@angular/core';
import { Router }   from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ComplianceForm,SiteData} from '../search//search.classes';
import {ReportService} from './report-service';
import { DialogService }  from '../shared/utils/dialog.service';
@Component({
  moduleId: module.id,
  
  templateUrl: 'report.component.html',
})

export class ReportComponent {

  public CompForms: ComplianceForm[];
  ComplianceFormId:any;
  selectedvalue:any;
  recId:string="";
   isActive:boolean;
  public _SiteData: SiteData = new SiteData;

  
  private dialogResponse: Promise<Boolean>;

  public fullMatch: string ="all";
  public partialMatch: string ="all";
  public nameToSearch: string = "";

  constructor(private service: ReportService,private route: ActivatedRoute, private dialog: DialogService){

  }
  ngOnInit(){
              this.LoadReportResults();
  }
  
  LoadReportResults(){
    this.service.getNamesFromClosedComplianceForms()
            .subscribe((item: any) => {
              this.CompForms = item;  
            },
            error => {

            });
  }
  
  get FiltersRecords(){

      let fullMatchLow:number = 0;
      let fullMatchHigh:number = 0;
      
      switch (this.fullMatch)
      {
        case "all" :
          fullMatchLow = -1;
          fullMatchHigh = 999999999999;
          break;
        case "match" :
          fullMatchLow = 0;
           fullMatchHigh = 999999999999;
          break;
         case "nomatch" :
          fullMatchLow = -1;
          fullMatchHigh = 1;
          break; 
       default : 
        fullMatchLow = 0;
        fullMatchHigh = 999999999999;
      }
 
      let partialMatchLow:number = 0;
      let partialMatchHigh:number = 0;
       switch (this.partialMatch)
      {
        case "all" :
          partialMatchLow = -1;
          partialMatchHigh = 999999999999;
          break;
        case "match" :
          partialMatchLow = 0;
           partialMatchHigh = 999999999999;
          break;
         case "nomatch" :
          partialMatchLow = -1;
          partialMatchHigh = 1;
          break; 
       default : 
        partialMatchLow = 0;
        partialMatchHigh = 999999999999;
      }
      
      if (this.CompForms == undefined){
        return null;
      }
      if (this.nameToSearch.length == 0){
          return this.CompForms.filter((a) =>
                        (a.Sites_FullMatchCount > fullMatchLow && a.Sites_FullMatchCount < fullMatchHigh)
                    &&  (a.Sites_PartialMatchCount > partialMatchLow && a.Sites_PartialMatchCount < partialMatchHigh)
                     )
      }
      else
      {
          return this.CompForms.filter((a) =>
                        (a.Sites_FullMatchCount > fullMatchLow && a.Sites_FullMatchCount < fullMatchHigh)
                    &&  (a.Sites_PartialMatchCount > partialMatchLow && a.Sites_PartialMatchCount < partialMatchHigh)
                    &&  (a.NameToSearch.toLowerCase().includes(this.nameToSearch))
                    )
      }
      
      
   }
  
  OnActivate(RecId : string){
    console.log("recid :  " + RecId);
    this.service.getActivateComplianceForm(RecId)
            .subscribe((item: any) => {
               this.isActive = item;
               if(this.isActive==true){
                 this.LoadReportResults();
               }
            },
            error => {

            });
  }
  
  OnDelete(RecId : string){

      this.service.getDeleteComplianceForm(RecId)
            .subscribe((item: any) => {
              this.CompForms = item;
              //  this.IsActive = item;
            },
            error => {

            });


  }
   get diagnostic() { return JSON.stringify(this.fullMatch); }
   
}