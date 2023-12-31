import { Component, ViewChild, OnChanges } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { ConfigService } from '../shared/utils/config.service';
import { ReportService } from './report-service';
import { CompFormFilter } from '../search/search.classes';
import { ReportFilters, InvestigationsReport } from './report.classes';
import { IMyDate, IMyDateModel, IMyInputFieldChanged } from '../shared/utils/my-date-picker/interfaces';

@Component({
    moduleId: module.id,
    templateUrl: 'output-report.component.html'
})

export class OutputReportComponent implements OnChanges {
    public myDatePickerOptions = {
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
    };
    public FromSelDate: string;  //default calendar start dates
    public ToSelDate: string;//default calendar start dates
    public FromDate: IMyDateModel;// Object = { date: { year: 2018, month: 10, day: 9 } };
    public ToDate: IMyDateModel;

    public generating: boolean = false;


    public downloadUrl: string;
    public OutputGenerationError: string;

    public ComplianceFormFilter: CompFormFilter;
    public ReportFilter: ReportFilters
    public InvestigationsCompletedReport: InvestigationsReport;
    public reportByUser: any;
    public pageNumber: number;

    constructor(
        private service: ReportService,
        private route: ActivatedRoute,
        private configService: ConfigService,
    ) {

    }

    ngOnInit() {
        this.ComplianceFormFilter = new CompFormFilter;
        this.ReportFilter = new ReportFilters;
        this.SetDefaultFilterValues();
    }

    ngOnChanges(changeRecord: any) {
        this.generating = false;
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

    GenerateOutputFile() {
        if (this.FromDate != null) {
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            this.ComplianceFormFilter.SearchedOnFrom = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day + 1);
        }

        if (this.ToDate != null) {
            this.ComplianceFormFilter.SearchedOnTo = new Date(this.ToDate.date.year, this.ToDate.date.month - 1, this.ToDate.date.day + 1);
        }

        this.generating = true;
        this.downloadUrl = "";
        this.service.generateOutputFile(this.ComplianceFormFilter)
            // .subscribe((item: any) => {
            //     this.downloadUrl = this.configService.getApiHost() + item;
            // },
            // error => {

            // });
            .subscribe((item : any) => {

            },
            error => {
                // console.log("Error downloading the file."),
                //     () => console.log('Completed file download.')
            });
    }

    GetInvestigationsCompleted() {
        if (this.FromDate != null) {
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            this.ComplianceFormFilter.SearchedOnFrom = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day + 1);
        }

        if (this.ToDate != null) {
            this.ComplianceFormFilter.SearchedOnTo = new Date(this.ToDate.date.year, this.ToDate.date.month - 1, this.ToDate.date.day + 1);
        }

        this.ReportFilter.FromDate = this.ComplianceFormFilter.SearchedOnFrom;
        this.ReportFilter.ToDate = this.ComplianceFormFilter.SearchedOnTo;
        this.ReportFilter.ReportPeriodEnum = 2;
        this.ReportFilter.AssignedTo = "user1";
        console.log('ReportFilter - ', this.ReportFilter);
        this.service.getInvestigationsCompletedReport(this.ReportFilter)
        .subscribe((item: InvestigationsReport) => {
            this.InvestigationsCompletedReport = item;
        },
        error => {

        });
    }

    get userName(){
        return this.InvestigationsCompletedReport.ReportByUsers;
    }

    get headers(){
        if(this.InvestigationsCompletedReport != null){
            return this.recordsByUserName[0].ReportItems;
        }
    }

    get recordsByUserName(){
        if(this.InvestigationsCompletedReport != null)
            return this.InvestigationsCompletedReport.ReportByUsers;
        else
            return null;
    }
    
    get diagnostic() { return JSON.stringify(this.InvestigationsCompletedReport); }

}