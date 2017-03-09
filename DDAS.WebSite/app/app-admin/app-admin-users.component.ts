import { Component, OnInit } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import {AppAdminService} from './app-admin.service'

@Component({
    moduleId: module.id,
    templateUrl: 'app-admin-users.component.html',
})
export class AppAdminUsersComponent implements OnInit {
   
   constructor(
        private service: AppAdminService
    ) { }
    
    ngOnInit(){
       
    }


}