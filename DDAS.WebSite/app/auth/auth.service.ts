import { Injectable } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/delay';
import { Http, Headers, Response,  RequestOptions } from '@angular/http';
import { ConfigService } from '../shared/utils/config.service';
import { ChangePasswordBindingModel }      from './auth.classes';
import { Component } from '@angular/core';
import { Router }   from '@angular/router';
//temp:
//import {SiteInfo,StudyNumbers,SearchSummaryItem,SearchSummary,NameSearch, SearchResultSaveData} from '../search/search.classes';


@Injectable()
export class AuthService {
  isLoggedIn: boolean = false;
  public token: string;
  public userName: string;
  
  public isAdmin: boolean;
  public isUser: boolean;
  public roles: string = ""; //comma separarted

  // store the URL so we can redirect after logging in
  redirectUrl: string;
  _baseUrl: string = '';

   constructor(private http: Http, private configService: ConfigService, private router: Router) {
        // set token if saved in local storage
        var currentUser = JSON.parse(localStorage.getItem('currentUser'));
        //this.token = currentUser && currentUser.token;
        this._baseUrl = configService.getApiHost();
    }
  

  // login(username:string, password:string) {
  //   return Observable.of(true).delay(100).do(val => this.isLoggedIn = true);
  // }

//Working Code:
 
 
 login(username:string, password:string) {
    
    this.token = null;
    this.isLoggedIn = false;
    
    var body = "grant_type=password&username=" +  username + "&password=" + password;
        let headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded' });
        let options = new RequestOptions({ headers: headers });
        return this.http.post(this._baseUrl + 'token', body, options )
            .map((response: Response) => {
                // login successful if there's a jwt token in the response
                this.isLoggedIn = true;
                 this.token = response.json().access_token;
                 //console.log("token: " + this.token)
                
                //let token = response.json() && response.json().access_token;
                //let token = response.json().access_token;
                this.userName = response.json().userName;
                
                //refactor code to handle dynamic addition of roles:
                let comma : string = ""
                this.roles = "";
                if (response.json().admin == null){
                    this.isAdmin = false;
                }
                else{
                    this.isAdmin = true;
                    this.roles = this.roles + comma + "Admin"
                    comma = ", "
                }
                
                if (response.json().user == null){
                    this.isUser = false;
                }
                else{
                    this.isUser = true;
                     this.roles = this.roles + comma + "User"
                     comma = ", "
                }
       
                
            })
            //.catch(this.handleError);
 
  }

   
  changePassword(changePassword: ChangePasswordBindingModel): Observable<Boolean> {
        let body = JSON.stringify(changePassword);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        headers.append("Authorization","Bearer " + this.token);
        let options = new RequestOptions({ headers: headers });
 
        return this.http.post(this._baseUrl + 'Account/SetPassword', body, options)
            .map((res: Response) => {
                console.log("success");
                return res.json();
            })
            .catch(this.handleError);
    }

  logout() {
   
        let headers = new Headers({ 'Content-Type': 'application/json' });
        headers.append("Authorization","Bearer " + this.token);
        let options = new RequestOptions({ headers: headers });
         return this.http.post(this.configService.getApiURI() + 'Account/Logout', null, options )
            .map((response: Response) => {
                  this.token = null;
                  this.isLoggedIn = false;
                  //this.router.navigate(['/']); //not working in prod
                  //window.location.reload();
              })
            .catch(this.handleError);
   
  }

  private handleError(error: any) {
   
        var applicationError = error.headers.get('Application-Error');
        var serverError = error.json();
        var modelStateErrors: string = '';
           
        if (!serverError.type) {
            
          
            for (var key in serverError) {
                if (serverError[key])
                    modelStateErrors += serverError[key] + '\n';
            }
        }

        modelStateErrors = modelStateErrors = '' ? null : modelStateErrors;
        console.log("serverError" + serverError);
        return Observable.throw(applicationError || modelStateErrors || 'Server error');
    }






}


