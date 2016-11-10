import {NgModule,  EventEmitter } from '@angular/core';
import { NgFileSelectDirective } from './src/directives/ng-file-select';
import { NgFileDropDirective } from './src/directives/ng-file-drop';

export * from './src/services/ng2-uploader';
export * from './src/directives/ng-file-select';
export * from './src/directives/ng-file-drop';

export const UPLOAD_DIRECTIVES: any[] = [
  NgFileDropDirective,
  NgFileSelectDirective
];

@NgModule({ 
  imports: [
    
  ],
  exports:[
    NgFileDropDirective,
    NgFileSelectDirective
  ],
  declarations: [
    NgFileDropDirective,
    NgFileSelectDirective

  ]
})
export class Ng2Uploader {
  
}