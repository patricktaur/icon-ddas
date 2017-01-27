import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { Role, User, UserViewModel } from './loggedinuser.classes';
import {loggedinuserService} from './loggedinuser.service';

@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'about-ddas.component.html',
})
export class AboutDDASComponent implements OnInit {
    public Admins: any[];

    constructor(
        private service: loggedinuserService
    ) { }

    ngOnInit(){
        console.log("***ngOnInit***");
        //this.Admins = [1,2,3,4,5];
        this.LoadAdmins();
    }

    LoadAdmins(){
        this.service.getAdminList()
        .subscribe((item: any[]) => {
            console.log("item :" + item);
            this.Admins = item;
        });
    }
}

