
import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {AppAdminDashboardComponent} from './app-admin-dashboard.component'

export const AppAdminRoutes: Routes = [
  { path: 'app-admin-dashboard', component: AppAdminDashboardComponent},
]


export const AppAdminRouting: ModuleWithProviders = RouterModule.forChild(AppAdminRoutes);