import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';

import {AboutDDASComponent} from './about-ddas.component';

import {myLoginHistoryComponent}  from './my-loginhistory.component';
import {LoggedInUserRouting} from './LoggedInUser.Routing';

import {loggedinuserService} from './loggedinuser.service';
import {SharedModule} from '../shared/shared.module'

@NgModule({ 
  imports: [
      LoggedInUserRouting,
      CommonModule,
      FormsModule,
      SharedModule
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
