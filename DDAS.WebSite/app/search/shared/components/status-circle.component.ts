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
            this.color ="#BDBDBD";
            break;
        case 1:
            this.color = "#BF360C"; //"red";
            break;
        case 2:
            this.color = "#388e3c";//"green";
            break;
        case 3:
            this.color ="#F44336";
            break;   
        case 4:
            this.color ="#F44336";
            break;   
        case 5:
            this.color = "#cddc39"; //"lawngreen";
            break;   
         case 6:
            this.color ="#BDBDBD";
            break; 
        case 7:
            this.color ="#F44336";
            break; 
        case 8:
            this.color ="#BDBDBD";
            break; 
        default:
            this.color ="#212121";
            break;
        }


    }
  
}

