import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
import { AuthGuard }             from '../auth/auth-guard.service';
import { HelpComponent }     from './help.component';

const HelpRoutes: Routes = [
  
 {
    path: '',
    redirectTo: '/help',
    pathMatch: 'full'
  },
  {
    path: 'help',
    component: HelpComponent, canActivate: [AuthGuard]
  }

];

export const helpRouting: ModuleWithProviders = RouterModule.forChild(HelpRoutes);
