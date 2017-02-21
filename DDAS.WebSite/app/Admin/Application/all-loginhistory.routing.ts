import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { LoginHistoryComponent } from './all-loginhistory.component';
import {ErrorImagesComponent} from './error-images.component';

export const LoggedInUserRoutes: Routes = [
  { path: 'all-loginhistory', component: LoginHistoryComponent},
<<<<<<< HEAD
  { path: 'error-images', component: ErrorImagesComponent}
=======
  //{ path: 'error-log', component: ErrorLogComponent},  

>>>>>>> a581a415be30b4f10a7e9b73f5bcd2960dab82dc
]


export const LoginHistoryRouting: ModuleWithProviders = RouterModule.forChild(LoggedInUserRoutes);