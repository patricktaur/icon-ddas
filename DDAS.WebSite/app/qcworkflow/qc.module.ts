import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ListQCComponent } from './list-qc/list-qc.component';
import { EditQCComponent } from './edit-qc/edit-qc.component';
import { QCService } from './qc-service';
import {reportRouting} from './qc.routing';

import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';

import { SharedModule } from '../shared/shared.module';
import {SearchModule} from '../search/search.module';
//import {ModalModule} from "ng2-modal";

import { QCVerifierGeneralCommentComponent } from 
'./qcComment/qc-verifier-comment/qc-verifier-general-comment.component';

import { ReviewerResponseToGeneralCommentComponent } from 
'./qcComment/qc-reviewer-response/reviewer-response-to-general-comment.component';

import { QCVerifierGeneralCommentViewComponent } from
'./qcComment/qc-verifier-comment-view/qc-verifier-general-comment-view.component';

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
    ListQCComponent,
    EditQCComponent,
    QCVerifierGeneralCommentComponent,
    ReviewerResponseToGeneralCommentComponent,
    QCVerifierGeneralCommentViewComponent
  ],
  exports: [
    QCVerifierGeneralCommentComponent,
    ReviewerResponseToGeneralCommentComponent,
    QCVerifierGeneralCommentViewComponent
  ],
  providers: [
    QCService
  ]
})
export class QCModule { }