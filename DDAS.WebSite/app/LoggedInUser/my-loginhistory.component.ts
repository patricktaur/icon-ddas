import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {loggedinuserService} from './loggedinuser.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'my-loginhistory.component.html',
})
export class myLoginHistoryComponent implements OnInit {
    public myLoginHistoryDetails: any[];
    public loggedInUserName: string = '';

    constructor(
        private service: loggedinuserService
    ) { }

    ngOnInit(){
        this.loggedInUserName = this.service.loggedInUserName;
        this.LoadLoginHistory();
    }

    LoadLoginHistory(){
        this.service.getMyLoginHistory(this.loggedInUserName)
        .subscribe((item: any[]) => {
            console.log("item :" + item);
            this.myLoginHistoryDetails = item;
        });
    }
}