import { Component, Input, OnInit  } from '@angular/core';
@Component({ 
    selector: '[toggle-show]',
    template: `
        
        <span class="caret" (click)="onClick()">
        
        <ng-content *ngIf="show"></ng-content>
        </span>
        
    `,
    

 })
export class ToggleShowComponent implements OnInit {
    
    public show: Boolean;
     ngOnInit() { }

     onClick(){
         this.show = !this.show;
     }
}
