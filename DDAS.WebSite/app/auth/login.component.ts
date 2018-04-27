import { Component, OnInit }        from '@angular/core';
import { Router,
          ActivatedRoute,
         NavigationExtras,
         } from '@angular/router';
         import {LocationStrategy} from '@angular/common';         
import { AuthService }      from './auth.service';
import { loginInfo }      from './auth.classes';
import { ConfigService } from '../shared/utils/config.service';
@Component({

  moduleId: module.id,
  templateUrl: 'login.component.html' 
})
export class LoginComponent implements OnInit{
  message: string;
  // model: any = {};
    public logInfo: loginInfo;
    loading = false;
    error = '';
    returnUrl: string;
    redirectFullURL: string;
    public rememberChecked:boolean=false;

    
  constructor(public authService: AuthService, 
    public router: Router, 
    private route: ActivatedRoute,
    private locationStrategy : LocationStrategy,
    private configService: ConfigService) {
    this.setMessage();
    
  }

  ngOnInit() {
      this.authService.logout();
      
      //not workinng:
      this.route.queryParams
      .subscribe(params => this.returnUrl = params['return'] || '/');
     
     console.log("redirectURL:" + this.redirectURL());
     this.returnUrl = this.redirectURL() || '/';
      
      this.logInfo = new loginInfo();
      this.logInfo.username=localStorage.getItem('currentUsername');
      this.logInfo.password=localStorage.getItem('currentUserpassword');
  }
  
  setMessage() {
    this.message = 'Logged ' + (this.authService.isLoggedIn ? 'in' : 'out');
  }
 
  login() {
  
    this.loading = true;
 
        this.error = "";
        this.authService.login(this.logInfo.username, this.logInfo.password)
            .subscribe(
                data => {
                  
                  this.router.navigateByUrl(this.returnUrl);
                  
                  //this.router.navigate(['/' + this.returnUrl]);   

                  //this.router.navigate(['/']); 
                  
                  //   if(this.authService.isAppAdmin){
                  //      this.router.navigate(['/']);                      
                  //   }

                  //   if (this.authService.isAdmin){
                  //      this.router.navigate(['/']);
                  //      // this.router.navigate(['/users']);
                  //   }
                  //   else{
                  //     if (this.authService.isUser){
                        
                  //       this.router.navigate(['/']);
                  //   }
                  //   if (this.rememberChecked == true){
                  //           localStorage.setItem('currentUsername', this.logInfo.username);
                  //           localStorage.setItem('currentUserpassword', this.logInfo.password);
                  //   }
                  // }
                },
                error => {
                    //this.test = error;
                    
                    this.error = error.error_description; //'Access denied'; //Username or password is incorrect';
                    this.loading = false;
                });
    }
    
    checkedChange(e: any){
      localStorage.setItem('currentUsername',this.logInfo.username);
      localStorage.setItem('currentUserpassword',this.logInfo.password);
    }
  
  get appLocaation(){
    return this.authService.appLocation;
  }
  
  redirectURL() : string{
    //link requirements:site url + /login?returnUrl=start/ + page path + /end
    //example:
    //http://localhost:3000/login?returnUrl=start/qc/edit-qc/a0cd3a08-8d76-45dc-a2d0-4c7a13726abd/admin1/end
    let locationPath1 = this.locationStrategy.path();
    if (locationPath1.indexOf("start") == 0){
      return null;
    }
    if (locationPath1.indexOf("end") == 0){
      return null;
    }

    var locationPath =  locationPath1.replace(/%252F/g, "/");
    let startPos = locationPath.indexOf("start")+5
    let lastPos = locationPath.lastIndexOf("end");
    let length = lastPos - startPos;

    return   locationPath.substr(startPos, length-1);
  }
  
  ///
  
  // login() {
     
  //   this.message = 'Trying to log in ...';

  //   this.authService.login().subscribe(() => {
  //     this.setMessage();
  //     if (this.authService.isLoggedIn) {
  //       // Get the redirect URL from our auth service
  //       // If no redirect has been set, use the default
  //       let redirect = this.authService.redirectUrl ? this.authService.redirectUrl : '/search';
        
  //       // Set our navigation extras object
  //       // that passes on our global query params and fragment
  //       let navigationExtras: NavigationExtras = {
  //         preserveQueryParams: true,
  //         preserveFragment: true
  //       };

  //       // Redirect the user
  //       this.router.navigate([redirect], navigationExtras);
  //     }
  //   });
  // }

  logout() {

    this.authService.logout()
            .subscribe(
                data => {
                    
                },
                error => {
                   
                    this.error = 'Could not logout.';
                    this.loading = false;
                });
  }


  get CurrentVersion(){
        return this.configService.getVer();
    }
  //get diagnostic() { return JSON.stringify(this.authService.test); }
}


