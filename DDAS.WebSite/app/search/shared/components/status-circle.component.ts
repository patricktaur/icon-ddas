import { Component, Input, OnInit } from '@angular/core';
//import { CircleComponent }  from '../components/circle.component';
import { ComplianceFormStatusEnum } from '../../search.classes';

@Component({ 
    selector: '[statusCircle]',
    template: '<div circle  [color]="color" [radius]="radius" title="{{title}}"></div>'
 })
export class StatusCircleComponent implements OnInit {
    @Input('status') status: ComplianceFormStatusEnum = ComplianceFormStatusEnum.NotScanned;
    @Input('size') size:number = 20;
    @Input('title') title:string = "";
    radius : number;
    color : string = "";
     
    ngOnInit() {
        this.radius = this.size / 2;
        
        switch (this.status) {
        case 0:
            this.color ="grey";
            break;
        case 1:
            this.color ="red";
            break;   
        case 2:
            this.color ="green";
            break;   
        case 3:
            this.color ="lightcoral";
            break;   
        case 4:
            this.color ="lightcoral";
            break;   
        case 5:
            this.color ="lawngreen";
            break;   
        default:
            this.color ="black";
            break;
        }
    }
  
}

