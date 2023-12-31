import { Injectable } from '@angular/core';
import { Http, Response, Headers, ResponseContentType, RequestOptions } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class AppAdminService {
    _baseUrl: string = '';
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

    LaunchLiveScanner(){
        
            return this.http.get(this._baseUrl + 'AppAdmin/LaunchLiveScanner'
                , this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);      
    }

   
    
    getLiveScannerInfo(){
        
            return this.http.get(this._baseUrl + 'AppAdmin/LiveScannerInfo'
                , this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);      
    }
    KillLiveScanner(){
            return this.http.get(this._baseUrl + 'AppAdmin/KillLiveScanner'
                , this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);      
    }

    getUploadsFolderPath(){
            return this.http.get(this._baseUrl + 'AppAdmin/GetUploadsFolderPath',
            this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getUploadedFiles(){
            return this.http.get(this._baseUrl + 'AppAdmin/GetUploadedFiles',
            this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    // getcberRecords(){
    //         return this.http.get(this._baseUrl + 'AppAdmin/GetCBERRecords',
    //         this._options)
    //         .map((res: Response) => {
    //             return res.json();
    //         })
    //         .catch(this.handleError);
    // }

    getUploadedFile(generatedFileName: string, originalFileName: string){
        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');

        let file = {};
        return this.http.get(this._baseUrl + 'Search/DownloadUploadedFile?GeneratedFileName=' + generatedFileName,
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                // return res.json();
                file = new Blob([res.arrayBuffer()], {
                    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                });

                var browser = res.headers.get('Browser');

                console.log("Original Filename: " + originalFileName);
                console.log("Browser: " + browser);

                if (browser.toLowerCase() == "edge" ||
                    browser.toLowerCase() == "ie") {
                    window.navigator.msSaveBlob(file, originalFileName);
                }

                if (browser.toLowerCase() == "chrome") {
                    var anchor = document.createElement("a");
                    anchor.download = originalFileName;
                    anchor.text = originalFileName;
                    anchor.href = window.URL.createObjectURL(file, originalFileName);
                    anchor.click();
                }
                if (browser.toLowerCase() == "unknown") {
                    alert("could not identify the browser. File download failed");
                }
                if (browser == null) {
                    //...
                }
                ////window.open(window.URL.createObjectURL(file));
            })
            .catch(this.handleError);
    }

    deleteUploadedFile(generatedFileName: string){
            return this.http.get(this._baseUrl + 'AppAdmin/DeleteUploadedFile?GeneratedFileName=' 
            + generatedFileName,
            this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);        
    }

    deleteAllUploadedFiles(){
            return this.http.get(this._baseUrl + 'AppAdmin/DeleteAllUploadedFiles',
            this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);
    }

    getOutputFilePath(){
            return this.http.get(this._baseUrl + 'AppAdmin/GetOutputFilePath',
            this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);        
    }

    getOutputFiles(){
            return this.http.get(this._baseUrl + 'AppAdmin/GetOutputFiles',
            this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);        
    }

    deleteOutputFile(outputFileName: string){
            return this.http.get(this._baseUrl + 'AppAdmin/DeleteOutputFile?OutputFileName=' 
            + outputFileName,
            this._options)
            .map((res: Response) => {
                return res.json();
            })
            .catch(this.handleError);        
    }

    deleteAllOutputFiles(){
            return this.http.get(this._baseUrl + 'AppAdmin/DeleteAllOutputFiles',
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