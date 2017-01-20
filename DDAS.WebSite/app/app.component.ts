// #docregion
// #docregion import
import { Component } from '@angular/core';
import { Router }   from '@angular/router';
import { AuthService } from './auth/auth.service';
// #enddocregion import

// #docregion metadata
@Component({
  // selector: 'my-app',
  // template: '<h1>My First Angular App</h1>'
 
  moduleId: module.id,
  selector: 'ddas-app',
  templateUrl: 'app.component.html',
  
})
// #enddocregion metadata
// #docregion class
export class AppComponent {

 constructor(
        public authService: AuthService,
        private router: Router
  ) { }

  // logout(){
    
  //   this.authService.logout();
  //   this.router.navigate(['/login']);
  // }  

  logout() {

    this.authService.logout()
            .subscribe(
                data => {
 
                    this.router.navigate(['/login']);
                    window.location.reload();
    
                },
                error => {
                   
                    //this.error = 'Could not logout.';
                    //this.loading = false;
                });
  }

  get diagnostic() { return JSON.stringify(this.authService.token); }
}

 
// #enddocregion class
