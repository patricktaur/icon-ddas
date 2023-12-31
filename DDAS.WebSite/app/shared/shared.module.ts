import { NgModule }            from '@angular/core';
import { CommonModule }        from '@angular/common';
import { FormsModule }         from '@angular/forms';
import { AwesomePipe }         from './awesome.pipe';
import { HighlightDirective }  from './highlight.directive';

import { CircleComponent }  from './components/circle.component';
import { ToggleShowComponent }  from './components/toggle-show.component';
import { ConfirmDialogComponent }  from './components/confirm-dialog.component';

 import { BoolToYesNoPipe }  from './pipes/bool-yes-no.pipe'; 
  import { PadPipe }  from './pipes/pad.pipe'; 
   import { HighlightPipe }  from './pipes/text-highlight.pipe'; 
  
  //import { MyDatePickerModule } from 'mydatepicker/dist/my-date-picker.module';
import { MyDatePickerModule } from './utils/my-date-picker/my-date-picker.module';
import { Ng2PaginationModule }   from './utils/ng2-pagination/ng2-pagination'; 
import { Ng2Uploader } from '../shared/utils/ng2-uploader1/ng2-uploader'

import {DateInputComponent} from './components/date-input.component'; 
import {CommonService} from './common.service';
//import {FileDownloadComponent} from './components/file-download.component';
import {FileDownloadModule} from './modules/file-download/file-download.module' 
import {FileUploadComponent} from './components/file-upload-component'
import {MultiFileSelectComponent} from './components/multi-file-select.component'
import {ProgressIndicatorComponent} from './components/progress-indicator.component'
import {NotFoundComponent} from '../shared/components/not-found.component'
 

@NgModule({
  imports:     
   [ CommonModule,
   MyDatePickerModule,
   FileDownloadModule,
   Ng2Uploader
    ],
  
  declarations: [ 
    AwesomePipe, 
  HighlightDirective, 
  CircleComponent,
  BoolToYesNoPipe,
  PadPipe,
  HighlightPipe,
  ToggleShowComponent,
  ConfirmDialogComponent,
  DateInputComponent,
  FileUploadComponent,
  MultiFileSelectComponent,
  ProgressIndicatorComponent,
  NotFoundComponent

  
  
   ],
  exports:      [ 
    AwesomePipe, 
    HighlightDirective, 
    CommonModule, 
    FormsModule, 
    CircleComponent, 
    BoolToYesNoPipe,
    PadPipe,
    HighlightPipe,
    ToggleShowComponent,
    MyDatePickerModule,
    Ng2PaginationModule,
    ConfirmDialogComponent,
    DateInputComponent,
    FileDownloadModule,
    FileUploadComponent,
    MultiFileSelectComponent,
    ProgressIndicatorComponent,
    NotFoundComponent
                   ],
                   
})
export class SharedModule { }
