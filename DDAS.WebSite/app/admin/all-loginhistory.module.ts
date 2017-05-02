import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';

import {LoginHistoryComponent} from './all-loginhistory.component';

import {LoginHistoryRouting} from './all-loginhistory.Routing';

import {LoginHistoryService} from './all-loginhistory.service';
import {SharedModule} from '../shared/shared.module'

import {ErrorImagesComponent} from './error-images.component';
import {ExtractionHistoryComponent} from './data-extraction-history.component';
import {DataExtractionComponent} from './data-extraction.component';
import {ManageSiteSourcesComponent} from './manage-site-sources.component';
import {EditSiteSourceComponent} from './edit-site-source.component';
import {AddCountryComponent} from './country-site.component';
import {ManageSponsorProtocolComponent} from './manage-sponsor-protocol.component';
import {DefaultSitesComponent} from './default-site-source.component';
import {DefaultSiteSourceEditComponent} from './default-site-source-edit.component'

import { Ng2Bs3ModalModule } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
//C:\Development\p926-ddas-admin\DDAS.WebSite\app\shared\components\site-source-toadd.component.ts
//import { SiteSourceToAddComponent } from '../shared/components/site-source-toadd.component';

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
      DefaultSitesComponent,
      DefaultSiteSourceEditComponent,
      //SiteSourceToAddComponent
  ],
 
  providers: [      
      LoginHistoryService
]
})

export class LoginHistoryModule {}