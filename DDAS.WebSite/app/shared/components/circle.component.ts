import { Component, Input, OnInit  } from '@angular/core';
@Component({ 
    selector: '[circle]',
    template: `<svg [attr.height]="size" [attr.width]="size">
    <circle [attr.cx]="radius" [attr.cy]="radius" [attr.r]="radius" stroke="black" 
      stroke-width="0" [attr.fill]="color" />
    </svg>`,
    

 })
export class CircleComponent implements OnInit {
    
     @Input('color') color:string = 'red';
     @Input('radius') radius:number = 20;
     size: number = 40;
     ngOnInit() {
         this.size = this.radius * 2;

     }

    //  template: `<svg height="24" width="24">
    // <circle cx="12" cy="12" [attr.r]="radius" stroke="black" 
    //   stroke-width="0" [attr.fill]="fillAttr" />
    // </svg>`,
    
}
