import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
import { AuthGuard }             from '../auth/auth-guard.service';
import { ListAuditsComponent } from './list-audits/list-audit.component';
import { EditAuditComponent } from './edit-audit/edit-audit.component';
import { AuditService } from './audit-service';

const reportRoutes: Routes = [
 {
    path: 'list-audit',
    //redirectTo: '/reports',
    component: ListAuditsComponent, canActivate: [AuthGuard]
    //pathMatch: 'full'
  },
  { 
    path: 'edit-audit/:AuditId', 
    component: EditAuditComponent,
    canActivate: [AuthGuard]
  }
];

export const reportRouting: ModuleWithProviders = RouterModule.forChild(reportRoutes);
