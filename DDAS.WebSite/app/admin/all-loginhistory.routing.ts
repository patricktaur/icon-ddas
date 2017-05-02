import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { LoginHistoryComponent } from './all-loginhistory.component';
import {ErrorImagesComponent} from './error-images.component';
import {ExtractionHistoryComponent} from './data-extraction-history.component';
import {DataExtractionComponent} from './data-extraction.component';
import {ManageSiteSourcesComponent} from './manage-site-sources.component';
import {EditSiteSourceComponent} from './edit-site-source.component';
import {AddCountryComponent} from './country-site.component';
import {ManageSponsorProtocolComponent} from './manage-sponsor-protocol.component';
import {DefaultSitesComponent} from './default-site-source.component';
import {DefaultSiteSourceEditComponent} from './default-site-source-edit.component'
import { AuthGuard } from '../auth/auth-guard.service';

export const LoggedInUserRoutes: Routes = [
  { path: 'all-loginhistory', component: LoginHistoryComponent, canActivate: [AuthGuard]},
  { path: 'error-images', component: ErrorImagesComponent, canActivate: [AuthGuard]},
  { path: 'data-extraction-history', component: ExtractionHistoryComponent, canActivate: [AuthGuard]},
  { path: 'data-extraction', component: DataExtractionComponent, canActivate: [AuthGuard]},
  { path: 'manage-site-sources', component: ManageSiteSourcesComponent, canActivate: [AuthGuard]},
  { path: 'edit-site-source/:RecId', component: EditSiteSourceComponent, canActivate: [AuthGuard]},
  { path:'', redirectTo:'/manage-site-sources', pathMatch:'full'},
  { path: 'add-country', component: AddCountryComponent, canActivate: [AuthGuard]},
  {path: 'manage-sponsor-protocol', component: ManageSponsorProtocolComponent, canActivate: [AuthGuard]},
  {path: 'default-sites', component: DefaultSitesComponent, canActivate: [AuthGuard]},
  {path: 'default-site-edit/:RecId', component: DefaultSiteSourceEditComponent, canActivate: [AuthGuard]},
  //{ path: 'error-log', component: ErrorLogComponent},
  
]


export const LoginHistoryRouting: ModuleWithProviders = RouterModule.forChild(LoggedInUserRoutes);