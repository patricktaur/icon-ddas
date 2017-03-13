import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';
import {AppAdminRouting} from './app-admin.routing'
import {AppAdminService} from './app-admin.service';
import {AppAdminDashboardComponent} from './app-admin-dashboard.component'
@NgModule({ 
  imports: [
      
      CommonModule,
      FormsModule,
      AppAdminRouting
  ],
  declarations: [
      AppAdminDashboardComponent
  ],
 
  providers: [      
      AppAdminService
]
})

export class AppAdminModule {}