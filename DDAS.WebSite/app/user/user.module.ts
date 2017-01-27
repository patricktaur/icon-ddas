import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';
import {UserComponent} from './user.component';
import {UserInputComponent} from './user-input.component';

import { UserRouting } from './user.routing';
import {UserService} from './user-service';
import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import {SharedModule} from '../shared/shared.module'
//import {ModalModule} from "ng2-modal";

@NgModule({ 
  imports: [
    CommonModule,
    FormsModule,
    UserRouting,
    Ng2Bs3ModalModule,
    SharedModule
    
  ],
  declarations: [
   UserComponent,
   UserInputComponent
  ],
 
  providers: [
    UserService
  ]
})
export class UserModule {}