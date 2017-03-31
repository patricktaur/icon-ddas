import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { PrincipalInvestigatorDetails, CompFormFilter, CalenderDate } from './search.classes';
import { SearchService } from './search-service';
import { ConfigService } from '../shared/utils/config.service';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../auth/auth.service';

import { IMyDate, IMyDateModel } from '../shared/utils/my-date-picker/interfaces';

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
        
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
     };
    public FromSelDate: string ;  //default calendar start dates
    public ToSelDate: string ;//default calendar start dates
    public FromDate: IMyDateModel;// Object = { date: { year: 2018, month: 10, day: 9 } };
    public ToDate: IMyDateModel;  // Object = { date: { year: 2018, month: 10, day: 9 } };
    
    public p: number;
    public formLoading: boolean;
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private service: SearchService,
        private configService: ConfigService,
        private authService: AuthService
    ) {

    }

    
    ngOnInit() {

        this.ComplianceFormFilter = new CompFormFilter;
        this.SetDefaultFilterValues();
        this.LoadPrincipalInvestigators();
    }

    SetDefaultFilterValues() {
        this.ComplianceFormFilter.InvestigatorName = null;
        this.ComplianceFormFilter.ProjectNumber = null;
        this.ComplianceFormFilter.SponsorProtocolNumber = null;
        
     var fromDay = new Date();
 
        fromDay.setDate(fromDay.getDate() - 30);
  
      
    this.FromDate = {
            date:{ year:fromDay.getFullYear(), month:fromDay.getMonth()+1, day:fromDay.getDate()
                
            },
            jsdate : '',
            formatted: '',
            epoc:null
 
        }

    
   
        var today = new Date();
    this.ToDate = {
            date:{
            year:today.getFullYear(), month:today.getMonth()+1, day:today.getDate()
            },
            jsdate : '',
            formatted: '',
            epoc:null
        }

    this.ComplianceFormFilter.Country = null;
    this.ComplianceFormFilter.Status = -1;

    }

    LoadPrincipalInvestigators() {
 
        if (this.FromDate != null){
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            this.ComplianceFormFilter.SearchedOnFrom = new Date(this.FromDate.date.year, this.FromDate.date.month-1,  this.FromDate.date.day+1);
        }

        if (this.ToDate != null){
              this.ComplianceFormFilter.SearchedOnTo = new Date(this.ToDate.date.year, this.ToDate.date.month-1,  this.ToDate.date.day+1);
        }
        
        this.service.getPrincipalInvestigatorsByFilters(this.ComplianceFormFilter)
            .subscribe((item: any) => {
              
                this.PrincipalInvestigators = item;
            });
    }

    setSelectedRecordDetails(Investigator: PrincipalInvestigatorDetails) {
        this.SelectedComplianceFormId = Investigator.RecId;
        this.SelectedInvestigatorName = Investigator.Name;
    }

    
    private Todate = new Date(); 
    private testDate: Date;
    dateChanged(event: Date){
            this.testDate = event;
    }

    get diagnostic() { return JSON.stringify(this.FromDate); }
   

}