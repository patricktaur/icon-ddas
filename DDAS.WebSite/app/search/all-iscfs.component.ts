import { Component, OnInit, OnDestroy,  ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { PrincipalInvestigatorDetails, CompFormFilter } from './search.classes';
import { SearchService } from './search-service';
import { ConfigService } from '../shared/utils/config.service';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../auth/auth.service';
 

@Component({
    moduleId: module.id,
    templateUrl: 'all-iscfs.component.html',
})
export class AllISCFsComponent implements OnInit {

    public PrincipalInvestigators: PrincipalInvestigatorDetails[];
    
    public Active: boolean;
    public AssignedTo: string;
    public SelectedInvestigatorName: string;
    public SelectedComplianceFormId: string;

    public filterStatus: number = -1;
    public filterInvestigatorName: string = "";
    public ComplianceFormFilter: CompFormFilter;
    public Users: any[];

    public myDatePickerOptions = {
        // other options...
        dateFormat: 'dd mmm yyyy',
    };

    public p:number;
 constructor(
        private route: ActivatedRoute,
        private router: Router,
        private service: SearchService,
        private configService: ConfigService,
        private authService: AuthService
    ) {
        
     }

  onDateChanged(event: any) {
        // event properties are: event.date, event.jsdate, event.formatted and event.epoc
 }
  
  ngOnInit(){
            this.ComplianceFormFilter = new CompFormFilter;
            this.SetDefaultFilterValues();
            this.LoadPrincipalInvestigators();
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

  LoadPrincipalInvestigators() {
    this.service.getPrincipalInvestigatorsByFilters(this.ComplianceFormFilter)
    .subscribe((item: any) => {
        console.log("item :" + item);
        this.PrincipalInvestigators = item;
    });
  }
     
   setSelectedRecordDetails(Investigator: PrincipalInvestigatorDetails)
   {
       this.SelectedComplianceFormId = Investigator.RecId;
       this.SelectedInvestigatorName = Investigator.Name;
   }

    get diagnostic() { return JSON.stringify(this.PrincipalInvestigators); }
}