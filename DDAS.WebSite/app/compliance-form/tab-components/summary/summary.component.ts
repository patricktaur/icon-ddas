import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';
import { SearchService } from '../../../search/search-service';
import { ConfigService } from '../../../shared/utils/config.service';

@Component({
    selector: '[comp-form-summary]',
    moduleId: module.id,
    templateUrl: 'summary.component.html'
})
export class ComplianceFormSummaryComponent {
    @Input() CompForm: ComplianceFormA;
    fileUploaded: any;

    constructor(
        private service: SearchService,
        private configService: ConfigService
    ) {

    }

    downloadUploadedFile(generatedFileName: string){
        this.service.getUploadedFile(generatedFileName, this.CompForm.UploadedFileName)
        .subscribe((item: any) => {
            this.fileUploaded = this.configService.getApiHost() + item;
        });
    }
}