// #docregion
// #docregion import
import { Component, Input, Renderer } from '@angular/core';
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
  styleUrls: ['vertical-menu.css']
  
})
// #enddocregion metadata
// #docregion class
export class AppComponent {

 private appLocation: string;
 constructor(
        public authService: AuthService,
        private router: Router,
        renderer: Renderer
  ) { 

      let rootElement = renderer.selectRootElement('ddas-app');
       this.appLocation = rootElement.getAttribute('app-location');
       authService.appLocation = this.appLocation;
  }


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

  get showLocation(){
      if (this.appLocation != null){
          return true;
          //return (this.appLocation.length >0);
      }
      else {
          return false;
      }
      
  }

  //get diagnostic() { return JSON.stringify(this.authService.TestValue); }
}

 
// #enddocregion class
