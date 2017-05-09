import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CanDeactivateGuard } from '../shared/services/can-deactivate-guard.service';

//import { SearchResultSummaryComponent } from './search-result-summary.component';
//import { SearchInputComponent } from './search-input.component';
import { OpenComplianceFormsComponent } from './open-compliance-search-forms.component';

import { DueDiligenceCheckComponent } from './due-diligence-check.component';
import { ClosedICFsComponent } from './closed-icfs.component';
import { ManageICFsComponent } from './manage-icfs.component';
import { AllISCFsComponent } from './all-iscfs.component';


import { SearchComponent } from './search.component';
//import { SearchDetailComponent }     from './search-detail.component';

import { ComplianceFormComponent } from './compliance-form.component'
import { CompFormEditComponent } from '../compliance-form/form/comp-form-edit.component'

import { InvestigatorSummaryComponent } from './investigator-summary.component'
import { FindingsComponent } from './findings.component'



import { AuthGuard } from '../auth/auth-guard.service';

const searchRoutes: Routes = [
  {
    path: '',
    redirectTo: '/search',
    pathMatch: 'full'
  },
  {
    path: 'search',
    component: SearchComponent, canActivate: [AuthGuard],
    children: [

      { path: '', component: DueDiligenceCheckComponent },

      {
        path: 'complianceform/:formId',
        component: ComplianceFormComponent, canDeactivate: [CanDeactivateGuard]
      },
      {
        path: 'comp-form-edit/:formId',
        component: CompFormEditComponent, canDeactivate: [CanDeactivateGuard]
      },
     
      {
        path: 'investigator-summary/:formId/:investigatorId',
        component: InvestigatorSummaryComponent,
      },
      {
        //path: 'findings/:formId/:investigatorId/:siteEnum',
        path: 'findings/:formId/:investigatorId/:siteId',
        component: FindingsComponent, canDeactivate: [CanDeactivateGuard]
      }
    ]
  },
  {
    path: 'closed-icfs1', component: ClosedICFsComponent
    , canActivate: [AuthGuard],

  },
  
  
  {
    path: 'manage-compliance-forms', component: SearchComponent
    , canActivate: [AuthGuard],
    children: [

      { path: '', component: ManageICFsComponent },

      {
        path: 'complianceform/:formId',
        component: ComplianceFormComponent, canDeactivate: [CanDeactivateGuard]
      },
      {
        path: 'comp-form-edit/:formId',
        component: CompFormEditComponent, canDeactivate: [CanDeactivateGuard]
      },
      {
        path: 'investigator-summary/:formId/:investigatorId',
        component: InvestigatorSummaryComponent,
      },
      {
         //path: 'findings/:formId/:investigatorId/:siteEnum',
        path: 'findings/:formId/:investigatorId/:siteId',
        component: FindingsComponent, canDeactivate: [CanDeactivateGuard]
      }
    ]
  },

{
  path: 'all-iscfs', component: AllISCFsComponent
    , canActivate: [AuthGuard]
},
    
 {
    path: 'closed-icfs', component: SearchComponent
    , canActivate: [AuthGuard],
    children: [

      { path: '', component: ClosedICFsComponent },

      {
        path: 'complianceform/:formId',
        component: ComplianceFormComponent, canDeactivate: [CanDeactivateGuard]
      },
        {
        path: 'comp-form-edit/:formId',
        component: CompFormEditComponent, canDeactivate: [CanDeactivateGuard]
      },
      {
        path: 'investigator-summary/:formId/:investigatorId',
        component: InvestigatorSummaryComponent,
      },
      {
        //path: 'findings/:formId/:investigatorId/:siteEnum',
        path: 'findings/:formId/:investigatorId/:siteId',
        component: FindingsComponent, canDeactivate: [CanDeactivateGuard]
      }
    ]
  }, 
   
];

export const searchRouting: ModuleWithProviders = RouterModule.forChild(searchRoutes);
