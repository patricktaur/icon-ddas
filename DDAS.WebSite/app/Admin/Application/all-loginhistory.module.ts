import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';

import {LoginHistoryComponent} from './all-loginhistory.component';

import {LoginHistoryRouting} from './all-loginhistory.Routing';

import {LoginHistoryService} from './all-loginhistory.service';

@NgModule({ 
  imports: [
      LoginHistoryRouting,
      CommonModule,
      FormsModule
  ],
  declarations: [
      LoginHistoryComponent
  ],
 
  providers: [      
      LoginHistoryService
]
})

export class LoginHistoryModule {}