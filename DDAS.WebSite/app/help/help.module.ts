import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HelpComponent } from './help.component';
import { HelpService } from './help-service';
import { helpRouting } from './help.routing';

import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';

import { SharedModule } from '../shared/shared.module'
//import {ModalModule} from "ng2-modal";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    helpRouting,
    Ng2Bs3ModalModule,
    SharedModule
    //ModalModule
  ],
  declarations: [
    HelpComponent,
    HelpComponent
  ],
  providers: [
    HelpService,
  ]
})
export class HelpModule { }