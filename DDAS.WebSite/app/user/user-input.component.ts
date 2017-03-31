import { Component } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { UserService } from './user-service';
//import { User, Role } from './user.classes';
import {UserViewModel} from './user.classes';
import { AuthService }      from '../auth/auth.service';

@Component({
    moduleId: module.id,
    selector: 'User-input',
    templateUrl: 'user-input.component.html',
})
export class UserInputComponent {

    public user: UserViewModel;
    private userId: string;

     private processing: boolean;
     public isNew: boolean = false;
    public isNewText: string = "";
    public LoggedInUserIsAppAdmin: boolean;
    constructor(
        private router: Router, private service: UserService, 
        private route: ActivatedRoute, private authService: AuthService) { }
    
    ngOnInit() {
        this.user = new UserViewModel;
       
        this.route.params.forEach((params: Params) => {
            this.userId = params['userid'];
             this.isNew = false;
             this.isNewText = "Edit";
            if (this.userId == ""){
                this.isNew = true;
                this.isNewText = "New";
            }
            this.LoadUser();
            this.LoggedInUserIsAppAdmin = this.authService.isAppAdmin;
        });
        
    }

    LoadUser() {
        this.processing = true;

        this.service.getUser(this.userId)
            .subscribe((item) => {
                this.processing = false;
                this.user = item;
 
            },
            error => {
                this.processing = false;
            });
    }

   

    Save() {
        //this.UpdateUserRoles();
        this.service.saveUser(this.user)
            .subscribe((item: any) => {
                this.router.navigate(["/users"]);
            },
            error => {

            });

    }

    CancelSave() {
        this.router.navigate(["/users"]);
    }


    get diagnostic() { return JSON.stringify(this.user); }

}