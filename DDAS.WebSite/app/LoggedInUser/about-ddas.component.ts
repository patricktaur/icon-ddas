import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { Role, User, UserViewModel } from './loggedinuser.classes';
import {loggedinuserService} from './loggedinuser.service';
import { ConfigService } from '../shared/utils/config.service';
@Component({
    moduleId: module.id,
    //selector: 'User-input',
    templateUrl: 'about-ddas.component.html',
})
export class AboutDDASComponent implements OnInit {
    public Admins: any[];

    constructor(
        private service: loggedinuserService,
        private configService: ConfigService,
    ) { }

    ngOnInit(){
        //this.Admins = [1,2,3,4,5];
        this.LoadAdmins();
    }

    LoadAdmins(){
        this.service.getAdminList()
        .subscribe((item: any[]) => {
            console.log('admins - ', item);
            this.Admins = item;
        });
    }

    get CurrentVersion(){
        return this.configService.getVer();
    }
}

