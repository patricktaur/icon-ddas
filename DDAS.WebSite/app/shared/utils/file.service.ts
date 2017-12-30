import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, ResponseContentType  } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { AuthService } from '../../auth/auth.service';
//import { AuthService } from './auth.service';
import { ConfigService } from '../../shared/utils/config.service';
 //npm install file-saver
 //import * as FileSaver from 'file-saver';
 //import {saveAs} from 'file-saver';
 //import {saveAs} from 'browser-filesaver';
//import { window } from 'rxjs/operator/window';
 //import {saveAs as importedSaveAs} from 'file-saver';

@Injectable()
export class FileService {
    _options: RequestOptions;
    _baseUrl: string = ''
    
    constructor(private http: Http,
        private configService: ConfigService,
        private authService: AuthService
    ) {
        this._baseUrl = configService.getApiURI() ;
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append("Authorization", "Bearer " + this.authService.token);
        this._options = new RequestOptions({ headers: headers });

     }

    public download(url:string) {
        //let saveAs = require('file-saver');
        
 
        let headers = new Headers({ 'Content-Type': 'text/csv' });

        //const authToken = 'Bearer ' + this.authService.accessToken;
        const authToken = 'Bearer ' + this.authService.token;
        //headers.append('Authorization', authToken);
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append("Accept", "text/csv" );
        
        let options = new RequestOptions({ headers: headers });
        //console.log("in download:" + );
        //this.http.post(this._baseUrl + url, "", {headers:headers, responseType: ResponseContentType.Blob })
        // this.http.get(this._baseUrl + url, {headers:headers, responseType: ResponseContentType.Blob })
        // //this.http.get(this._baseUrl + url, options)
        // .subscribe(
        // (response: any) =>  {
        //     console.log("in response"  );

            
        //      let filename = "report.csv"
        //      console.log("fileName:" + filename);
        //      //saveAs(blob, filename);
            
        //      let blob = response.blob();
        //      var anchor = document.createElement("a");
        //      anchor.download = filename;
        //      anchor.text = filename;
        //      anchor.href = window.URL.createObjectURL(blob);
        //      anchor.click();
            
        // });

        this.http.get(this._baseUrl + url, {headers:headers, responseType: ResponseContentType.Blob })
        //this.http.get(this._baseUrl + url, options)
        .subscribe(
        (response: any) =>  this.downloadFile(response)),
        (error:any) => console.log("Error downloading the file."),
        () => console.info("OK");
    }

    downloadFile(response: any){
        // var blob = new Blob([data], { type: 'text/csv' });
        // var url= window.URL.createObjectURL(blob);
        // window.open(url);
        console.log("Inside downloadFile");
        let filename = "report.csv"
        let blob = response.blob();

        // let file = new Blob([response.arrayBuffer()], {
        //     type: 'text/csv'
        // });
        
        var anchor = document.createElement("a");
        anchor.download = filename;
        anchor.text = filename;
        anchor.href = window.URL.createObjectURL(blob);
        anchor.click();
        
        //window.navigator.msSaveBlob(blob);
        

       //saveAs(blob, filename);
       
    }

}
