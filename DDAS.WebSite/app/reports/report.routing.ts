import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';

import { ReportComponent }     from './report.component';
  
import { AuthGuard }             from '../auth/auth-guard.service';

const reportRoutes: Routes = [
  {
    path: '',
    redirectTo: '/report',
    pathMatch: 'full'
  },
  {
    path: 'report',
    component: ReportComponent , canActivate: [AuthGuard],
  }
];

export const reportRouting: ModuleWithProviders = RouterModule.forChild(reportRoutes);
