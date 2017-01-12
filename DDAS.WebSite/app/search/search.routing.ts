import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

//import { SearchResultSummaryComponent } from './search-result-summary.component';

import { OpenComplianceFormsComponent } from './open-compliance-search-forms.component';

import { DueDiligenceCheckComponent } from './due-diligence-check.component';
import { ClosedICFsComponent } from './closed-icfs.component';
import { ManageICFsComponent } from './manage-icfs.component';

import { SearchComponent }     from './search.component';
//import { SearchDetailComponent }     from './search-detail.component';
  
import {ComplianceFormComponent} from './compliance-form.component'

import {InvestigatorSummaryComponent} from './investigator-summary.component'
import {FindingsComponent} from './findings.component'

//pradeep 9Jan2017
import {AllISCFsComponent} from './all-iscfs.component'

import { AuthGuard } from '../auth/auth-guard.service';

const searchRoutes: Routes = [
  {
    path: '',
    redirectTo: '/search',
    pathMatch: 'full'
  },
  {
    path: 'search',
    component: SearchComponent , canActivate: [AuthGuard],
    children: [
          //{ path: '',  component: SearchInputComponent },
            
            { path: '',  component: DueDiligenceCheckComponent },
             
           
          //  { path: 'details/:siteEnum/:formid/:NameToSearch',
          //  component: SearchDetailComponent,
          //  },
           //can be removed?
          //  {
          //   path: 'summary/:NameToSearch/:formid/:FullMatchCount/:PartialMatchCount',
          //   component: SearchResultSummaryComponent,
          // },
          {
            path: 'complianceform/:formid',
            component: ComplianceFormComponent,
          },
          {
            path: 'investigator-summary/:formid/:investigatorid',
            component: InvestigatorSummaryComponent,
          },
          {
            path: 'findings/:formid/:investigatorid/:siteenum',
            component: FindingsComponent,
          }
    ]
  },
   { path: 'closed-icfs',  component: ClosedICFsComponent 
    , canActivate: [AuthGuard] },
     { path: 'open-compliance-forms',  component: OpenComplianceFormsComponent 
    , canActivate: [AuthGuard] },
   { path: 'manage-compliance-forms',  component: ManageICFsComponent 
    , canActivate: [AuthGuard] },
    { path: 'all-iscfs', component: AllISCFsComponent}
];

export const searchRouting: ModuleWithProviders = RouterModule.forChild(searchRoutes);
