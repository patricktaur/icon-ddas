import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
import { AuthGuard }             from '../auth/auth-guard.service';
import { ReportComponent }     from './report.component';
import { OutputReportComponent }     from './output-report.component';
import {InvestigationsCompletedReportComponent} from './investigations-completed.component';
import {OpenInvestigationsComponent} from './open-investigations.component';
import { AdminDashboardComponent } from './admin-dashboard.component';
import { AssignmentHistoryComponent } from './assignment-history.component';
import { InvestigatorFindingsComponent } from './investigator-findings.component';

const reportRoutes: Routes = [
 {
    path: 'output-report',
    //redirectTo: '/reports',
    component: OutputReportComponent, canActivate: [AuthGuard]
    //pathMatch: 'full'
  },
  // {
  //   path: 'reports',
  //   component: ReportComponent, canActivate: [AuthGuard],
  //   children: [
  //     { path: '', component: OutputReportComponent },

  //   ]
  // },
  {
    path: 'investigations-completed',
    component: InvestigationsCompletedReportComponent, canActivate: [AuthGuard]
  },
  {
    path: 'open-investigations',
    component: OpenInvestigationsComponent, canActivate: [AuthGuard]
  },
  {
    path: 'admin-dashboard',
    component: AdminDashboardComponent, canActivate: [AuthGuard]
  },
  {
    path: 'assignment-history',
    component: AssignmentHistoryComponent, canActivate: [AuthGuard]
  },
  {
    path: 'investigator-findings',
    component: InvestigatorFindingsComponent, canActivate: [AuthGuard]
  }
];

export const reportRouting: ModuleWithProviders = RouterModule.forChild(reportRoutes);
