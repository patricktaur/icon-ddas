import { Component, Input, OnInit, NgZone  } from '@angular/core';
import { AuthService } from '../../auth/auth.service';

@Component({ 
    selector: 'file-upload',
    template: `
    Inside file upload component:{{uploadUrl}}
    <label for="fileupload">{{title}}</label>
					<input type="file" id="fileupload" ngFileSelect [options]="basicOptions" (onUpload)="handleUpload($event)" class="form-control"
					 name="Upload">
    `,
    

 })
export class FileUploadComponent implements OnInit {
    public basicOptions: Object;
     @Input('UploadUrl') uploadUrl:string ;
     @Input('Title') title: string;
     
     private zone: NgZone;
     public Loading: boolean = false;
     public validationMessage: string;
     public progress: number = 0;
     constructor(
        private authService: AuthService
    ) { }
     
    ngOnInit() {
        
        this.zone = new NgZone({ enableLongStackTrace: false });
        this.basicOptions = {
            url: this.uploadUrl,
            authToken: this.authService.token,
            authTokenPrefix: 'Bearer'
        };
    }

    handleUpload(data: any): void {
        this.Loading = true;
        this.zone.run(() => {
            //this.response = data.response;
            if (data.response == null) {

            }
            else {
                this.Loading = false;
                
                this.validationMessage = data.response; 
                let ok = "\"ok\"";
                if (this.validationMessage == ok){
                    //this.modal.close();
                    //this.LoadPrincipalInvestigators();
                }
                else{
                    
                }
   
            }

            this.progress = data.progress.percent / 100;
            //this.Loading = false;
        });
    }    
  
    
}