import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { LoginHistoryComponent }     from './all-loginhistory.component';

export const LoggedInUserRoutes: Routes = [
  { path: 'all-loginhistory', component: LoginHistoryComponent}
]


export const LoginHistoryRouting: ModuleWithProviders = RouterModule.forChild(LoggedInUserRoutes);