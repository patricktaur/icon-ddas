import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {
    PrincipalInvestigatorDetails, CompFormFilter,
    CalenderDate, ReviewStatusEnum,
    UndoEnum, CurrentReviewStatusViewModel, ReviewerRoleEnum
} from './search.classes';
import { SearchService } from './search-service';
import { ConfigService } from '../shared/utils/config.service';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../auth/auth.service';
import { IMyDate, IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { CompFormLogicService } from './shared/services/comp-form-logic.service';

@Component({
    moduleId: module.id,
    templateUrl: 'completed-icsf.component.html',
})
export class CompletedICSFComponent implements OnInit {

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

    public formLoading: boolean;
    public currentReviewStatus: CurrentReviewStatusViewModel;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private service: SearchService,
        private configService: ConfigService,
        private authService: AuthService,
        private compFormLogic: CompFormLogicService) {
    }

    ngOnInit() {
        this.ComplianceFormFilter = new CompFormFilter;
        this.SetDefaultFilterValues();
        this.LoadPrincipalInvestigators();
        this.LoadUsers();
    }

    LoadUsers() {
        this.service.getAllUsers()
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
        fromDay.setDate(fromDay.getDate() - 10);

        this.FromDate = {
            date: {
                year: fromDay.getFullYear(), month: fromDay.getMonth() + 1, day: fromDay.getDate()
            },
            jsdate: '',
            formatted: '',
            epoc: null
        }

        var today = new Date();
        this.ToDate = {
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
        if (this.FromDate != null) {
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            this.ComplianceFormFilter.SearchedOnFrom = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day + 1);
        }

        if (this.ToDate != null) {
            this.ComplianceFormFilter.SearchedOnTo = new Date(this.ToDate.date.year, this.ToDate.date.month - 1, this.ToDate.date.day + 1);
        }

        this.service.getPrincipalInvestigatorsByFilters(this.ComplianceFormFilter)
            .subscribe((item: any) => {
                this.PrincipalInvestigators = item;
            });
    }

    get filteredPrincipalInvestigators() {
        if (this.PrincipalInvestigators) {
            return this.PrincipalInvestigators.filter(x =>
                x.CurrentReviewStatus == ReviewStatusEnum.Completed);
        }
        else
            return null;
    }

    canUndoQC(item: PrincipalInvestigatorDetails){
        if(item.QCVerifier.toLowerCase() == this.authService.userName.toLowerCase() &&
            item.UndoQCSubmit)
            return true;
        else if(item.Reviewer.toLowerCase() == this.authService.userName.toLowerCase() &&
            item.UndoQCResponse)
            return true;
        else
            return false;
    }

    undoQC(undoQCSubmit: boolean, undoQCResponse: boolean, complianceFormId: string) {
        let undoEnum = 0;

        if(undoQCSubmit)
            undoEnum = UndoEnum.UndoQCSubmit;
        else if(undoQCResponse)
            undoEnum = UndoEnum.UndoQCResponse;

        if(undoEnum == UndoEnum.UndoQCSubmit || undoEnum == UndoEnum.UndoQCResponse){
            this.service.undo(complianceFormId, undoEnum)
            .subscribe((item: boolean) => {
                this.LoadPrincipalInvestigators();
            },
            error => {

            });
        }
        else
            alert('undo request not sent');
    }

    get diagnostic() { return JSON.stringify(this.PrincipalInvestigators); }

}