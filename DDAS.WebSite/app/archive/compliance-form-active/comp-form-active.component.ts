import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {
    PrincipalInvestigatorDetails, CompFormFilter,
    CalenderDate, ReviewStatusEnum,
    UndoEnum, CurrentReviewStatusViewModel, ReviewerRoleEnum
} from '../../search/search.classes'; 
import { SearchService } from '../../search/search-service';
import { ConfigService } from '../../shared/utils/config.service';  '../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { IMyDate, IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';
import { CompFormLogicService } from '../../search/shared/services/comp-form-logic.service';

import {ComplianceFormArchiveService} from '../comp-form-archive-service';

@Component({
    moduleId: module.id,
    templateUrl: 'comp-form-active.component.html',
})
export class CompFormActiveComponent implements OnInit {

    public PrincipalInvestigators: PrincipalInvestigatorDetails[];
    public Users: any[];
    public ComplianceFormFilter: CompFormFilter;

    public myDatePickerOptions = {
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
    };
    public FromSelDate: string;  //default calendar start dates
    public ToSelDate: string;//default calendar start dates
    public FromDate: IMyDateModel;// Object = { date: { year: 2018, month: 10, day: 9 } };
    public ToDate: IMyDateModel;  // Object = { date: { year: 2018, month: 10, day: 9 } };
    public ReviewCompletedOnFromDate :  IMyDateModel;
    public ReviewCompletedOnToDate : IMyDateModel;
    public formLoading: boolean;
    public currentReviewStatus: CurrentReviewStatusViewModel;
    public undoQCSubmit: boolean;
    public undoQCResponse: boolean;
    public undoCompleted: boolean;
    public recId: string;
    public pageNumber: number = 1;
    public undoComment: string = "";
    public exportToiSprintResult: string = "";

    // public archivedForms : any[];
    public archiveType: string = "review";
    public archiveSearchOlderThan: number = 365;
    public archiveLimit: number = 100;
    public archiveRecordCountResult: string;

