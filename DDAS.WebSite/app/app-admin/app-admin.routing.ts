
import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {AppAdminUsersComponent} from './app-admin-users.component'

export const AppAdminRoutes: Routes = [
  { path: 'app-admin-users', component: AppAdminUsersComponent},
]


export const AppAdminRouting: ModuleWithProviders = RouterModule.forChild(AppAdminRoutes);