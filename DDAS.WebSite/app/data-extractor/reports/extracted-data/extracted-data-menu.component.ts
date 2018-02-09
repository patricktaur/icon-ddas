import { Component} from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'exctracted-data-menu',
    templateUrl: 'extracted-data-menu.component.html',
    styles:[`
    .container {
        margin-bottom: 40px;
      }
    .border {
        border: 2px solid black;
        background-position: center;
        background-size: cover;
        background-repeat: no-repeat;
        margin: 0 -2px -2px 0;
      }
    `]
})
export class ExtractedDataMenuComponent  {
}