    public archiveResult: string;
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private service: SearchService,
        private configService: ConfigService,
        private authService: AuthService,
        private compFormLogic: CompFormLogicService,
        // private archvService: ArchvService,
        private compFormArchService : ComplianceFormArchiveService
        ) {
    }

    ngOnInit() {
        this.exportToiSprintResult = "";
        this.ComplianceFormFilter = new CompFormFilter;
        this.SetDefaultFilterValues();
        this.LoadPrincipalInvestigators();
        // this.LoadRecords();
        this.LoadUsers();
    }

    LoadUsers() {
        this.service.getPrincipalInvestigators()
            .subscribe((item: any[]) => {
                this.Users = item;
            });
    }

    SetDefaultFilterValues() {
        this.ComplianceFormFilter.InvestigatorName = null;
        this.ComplianceFormFilter.ProjectNumber = null;
        this.ComplianceFormFilter.SponsorProtocolNumber = null;
        this.ComplianceFormFilter.AssignedTo = "-1";
        this.ComplianceFormFilter.Country = null;
        this.ComplianceFormFilter.Status = -1;
        this.ComplianceFormFilter.SearchedOnFrom = null;
        this.ComplianceFormFilter.SearchedOnTo = null;

        var fromDay = new Date();
        fromDay.setDate(fromDay.getDate() - (365 * 2));

        this.FromDate = {
            date: {
                year: fromDay.getFullYear(), month: fromDay.getMonth() + 1, day: fromDay.getDate()
            },
            jsdate: '',
            formatted: '',
            epoc: null
        }

        var today = new Date();
        today.setDate(today.getDate() - (365));
        this.ToDate = {
            date: {
                year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate()
            },
            jsdate: '',
            formatted: '',
            epoc: null
        }

        this.ReviewCompletedOnFromDate = {
            date: {
                year: fromDay.getFullYear(), month: fromDay.getMonth() + 1, day: fromDay.getDate()
            },
            jsdate: '',
            formatted: '',
            epoc: null
        }

        
        this.ReviewCompletedOnToDate = {
            date: {
                year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate()
            },
            jsdate: '',
            formatted: '',
            epoc: null
        }

        this.ComplianceFormFilter.Country = null;
        this.ComplianceFormFilter.Status = -1;
    }

    LoadRecords(){
        this.service.getPrincipalInvestigators()
            .subscribe((item: any) => {
                this.PrincipalInvestigators = item;
                

            });
    }
    
    LoadPrincipalInvestigators() {
        this.ComplianceFormFilter.SearchedOnFrom = null;
        if (this.FromDate.date) {
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            this.ComplianceFormFilter.SearchedOnFrom = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day + 1);
        }
        this.ComplianceFormFilter.SearchedOnTo = null;
        if (this.ToDate.date) {
            this.ComplianceFormFilter.SearchedOnTo = new Date(this.ToDate.date.year, this.ToDate.date.month - 1, this.ToDate.date.day + 1);
        }

        this.ComplianceFormFilter.ReviewCompletedOnFrom = null;
        if (this.ReviewCompletedOnFromDate.date) {
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            this.ComplianceFormFilter.ReviewCompletedOnFrom = new Date(this.ReviewCompletedOnFromDate.date.year, this.ReviewCompletedOnFromDate.date.month - 1, this.ReviewCompletedOnFromDate.date.day + 1);
        }
        this.ComplianceFormFilter.ReviewCompletedOnTo = null;
        if (this.ReviewCompletedOnToDate.date) {
            this.ComplianceFormFilter.ReviewCompletedOnTo = new Date(this.ReviewCompletedOnToDate.date.year, this.ReviewCompletedOnToDate.date.month - 1, this.ReviewCompletedOnToDate.date.day + 1);
        }
         
    // getPrincipalInvestigatorsWithReviewDateFilters(Filters: CompFormFilter): Observable<PrincipalInvestigatorDetails[]> {
        //
        this.service.getPrincipalInvestigatorsWithReviewDateFilters(this.ComplianceFormFilter)

        // this.service.getPrincipalInvestigatorsByFilters(this.ComplianceFormFilter)
            .subscribe((item: any) => {
                this.PrincipalInvestigators = item;
            });
            


    }

    get filteredRecords() {
        
        return this.PrincipalInvestigators;
        // if (this.PrincipalInvestigators) {
        //     return this.PrincipalInvestigators.filter(x =>
        //         x.CurrentReviewStatus == ReviewStatusEnum.Completed &&
        //         x.AssignedTo.toLowerCase() == this.authService.userName.toLowerCase());
        // }
        // else
        //     return null;
    }

    recordCount(){
        if (this.PrincipalInvestigators){
            return this.PrincipalInvestigators.length;
        }else{
            return 0;
        }
    }
    openICSF(compFormId: string){
        this.router.navigate(['comp-form-edit', compFormId, { rootPath: 'completed-icsf', page: this.pageNumber }], { relativeTo: this.route });
    }

    // canUndoQC(item: PrincipalInvestigatorDetails){
    //     if(item.QCVerifier && item.QCVerifier.toLowerCase() == this.authService.userName.toLowerCase() &&
    //         item.UndoQCSubmit)
    //         return true;
    //     else if(item.Reviewer.toLowerCase() == this.authService.userName.toLowerCase() &&
    //         item.UndoQCResponse)
    //         return true;
    //     else if(item.Reviewer.toLowerCase() == this.authService.userName.toLowerCase() &&
    //         item.UndoCompleted)
    //         return true;
    //     else
    //         return false;
    // }

    // getUndoAction(){
    //     if(this.undoQCSubmit)
    //         return "QC Submit";
    //     else if(this.undoQCResponse)
    //         return "QC Corrections";
    // }

    setSelectedRecord(UndoQCSubmit: boolean, UndoQCResponse: boolean, UndoCompleted: boolean, RecId: string){
        this.undoQCSubmit = UndoQCSubmit;
        this.undoQCResponse = UndoQCResponse;
        this.undoCompleted = UndoCompleted;
        this.recId = RecId;
    }

    
    ArchiveCompFormsRecordCount(){
        this.archiveRecordCountResult = "Processing ....";
        this.compFormArchService.archiveCompFormsRecordCount(this.archiveSearchOlderThan,  this.archiveType)
            .subscribe((result: any) => {
                this.archiveRecordCountResult = "Records found to archive:" + result;
                
            });
    }
    
    ArchiveSearchOnIsOlderThan(){
        this.archiveResult = "Processing ....";
        this.compFormArchService.archiveComplianceFormWithSearchedOnGreaterthan(this.archiveSearchOlderThan, this.archiveLimit, this.archiveType)
            .subscribe((result: any) => {
                this.archiveResult = result;
                this.LoadPrincipalInvestigators();
            });
    }

    

    get diagnostic() { return JSON.stringify(this.PrincipalInvestigators); }

}