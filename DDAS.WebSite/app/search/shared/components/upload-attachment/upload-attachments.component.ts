import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { ConfigService } from '../../../../shared/utils/config.service';

import { Attachment } from '../../../search.classes';

@Component({ 
	selector: 'upload-attachments',
	moduleId: module.id,
	templateUrl: 'upload-attachments.component.html'
})
export class UploadAttachmentsComponent implements OnInit {
	//@Input('SessionId') SessionId:string ;

	@Output() AttachmentsOutputChange = new EventEmitter<Attachment[]>();
    @Input() SessionId: string = "";
	//public uploadURL: string;
	public Attachments: Attachment[];
	constructor(
        private configService: ConfigService,
    ) { }
	ngOnInit() {
        
	}
	
	get uploadURL(){
		return this.configService.getApiURI() + "search/UploadComplianceFormAttachments?ComplianceFormId=" +  this.SessionId ;
	}


}