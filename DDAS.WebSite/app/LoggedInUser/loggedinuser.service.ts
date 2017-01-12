import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { Pagination, PaginatedResult } from '../shared/interfaces';
import {
    User, Role
} 
from './loggedinuser.classes';

import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class loggedinuserService {
    _baseUrl: string = '';
    //_controller: string = 'search/'; 
    _controller: string = '';
    _options: RequestOptions;

    constructor(private http: Http,
        private configService: ConfigService,
        private authService: AuthService
        ) {
        this._baseUrl = configService.getApiURI() + this._controller;
          let headers = new Headers();
            headers.append('Content-Type', 'application/json');
            headers.append("Authorization","Bearer " + this.authService.token);
            this._options = new RequestOptions({headers: headers});
    }

    getAdminList(){
                return this.http.get(this._baseUrl + 'Account/GetAdminList', this._options)
            .map((res: Response) => {
                return res.json();
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