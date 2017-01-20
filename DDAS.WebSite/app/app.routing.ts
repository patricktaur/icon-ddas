import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule }   from '@angular/router';
import {ReportTempComponent} from './report-temp.component';

import { AuthGuard }             from './auth/auth-guard.service';

import { loginRoutes,  authProviders }  from './auth/login.routing';

import { CanDeactivateGuard } from './shared/services/can-deactivate-guard.service';
//import {  LoginComponent }  from './auth/login.component';
 
const appRoutes: Routes = [
  //{ path: 'login', component: LoginComponent  },
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