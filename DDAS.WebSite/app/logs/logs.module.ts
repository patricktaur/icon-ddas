import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {logsRouting} from './logs.routing';

import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';

import { SharedModule } from '../shared/shared.module';
import {SearchModule} from '../search/search.module';
//import {ModalModule} from "ng2-modal";
import {LogsMainComponent} from './logs-main/logs-main.component';
import {LogsService} from './logs-service';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    logsRouting,
    Ng2Bs3ModalModule,
    SharedModule,
    
  ],
  declarations: [
    LogsMainComponent
  ],
  exports: [
    
  ],
  providers: [
    LogsService
  ]
})
export class LogsModule { }