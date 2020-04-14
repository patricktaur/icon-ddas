import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../auth/auth-guard.service';
import {ExecuteDataExtractorComponent} from './execute-data-extractor/execute-data-extractor.component'
import {DataExtractionStatusComponent} from './reports/data-extraction-status.component'

//import {DataFileComponent} from '../Admin/data-file.component'
import {DataFileComponent} from './download-datafiles/download-datafile.component'
import {ExtractedDataMenuComponent} from './reports/extracted-data/extracted-data-menu.component'

import {FDADebarPageSiteDataComponent} from "./reports/extracted-data/fda-debar-page-site-data/fda-debar-page-site-data.component"
import {ERRProposalToDebarPageSiteDataComponent} from "./reports/extracted-data/err-proposal-to-debar-page-site-data/err-proposal-to-debar-page-site-data.component"
import {AdequateAssuranceListsiteDataComponent} from "./reports/extracted-data/adequate-assurance-list-site-data/adequate-assurance-list-site-data.component"
import {ClinicalInvestigatorDisqualificationSiteDataComponent} from "./reports/extracted-data/clinical-investigator-disqualification-site-data/clinical-investigator-disqualification-site-data.component"
import {PHSAdministrativeActionListingSiteDataComponent} from "./reports/extracted-data/phs-administrative-action-listing-site-data/phs-administrative-action-listing-site-data.component"
import  {CBERClinicalInvestigatorInspectionSiteDataComponent}  from './reports/extracted-data/cber-clinical-investigator-inspection-site-data/cber-clinical-investigator-inspection-site-data.component'
import  {CorporateIntegrityAgreementListSiteDataComponent}  from './reports/extracted-data/corporate-integrity-agreement-list-site-data/corporate-integrity-agreement-list-site-data.component'
import {FDAWarningLetterSiteDataComponent} from "./reports/extracted-data/fda-warning-letter-site-data/fda-warning-letter-site-data.component";


const dataExtractorRoutes: Routes = [
  {
    path: 'data-extractor',
    component: ExecuteDataExtractorComponent,
    canActivate:[AuthGuard]
  },
  { path: 'data-extraction-status', component: DataExtractionStatusComponent, canActivate: [AuthGuard] },
  { path: 'extracted-data-menu', component: ExtractedDataMenuComponent, canActivate: [AuthGuard] },

  { path: 'data-file', component: DataFileComponent, canActivate: [AuthGuard] },
  //{ path: 'cber-investigate', component: CBERClinicalInvestigatorInspectionSiteDataComponent},
  
  { path: 'fda-debar-page-site-data', component: FDADebarPageSiteDataComponent},
  { path: 'err-proposal-to-debar-page-site-data', component: ERRProposalToDebarPageSiteDataComponent},
  { path: 'adequate-assurance-list-site-data', component: AdequateAssuranceListsiteDataComponent},
  { path: 'clinical-investigator-disqualification-site-data', component: ClinicalInvestigatorDisqualificationSiteDataComponent},
  { path: 'phs-administrative-action-listing-site-data', component: PHSAdministrativeActionListingSiteDataComponent},
  { path: 'cber-clinical-investigator-inspection-site-data', component: CBERClinicalInvestigatorInspectionSiteDataComponent},
  { path: 'corporate-integrity-agreement-list-site-data', component: CorporateIntegrityAgreementListSiteDataComponent},
  { path: 'corporate-integrity-agreement-list-site-data', component: CorporateIntegrityAgreementListSiteDataComponent},
  { path: 'fda-warning-letter-site-data', component: FDAWarningLetterSiteDataComponent},

];

export const dataExtractorRouting: ModuleWithProviders = RouterModule.forChild(dataExtractorRoutes);
