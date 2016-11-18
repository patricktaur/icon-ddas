import { Component} from '@angular/core';
import { Router }   from '@angular/router';
// import {complianceForms} from './report.classes';
import { ActivatedRoute, Params } from '@angular/router';
import { ComplianceForm,SiteData} from '../search//search.classes';
import {ReportService} from './report-service';

@Component({
  moduleId: module.id,
  template: ""
  //templateUrl: 'report.component.html',
})

export class ReportComponent {
  // private Reportdetails: complianceForms; 

  private CompForms: ComplianceForm[];
  ComplianceFormId:any;
  selectedvalue:any;
  recId:string="";
  // private IsActive : ComplianceForm;
  isActive:boolean;
  public _SiteData: SiteData = new SiteData;
  constructor(private service: ReportService,private route: ActivatedRoute){

  }
  // ngOnInit(){
  //   //  this.route.params.forEach((params: Params) => {
  
  //   //         this._SiteData.RecId = params['formid'];  //RecId = compid
  //   //         // this._SiteData.SiteEnum = params['siteEnum']; 
  //   //         // this._SiteData.SiteName ="FDA Debarred Person List";
  //             this.LoadReportResults();
  //   //     });
  //   // this._SiteData.RecId = "2ca19000-22cb-4bd5-8cd5-826bb79b0806";
  //   // this.OnActivate();
  
  // }
  // LoadReportResults(){
  //   this.service.getNamesFromClosedComplianceForms()
  //           .subscribe((item: any) => {
  //             this.CompForms = item;  
  //           },
  //           error => {

  //           });
  // }
  
  // OnActivate(RecId : string){
  //   console.log("recid :  " + RecId);
  //   this.service.getActivateComplianceForm(RecId)
  //           .subscribe((item: any) => {
  //              this.isActive = item;
  //              if(this.isActive==true){
  //                this.LoadReportResults();
  //              }
  //           },
  //           error => {

  //           });
  // }
  // OnDelete(RecId : string){
  //   this.service.getDeleteComplianceForm(RecId)
  //           .subscribe((item: any) => {
  //             this.CompForms = item;
  //             //  this.IsActive = item;
  //           },
  //           error => {

  //           });

  // }
   get diagnostic() { return JSON.stringify(this.CompForms); }
   get diagnostic1() { return JSON.stringify(this.isActive); }
}