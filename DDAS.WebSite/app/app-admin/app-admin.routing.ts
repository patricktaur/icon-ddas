
import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {AppAdminDashboardComponent} from './app-admin-dashboard.component'
import { ManageUploadsComponent } from './manage-uploads.component';
//import {CBERComponent} from './cber-investigate.component';
import {CompFormArchiveComponent} from '../archive/compliance-form/comp-form-archv.component';
import {CompFormActiveComponent} from '../archive/compliance-form-active/comp-form-active.component';
export const AppAdminRoutes: Routes = [
  //{ path: 'app-admin-dashboard', component: AppAdminDashboardComponent},
  { path: 'manage-uploads', component: ManageUploadsComponent },
  { path: 'comp-form-archive', component: CompFormArchiveComponent },
  { path: 'comp-form-active', component: CompFormActiveComponent },
 
]


export const AppAdminRouting: ModuleWithProviders = RouterModule.forChild(AppAdminRoutes);