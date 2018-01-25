import { Component, Input, OnInit } from '@angular/core';
import { ConfigService } from '../../../../shared/utils/config.service';

import { ComplianceFormStatusEnum } from '../../../search.classes';

@Component({ 
	selector: 'upload-attachments',
	moduleId: module.id,
	templateUrl: 'upload-attachments.component.html'
})
export class UploadAttachmentsComponent implements OnInit {
	@Input('SessionId') SessionId:string ;
	//public uploadURL: string;
	constructor(
        private configService: ConfigService,
    ) { }
	ngOnInit() {
        
	}
	
	get uploadURL(){
		return this.configService.getApiURI() + "search/UploadAttachments?SessionId=" + this.SessionId;
	}
}