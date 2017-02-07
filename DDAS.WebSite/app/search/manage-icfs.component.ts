import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { PrincipalInvestigatorDetails, ComplianceFormManage, CompFormFilter } from './search.classes';
import { SearchService } from './search-service';
import { ConfigService } from '../shared/utils/config.service';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../auth/auth.service';

//import { Http, Response, Headers , RequestOptions } from '@angular/http';

@Component({
    moduleId: module.id,
    templateUrl: 'manage-icfs.component.html',
})
export class ManageICFsComponent implements OnInit {

    public PrincipalInvestigators: PrincipalInvestigatorDetails[];
    //public ComplianceFormIdToDelete: string;
    //public InvestigatorNameToDelete: string;
    //public ComplianceFormIdToManage: string;
    
    public Active: boolean;
    public AssignedTo: string;
    public SelectedInvestigatorName: string;
    public SelectedComplianceFormId: string;

    public filterStatus: number = -1;
    public filterInvestigatorName: string = "";
    public ComplianceFormFilter: CompFormFilter;
    public Users: any[];

    public pageNumber: number;
 constructor(
        private route: ActivatedRoute,
        private router: Router,
        private service: SearchService,
        private configService: ConfigService,
        private authService: AuthService
    ) { }
  ngOnInit(){
            
    this.route.params.forEach((params: Params) => {

        let page = +params['page'];
        if (page != null){
            this.pageNumber = page;
        }

    });
      
            
    this.ComplianceFormFilter = new CompFormFilter;
    this.SetDefaultFilterValues();
    this.LoadPrincipalInvestigators();
    this.LoadUsers();

  }
 
  SetDefaultFilterValues()
  {
    this.ComplianceFormFilter.InvestigatorName = null;
    this.ComplianceFormFilter.ProjectNumber = null;
    this.ComplianceFormFilter.SponsorProtocolNumber = null;
    this.ComplianceFormFilter.SearchedOnFrom = null;
    this.ComplianceFormFilter.SearchedOnTo = null;
    this.ComplianceFormFilter.Country = null;
    this.ComplianceFormFilter.Status = -1;
  }

  LoadUsers()
  {
    this.service.getAllUsers()
    .subscribe((item: any[]) => {
       
        this.Users = item;
    });
  }

  LoadPrincipalInvestigators() {
    this.service.getPrincipalInvestigatorsByFilters(this.ComplianceFormFilter)
    .subscribe((item: any) => {
        
        this.PrincipalInvestigators = item;
    });
  }
  
     get filteredRecords() {
       

        let filter1: PrincipalInvestigatorDetails[];

        filter1 = this.PrincipalInvestigators;
        if (this.filterInvestigatorName.length > 0) {
            filter1 =  this.PrincipalInvestigators.filter((a) =>
                a.Name.toLowerCase().includes(this.filterInvestigatorName.toLowerCase())
           )
        }
        
        let filter2: PrincipalInvestigatorDetails[];
        filter2 = filter1;
        if (this.filterStatus >=0){
            filter2 = filter1.filter((a) => 
                a.StatusEnum == this.filterStatus
             )
        }
        return filter2;
    }
 
   
   setComplianceFormIdToDelete(inv: PrincipalInvestigatorDetails){
     //this.ComplianceFormIdToDelete = inv.RecId;
     this.SelectedInvestigatorName = inv.Name;
     this.SelectedComplianceFormId = inv.RecId;
   }
   
   DeleteComplianceForm(){ 
      //CompFormId to be set by the delete button

      this.service.deleteComplianceForm(this.SelectedComplianceFormId)
            .subscribe((item: any) => {
              this.LoadPrincipalInvestigators();
            },
            error => {

            });
   }

 
   setSelectedRecordDetails(Investigator: PrincipalInvestigatorDetails)
   {
       this.SelectedComplianceFormId = Investigator.RecId;
       this.SelectedInvestigatorName = Investigator.Name;
   }

  setComplianceFormToManage(Investigator: PrincipalInvestigatorDetails){
      
      this.setSelectedRecordDetails(Investigator);
      this.AssignedTo = Investigator.AssignedTo;
      this.Active = Investigator.Active;
  }

  manageComplianceForm(){
      
      this.service.SaveAssignedTo(this.AssignedTo, this.Active, this.SelectedComplianceFormId)
        .subscribe((item: boolean) => {
        
        this.LoadPrincipalInvestigators();
        },
        error => {
        });
 
 }

  OpenForEdit(DataItem: PrincipalInvestigatorDetails) {

  
        this.router.navigate(['complianceform', DataItem.RecId, {rootPath:'manage-compliance-forms', page:this.pageNumber}], { relativeTo: this.route });


    }

    get diagnostic() { return JSON.stringify(this.PrincipalInvestigators); }
}