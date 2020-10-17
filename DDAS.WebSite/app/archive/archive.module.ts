import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';
// import {AppAdminRouting} from './app-admin.routing'
// import {AppAdminService} from './app-admin.service';
// import {AppAdminDashboardComponent} from './app-admin-dashboard.component';
// import { ManageUploadsComponent } from './manage-uploads.component';
//import {CBERComponent} from './cber-investigate.component';
import { SharedModule } from '../shared/shared.module';
import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';

// import {ArchvService} from './archv-service';
import {ComplianceFormArchiveService} from './comp-form-archive-service';
import {CompFormArchiveComponent} from './compliance-form/comp-form-archv.component'
import {CompFormActiveComponent} from './compliance-form-active/comp-form-active.component';


@NgModule({ 
  imports: [
      CommonModule,
      FormsModule,
      SharedModule,
      Ng2Bs3ModalModule
    //   AppAdminRouting
  ],
  declarations: [
    //   AppAdminDashboardComponent,
    //   ManageUploadsComponent,
      //CBERComponent
      CompFormArchiveComponent,
      CompFormActiveComponent

  ],
  providers: [      
    // ArchvService,
    ComplianceFormArchiveService
]
})

export class ArchiveModule {}