import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';
import { ReportComponent} from './report.component';
import {ReportService} from './report-service';

import { ReportRouting} from './report.routings';



@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReportRouting
  ],
  declarations: [
   ReportComponent

  ],
  
  providers: [
    ReportService,

  ]
})
export class ReportModule {}