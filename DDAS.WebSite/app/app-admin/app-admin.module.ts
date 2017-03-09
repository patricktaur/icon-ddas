import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';
import {AppAdminRouting} from './app-admin.routing'
import {AppAdminService} from './app-admin.service';
import {AppAdminUsersComponent} from './app-admin-users.component'
@NgModule({ 
  imports: [
      
      CommonModule,
      FormsModule,
      AppAdminRouting
  ],
  declarations: [
      AppAdminUsersComponent
  ],
 
  providers: [      
      AppAdminService
]
})

export class AppAdminModule {}