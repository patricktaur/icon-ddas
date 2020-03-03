import { Component, OnInit, Input } from '@angular/core';
//import {LoginHistoryService} from '../../../Admin/all-loginhistory.service'
import {DataExtractorService} from '../../data-extractor-service';
import {StatusActivatorService} from '../service/status.activator.service'
@Component({
    selector: 'data-extraction-indicator1',
    template: `
        
    
    <div ><span class="glyphicon glyphicon-cloud-download {{color}}" title="{{message}}"></span>
        <span *ngIf="SiteExtractionErrorCount > 0" class="badge" title="{{message}}">{{SiteExtractionErrorCount}}
        </span>
        <a *ngIf="ShowLinkToExecuteExtractorPage" routerLink="/data-extractor"  routerLinkActive="active">Run Extractor</a>
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
export class DataExtractionIndicatorComponent implements OnInit {
    @Input() ShowLinkToExecuteExtractorPage: boolean = false;
    color: string = "";  
    SiteExtractionErrorCount  : number = 0;  
    message: string = "";
    constructor(
        private service: DataExtractorService,
        private statusActivator: StatusActivatorService
    ){

    }

    ngOnInit() {
        
        this.LoadSiteExtractionErrorCount();
        this.statusActivator.events$.forEach(event => this.LoadSiteExtractionErrorCount());
    }

    LoadSiteExtractionErrorCount(){
        this.service.getDataExtractionErrorSiteCount()
            .subscribe((item: number) => {
                this.SiteExtractionErrorCount = item;
                console.log(item);
                if (item === 0){
                    this.color = "green";
                    this.message = "No Data Extraction error."
                }else{
                    this.message = "Data Extraction Error for " + this.SiteExtractionErrorCount + " Sites."
                    this.color = "black";
                }
            });
    }

}