import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';


  
import { AuthGuard }             from '../auth/auth-guard.service';
import { ReportComponent }     from './report.component'; 
import { OutputReportComponent }     from './output-report.component'; 

const reportRoutes: Routes = [
  
 {
    path: '',
    redirectTo: '/reports',
    pathMatch: 'full'
  },
  {
    path: 'reports',
    component: ReportComponent, canActivate: [AuthGuard],
    children: [
      { path: '', component: OutputReportComponent },

    ]
  }

];

export const reportRouting: ModuleWithProviders = RouterModule.forChild(reportRoutes);
