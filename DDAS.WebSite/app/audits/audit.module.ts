import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ListAuditsComponent } from './list-audits/list-audit.component';
import { EditAuditComponent } from './edit-audit/edit-audit.component';
import { AuditService } from './audit-service';
import {reportRouting} from './audit.routing';

import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';

import { SharedModule } from '../shared/shared.module';
import {SearchModule} from '../search/search.module';
//import {ModalModule} from "ng2-modal";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    reportRouting,
    Ng2Bs3ModalModule,
    
    SharedModule,
    SearchModule
    //ModalModule
  ],
  declarations: [
    ListAuditsComponent,
    EditAuditComponent
  ],

  providers: [
    AuditService
  ]
})
export class AuditModule { }