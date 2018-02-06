import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../auth/auth-guard.service';
import {ExecuteDataExtractorComponent} from './execute-data-extractor/execute-data-extractor.component'
import {DataExtractionStatusComponent} from './reports/data-extraction-status.component'
const dataExtractorRoutes: Routes = [
  {
    path: 'data-extractor',
    component: ExecuteDataExtractorComponent,
    canActivate:[AuthGuard]
  },
  { path: 'data-extraction-status', component: DataExtractionStatusComponent, canActivate: [AuthGuard] },
];

export const dataExtractorRouting: ModuleWithProviders = RouterModule.forChild(dataExtractorRoutes);
