import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { LoginHistoryService } from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';
import { SiteSourceViewModel } from './appAdmin.classes';
import { ApiKey } from './appAdmin.classes';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'sam-api-key-edit.component.html',
})

export class SamApiKeyEditComponent implements OnInit {
    public samApiKey: ApiKey = new ApiKey;

    public ValidTill: IMyDateModel;
    public saveMessage : string;

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
        this.LoadKey();
    }

    
    
    LoadKey(){
        this.service.getSamApiKey()
        .subscribe((item: ApiKey) => {
             this.samApiKey = item;
            this.setValidTillDate(item.ValidTill);
        });        
    }

    
    Save() {
        // this.reportFilter.FromDate = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day + 1);
        this.samApiKey.ValidTill = new Date(this.ValidTill.date.year, this.ValidTill.date.month - 1, this.ValidTill.date.day) ;
        this.service.saveSamApiKey(this.samApiKey)
            .subscribe((item: any) => {
                let date: Date = new Date(); 
                this.saveMessage ="- Saved,  " + date;
                //this.router.navigate(["/country-site"]);
            },
            error => {
                let date: Date = new Date(); 
                this.saveMessage ="- Error.Could not Save, " + date;
            });
    }

  
    setValidTillDate(validTill : any){
        
        validTill = new Date(validTill);
        this.ValidTill = {
            date: {
                year: validTill.getFullYear(), month: validTill.getMonth() + 1, day: validTill.getDate()
            },
            jsdate: '',
            formatted: '',
            epoc: null
        }
    }    
    
    onChange(newValue : any) {
        this.saveMessage = "";
    }
    get diagnostic() { return JSON.stringify(this.samApiKey); }
}
