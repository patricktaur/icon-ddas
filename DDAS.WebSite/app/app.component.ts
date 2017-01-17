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
  //styleUrls: ['..assets/css/styles.css'],  
})
// #enddocregion metadata
// #docregion class
export class AppComponent {

 constructor(
        public authService: AuthService,
        private router: Router
  ) {
    
    //authService.isAdmin
    //authService.isUser
  }

  
  logout(){
    this.router.navigate(['/login']);

    //this.authService.userName
    //this.authService.logout();

      // this.authService.logout()
      //       .subscribe((item: any) => {
      //          //this.router.navigate(['/login']);
      //       },
      //       error => {
                 
      //       });


  }  

  get diagnostic() { return JSON.stringify(this.authService.token); }
}

 
// #enddocregion class
