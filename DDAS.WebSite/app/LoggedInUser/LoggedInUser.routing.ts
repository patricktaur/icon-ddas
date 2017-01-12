import { ModuleWithProviders }   from '@angular/core';
import { Routes, RouterModule }  from '@angular/router';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { AboutDDASComponent }     from './about-ddas.component';

export const LoggedInUserRoutes: Routes = [
  { path: 'about-ddas', component: AboutDDASComponent}
]


export const LoggedInUserRouting: ModuleWithProviders = RouterModule.forChild(LoggedInUserRoutes);