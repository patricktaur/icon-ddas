
import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {AppAdminDashboardComponent} from './app-admin-dashboard.component'
import { ManageUploadsComponent } from './manage-uploads.component';
import {CBERComponent} from './cber-investigate.component';

export const AppAdminRoutes: Routes = [
  //{ path: 'app-admin-dashboard', component: AppAdminDashboardComponent},
  { path: 'manage-uploads', component: ManageUploadsComponent },
  { path: 'cber-investigate', component: CBERComponent}
]


export const AppAdminRouting: ModuleWithProviders = RouterModule.forChild(AppAdminRoutes);