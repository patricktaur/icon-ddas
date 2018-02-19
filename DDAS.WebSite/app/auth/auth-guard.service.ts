import { Injectable }             from '@angular/core';
import { CanActivate, Router,
         ActivatedRouteSnapshot,
         RouterStateSnapshot,
         NavigationExtras }       from '@angular/router';

import { AuthService }            from './auth.service';
import { QCListViewModel } from '../qcworkflow/qc.classes';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService, 
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this.authService.isLoggedIn) 
    { 
      console.log("Logged In");
      return true; 
    }
    else{
      console.log("Logged out");
      console.log("state.url: " + state.url);
      let qcUrls = state.url.split("openqc");
      let qcUrl :string;
      if (qcUrls.length > 0){
        qcUrl = qcUrls[0];
      }
      console.log("qcURl:" + qcUrl);
      this.router.navigate(['/login'], { queryParams: { returnUrl: qcUrl }});
      return false;
    }

    // // Store the attempted URL for redirecting
    // this.authService.redirectUrl = state.url;

    // // Create a dummy session id
    // let sessionId = 123456789;

    // // Set our navigation extras object
    // // that contains our global query params and fragment
    // let navigationExtras: NavigationExtras = {
    //   queryParams: { 'session_id': sessionId },
    //   fragment: 'anchor'
    // };

    // Navigate to the login page with extras
    //this.router.navigate(['/login'], navigationExtras);
   
    
  }
}


/*
Copyright 2016 Google Inc. All Rights Reserved.
Use of this source code is governed by an MIT-style license that
can be found in the LICENSE file at http://angular.io/license
*/