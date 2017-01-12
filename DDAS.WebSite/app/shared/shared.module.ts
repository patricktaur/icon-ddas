import { NgModule }            from '@angular/core';
import { CommonModule }        from '@angular/common';
import { FormsModule }         from '@angular/forms';
import { AwesomePipe }         from './awesome.pipe';
import { HighlightDirective }  from './highlight.directive';

import { CircleComponent }  from './components/circle.component';
 import { BoolToYesNoPipe }  from './pipes/bool-yes-no.pipe'; 
  import { PadPipe }  from './pipes/pad.pipe'; 

@NgModule({
  imports:      [ CommonModule ],
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
    PadPipe
                   ]
})
export class SharedModule { }
