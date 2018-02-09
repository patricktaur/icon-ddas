import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module'
import {DataExtractorService} from './data-extractor-service';
import {dataExtractorRouting} from './data-extractor.routing';
import {DataExtractionIndicatorComponent} from './shared/components/data-extraction-indicator.component'
import {DataExtractionStatusComponent } from './reports/data-extraction-status.component'
import {ExecuteDataExtractorComponent} from './execute-data-extractor/execute-data-extractor.component'
import {StatusActivatorService} from './shared/service/status.activator.service'
//import { DataFileComponent } from './data-file.component';
import {DataFileComponent} from './download-datafiles/download-datafile.component'
import {ExtractedDataMenuComponent} from './reports/extracted-data/extracted-data-menu.component'

import {FDADebarPageSiteDataComponent} from "./reports/extracted-data/fda-debar-page-site-data/fda-debar-page-site-data.component"
import {ERRProposalToDebarPageSiteDataComponent} from "./reports/extracted-data/err-proposal-to-debar-page-site-data/err-proposal-to-debar-page-site-data.component"
import {AdequateAssuranceListsiteDataComponent} from "./reports/extracted-data/adequate-assurance-list-site-data/adequate-assurance-list-site-data.component"
import {ClinicalInvestigatorDisqualificationSiteDataComponent} from "./reports/extracted-data/clinical-investigator-disqualification-site-data/clinical-investigator-disqualification-site-data.component"
import {PHSAdministrativeActionListingSiteDataComponent} from "./reports/extracted-data/phs-administrative-action-listing-site-data/phs-administrative-action-listing-site-data.component"
import  {CBERClinicalInvestigatorInspectionSiteDataComponent}  from './reports/extracted-data/cber-clinical-investigator-inspection-site-data/cber-clinical-investigator-inspection-site-data.component'
import  {CorporateIntegrityAgreementListSiteDataComponent}  from './reports/extracted-data/corporate-integrity-agreement-list-site-data/corporate-integrity-agreement-list-site-data.component'


@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    dataExtractorRouting
  ],
  declarations: [
    DataExtractionIndicatorComponent,
    ExecuteDataExtractorComponent,
    DataExtractionStatusComponent,
    DataFileComponent,
    ExtractedDataMenuComponent,

    //Extracted Data:
    FDADebarPageSiteDataComponent,
    ERRProposalToDebarPageSiteDataComponent,
    AdequateAssuranceListsiteDataComponent,
    ClinicalInvestigatorDisqualificationSiteDataComponent,
    PHSAdministrativeActionListingSiteDataComponent,
    PHSAdministrativeActionListingSiteDataComponent,
    CBERClinicalInvestigatorInspectionSiteDataComponent,
    CorporateIntegrityAgreementListSiteDataComponent
  ],

  providers: [
    DataExtractorService,
    StatusActivatorService
  ],
  exports: [
    DataExtractionIndicatorComponent
   ]
})
export class DataExtractorModule { }