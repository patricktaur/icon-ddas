import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'all-loginhistory.component.html',
})
export class LoginHistoryComponent implements OnInit {
    public loginHistoryDetails: any[];

    public pageNumber: number;
    public formLoading: boolean;
    
    public myDatePickerOptions = {
        
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
     };
     public FromDate: IMyDateModel;
    public ToDate: IMyDateModel;  
    constructor(
        private service: LoginHistoryService
    ) { }

    ngOnInit(){
         this.setDefaultFilters();
        this.LoadLoginHistory();
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

    LoadLoginHistory(){
         let from: Date;
        let to: Date;
        this.formLoading = true;
        if (this.FromDate != null){
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            from = new Date(this.FromDate.date.year, this.FromDate.date.month-1,  this.FromDate.date.day+1);
        }

        if (this.ToDate != null){
              to = new Date(this.ToDate.date.year, this.ToDate.date.month-1,  this.ToDate.date.day+1);
        }

        this.service.getLoginHistory(from, to)
        .subscribe((item: any[]) => {
            console.log("item :" + item);
            this.loginHistoryDetails = item;
            this.formLoading = false;
        },
        error => {
            this.formLoading = false;
        });
    }
}