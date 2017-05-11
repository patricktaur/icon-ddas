import { Component, OnInit }        from '@angular/core';
import { Router } from '@angular/router';
import { AuthService }      from './auth.service';
import { ChangePasswordBindingModel }      from './auth.classes';
import {UserService} from '../user/user-service';
@Component({

  moduleId: module.id,
  templateUrl: 'change-password.component.html' 
})
export class ChangePasswordComponent {
    changePasswordModel:   ChangePasswordBindingModel;
    error: string;
    result: boolean;
    
  constructor(private authSerive: AuthService , private service:UserService) {
     this.changePasswordModel = new ChangePasswordBindingModel;
    
  }

    changepassword(){
         
         //Validate:
         this.error = "";
         if (this.changePasswordModel.NewPassword != this.changePasswordModel.ConfirmPassword){
             this.error = "New Password and Confirm New Password do not match";
         }
         
         
         if (!this.error){
             this.service.changePassword(this.changePasswordModel)
            .subscribe((item: any) => {
               this.result = item;
            },
            error => {
                this.error = "Password change not successful";
            });
         }
 
 }

}


