import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportComponent } from './report.component';
import { ReportService } from './report-service';
import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { SearchModule } from '../search/search.module'
import { SharedModule } from '../shared/shared.module'
//import {ModalModule} from "ng2-modal";


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    Ng2Bs3ModalModule,
    SearchModule,
    SharedModule
    //ModalModule
  ],
  declarations: [
    ReportComponent
  ],

  providers: [
    ReportService,
  ]

})
export class ReportModule { }