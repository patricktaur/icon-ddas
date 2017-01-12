import { Component } from '@angular/core';
import { Router, Params, ActivatedRoute } from '@angular/router';
import { UserService } from './user-service';
//import { User, Role } from './user.classes';
import {UserViewModel} from './user.classes';

@Component({
    moduleId: module.id,
    selector: 'User-input',
    templateUrl: 'user-input.component.html',
})
export class UserInputComponent {

    public user: UserViewModel;
    private userId: string;

    //public allRoles: Role[];

    private processing: boolean;

    constructor(private router: Router, private service: UserService, private route: ActivatedRoute) { }
    ngOnInit() {
        this.user = new UserViewModel;
        //this.LoadAllRoles();
        this.route.params.forEach((params: Params) => {
            this.userId = params['userid'];

            this.LoadUser();

        });

    }

    LoadUser() {
        this.processing = true;

        this.service.getUser(this.userId)
            .subscribe((item) => {
                this.processing = false;
                this.user = item;
                //this.MapFromUserRolesToRoles();

            },
            error => {
                this.processing = false;
            });
    }

    // LoadAllRoles() {
    //     this.processing = true;

    //     this.service.getAllRoles()
    //         .subscribe((item) => {
    //             this.processing = false;
    //             this.allRoles = item;

    //         },
    //         error => {
    //             this.processing = false;
    //         });
    // }

    // MapFromUserRolesToRoles() {

    //     if (this.allRoles != null) {
    //         for (let item of this.allRoles) {

    //             let role = this.user.Roles.find(x => x.Name == item.Name);

    //             if (role != null) {
    //                 item.Selected = true;
    //             }
    //         }
    //     }

    // }

    // UpdateUserRoles() {

    //     //No method found to selectively delete roles.
    //     //Deleting all roles and add currently selected ones.
    //     this.user.Roles = [];
    //     if (this.allRoles != null) {
    //         for (let item of this.allRoles) {
    //             if (item.Selected == true) {
    //                 this.user.Roles.push(item)
    //             }
    //         }
    //     }

    // }

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