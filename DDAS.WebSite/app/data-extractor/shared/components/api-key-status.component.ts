import { Component, OnInit, Input } from '@angular/core';
import {LoginHistoryService} from '../../../Admin/all-loginhistory.service'
// import {DataExtractorService} from '../../data-extractor-service';

import {ApiKey} from '../../../../app/Admin/appAdmin.classes';
import {StatusActivatorService} from '../service/status.activator.service'
@Component({
    selector: 'api-key-status',
    template: `
        
    <div ><span class="glyphicon glyphicon-time {{color}}" title="{{message}}"></span>
        <span *ngIf="!ShowLinkToEditPage">Api Keys</span>
        <a *ngIf="ShowLinkToEditPage" routerLink="/sam-api-key-edit"  routerLinkActive="active"> - Manage Api Keys</a>
        </div>
    `,
    styles:[
        `
        .green {
            color: green;
        }
        .red {
            color: red;
        }
        .yellow {
            color: white;
            background-color: yellow;
        }
        .badge {
            color: #fff;
            background-color:#FF0000;
            position:relative;
            top: -15px;
            left: -10px;
        }
        
    .glyphicon.glyphicon-cloud-download {
        font-size: 25px;
    }
        `
    ]
})
export class ApiKeyStatusComponent implements OnInit {
    @Input() ShowLinkToEditPage: boolean = false;
    color: string = "";  
    apiKeyIsValid : boolean = false;
    message: string = "";
    constructor(
        private service: LoginHistoryService,
        private statusActivator: StatusActivatorService
    ){

    }

    ngOnInit() {
        
        this.LoadSiteExtractionErrorCount();
        this.statusActivator.events$.forEach(event => this.LoadSiteExtractionErrorCount());
    }

    LoadSiteExtractionErrorCount(){
        this.service.getSamApiKey()
            .subscribe((apiKey: ApiKey) => {
                
                var validTill = new Date(apiKey.ValidTill.toString());
                var willExpireIn = this.daysDiff(validTill);
                switch (true) {
                    case (willExpireIn < 0):
                        this.color = "red";
                        this.message = "Api key(s) expired " + willExpireIn + " days ago.";
                        break;
                    case (willExpireIn === 0):
                        this.color = "yellow";
                        this.message = "Api key(s) will expire today."; 
                        break;
                            
                    case (willExpireIn < 10):
                        this.color = "yellow";
                        this.message = "Api key(s) will expire in " + willExpireIn + " days."; 
                        break;
                    
                            
                    default:
                        this.color = "green";
                        this.message = "Api key(s) are valid."
                        break;
                }

                if (this.ShowLinkToEditPage === false && willExpireIn < 11){
                            this.message += " Please alert the App Admin.";
                }
                
                
            });
    }

    daysDiff(validTill : any){
        var currentDate : any = new Date();
        return Math.round((validTill - currentDate)/(1000 * 60 * 60 * 24)) + 1;
    }

}