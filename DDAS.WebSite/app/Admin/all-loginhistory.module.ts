import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';

import {LoginHistoryComponent} from './all-loginhistory.component';

import {LoginHistoryRouting} from './all-loginhistory.Routing';

import {LoginHistoryService} from './all-loginhistory.service';
import {SharedModule} from '../shared/shared.module'

import {ErrorImagesComponent} from './error-images.component';
//import {ExtractionHistoryComponent} from './data-extraction-history.component';
//import {DataExtractionStatusComponent} from '../data-extractor/reports/data-extraction-status.component'
import {DataExtractionComponent} from './data-extraction.component';
import {ManageSiteSourcesComponent} from './manage-site-sources.component';
import {EditSiteSourceComponent} from './edit-site-source.component';
import {AddCountryComponent} from './country-site.component';
import {ManageSponsorProtocolComponent} from './manage-sponsor-protocol.component';
import {DefaultSitesComponent} from './default-site-source.component';
import {DefaultSiteSourceEditComponent} from './default-site-source-edit.component';
import { CountrySiteEditComponent } from './country-site-edit.component';
import { SponsorSpecificSiteEditComponent} from './sponsor-protocol-edit.component';
import { ExceptionLogComponent } from './exception-logger.component';
import { ExtractionLogComponent } from './extraction-log.component';
// import { DataFileComponent } from './data-file.component';
import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
//import { SiteSourceToAddComponent } from '../shared/components/site-source-toadd.component';
import { ISprintToDDASLogComponent } from './isprint-to-ddas-log.component';
import { DDAStoiSprintLogComponent } from './ddas-to-isprint-log.component';
import {SamApiKeyEditComponent} from './sam-api-key-edit.component'
@NgModule({ 
  imports: [
      LoginHistoryRouting,
      CommonModule,
      FormsModule,
      SharedModule,
      Ng2Bs3ModalModule
  ],
  declarations: [
      LoginHistoryComponent,
      ErrorImagesComponent,
      //DataExtractionStatusComponent,
      DataExtractionComponent,
      ManageSiteSourcesComponent,
      EditSiteSourceComponent,
      AddCountryComponent,
      ManageSponsorProtocolComponent,
      DefaultSitesComponent,
      DefaultSiteSourceEditComponent,
      CountrySiteEditComponent,
      SponsorSpecificSiteEditComponent,
      ExceptionLogComponent,
      ExtractionLogComponent,
      //DataFileComponent,
      ISprintToDDASLogComponent,
      DDAStoiSprintLogComponent,
      SamApiKeyEditComponent
      //SiteSourceToAddComponent
  ],
 
  providers: [      
      LoginHistoryService
]
})

export class LoginHistoryModule {}