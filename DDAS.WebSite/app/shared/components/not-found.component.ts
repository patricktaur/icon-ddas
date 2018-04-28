import { Component, Input, OnInit  } from '@angular/core';
@Component({ 
    template: `
    <div class="well">
        <h3>The information you were looking for is not found.</h3>    
        <p>Please use the menu options on the left to navigate to the page you are looking for.</p>    

   
    </div>
    `,
    

 })
export class NotFoundComponent implements OnInit {

     ngOnInit() {
         

     }

    
    
}
