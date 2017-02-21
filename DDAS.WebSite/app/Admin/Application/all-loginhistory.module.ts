import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';

import {LoginHistoryComponent} from './all-loginhistory.component';

import {LoginHistoryRouting} from './all-loginhistory.Routing';

import {LoginHistoryService} from './all-loginhistory.service';
import {SharedModule} from '../../shared/shared.module'

import {ErrorImagesComponent} from './error-images.component';

@NgModule({ 
  imports: [
      LoginHistoryRouting,
      CommonModule,
      FormsModule,
      SharedModule
  ],
  declarations: [
      LoginHistoryComponent,
      ErrorImagesComponent
  ],
 
  providers: [      
      LoginHistoryService
]
})

export class LoginHistoryModule {}