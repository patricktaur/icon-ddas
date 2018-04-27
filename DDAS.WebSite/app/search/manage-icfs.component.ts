import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { PrincipalInvestigatorDetails, ComplianceFormManage, CompFormFilter } from './search.classes';
import { SearchService } from './search-service';
import { ConfigService } from '../shared/utils/config.service';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../auth/auth.service';
import { IMyDate, IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { CompFormLogicService } from './shared/services/comp-form-logic.service'
//import { Http, Response, He   aders , RequestOptions } from '@angular/http';

@Component({
    moduleId: module.id,
    templateUrl: 'manage-icfs.component.html',
})
export class ManageICFsComponent implements OnInit {

    public PrincipalInvestigators: PrincipalInvestigatorDetails[];
    //public ComplianceFormIdToDelete: string;
    //public InvestigatorNameToDelete: string;
    //public ComplianceFormIdToManage: string;

    //public Active: boolean;
    public AssignedTo: string = "";
    public AssignedFrom: string;
    public SelectedInvestigatorName: string;
    public SelectedComplianceFormId: string;
    public LoggedInUserIsAppAdmin: boolean;
    public filterStatus: number = -1;
    public filterInvestigatorName: string = "";
    public ComplianceFormFilter: CompFormFilter;
    public Users: any[];
    public FromDate: IMyDateModel;
    public ToDate: IMyDateModel;
    public pageNumber: number = 1;
    public loading: boolean;
    
    public myDatePickerOptions = {
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
    };
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private service: SearchService,
        private configService: ConfigService,
        private authService: AuthService,
        private compFormLogicService: CompFormLogicService
    ) { }

    ngOnInit() {
        this.route.params.forEach((params: Params) => {
            let page = + params['page'];
            if (page != null) {
                this.pageNumber = page;
            }
            this.LoggedInUserIsAppAdmin = this.authService.isAppAdmin;
        });
        this.ComplianceFormFilter = new CompFormFilter;
        this.SetDefaultFilterValues();
        this.LoadPrincipalInvestigators();
        this.LoadUsers();
    }

    SetDefaultFilterValues() {
        this.ComplianceFormFilter.InvestigatorName = null;
        this.ComplianceFormFilter.ProjectNumber = null;
        this.ComplianceFormFilter.SponsorProtocolNumber = null;
        this.ComplianceFormFilter.SearchedOnFrom = null;
        this.ComplianceFormFilter.SearchedOnTo = null;
        this.ComplianceFormFilter.Country = null;
        this.ComplianceFormFilter.AssignedTo = "-1";
        this.ComplianceFormFilter.Status = -1;

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
    }

    canReassignOrClearReassignment(currentReviewStatus: number) {
        return this.compFormLogicService.canReassignOrClearReassignment(currentReviewStatus);
    }

    get getLoggedInUserFullName() {
        return this.compFormLogicService.getUserFullName();
    }

    LoadUsers() {
        this.loading = true;
        this.service.getAllUsers()
            .subscribe((item: any[]) => {
                this.Users = item;
                this.loading = false;
            },
            error => {
                this.loading = false;
            });
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

        this.loading = true;
        this.service.getPrincipalInvestigatorsByFilters(this.ComplianceFormFilter)
            .subscribe((item: any) => {
                this.PrincipalInvestigators = item;
                this.loading = false;
            },
            error => {
                this.loading = false;
            });
    }

    get filteredRecords() {
        let filter1: PrincipalInvestigatorDetails[];

        filter1 = this.PrincipalInvestigators;
        if (this.filterInvestigatorName.length > 0) {
            filter1 = this.PrincipalInvestigators.filter((a) =>
                a.Name.toLowerCase().includes(this.filterInvestigatorName.toLowerCase()));
        }

        let filter2: PrincipalInvestigatorDetails[];
        filter2 = filter1;
        if (this.filterStatus >= 0) {
            filter2 = filter1.filter((a) =>
                a.StatusEnum == this.filterStatus);
        }
        return filter2;
    }

    setComplianceFormIdToDelete(inv: PrincipalInvestigatorDetails) {
        //this.ComplianceFormIdToDelete = inv.RecId;
        this.SelectedInvestigatorName = inv.Name;
        this.SelectedComplianceFormId = inv.RecId;
    }

    DeleteComplianceForm() {
        //CompFormId to be set by the delete button
        this.service.deleteComplianceForm(this.SelectedComplianceFormId)
            .subscribe((item: any) => {
                this.LoadPrincipalInvestigators();
            },
            error => {

            });
    }

    setSelectedRecordDetails(Investigator: PrincipalInvestigatorDetails) {
        this.SelectedComplianceFormId = Investigator.RecId;
        this.SelectedInvestigatorName = Investigator.Name;
    }

    setComplianceFormToManage(Investigator: PrincipalInvestigatorDetails) {

        this.setSelectedRecordDetails(Investigator);

        //this.AssignedTo Bound to drop down user list:
        this.AssignedFrom = Investigator.AssignedTo + " ";
    }

    assignToEnabled() {

        if (this.AssignedTo) {
            return true;
        } else {
            return false;
        }
    }

    setComplianceFormToClear(Investigator: PrincipalInvestigatorDetails) {

        this.setSelectedRecordDetails(Investigator);
        //this.AssignedTo Bound to drop down user list:
        // 
        //From existing Value, for concurrency check on server:
        this.AssignedFrom = Investigator.AssignedTo + " ";

    }


    manageComplianceForm() {
        this.service.SaveAssignedTo(this.AssignedTo, this.AssignedFrom, this.SelectedComplianceFormId)
            .subscribe((item: boolean) => {
                this.LoadPrincipalInvestigators();
            },
            error => {
            });
    }



    ClearAssignedTo() {
        this.service.ClearAssignedTo(this.SelectedComplianceFormId, this.AssignedFrom)
            .subscribe((item: boolean) => {
                this.LoadPrincipalInvestigators();
            },
            error => {
            });
    }

    OpenForEdit(DataItem: PrincipalInvestigatorDetails) {
        //this.router.navigate(['complianceform', DataItem.RecId, {rootPath:'manage-compliance-forms', page:this.pageNumber}], { relativeTo: this.route });
        this.router.navigate(['comp-form-edit', DataItem.RecId, { rootPath: 'manage-compliance-forms', page: this.pageNumber }], { relativeTo: this.route });
    }

    generateComplianceForm(formId: string) {
        this.service.generateComplianceForm(formId)
            .subscribe((item: any) => {

            });
    }

    get diagnostic() { return JSON.stringify(this.PrincipalInvestigators); }
}