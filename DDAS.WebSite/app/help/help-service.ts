import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';
//Grab everything with import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { ConfigService } from '../shared/utils/config.service';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class HelpService {
    _baseUrl: string = '';
    _controller: string = '';
    _options: RequestOptions;

    constructor(private http: Http,
        private configService: ConfigService,
        private authService: AuthService) {
        this._baseUrl = configService.getApiURI() + this._controller;
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append("Authorization", "Bearer " + this.authService.token);
        this._options = new RequestOptions({ headers: headers });
    }

    downloadUserManual() {
        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');

        let file = {};
        return this.http.get(this._baseUrl + 'Reports/UserManual',
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                //return res.json();
                file = new Blob([res.arrayBuffer()], {
                    type: 'application/pdf'
                });
                var fileNameHeader = res.headers.get('Filename');
                var fileName = fileNameHeader.split(' ')[0].trim();
                
                //header 'Browser' in the response is not read by Microsoft 'Edge'. Not sure why
                //hence the work around of 'split with space'!
                // var browser = res.headers.get('Browser');
                var browser = fileNameHeader.split(' ')[1].trim();

                console.log("Filename header: " + fileNameHeader);
                console.log("File Name: " + fileName);
                console.log("Browser: " + browser);

                if (browser.toLowerCase() == "edge" ||
                    browser.toLowerCase() == "ie") {
                    window.navigator.msSaveBlob(file, fileName);
                }

                if (browser.toLowerCase() == "chrome") {
                    console.log('Identified browser: ', browser);
                    var anchor = document.createElement("a");
                    anchor.download = fileName;
                    anchor.text = fileName;
                    anchor.href = window.URL.createObjectURL(file, fileName);
                    anchor.click();
                }
                if (browser.toLowerCase() == "unknown") {
                    alert("could not identify the browser. File download failed");
                }
                if (browser == null) {
                    //...
                }
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