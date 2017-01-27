import { Component} from '@angular/core';
import { Router }   from '@angular/router';
import {UserService} from './user-service';
import {User, Role} from './user.classes';

import {UserViewModel} from './user.classes';

import { ActivatedRoute, Params } from '@angular/router';
import { ConfigService } from '../shared/utils/config.service';
@Component({
  moduleId: module.id,
  selector: 'User',
  templateUrl: 'user.component.html',
})
export class UserComponent {
 
    public Users: UserViewModel[];
    private userIdToDelete: string;
    public userNameToDelete: string;
    public passwordReset: boolean; // temp until email is ready.

    constructor(private router: Router, private route: ActivatedRoute,private service:UserService){}
    
    ngOnInit(){
        this.LoadUsers();
        this.passwordReset = false;
    }
    
    LoadUsers()
    {
        this.service.getUsers()
        .subscribe((item: any) => {
            this.Users = item;
         },
        error => {
        });
    }
 
    OpenForEdit(userId : string){
         this.router.navigate(['user-input', userId], { relativeTo: this.route.parent});
    }

    AddNew(){
        this.router.navigate(['user-input', ""], { relativeTo: this.route.parent});
    }

    setUserToDelete(user: UserViewModel){
        this.userIdToDelete= user.UserId;
        this.userNameToDelete = user.UserName;

   }

    DeleteUser(){ 
        this.service.deleteUser(this.userIdToDelete)
            .subscribe((item: any) => {
              this.LoadUsers();
            },
            error => {

            });
   }

   ResetPassword(userId: string){
       this.passwordReset = false;
       this.service.resetPassword(userId)
            .subscribe((item: any) => {
              this.passwordReset = item;
            },
            error => {

            });
   }
    
   UserRoles(roles: Role[]): string{
       let retRoles: string = "";
       let comma: string = ""

        for (let role of roles) {
           retRoles += comma + role.Name;
           comma = ", ";
         }
       return retRoles;
   }
   
    get diagnostic() { return JSON.stringify(this.Users); }
        
}