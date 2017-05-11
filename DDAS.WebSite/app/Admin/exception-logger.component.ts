import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';
import {  IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
import { ConfigService } from '../shared/utils/config.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'exception-logger.component.html',
})

export class ExceptionLogComponent implements OnInit {
    public exceptionLogs: any[];
    public pageNumber: number;
    public formLoading: boolean;
    public message: string;

    constructor(
        private service: LoginHistoryService,
        private configService: ConfigService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(){
        this.message = "";
        this.loadExceptionLogs();
    }

    loadExceptionLogs(){
        this.formLoading = false;
        this.service.getExceptionLogs()
        .subscribe((item : any[]) => {
            this.exceptionLogs = item;
            this.formLoading = true;
            console.log("done");
        },
        error => {
            // this.formLoading = false;
        });
    }
}
