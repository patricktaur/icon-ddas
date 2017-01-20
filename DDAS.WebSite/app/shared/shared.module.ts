import { NgModule }            from '@angular/core';
import { CommonModule }        from '@angular/common';
import { FormsModule }         from '@angular/forms';
import { AwesomePipe }         from './awesome.pipe';
import { HighlightDirective }  from './highlight.directive';

import { CircleComponent }  from './components/circle.component';
 import { BoolToYesNoPipe }  from './pipes/bool-yes-no.pipe'; 
  import { PadPipe }  from './pipes/pad.pipe'; 
  
  //import { MyDatePickerModule } from 'mydatepicker/dist/my-date-picker.module';
import { MyDatePickerModule } from './utils/my-date-picker/my-date-picker.module';
import { Ng2PaginationModule }   from './utils/ng2-pagination/ng2-pagination'; 

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
  PadPipe
  
   ],
  exports:      [ 
    AwesomePipe, 
    HighlightDirective, 
    CommonModule, 
    FormsModule, 
    CircleComponent, 
    BoolToYesNoPipe,
    PadPipe,
    MyDatePickerModule,
    Ng2PaginationModule
                   ]
})
export class SharedModule { }
