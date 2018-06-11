//source: https://scotch.io/tutorials/angular-file-uploads-with-an-express-backend
//import component, ElementRef, input and the oninit method from angular core
import { Component, OnInit, ElementRef, Input, EventEmitter, HostListener } from '@angular/core';

import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';

// //import the do function to be used with the http library.
// import "rxjs/add/operator/do";
// //import the map function to be used with the http library
// import "rxjs/add/operator/map";

@Component({
    //define the element to be selected from the html structure.
    selector: 'multi-file-select',
    styles:[
        `
        .error{ color: #f00; }
        .dragarea{
            font-size: 24px;
            border: 3px solid #bbb; 
            padding: 20px ;
            background-color: #fff;
            color: #bbb;
        }
        .droparea{
            font-size: 24px;
            border: 3px dashed #bbb; 
            padding: 20px ;
            background-color: #eff;
            color: #aaa;
        }
        `],
    //location of our template rather than writing inline templates.
    template: `
        
        
    
        <div draggable="true" ngClass="{{dragAreaClass}}">
            <div class="row">
                <div class="col-md-12 text-center">

                    <a href="javascript:void(0)" (click)="file.click()">
                        Click to browse
                    </a> Or Drag & Drop to upload the Attachment

                    <input type="file" #file  (change)="fileChange($event)" style="display:none" multiple/>

                </div>
            </div>

        </div>
        <div class="row">
        </div>
        <div class="row error " *ngIf="errors.length> 0">
            <div class="col-md-12 text-center">
                <ul>
                    <li *ngFor="let err of errors">{{err}}</li>
                </ul>
            </div>
        </div>

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
            <!--<tr>
                <td colspan="2">
                <span class="glyphicon glyphicon-warning-sign">  When files are uploaded the filenames longer than 50 characters will be shortened.</span>
                </td>
            </tr>-->
		</tbody>
	</table>
    `,
})
export class MultiFileSelectComponent implements OnInit {
    @Input('Files') Files: File[] = [];
    public errors: Array<string> =[];
    _options: RequestOptions;
    dragAreaClass: string = 'dragarea';
    ngOnInit() {
        
    }
    
    constructor(private http: Http, 
        private el: ElementRef,
    ) { }
    

    fileChange(event: any) {
        
        // let fileList: FileList = event.target.files;
        // if(fileList.length > 0) {
            
        //     let file: File = fileList[0];
        //     this.Files.push(file);
        // }
        
        let files = [].slice.call(event.target.files);
        files.map((f : any) =>  this.Files.push(f));
    }

    Remove(file: any){
        var index = this.Files.indexOf(file);
        if (index > -1) {
            this.Files.splice(index, 1);
        }
    }

    @HostListener('dragover', ['$event']) onDragOver(event: any) {
        this.dragAreaClass = "droparea";
        event.preventDefault();
    }

    @HostListener('dragenter', ['$event']) onDragEnter(event: any) {
        this.dragAreaClass = "droparea";
        event.preventDefault();
    }

    @HostListener('dragend', ['$event']) onDragEnd(event: any) {
        this.dragAreaClass = "dragarea";
        event.preventDefault();
    }

    @HostListener('dragleave', ['$event']) onDragLeave(event: any) {
        this.dragAreaClass = "dragarea";
        event.preventDefault();
    }

    @HostListener('drop', ['$event']) onDrop(event: any) {   
        this.dragAreaClass = "dragarea";           
        event.preventDefault();
        event.stopPropagation();
        
        // var fileList = event.dataTransfer.files;
        // if(fileList.length > 0) {
        //     let file: File = fileList[0];
        //     //this.formData.append('uploadFile', file, file.name);
        //     this.Files.push(file);
        // }
        
        let files = [].slice.call(event.dataTransfer.files);
        files.map((f : any) => this.Files.push(f));
    }
    //get diagnostic() { return JSON.stringify(this.Files); }

}