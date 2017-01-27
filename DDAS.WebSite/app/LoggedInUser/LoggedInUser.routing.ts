import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { AboutDDASComponent }     from './about-ddas.component';
import {myLoginHistoryComponent} from './my-loginhistory.component';

export const LoggedInUserRoutes: Routes = [
  { path: 'about-ddas', component: AboutDDASComponent},
  {path: 'my-loginhistory', component: myLoginHistoryComponent}
]


export const LoggedInUserRouting: ModuleWithProviders = RouterModule.forChild(LoggedInUserRoutes);