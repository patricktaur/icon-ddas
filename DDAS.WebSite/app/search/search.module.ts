import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';

import { OpenComplianceFormsComponent }       from './open-compliance-search-forms.component';

import { DueDiligenceCheckComponent }       from './due-diligence-check.component';

import { ClosedICFsComponent }       from './closed-icfs.component';

import { ManageICFsComponent }       from './manage-icfs.component';

import { SearchComponent }     from './search.component';

import {AllISCFsComponent} from './all-iscfs.component';

import { searchRouting } from './search.routing';


 import {Ng2Uploader} from '../shared/utils/ng2-uploader1/ng2-uploader'

 import {ComplianceFormComponent} from './compliance-form.component'
 import {InvestigatorSummaryComponent} from './investigator-summary.component' 
 import {FindingsComponent} from './findings.component'

 import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
 import {SharedModule} from '../shared/shared.module'
 import { StatusCircleComponent}  from './shared/components/status-circle.component';
 import { StatusCircleLegendVerticalComponent}  from './shared/components/status-circle-legend-vertical';
 import { StatusCircleLegendHorizontalComponent}  from './shared/components/status-circle-legend-horizontal';
 
 import { DownloadComplianceFormComponent}  from './shared/components/download-compliance-form.component';
 


@NgModule({ 
  imports: [
    CommonModule,
    FormsModule,
    searchRouting,
    Ng2Uploader,
    Ng2Bs3ModalModule,
    SharedModule
  ],
  declarations: [
  
  SearchComponent,

  OpenComplianceFormsComponent,
  DueDiligenceCheckComponent,
  ClosedICFsComponent,
  ManageICFsComponent,
  ComplianceFormComponent,
  InvestigatorSummaryComponent,
  FindingsComponent,
  StatusCircleComponent,
  StatusCircleLegendVerticalComponent,
  StatusCircleLegendHorizontalComponent,
  DownloadComplianceFormComponent,
  AllISCFsComponent
  ],
 
  providers: [
    
  ],
  exports:      [ 
    StatusCircleComponent,
    StatusCircleLegendVerticalComponent,
    StatusCircleLegendHorizontalComponent,
    DownloadComplianceFormComponent
    ]
})
export class SearchModule {}