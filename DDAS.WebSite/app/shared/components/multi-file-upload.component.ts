//source: https://scotch.io/tutorials/angular-file-uploads-with-an-express-backend
//import component, ElementRef, input and the oninit method from angular core
import { Component, OnInit, ElementRef, Input } from '@angular/core';
//import the native angular http and respone libraries
//import { Http, Response, RequestOptions } from '@angular/http';
import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';

//import the do function to be used with the http library.
import "rxjs/add/operator/do";
//import the map function to be used with the http library
import "rxjs/add/operator/map";
import { AuthService } from '../../auth/auth.service';
//const URL = 'http://localhost:8000/api/upload';

//create the component properties
@Component({
    //define the element to be selected from the html structure.
    selector: 'multi-file-upload',
    //location of our template rather than writing inline templates.
    template: `
   
    <input id="photo" 
    type="file" 
    (change)="fileChange($event)"/>
    
    
    <table class="table table-bordered table-hover table-striped">
		<thead>
			<tr>
				<th >File Name </th>
				<th >Remove</th>
				
			</tr>
		</thead>

		<tbody>
			<tr *ngFor="let file of Files" >
                <td >
                    {{file.name}}
				</td>
                <td >
                <button type="button" class="btn btn-sm btn-primary" (click)="Remove(file);"
                 title="Remove">
                <span class="glyphicon glyphicon-remove"></span>
            </button>

				</td>
			</tr>
		</tbody>
	</table>

    `,
})
export class MultiFileUploadComponent implements OnInit {
    //<button type="button" class="btn btn-default" (click)="upload(); ">Test Upload</button>
    //@Input('UploadUrl') uploadUrl:string ="http://localhost:56846/api/QC/RequestQC1";
    @Input('Files') Files: File[] = [];
    _options: RequestOptions;
    private formData:FormData = new FormData();
    //private files : File[] = [];
    ngOnInit() {
        
    }
    //declare a constroctur, so we can pass in some properties to the class, which can be    //accessed using the this variable
    constructor(private http: Http, 
        private el: ElementRef,
        private authService: AuthService
    ) {
        let headers = new Headers();

        //headers.append('Accept', 'application/json');
        //headers.append('Content-Type', 'multipart/form-data');
        headers.append("Authorization", "Bearer " + this.authService.token);
        this._options = new RequestOptions({ headers: headers });
        
    }
    // //the function which handles the file upload without using a plugin.
    // upload() {
    // //locate the file element meant for the file upload.
    //     let inputEl: HTMLInputElement = this.el.nativeElement.querySelector('#photo');
    // //get the total amount of files attached to the file input.
    //     let fileCount: number = inputEl.files.length;
    // //create a new fromdata instance
    //     let formData = new FormData();
    // //check if the filecount is greater than zero, to be sure a file was selected.
    //     if (fileCount > 0) { // a file was selected
    //         //append the key name 'photo' with the first file in the element
    //             formData.append('photo', inputEl.files.item(0));
    //         //call the angular http method
    //         this.http
    //         //post the form data to the url defined above and map the response. Then subscribe //to initiate the post. if you don't subscribe, angular wont post.
    //             .post(this.uploadUrl, formData, this._options).map((res:Response) => res.json()).subscribe(
    //             //map the success function and alert the response
    //              (success) => {
    //                      alert(success._body);
    //             },
    //             (error) => alert(error))
    //     }
    // }


    fileChange(event) {
        
        let fileList: FileList = event.target.files;
        if(fileList.length > 0) {
            let file: File = fileList[0];
            this.formData.append('uploadFile', file, file.name);
            this.Files.push(file);
        }
    }

    Remove(file: any){
        var index = this.Files.indexOf(file);
        if (index > -1) {
            this.Files.splice(index, 1);
        }
    }
    get diagnostic() { return JSON.stringify(this.formData.getAll); }

}