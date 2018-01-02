import { NgModule } from '@angular/core';

import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { SearchResultSummaryComponent } from './search-result-summary.component';
import { SearchInputComponent } from './search-input.component';

import { OpenComplianceFormsComponent } from './open-compliance-search-forms.component';

import { DueDiligenceCheckComponent } from './due-diligence-check.component';

import { ReviewCompletedICSFComponent } from './review-completed-icsf.component';

import { ManageICFsComponent } from './manage-icfs.component';
import { AllISCFsComponent } from './all-iscfs.component';

import { SearchComponent } from './search.component';

import { searchRouting } from './search.routing';

import { SearchDetailComponent } from './search-detail.component';
import { Ng2Uploader } from '../shared/utils/ng2-uploader1/ng2-uploader'

import { ComplianceFormComponent } from './compliance-form.component'
import { CompFormEditComponent } from '../compliance-form/form/comp-form-edit.component'

import { InvestigatorSummaryComponent } from './investigator-summary.component'
import { FindingsComponent } from './findings.component'

import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { SharedModule } from '../shared/shared.module'
import { StatusCircleComponent } from './shared/components/status-circle.component';
import { StatusCircleLegendVerticalComponent } from './shared/components/status-circle-legend-vertical';
import { StatusCircleLegHorComponent } from './shared/components/status-circle-legend-horizontal';
import { DownloadComplianceFormComponent } from './shared/components/download-compliance-form.component';
import { GenerateOutputFileComponent } from './shared/components/generate-output-file.component';

import { InstituteFindingsSummaryComponent } from './institute-findings-summary.component'
import {InstituteFindingsComponent} from './institute-findings.component' 
import {ComplianceFormViewComponent} from './shared/components/compliance-form-view/compliance-form-view.component'

import {ComplianceFormGeneralEditComponent} from "../compliance-form/tab-components/general-edit/general-edit.component"
import {ComplianceFormGeneralViewComponent} from "../compliance-form/tab-components/general-view/general-view.component"
import {ComplianceFormInvestigatorEditComponent} from "../compliance-form/tab-components/investigators-edit/investigator-edit.component"
import {ComplianceFormInvestigatorViewComponent} from "../compliance-form/tab-components/investigators-view/investigator-view.component"

import {ComplianceFormInstituteEditComponent} from "../compliance-form/tab-components/institute-edit/institute-edit.component"
import {ComplianceFormInstituteViewComponent} from "../compliance-form/tab-components/institute-view/institute-view.component"

import {ComplianceFormMandatorySitesEditComponent} from "../compliance-form/tab-components/mandatory-sites-edit/mandatory-sites-edit.component"
import {ComplianceFormMandatorySitesViewComponent} from "../compliance-form/tab-components/mandatory-sites-view/mandatory-sites-view.component"

import {ComplianceFormAdditionalSitesEditComponent} from "../compliance-form/tab-components/additional-sites-edit/additional-sites-edit.component"
import {ComplianceFormAdditionalSitesViewComponent}    from "../compliance-form/tab-components/additional-sites-view/additional-sites-view.component"



import {  ComplianceFormFindingsComponent } from "../compliance-form/tab-components/findings/findings.component"
import {ComplianceFormSearchedByComponent} from "../compliance-form/tab-components/searched-by/searched-by.component"
import{ComplianceFormSummaryComponent} from "../compliance-form/tab-components/summary/summary.component"

import {SelectedFindingComponent} from "../compliance-form/findings/selected-finding/selected-finding.component";

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,

    searchRouting,
    Ng2Uploader,
    Ng2Bs3ModalModule,
    SharedModule
  ],
  declarations: [

    SearchComponent,

    OpenComplianceFormsComponent,
    DueDiligenceCheckComponent,
    ReviewCompletedICSFComponent,
    ManageICFsComponent,
    ComplianceFormComponent,
    CompFormEditComponent,
    InvestigatorSummaryComponent,
    FindingsComponent,
    StatusCircleComponent,
    StatusCircleLegendVerticalComponent,
    StatusCircleLegHorComponent,
    DownloadComplianceFormComponent,
    AllISCFsComponent,
    GenerateOutputFileComponent,
    InstituteFindingsSummaryComponent,
    InstituteFindingsComponent,
    ComplianceFormViewComponent,
    ComplianceFormGeneralEditComponent,
    ComplianceFormGeneralViewComponent,
    
    ComplianceFormInstituteEditComponent,
    ComplianceFormInstituteViewComponent,

    ComplianceFormInvestigatorEditComponent,
    ComplianceFormInvestigatorViewComponent,
    ComplianceFormMandatorySitesEditComponent,
    ComplianceFormMandatorySitesViewComponent,
    ComplianceFormAdditionalSitesEditComponent,
    ComplianceFormAdditionalSitesViewComponent,
    ComplianceFormFindingsComponent,
    ComplianceFormSearchedByComponent,
    ComplianceFormSummaryComponent,
    SelectedFindingComponent
  ],

  providers: [

  ],
  exports: [StatusCircleComponent,
    StatusCircleLegendVerticalComponent,
    StatusCircleLegHorComponent,
    DownloadComplianceFormComponent,
    GenerateOutputFileComponent,
    ComplianceFormViewComponent
  ]
})
export class SearchModule { }