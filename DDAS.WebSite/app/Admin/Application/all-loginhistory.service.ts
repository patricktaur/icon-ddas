import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

//import { Pagination, PaginatedResult } from '../shared/interfaces';

//import { ConfigService } from '../shared/utils/config.service';
//import { AuthService } from '../auth/auth.service';
import { ConfigService } from '../../shared/utils/config.service';
import { AuthService } from '../../auth/auth.service';

@Injectable()
export class LoginHistoryService {
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

    getLoginHistory(from:Date, to: Date){
        
                return this.http.get(this._baseUrl + 'Account/getLogHistory'
                + '?DateFrom=' + from.toISOString().substring(0, 10)
                + '&DateTo=' + to.toISOString().substring(0, 10)
                , this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);      
    }

    getAllErrorImages(){
        return this.http.get(this._baseUrl + 'AppAdmin/GetErrorImages',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    deleteErrorImage(FileName: string){
        return this.http.get(this._baseUrl + 'AppAdmin/DeleteErrorImage?FileName=' + FileName,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    deleteAllErrorImages(){
        return this.http.get(this._baseUrl + 'AppAdmin/DeleteAllErrorImages',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getDataExtractionPerSite(SiteEnum:number){
        return this.http.get(this._baseUrl + 'AppAdmin/GetDataExtractionPerSite?Enum=' + SiteEnum,
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getExtractionHistory(){
        return this.http.get(this._baseUrl + 'AppAdmin/GetDataExtractionHistory',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);
    }

    getErrorImageFolderPath(){
        return this.http.get(this._baseUrl + 'AppAdmin/DownloadErrorImage',
        this._options)
        .map((res: Response) => {
            return res.json();
        })
        .catch(this.handleError);        
    }

    deleteExtractionData(RecId: string, Enum: number){
        return this.http.get(this._baseUrl + 'AppAdmin/DeleteExtractionData?RecId=' + RecId
        + '&Enum=' + Enum,
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