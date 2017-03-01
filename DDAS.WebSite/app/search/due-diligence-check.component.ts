import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { PrincipalInvestigatorDetails } from './search.classes';
import { SearchService } from './search-service';
import { ConfigService } from '../shared/utils/config.service';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../auth/auth.service';

//import { Http, Response, Headers , RequestOptions } from '@angular/http';

@Component({
    moduleId: module.id,
    templateUrl: 'due-diligence-check.component.html',
})
export class DueDiligenceCheckComponent implements OnInit {
    private PrincipalInvestigators: PrincipalInvestigatorDetails[];
    private zone: NgZone;
    public basicOptions: Object;
    public progress: number = 0;
    public response: any = {};
    public Loading: boolean = false;
    public uploadUrl: string;
    public validationMessage: string;
   

    private error: any;

    public downloadUrl: string;
    public downloadTemplateUrl: string;
    public PrincipalInvestigatorNameToDownload: string;

    public ComplianceFormGenerationError: string;

    public filterStatus: number = -1;
    public filterInvestigatorName: string = "";


    @ViewChild('UploadComplianceFormInputsModal') modal: ModalComponent;

    public pageNumber: number;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private service: SearchService,
        private configService: ConfigService,
        private authService: AuthService
    ) { }

    ngOnInit() {

        this.uploadUrl = this.configService.getApiURI() + "search/Upload";
        this.downloadUrl = this.configService.getApiHost() + "Downloads";

        this.downloadTemplateUrl = this.configService.getApiHost() + "DataFiles/Templates/DDAS_Upload_Template.xlsx";
     
        this.zone = new NgZone({ enableLongStackTrace: false });
        this.basicOptions = {
            url: this.uploadUrl,
            authToken: this.authService.token,
            authTokenPrefix: 'Bearer'
        };

        this.route.params.forEach((params: Params) => {
            let page = +params['page'];
            if (page != null){
                this.pageNumber = page;
            }
        });
        this.LoadPrincipalInvestigators();

        // this.validationMessages.push("aaa");
        // this.validationMessages.push("bbb");
    }

    LoadPrincipalInvestigators() {
        this.service.getMyReviewPendingPrincipalInvestigators()
            .subscribe((item: any) => {
                this.PrincipalInvestigators = item;
            },
            error => {
            });
    }

    get filteredRecords() {
        return this.PrincipalInvestigators;
    }

    get extractionPendingRecordCount(): number {
        if (this.PrincipalInvestigators == null){
            return null;
        }
        else{
            return this.PrincipalInvestigators.filter(x => x.ExtractionPendingInvestigatorCount > 0).length;
        }
        
    }
    
      get reviewPendingRecordCount(): number {
        if (this.PrincipalInvestigators == null){
            return null;
        }
        else{
            //Records are already filtered for Review Pending.  
            return this.PrincipalInvestigators.filter(x => x.ExtractionPendingInvestigatorCount == 0).length;
        }
        
    }
    OpenForEdit(DataItem: PrincipalInvestigatorDetails) {

        //this.router.navigate(['complianceform', DataItem.RecId, {rootPath:'search', page:this.pageNumber}], { relativeTo: this.route });
        this.router.navigate(['comp-form-edit', DataItem.RecId, {rootPath:'search', page:this.pageNumber}], { relativeTo: this.route });


    }

    OpenNew() {
        this.router.navigate(['comp-form-edit', ""], { relativeTo: this.route });
    }

    CleanUpValidationMessage() {
        this.validationMessage = null;
        this.Loading = false;
    }



    handleUpload(data: any): void {
        this.Loading = true;
        this.zone.run(() => {
            //this.response = data.response;
            if (data.response == null) {

            }
            else {
                this.Loading = false;
                
                this.validationMessage = data.response; 
                let ok = "\"ok\"";
                if (this.validationMessage == ok){
                    this.modal.close();
                    this.LoadPrincipalInvestigators();
                }
                else{
                    
                }
   
            }

            this.progress = data.progress.percent / 100;
            //this.Loading = false;
        });
    }

    DownloadCompForm(formid: string) {

        this.service.downLoadComplianceForm(formid)
            .subscribe((item: any) => {

            },
            error => {
            });
    }

  Reload(){

      this.LoadPrincipalInvestigators();
  }

    get diagnostic() { return JSON.stringify(this.validationMessage); }
}