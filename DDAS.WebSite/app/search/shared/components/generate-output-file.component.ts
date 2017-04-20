import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ModalComponent } from '../../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { PrincipalInvestigatorDetails, ComplianceFormStatusEnum } from '../../search.classes';
import { SearchService } from '../../search-service';
import { ConfigService } from '../../../shared/utils/config.service';


@Component({
    selector: '[generate-output-file]',
    template: `
        
        <!--
        <button type="button" class="btn btn-primary" (click)=" open(); "
						title="Generate Output File"> <span class="glyphicon glyphicon-download-alt"></span> </button>
       -->
    <div>
    <modal #GenerateOutputFileModal cancelButtonLabel="Close">
	<modal-header>
        <h3>Generate Output File </h3>
		<h3>{{caption}}</h3>
	</modal-header>
	<modal-body>
		<h4>
			<div *ngIf="!downloadUrl">Generating Output File, Please wait ... </div>
			<div *ngIf="downloadUrl">
				<a href="{{downloadUrl}}" download>Output File is ready. Click here to download</a>
			</div>
			<p *ngIf="ComplianceFormGenerationError">{{ComplianceFormGenerationError}} </p>
		</h4>
	</modal-body>
	<modal-footer>
		<button type="button" class="btn btn-default" data-dismiss="GenerateOutputFileModal" (click)="close();">Close</button>
	</modal-footer>

</modal>
</div>
    `,
})
export class GenerateOutputFileComponent implements OnInit {
    @ViewChild('GenerateOutputFileModal') modal: ModalComponent;
    public PrincipalInvestigatorNameToDownload: string;
     public Loading: boolean = false;
 
    private error: any;

    public downloadUrl: string;
    public ComplianceFormGenerationError: string;

    @Input('formId') formId:string = "";
    @Input('caption') caption:string = "";

    constructor(
        private service: SearchService,
        private configService: ConfigService
        
    ) { }
    
    ngOnInit() {
        this.ComplianceFormGenerationError = "";
    }

    open(){
        console.log("opening the modal. open");
        this.modal.open();
        this.GenerateOutputFile();
    }
    
    generate(){
        console.log("opening the modal, generate");
        //this.formId = formId;
        //this.caption = caption;
        this.modal.open();
        this.GenerateOutputFile();
    }

    close(){
        this.modal.close();
    }

     GenerateOutputFile() {   //(formid: string){

        this.ComplianceFormGenerationError = "";
        this.downloadUrl = "";
        this.service.generateOutputFile()
            .subscribe((item: any) => {
                this.downloadUrl = this.configService.getApiHost() +  item;
                console.log("item:" + item);
                console.log("this.downloadUrl:" + this.downloadUrl);
            },
            error => {
                this.ComplianceFormGenerationError = "Error: Compliance Form could not be generated."
            });
    }
}