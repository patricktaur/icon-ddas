import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ModalComponent } from '../../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { PrincipalInvestigatorDetails, ComplianceFormStatusEnum } from '../../search.classes';
import { SearchService } from '../../search-service';
import { ConfigService } from '../../../shared/utils/config.service';


@Component({
    selector: '[download-compliance-form]',
    template: `
        
        <!--
        <button type="button" class="btn btn-primary" (click)=" open(); "
						title="Download Compliance Form"> <span class="glyphicon glyphicon-download-alt"></span> </button>
       -->
    <modal #DownloadComplianceFormModal cancelButtonLabel="Close">
	<modal-header>
        <h3>Download Compliance Form </h3>
		<h3>{{caption}}</h3>
	</modal-header>
	<modal-body>
		<h4>
			<div *ngIf="!downloadUrl">Generating Compliance Form, Please wait ... </div>
			<div *ngIf="downloadUrl">
				<a href="{{downloadUrl}}" download>Compliance Form is ready. Click here to download</a>
			</div>
			<p *ngIf="ComplianceFormGenerationError">{{ComplianceFormGenerationError}} </p>
		</h4>
	</modal-body>
	<modal-footer>
		<button type="button" class="btn btn-default" data-dismiss="DownloadComplianceFormModal" (click)="close();">Close</button>
	</modal-footer>

</modal>

    `,
})
export class DownloadComplianceFormComponent implements OnInit {
    @ViewChild('DownloadComplianceFormModal') modal: ModalComponent;
    public PrincipalInvestigatorNameToDownload: string;
    public Loading: boolean = false;

    private error: any;

    public downloadUrl: string;
    public ComplianceFormGenerationError: string;

    @Input('formId') formId: string = "";
    @Input('caption') caption: string = "";

    constructor(

        private service: SearchService,
        private configService: ConfigService

    ) { }

    ngOnInit() {
        this.ComplianceFormGenerationError = "";
    }

    open() {
        this.modal.open();
        this.GenerateComplianceForm();
    }

    generate(formId: string, caption: string) {
        this.formId = formId;
        this.caption = caption;
        this.modal.open();
        this.GenerateComplianceForm();
    }

    canGenerate(status: ComplianceFormStatusEnum): boolean {
        if (status == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
            status == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified) {
            return true;
        }
        else {
            return false;
        }

    }

    Info(status: ComplianceFormStatusEnum): string {
        if (status == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
            status == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified) {
            return "Generate and download Compliance Form";
        }
        else {
            return "Review not completed.  Cannot generate Compliance Form";
        }
    }

    close() {
        this.modal.close();
    }

    GenerateComplianceForm() {   //(formid: string){
        this.ComplianceFormGenerationError = "";
        this.downloadUrl = "";
        this.service.generateComplianceForm(this.formId)
            .subscribe((item: any) => {
                this.downloadUrl = this.configService.getApiHost() + item;
                console.log("item:" + item);
                console.log("this.downloadUrl:" + this.downloadUrl);
            },
            error => {
                this.ComplianceFormGenerationError = "Error: Compliance Form could not be generated."
            });
    }
}