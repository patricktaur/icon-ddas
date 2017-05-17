import { Injectable, OnInit } from '@angular/core';
import { Http, Response, Headers , RequestOptions } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import {Observer} from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import {Role,namelist,User} from './user.classes';
import {UserViewModel} from './user.classes';

import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';
import { ChangePasswordBindingModel }      from '../auth/auth.classes';
@Injectable()
export class UserService {
    _baseUrl: string = '';
    //_controller: string = 'search/'; 
    _controller: string = ''; 
    _options: RequestOptions;

    constructor(private http: Http,
        private configService: ConfigService,
        private authService: AuthService
        ) { 

             this._baseUrl = this.configService.getApiURI() + this._controller;
             let headers = new Headers();
            headers.append('Content-Type', 'application/json');
            headers.append("Authorization","Bearer " + this.authService.token);
            this._options = new RequestOptions({headers: headers});
        }
  
    
    getUsers(): Observable<UserViewModel[]>{
       
        return this.http.get(this._baseUrl + 'Account/GetUsers', this._options )
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
  
    getUser(userId: string): Observable<UserViewModel>{
        
        return this.http.get(this._baseUrl + 'Account/GetUser?UserId=' + userId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
    // getAllRoles(): Observable<Role[]>{
    //     return this.http.get(this._baseUrl + 'Account/GetAllRoles' )
    //         .map((res: Response) => {
    //             return res.json();
    //         })
    //         .catch(this.handleError);
    // }
    
    
    saveUser(user:UserViewModel): Observable<UserViewModel>{
        let body = JSON.stringify(user);
        // let headers = new Headers({ 'Content-Type': 'application/json' });
        // let options = new RequestOptions({ headers: headers });
 
        return this.http.post(this._baseUrl + 'Account/SaveUser/', body, this._options)
                   .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
   changePassword(changePassword: ChangePasswordBindingModel): Observable<Boolean> {
        let body = JSON.stringify(changePassword);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        headers.append("Authorization","Bearer " + this.authService.token);
        let options = new RequestOptions({ headers: headers });

        return this.http.post(this._baseUrl + 'Account/SetPassword', body, options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
   
     resetPassword(userId: string): Observable<string>{
        
        return this.http.get(this._baseUrl + 'Account/ResetPassword?UserId=' + userId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
    
     deleteUser(userId:string): Observable<boolean>{
         return this.http.delete(this._baseUrl + 'Account/DeleteUser?UserId=' + userId, this._options)
            .map((res: Response) => {
                return true;
            })
            .catch(this.handleError);
    }
    
 
     
     
     private handleError(error: any) {
        var applicationError = error.headers.get('Application-Error');
        var serverError = error.json();
        var modelStateErrors: string = '';
           
        if (!serverError.type) {
            
            console.log(serverError);
            for (var key in serverError) {
                if (serverError[key])
                    modelStateErrors += serverError[key] + '\n';
            }
        }

        modelStateErrors = modelStateErrors = '' ? null : modelStateErrors;

        return Observable.throw(applicationError || modelStateErrors || 'Server error');
    }
}