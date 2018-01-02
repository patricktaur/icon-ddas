import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
import { AuthGuard }             from '../auth/auth-guard.service';
import { CanDeactivateGuard } from '../shared/services/can-deactivate-guard.service';
import { ListQCComponent } from './list-qc/list-qc.component';
import { EditQCComponent } from './edit-qc/edit-qc.component';
import { QCService } from './qc-service';
import { CompFormEditComponent } from './../compliance-form/form/comp-form-edit.component';
import { InvestigatorSummaryComponent } from './../search/investigator-summary.component'
import { FindingsComponent } from './../search/findings.component'
import { InstituteFindingsSummaryComponent } from './../search/institute-findings-summary.component'
import {InstituteFindingsComponent} from './../search/institute-findings.component'

const reportRoutes: Routes = [
  {
    path: 'qc'
    , canActivate: [AuthGuard],
    children: [
  
      { path: '', component: ListQCComponent },
      { path: 'edit-qc/:complianceFormId/:qcAssignedTo', component: EditQCComponent },
     
        {
        path: 'comp-form-edit/:formId',
        component: CompFormEditComponent, canDeactivate: [CanDeactivateGuard]
      },
      {
        path: 'investigator-summary/:formId/:investigatorId',
        component: InvestigatorSummaryComponent,
      },
      {
         path: 'findings/:formId/:investigatorId/:siteSourceId',
        component: FindingsComponent, canDeactivate: [CanDeactivateGuard]
      },
      {
        path: 'institute-findings-summary/:formId',
        component: InstituteFindingsSummaryComponent,
      },
      {
        path: 'institute-findings/:formId/:siteSourceId',
        component: InstituteFindingsComponent, canDeactivate: [CanDeactivateGuard]
      },
      
    ]
  },
];

export const reportRouting: ModuleWithProviders = RouterModule.forChild(reportRoutes);
