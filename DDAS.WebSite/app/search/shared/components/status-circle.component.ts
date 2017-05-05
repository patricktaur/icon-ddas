import { Component, Input, OnInit } from '@angular/core';
//import { CircleComponent }  from '../components/circle.component';
import { ComplianceFormStatusEnum } from '../../search.classes';

@Component({ 
    selector: '[statusCircle]',
    //template: '<div circle  [color]="color" [radius]="radius" title="{{title}}"></div>'
    template: '<div circle  [color]="getColor" [radius]="radius" title="{{title}}"></div>'

 })
export class StatusCircleComponent implements OnInit {
    @Input('status') status: ComplianceFormStatusEnum = ComplianceFormStatusEnum.NotScanned;
    @Input('size') size:number = 20;
    @Input('title') title:string = "";
    radius : number;
    color : string = "";
    colorArray: string[] = [];
    loading: boolean;
    
    ngOnInit() {
        this.loading = true;
        this.radius = this.size / 2;
        
        //Not used: can be removed:
        // switch (this.status) {
        // case 0:
        //     this.color ="#BDBDBD";
        //     break;
        // case 1:
        //     this.color = "#BF360C"; //"red";
        //     break;
        // case 2:
        //     this.color = "#388e3c";//"green";
        //     break;
        // case 3:
        //     this.color ="#F44336";
        //     break;   
        // case 4:
        //     this.color ="#F44336";
        //     break;   
        // case 5:
        //     this.color = "#cddc39"; //"lawngreen";
        //     break;   
        //  case 6:
        //     this.color ="#BDBDBD";
        //     break; 
        // case 7:
        //     this.color ="#F44336";
        //     break; 
        // case 8:
        //     this.color ="#BDBDBD";
        //     break; 
        // default:
        //     this.color ="#212121";
        //     break;
        // }
        
        this.colorArray.push("#BDBDBD"); //0 - grey
        this.colorArray.push("#BF360C"); //1 - "red";
        this.colorArray.push("#388e3c"); //2 "green";
        this.colorArray.push("#F44336"); //3 full match
        this.colorArray.push("#F44336"); //4 partial match
        this.colorArray.push("#cddc39"); //5 "lawngreen";
        this.colorArray.push("#BDBDBD"); //6 
        this.colorArray.push("#F44336"); // 7 
        this.colorArray.push("#BDBDBD"); // 8
        this.colorArray.push("#F44336"); // 9 single match
        this.loading = false;
    }
  
    get getColor(){
        if (this.loading){
            return null;
        }
        else{
             return this.colorArray[this.status];
        }
       
    }
}

