import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportService } from './report-service';
import { InvestigatorFindingViewModel, ReportFilterViewModel } from './report.classes';
import { IMyDate, IMyDateModel, IMyInputFieldChanged } from '../shared/utils/my-date-picker/interfaces';

@Component({
    moduleId: module.id,
    templateUrl: 'investigator-findings.component.html'
})

export class InvestigatorFindingsComponent {
    public generating: boolean = false;

    public myDatePickerOptions = {
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
    };
    public FromSelDate: string;  //default calendar start dates
    public ToSelDate: string;//default calendar start dates
    public FromDate: IMyDateModel;// Object = { date: { year: 2018, month: 10, day: 9 } };
    public ToDate: IMyDateModel;

    public investigatorByFinding: InvestigatorFindingViewModel[];
    public filterValue: number;
    public pageNumber: number;
    public formLoading: boolean;
    public reportFilter: ReportFilterViewModel;
    public users: any[];
    public assignedTo: string;

    constructor(
        private service: ReportService,
        private route: ActivatedRoute) {

    }

    ngOnInit() {
        this.formLoading = true;

        this.reportFilter = new ReportFilterViewModel;
        this.SetDefaultFilterValues();
        this.LoadUsers();
        this.getInvestigatorByFinding();

    }

    SetDefaultFilterValues() {

        this.reportFilter.AssignedTo = "All";
        this.reportFilter.ProjectNumber = null;
        this.filterValue = -1;
        
        var fromDay = new Date();

        fromDay.setDate(fromDay.getDate() - 30);

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

    LoadUsers() {
        this.service.getAllUsers()
            .subscribe((item: any[]) => {
                this.users = item;
            });
    }
    
    getInvestigatorByFinding() {
        this.ResetReportFilter();
        this.service.getInvestigatorsByFinding(this.reportFilter)
            .subscribe((item: InvestigatorFindingViewModel[]) => {
                this.investigatorByFinding = item;
                this.formLoading = false;
                this.filterValue = -1;
            },
            error => {
                this.formLoading = false;
            });
    }

    onDateChanged(event: IMyDateModel) {
        this.generating = false;
    }

    ResetReportFilter(){
        this.reportFilter.FromDate = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day + 1);
        this.reportFilter.ToDate = new Date(this.ToDate.date.year, this.ToDate.date.month - 1, this.ToDate.date.day + 1);
    }
    
    get filterRecords() {
        if (this.filterValue == -1)
            return this.investigatorByFinding;
        else if (this.filterValue == 0)
            return this.investigatorByFinding.filter(x => x.FindingObservation == null);
        else if (this.filterValue == 1)
            return this.investigatorByFinding.filter(x => x.FindingObservation != null);
    }

    diagnostic() {
        return JSON.stringify(this.investigatorByFinding);
    }
}