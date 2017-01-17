import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule }   from '@angular/router';


import { AuthGuard }             from './auth/auth-guard.service';

import { loginRoutes,  authProviders }  from './auth/login.routing';
import { CanDeactivateGuard } from './shared/services/can-deactivate-guard.service';
 
const appRoutes: Routes = [
  //{ path: 'report', component: ReportTempComponent , canActivate: [AuthGuard] },
  ...loginRoutes
  
   
];

export const appRoutingProviders: any[] = [
   authProviders,
   CanDeactivateGuard
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);


/*
Copyright 2016 Google Inc. All Rights Reserved.
Use of this source code is governed by an MIT-style license that
can be found in the LICENSE file at http://angular.io/license
*/