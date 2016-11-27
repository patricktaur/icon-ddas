import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';
import { ReportComponent} from './report.component';
import {ReportService} from './report-service';

import { ReportRouting} from './report.routings';
import {ModalModule} from "ng2-modal";


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReportRouting,
    ModalModule
  ],
  declarations: [
   ReportComponent

  ],
  
  providers: [
    ReportService,

  ]
})
export class ReportModule {}