import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {
    PrincipalInvestigatorDetails, 
    // CompFormFilter,
    CalenderDate, ReviewStatusEnum,
    UndoEnum, CurrentReviewStatusViewModel, ReviewerRoleEnum
} from '../../search/search.classes'; 

import {CompFormArchiveFilter} from '../archive.classes';
import { SearchService } from '../../search/search-service';
import { ConfigService } from '../../shared/utils/config.service';  '../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { IMyDate, IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';
import { CompFormLogicService } from '../../search/shared/services/comp-form-logic.service';
// import {ArchvService} from '../archv-service';
import {ComplianceFormArchiveService} from '../comp-form-archive-service';

@Component({
    moduleId: module.id,
    templateUrl: 'comp-form-archv.component.html',
})
export class CompFormArchiveComponent implements OnInit {

    public PrincipalInvestigators: PrincipalInvestigatorDetails[];
    public Users: any[];
    public ComplianceFormFilter: CompFormArchiveFilter;

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
    public ArchivedOnFromDate :  IMyDateModel;
    public ArchivedOnToDate : IMyDateModel;

    public formLoading: boolean;
    public currentReviewStatus: CurrentReviewStatusViewModel;
    public undoQCSubmit: boolean;
    public undoQCResponse: boolean;
    public undoCompleted: boolean;
    public recId: string;
    public pageNumber: number = 1;
    public undoComment: string = "";
    public exportToiSprintResult: string = "";

    public archivedForms : any[];
    public undoArchiveResult: string;
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
        this.ComplianceFormFilter = new CompFormArchiveFilter;
        this.SetDefaultFilterValues();
         this.LoadPrincipalInvestigators();
        // this.LoadRecords();
        this.LoadUsers();
    }

    LoadUsers() {
        this.compFormArchService.getPrincipalInvestigators()
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

        var today = new Date();
        this.ArchivedOnFromDate = {
            date: {
                year: fromDay.getFullYear(), month: fromDay.getMonth() + 1, day: fromDay.getDate()
            },
            jsdate: '',
            formatted: '',
            epoc: null
        }

        
        this.ArchivedOnToDate = {
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


        this.ComplianceFormFilter.ArchivedOnFrom = null;
        if (this.ArchivedOnFromDate.date) {
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            this.ComplianceFormFilter.ArchivedOnFrom = new Date(this.ArchivedOnFromDate.date.year, this.ArchivedOnFromDate.date.month - 1, this.ArchivedOnFromDate.date.day + 1);
        }
        this.ComplianceFormFilter.ArchivedOnTo = null;
        if (this.ArchivedOnToDate.date) {
            this.ComplianceFormFilter.ArchivedOnTo = new Date(this.ArchivedOnToDate.date.year, this.ArchivedOnToDate.date.month - 1, this.ArchivedOnToDate.date.day + 1);
        }
         
        this.compFormArchService.getPrincipalInvestigatorsWithReviewDateFilters(this.ComplianceFormFilter)
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

    
    undoArchive(item: PrincipalInvestigatorDetails){
        this.undoArchiveResult = "Processing ....";
        let recId = item.RecId;
        this.compFormArchService.undoArchive(recId)
            .subscribe((result: any) => {
                this.undoArchiveResult = result;
                this.LoadPrincipalInvestigators();
            });
    }

    undoQC() {
        let undoEnum = 0;

        if(this.undoQCSubmit)
            undoEnum = UndoEnum.UndoQCSubmit;
        else if(this.undoQCResponse)
            undoEnum = UndoEnum.UndoQCResponse;
        else if(this.undoCompleted)
            undoEnum = UndoEnum.UndoCompleted;

        if(undoEnum == UndoEnum.UndoQCSubmit || undoEnum == UndoEnum.UndoQCResponse ||
            undoEnum == UndoEnum.UndoCompleted) {
            this.service.undo(this.recId, undoEnum, this.undoComment)
            .subscribe((item: boolean) => {
                this.LoadPrincipalInvestigators();
            },
            error => {

            });
        }
        else
            alert('undo request not sent');
    }

    exportToiSprint(complianceFormId: string){
            this.service.exportToiSprint(complianceFormId)
            .subscribe((item: string) => {
                if(item.indexOf("Failed") > -1){
                    alert(item);
                }
                this.exportToiSprintResult = item;
                this.LoadPrincipalInvestigators();
            },
            error => {
                this.exportToiSprintResult = "failed to export data to iSprint";
            });
    }

    get diagnostic() { return JSON.stringify(this.PrincipalInvestigators); }

}