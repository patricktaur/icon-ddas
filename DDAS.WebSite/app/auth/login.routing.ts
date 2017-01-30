import { Routes }         from '@angular/router';
import { AuthGuard }      from './auth-guard.service';
import { AuthService }    from './auth.service';
import { LoginComponent } from './login.component';
import { ChangePasswordComponent } from './change-password.component';

export const loginRoutes: Routes = [
 
  
  { path: 'login', component: LoginComponent },
  { path: 'logout', redirectTo: '/login' },
    { path: 'changepassword', component: ChangePasswordComponent },
     {path: '',     redirectTo: '/login',  pathMatch: 'full'  },
      { path: '**',redirectTo: '/login', pathMatch: 'full'  }
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