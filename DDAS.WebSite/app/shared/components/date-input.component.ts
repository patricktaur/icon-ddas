import { Component, Directive, Input, Output, EventEmitter, OnInit  } from '@angular/core';
import { FormsModule }    from '@angular/forms';
import { CommonModule } from "@angular/common";
import { IMyDate, IMyDateModel } from '../utils/my-date-picker/interfaces';
//import {IMyOptions} from 'mydatepicker';
import {IMyOptions} from '../utils/my-date-picker';


@Component({ 
    selector: '[date-picker]',
    template: `
    <my-date-picker [options]="myDatePickerOptions" (dateChanged)="onDateChanged($event)" [selDate]="selDate1" ></my-date-picker>
    `,
    

 })
export class DateInputComponent implements OnInit {
    public defaultDatePickerValue: IMyDateModel;
    
     private myDatePickerOptions: IMyOptions = {
        // other options...
        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: '14'
    };

     //@Input('defaultDate') defaultDate: Date;
     //@Output('selectedDate') selectedDate = new EventEmitter();
     
     public selDate1: IMyDate;
     
    
     @Output ('DateValue') outputDate = new EventEmitter();
     
     @Input() DateValue: Date;
     @Output() DateValueChange = new EventEmitter();

     
     ngOnInit() {
         //Convert defaultDate input Date to IMyDate format
         //"2017-03-01T00:00:00+05:30"
         if (this.DateValue != null){
               
                //Error this.DateValue.getdate function, hence this code:
                let dateSection = this.DateValue.toLocaleString().split("T");
                let datePart = dateSection[0];
                let dateParts = datePart.split("-");
                let day = Number.parseInt(dateParts[2]);
                let month = Number.parseInt(dateParts[1]) - 1;
                let year = Number.parseInt(dateParts[0]);
                this.selDate1 = {day:day, month:month+1 , year:year}

                //this.selDate = {day:this.DateValue.getDay(), month:this.DateValue.getMonth()+1 , year:this.DateValue.getFullYear()}
                    

        }
         
     }


   onDateChanged(event: IMyDateModel){
       if (event.date.year< 1900){
           this.DateValueChange.emit(null);
       }
       else
       {
            let selDate = new Date(event.date.year, event.date.month-1, event.date.day);
            this.DateValueChange.emit(selDate);
       }
         
        
}
    
get diagnostic() { return JSON.stringify(this.selDate1); }

}