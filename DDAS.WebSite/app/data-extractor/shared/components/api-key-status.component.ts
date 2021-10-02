import { Component, OnInit, Input } from '@angular/core';
import {LoginHistoryService} from '../../../Admin/all-loginhistory.service'
// import {DataExtractorService} from '../../data-extractor-service';

import {ApiKey} from '../../../../app/Admin/appAdmin.classes';
import {StatusActivatorService} from '../service/status.activator.service'
@Component({
    selector: 'api-key-status',
    template: `
        
    <div ><span class="glyphicon glyphicon-star {{color}}" title="{{message}}"></span>
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
                var currentDate = new Date();
                var validTill = new Date(apiKey.ValidTill.toString());

                if (apiKey.Value.length > 0 && currentDate.getTime() < validTill.getTime()){
                    this.apiKeyIsValid = true;
                }
                if (this.apiKeyIsValid === true){
                    this.color = "green";
                    this.message = "Api key(s) are valid."
                }else{
                    this.message = "Invalid Api Keys."
                    if (this.ShowLinkToEditPage === false){
                        this.message += " Alert the App Admin.";
                    }
                    this.color = "red";
                }
            });
    }

}