import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportComponent } from './report.component';
import { ReportService } from './report-service';
import {reportRouting} from './report.routing';

import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';

import { SharedModule } from '../shared/shared.module'
//import {ModalModule} from "ng2-modal";
import { OutputReportComponent } from './output-report.component';
import { InvestigationsCompletedReportComponent } from './investigations-completed.component';
import { OpenInvestigationsComponent } from './open-investigations.component';
import { AdminDashboardComponent } from './admin-dashboard.component';
import { AssignmentHistoryComponent } from './assignment-history.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    reportRouting,
    Ng2Bs3ModalModule,
    
    SharedModule
    //ModalModule
  ],
  declarations: [
    ReportComponent,
    OutputReportComponent,
    InvestigationsCompletedReportComponent,
    OpenInvestigationsComponent,
    AdminDashboardComponent,
    AssignmentHistoryComponent
  ],

  providers: [
    ReportService,
  ]
})
export class ReportModule { }