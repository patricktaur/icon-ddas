
import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/Rx';




import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';

import {SiteEnum} from "./data-extractor.classes"

@Injectable()
export class DataExtractorService {
    _baseUrl: string = '';
    _controller: string = 'DataExtractor/';
    _options: RequestOptions;

    constructor(private http: Http,
        private configService: ConfigService,
        private authService: AuthService
    ) {
        this._baseUrl = configService.getApiURI() + this._controller;
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append("Authorization", "Bearer " + this.authService.token);
        this._options = new RequestOptions({ headers: headers });
    }

    executeDataExtractor(siteEnum : SiteEnum) {
        return this.http.get(this._baseUrl + 'ExtractDataFromSingleSite?siteEnum=' + siteEnum,
            this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getLatestExtractionStatus(){
        return this.http.get(this._baseUrl + 'GetLatestExtractionStatus',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getDataExtractionErrorSiteCount(){
        return this.http.get(this._baseUrl + 'GetDataExtractionErrorSiteCount',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }
    

    private handleError(error: any) {
        var applicationError = error.headers.get('Application-Error');
        var serverError = error.json();
        var modelStateErrors: string = '';
        console.log("serverError:" +JSON.stringify(serverError));
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