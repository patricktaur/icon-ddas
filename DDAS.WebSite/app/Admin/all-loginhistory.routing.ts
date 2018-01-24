import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// import { routing,
//          appRoutingProviders } from './app.routing';
import { LoginHistoryComponent } from './all-loginhistory.component';
import { ErrorImagesComponent } from './error-images.component';
import { ExtractionHistoryComponent } from './data-extraction-history.component';
import { DataExtractionComponent } from './data-extraction.component';
import { ManageSiteSourcesComponent } from './manage-site-sources.component';
import { EditSiteSourceComponent } from './edit-site-source.component';
import { AddCountryComponent } from './country-site.component';
import { ManageSponsorProtocolComponent } from './manage-sponsor-protocol.component';
import { DefaultSitesComponent } from './default-site-source.component';
import { DefaultSiteSourceEditComponent } from './default-site-source-edit.component';
import { CountrySiteEditComponent } from './country-site-edit.component';
import { SponsorSpecificSiteEditComponent } from './sponsor-protocol-edit.component';
import { ExceptionLogComponent } from './exception-logger.component';
import { ExtractionLogComponent } from './extraction-log.component';
import { DataFileComponent } from './data-file.component';
import { AuthGuard } from '../auth/auth-guard.service';
import { ISprintToDDASLogComponent } from './isprint-to-ddas-log.component';

export const LoggedInUserRoutes: Routes = [
  { path: 'all-loginhistory', component: LoginHistoryComponent, canActivate: [AuthGuard] },
  { path: 'error-images', component: ErrorImagesComponent, canActivate: [AuthGuard] },
  { path: 'data-extraction-history', component: ExtractionHistoryComponent, canActivate: [AuthGuard] },
  { path: 'data-extraction', component: DataExtractionComponent, canActivate: [AuthGuard] },
  { path: 'manage-site-sources', component: ManageSiteSourcesComponent, canActivate: [AuthGuard] },
  { path: 'edit-site-source/:RecId', component: EditSiteSourceComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/manage-site-sources', pathMatch: 'full' },
  { path: 'country-site', component: AddCountryComponent, canActivate: [AuthGuard] },
  { path: 'manage-sponsor-protocol', component: ManageSponsorProtocolComponent, canActivate: [AuthGuard] },
  { path: 'default-sites', component: DefaultSitesComponent, canActivate: [AuthGuard] },
  { path: 'default-site-edit/:RecId', component: DefaultSiteSourceEditComponent, canActivate: [AuthGuard] },
  { path: 'country-site-edit/:RecId', component: CountrySiteEditComponent, canActivate: [AuthGuard] },
  { path: 'sponsor-protocol-edit/:RecId', component: SponsorSpecificSiteEditComponent, canActivate: [AuthGuard] },
  { path: 'exception-logger', component: ExceptionLogComponent, canActivate: [AuthGuard] },
  { path: 'extraction-log', component: ExtractionLogComponent, canActivate: [AuthGuard] },
  { path: 'data-file', component: DataFileComponent, canActivate: [AuthGuard] },
  { path: 'isprint-to-ddas-log', component: ISprintToDDASLogComponent, canActivate: [AuthGuard] }
  //{ path: 'error-log', component: ErrorLogComponent},
]

export const LoginHistoryRouting: ModuleWithProviders = RouterModule.forChild(LoggedInUserRoutes);