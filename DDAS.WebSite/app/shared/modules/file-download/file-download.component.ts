import { Component, Input, Output, OnInit, EventEmitter, } from '@angular/core';
import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ConfigService } from '../../../shared/utils/config.service';
import { AuthService } from '../../../auth/auth.service';

@Component({ 
    selector: '[file-download]',
    template: `<button class="btn btn-primary" (click)="Download()" title="Download ..."><span class="glyphicon glyphicon-download-alt" > </span></button>
    `,
    

 })
export class FileDownloadComponent implements OnInit {
    
     @Input('url') url:string = '';
     @Input() filter: any;
     @Output() ButtonClicked = new EventEmitter();
     
     _baseUrl: string = '';
     constructor(private http: Http,
        private configService: ConfigService,
        private authService: AuthService
    ) {
        this._baseUrl = configService.getApiURI();
    }
     
     ngOnInit() {
        

     }

    public Download(){
        this.ButtonClicked.emit();
        
        if (this.filter){
            this.downLoadFileByPost(this.url, this.filter)
            .subscribe(
                (response: any) =>  {}),
                (error: any) => console.log("Error downloading the file."),
                () => console.info("OK");
        }else{
            this.downLoadFileByGet(this.url)
            .subscribe(
                (response: any) =>  {}),
                (error:any) => console.log("Error downloading the file."),
                () => console.info("OK");
        }
    }
    
     downLoadFileByGet(url: string) {
        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');
        headers.append("Accept", "text/csv" );
        let file = {};
        return this.http.get(this._baseUrl + url,
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                this.saveFile(res);
                
            });
            //.catch(this.handleError);
    }
    
    downLoadFileByPost(url: string, filter:any) {
        let headers = new Headers();
        headers.append("Authorization", "Bearer " + this.authService.token);
        headers.append('Content-Type', 'application/json');
        
        let filter1 = JSON.stringify(filter);
        return this.http.post(this._baseUrl + url, filter1,
            { headers: headers, responseType: ResponseContentType.ArrayBuffer })
            .map((res: Response) => {
                this.saveFile(res);
            });
            //.catch(this.handleError);

    }
    
    saveFile(res: Response){
        let file = {};
        file = new Blob([res.arrayBuffer()], {
            type: 'application/ms-word'
        });
        // console.log("Inside Save File");
        // console.log("res.headers.get('Filename')" + res.headers.get('Filename'));

        //header 'Browser' in the response is not read by Microsoft 'Edge'. Not sure why
        //hence the work around of 'split with space'!
        var browser = res.headers.get('Browser');
        var fileNameHeader = res.headers.get('Filename');
        var fileName = fileNameHeader.split(' ')[0].trim();
        // var browser = res.headers.get('Browser');
        var browser = fileNameHeader.split(' ')[1].trim();
        
        //var browser = "chrome";
        //var fileName = "test.xlsx";

        // console.log("FileName: " + fileName);
        if (browser.toLowerCase() == "edge" ||
            browser.toLowerCase() == "ie") {
            window.navigator.msSaveBlob(file, fileName);
        }

        if (browser.toLowerCase() == "chrome") {
            var anchor = document.createElement("a");
            anchor.download = fileName;
            anchor.text = fileName;
            anchor.href = window.URL.createObjectURL(file);
            anchor.click();
        }
        if (browser.toLowerCase() == "unknown") {
            alert("could not identify the browser. File download failed");
        }
        if (browser == null) {
            //...
        }
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
