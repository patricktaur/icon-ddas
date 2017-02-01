import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {loggedinuserService} from './loggedinuser.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'my-loginhistory.component.html',
})
export class myLoginHistoryComponent implements OnInit {
    public myLoginHistoryDetails: any[];
    public loggedInUserName: string = '';

    public pageNumber: number;

       public myDatePickerOptions = {
        
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
     };
     public FromDate: IMyDateModel;
    public ToDate: IMyDateModel;  
    constructor(
        private service: loggedinuserService
    ) { }

    ngOnInit(){
        this.loggedInUserName = this.service.loggedInUserName;
        this.setDefaultFilters();
        this.LoadLoginHistory();
    }

    setDefaultFilters(){
        var fromDay = new Date();
        fromDay.setDate(fromDay.getDate() - 10);
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
        let frm:Date;
        let to: Date;
        if (this.FromDate != null){
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            frm = new Date(this.FromDate.date.year, this.FromDate.date.month-1,  this.FromDate.date.day+1);
        }

        if (this.ToDate != null){
              to = new Date(this.ToDate.date.year, this.ToDate.date.month-1,  this.ToDate.date.day+1);
        }

        this.service.getMyLoginHistory(this.loggedInUserName, frm, to)
        .subscribe((item: any[]) => {
            console.log("item :" + item);
            this.myLoginHistoryDetails = item;
        });
    }
}