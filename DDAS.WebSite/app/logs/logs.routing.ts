import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../auth/auth-guard.service';
import { CanDeactivateGuard } from '../shared/services/can-deactivate-guard.service';
import {LogsMainComponent} from './logs-main/logs-main.component'

const logsRoutes: Routes = [
  {
    path: 'app-logs',
     component: LogsMainComponent,
    canActivate: [AuthGuard]
  },

  
];

export const logsRouting: ModuleWithProviders = RouterModule.forChild(logsRoutes);
