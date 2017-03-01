import { Component, Input, OnInit  } from '@angular/core';
@Component({ 
    selector: '[confirm-dialog]',
    template: `
        
    `,
    

 })
export class ConfirmDialogComponent implements OnInit {
    
     @Input('title') title:string = 'Delete Confirm';
     @Input('message') message:string = "";

     ngOnInit() {
         

     }

    
}