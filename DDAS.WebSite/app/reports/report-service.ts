import { Injectable } from '@angular/core';
import { Http, Response, Headers , RequestOptions, ResponseContentType } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import {Observer} from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { ComplianceForm} from '../search//search.classes';
import {
    CompFormFilter,
} from '../search//search.classes';

import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';


@Injectable()
export class ReportService {
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
   
    
    getPrincipalInvestigators() {

        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this.http.get(this._baseUrl + 'search/GetPrincipalInvestigators', this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
    getDeleteComplianceForm(RecId:string){
             return this.http.get(this._baseUrl + 'Reports/DeleteComplianceForm?ComplianceFormId=' +RecId, this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }
    
    //   generateOutputFile(){
    //     let headers = new Headers();
    //     headers.append('Content-Type', 'application/json');

    //     return this.http.get(this._baseUrl + 'Reports/GenerateOutputFile', this._options)
    //         .map((res: Response) => {
    //             return res.json();
    //         })
    //         .catch(
    //             this.handleError
    //         );        
    // }
     
    generateOutputFile(Filters: CompFormFilter){
        let Filter1 = JSON.stringify(Filters);
        // let headers = new Headers();
        // headers.append('Content-Type', 'application/json');

        // return this.http.post(this._baseUrl + 'Reports/GenerateOutputFile', Filter1, this._options)
        //     .map((res: Response) => {
        //         return res.json();
        //     })
        //     .catch(
        //         this.handleError
        //     );

        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');
        
        let file = {};
        return this.http.post(this._baseUrl + 'Reports/GenerateOutputFile', Filter1,
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                file = new Blob([res.arrayBuffer()], {
                    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                });
                var filename = res.headers.get('FileName');
                console.log("Downloaded filename: " + filename);
                var anchor = document.createElement("a");
                anchor.download = filename;
                anchor.href = window.URL.createObjectURL(file);
                anchor.click();
                //window.open(window.URL.createObjectURL(file));
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