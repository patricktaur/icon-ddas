import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';

import {LoginHistoryComponent} from './all-loginhistory.component';

import {LoginHistoryRouting} from './all-loginhistory.Routing';

import {LoginHistoryService} from './all-loginhistory.service';
import {SharedModule} from '../../shared/shared.module'

import {ErrorImagesComponent} from './error-images.component';
import {ExtractionHistoryComponent} from './data-extraction-history.component';
import {DataExtractionComponent} from './data-extraction.component';
import {ManageSiteSourcesComponent} from './manage-site-sources.component';
import {EditSiteSourceComponent} from './edit-site-source.component';
import {AddCountryComponent} from './add-country.component';
import {ManageSponsorProtocolComponent} from './manage-sponsor-protocol.component';
import {DefaultSitesComponent} from './default-site-source.component';
import { Ng2Bs3ModalModule } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';

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
      ExtractionHistoryComponent,
      DataExtractionComponent,
      ManageSiteSourcesComponent,
      EditSiteSourceComponent,
      AddCountryComponent,
      ManageSponsorProtocolComponent,
      DefaultSitesComponent
  ],
 
  providers: [      
      LoginHistoryService
]
})

export class LoginHistoryModule {}