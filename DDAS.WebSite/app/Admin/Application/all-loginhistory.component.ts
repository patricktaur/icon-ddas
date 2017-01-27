import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {LoginHistoryService} from './all-loginhistory.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'all-loginhistory.component.html',
})
export class LoginHistoryComponent implements OnInit {
    public loginHistoryDetails: any[];

    public pageNumber: number;
    constructor(
        private service: LoginHistoryService
    ) { }

    ngOnInit(){
        console.log("***ngOnInit***");
        this.LoadLoginHistory();
    }

    LoadLoginHistory(){
        this.service.getLoginHistory()
        .subscribe((item: any[]) => {
            console.log("item :" + item);
            this.loginHistoryDetails = item;
        });
    }
}