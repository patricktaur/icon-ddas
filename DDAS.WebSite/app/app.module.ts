// #docregion
import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }    from '@angular/forms';

import { AppComponent }   from './app.component';
import { AuthService } from './auth/auth.service';
import { ConfigService } from './shared/utils/config.service';

import { DialogService }  from './shared/utils/dialog.service';

import { routing,
         appRoutingProviders } from './app.routing';

import { HttpModule, JsonpModule } from '@angular/http';       

import { LoginComponent } from './auth/login.component';
import { ChangePasswordComponent } from './auth/change-password.component';
import { SearchModule }         from './search/search.module';
import {SearchService} from './search/search-service';
import { ReportModule }         from './reports/report.module';
import { UserModule }         from './user/user.module';
import {LoggedInUserModule} from './LoggedInUser/LoggedInUser.module';

import {LoginHistoryModule} from './admin/all-loginhistory.module';
import {AppAdminModule} from './app-admin/app-admin.module';
import { HelpModule } from './help/help.module';
import { QCModule } from './qcworkflow/qc.module';
import {CommonService} from './shared/common.service';

@NgModule({
  imports:      [ 
    BrowserModule ,
     FormsModule,
     HttpModule,
    routing,
    SearchModule,
    ReportModule,
    UserModule,
    LoggedInUserModule,
    LoginHistoryModule,
    AppAdminModule,
    HelpModule,
    QCModule
  ],
  declarations: [ 
    AppComponent,
    //Move to Auth-Module
    LoginComponent,
    ChangePasswordComponent

   ],
  providers: [
    AuthService,
    appRoutingProviders,
    DialogService,
    ConfigService,
    SearchService,
    CommonService
    ],
  bootstrap:    [ AppComponent ]
})

export class AppModule { }
