import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { LoginHistoryComponent } from './all-loginhistory.component';
import {ErrorImagesComponent} from './error-images.component';
import {ExtractionHistoryComponent} from './data-extraction-history.component';
import {DataExtractionComponent} from './data-extraction.component';
import {ManageSiteSourcesComponent} from './manage-site-sources-component';

export const LoggedInUserRoutes: Routes = [
  { path: 'all-loginhistory', component: LoginHistoryComponent},
  { path: 'error-images', component: ErrorImagesComponent},
  { path: 'data-extraction-history', component: ExtractionHistoryComponent},
  { path: 'data-extraction', component: DataExtractionComponent},
  {path: 'manage-site-sources', component: ManageSiteSourcesComponent}
  //{ path: 'error-log', component: ErrorLogComponent}, 
]


export const LoginHistoryRouting: ModuleWithProviders = RouterModule.forChild(LoggedInUserRoutes);