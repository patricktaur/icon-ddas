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

import {DateInputComponent} from './components/date-input.component'; 
import {CommonService} from './common.service';


 

@NgModule({
  imports:     
   [ CommonModule,
   MyDatePickerModule
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
   
                   ],
                   
})
export class SharedModule { }
