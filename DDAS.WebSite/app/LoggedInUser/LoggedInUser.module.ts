import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';

import {AboutDDASComponent} from './about-ddas.component';

import {myLoginHistoryComponent} from './my-loginhistory.component';

import {LoggedInUserRouting} from './loggedinuser.routing';

import {loggedinuserService} from './loggedinuser.service';

@NgModule({ 
  imports: [
      LoggedInUserRouting,
      CommonModule,
      FormsModule
  ],
  declarations: [
      AboutDDASComponent,
      myLoginHistoryComponent
  ],
 
  providers: [      
      loggedinuserService
]
})

export class LoggedInUserModule {}
