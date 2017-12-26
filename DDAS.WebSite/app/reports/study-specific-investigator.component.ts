import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportService } from './report-service';
import { ReportFilterViewModel } from './report.classes';
import { IMyDate, IMyDateModel, IMyInputFieldChanged } from '../shared/utils/my-date-picker/interfaces';

@Component({
    moduleId: module.id,
    templateUrl: 'study-specific-investigator.component.html'
})

export class StudySpecificInvestigatorComponent {
    public generating: boolean = false;

    public myDatePickerOptions = {
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
    };
    public FromSelDate: string;  //default calendar start dates
    public ToSelDate: string;//default calendar start dates
    public FromDate: IMyDateModel;// Object = { date: { year: 2018, month: 10, day: 9 } };
    public ToDate: IMyDateModel;

    public studySpecificInvestigators: any[];
    public pageNumber: number;
    public formLoading: boolean;
    public projectNumber: string;
    public reportFilter: ReportFilterViewModel;

    constructor(
        private service: ReportService,
        private route: ActivatedRoute) {

    }

    ngOnInit() {
        this.reportFilter = new ReportFilterViewModel;
        this.SetDefaultFilterValues();
    }
    
    SetDefaultFilterValues() {

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

    onDateChanged(event: IMyDateModel) {
        this.generating = false;
    }

    getInvestigators(){
        if(this.reportFilter.ProjectNumber == null || this.reportFilter.ProjectNumber == ""){
            alert('Please enter a valid project number');
            return;
        }

        this.formLoading = true;
        this.reportFilter.FromDate = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day + 1);
        this.reportFilter.ToDate = new Date(this.ToDate.date.year, this.ToDate.date.month - 1, this.ToDate.date.day + 1);        
        this.service.getStudySpecificInvestigators(this.reportFilter)
            .subscribe((item: any[]) => {
                this.studySpecificInvestigators = item;
                this.formLoading = false;
            },
            error => {
                this.formLoading = false;
            });
    }

    diagnostic(){
        return JSON.stringify(this.studySpecificInvestigators);
    }
}