import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'extraction-log.component.html',
})

export class ExtractionLogComponent implements OnInit {
    public extractionLog: any[];
    public pageNumber: number;
    public formLoading: boolean;
    public message: string;

    public FromDate: IMyDateModel;
    public ToDate: IMyDateModel;  
    public siteFilter : string = "";
    public stepFilter: string = "Final";
    public statusFilter : string =  "Error";

    public deleteFilesOlderThan : number = 30;
    public deletedResponse : string = "";

    public myDatePickerOptions = {
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
    };

    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.message = "";
        this.setDefaultFilters();
        this.loadExtractionLog();
    }

    loadExtractionLog(){
        this.formLoading = false;
        this.service.getExtractionLog()
        .subscribe((item : any[]) => {
            this.extractionLog = item;
            this.formLoading = true;
            console.log("done");
        },
        error => {
            // this.formLoading = false;
        });
    }

    setDefaultFilters(){
        var fromDay = new Date();
        fromDay.setDate(fromDay.getDate() - 3);
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

    }

    get filterRecords() {
        if (!this.extractionLog){
            return;
        }
    
        let filter1 = this.extractionLog;
        if (this.siteFilter && this.siteFilter.length > 0) {
          filter1 = null;
          filter1 = this.extractionLog.filter((n: any) => this.siteFilter.indexOf(n.SiteEnumString) != -1);
        }
 
    
        let filter2 = filter1;
        if (this.statusFilter && this.statusFilter.length > 0) {
          filter2 = null;
          filter2 = filter1.filter((n: any) => this.statusFilter.indexOf(n.Status) != -1);
        }
        
        let filter3 = filter2;
        if (this.FromDate.date) {
          filter3 = null;
        //   let dt = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day - 2);
          let dt = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day);
         
          filter3 = filter2.filter((n: any) =>  new Date(n.CreatedOn) >= dt);
        }

        let filter4 = filter3;
        if (this.ToDate.date) {
          filter4 = null;
          let dt = new Date(this.ToDate.date.year, this.ToDate.date.month - 1, this.ToDate.date.day +1);
         
          
          filter4 = filter3.filter((n: any) =>  new Date(n.CreatedOn) < dt);
        }
        
        return filter4;
       
      }

}