import { Routes }         from '@angular/router';
import { AuthGuard }      from './auth-guard.service';
import { AuthService }    from './auth.service';
import { LoginComponent } from './login.component';
import { ChangePasswordComponent } from './change-password.component';
import {NotFoundComponent} from '../shared/components/not-found.component'
import { CanDeactivateGuard } from '../shared/services/can-deactivate-guard.service';
import {EditQCComponent}  from '../qcworkflow/edit-qc/edit-qc.component';

export const loginRoutes: Routes = [
 
  
  { path: 'login', component: LoginComponent },
  { path: 'logout', redirectTo: '/login' },
  { path: 'changepassword', component: ChangePasswordComponent, canActivate: [AuthGuard] },
  {path: '',     redirectTo: '/login',  pathMatch: 'full'  },
      // { path: '**', redirectTo: '/login', pathMatch: 'full'  },
      { path: "**", component: NotFoundComponent, data: { title: "Page Not Found" } }

    ];

export const authProviders = [
  AuthGuard,
  AuthService
];


/*
Copyright 2016 Google Inc. All Rights Reserved.
Use of this source code is governed by an MIT-style license that
can be found in the LICENSE file at http://angular.io/license
*/