import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module'
import {DataExtractorService} from './data-extractor-service';
import {dataExtractorRouting} from './data-extractor.routing';
import {DataExtractionIndicatorComponent} from './shared/components/data-extraction-indicator.component'
import {DataExtractionStatusComponent } from './reports/data-extraction-status.component'
import {ExecuteDataExtractorComponent} from './execute-data-extractor/execute-data-extractor.component'
import {StatusActivatorService} from './shared/service/status.activator.service'
@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    dataExtractorRouting
  ],
  declarations: [
    DataExtractionIndicatorComponent,
    ExecuteDataExtractorComponent,
    DataExtractionStatusComponent
  ],

  providers: [
    DataExtractorService,
    StatusActivatorService
  ],
  exports: [
    DataExtractionIndicatorComponent
   ]
})
export class DataExtractorModule { }