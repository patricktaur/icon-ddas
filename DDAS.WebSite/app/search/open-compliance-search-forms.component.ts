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
    templateUrl: 'open-compliance-search-forms.component.html',

})
export class OpenComplianceFormsComponent implements OnInit {
    private PrincipalInvestigators: PrincipalInvestigatorDetails[];
    private zone: NgZone;
    public basicOptions: Object;
    public progress: number = 0;
    public response: any = {};
    public Loading: boolean = false;
    public uploadUrl: string;
    private error: any;

    public downloadUrl: string;
    public downloadTemplateUrl: string;
    public PrincipalInvestigatorNameToDownload: string;

    public ComplianceFormGenerationError: string;

    public filterStatus: number = -1;
    public filterInvestigatorName: string = "";

    @ViewChild('UploadComplianceFormInputsModal') modal: ModalComponent;


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
        });
        this.LoadPrincipalInvestigators();
    }

    LoadPrincipalInvestigators() {
        this.service.getPrincipalInvestigators()
            .subscribe((item: any) => {
                this.PrincipalInvestigators = item;
            },
            error => {
            });
    }

    get filteredRecords() {
        
        let filter1: PrincipalInvestigatorDetails[];

        filter1 = this.PrincipalInvestigators;
        if (this.filterInvestigatorName.length > 0) {
            filter1 =  this.PrincipalInvestigators.filter((a) =>
                a.Name.toLowerCase().includes(this.filterInvestigatorName.toLowerCase())
           )
        }
        
        let filter2: PrincipalInvestigatorDetails[];
        filter2 = filter1;
        if (this.filterStatus >=0){
            filter2 = filter1.filter((a) => 
                a.StatusEnum == this.filterStatus
            )
        }
        return filter2;
    }

    OpenForEdit(DataItem: PrincipalInvestigatorDetails) {

        this.router.navigate(['complianceform', DataItem.RecId], { relativeTo: this.route });

    }

    OpenNew() {
        this.router.navigate(['complianceform', ""], { relativeTo: this.route });
    }

    UploadFile() {
        this.Loading = false;
    }

    GenerateComplianceForm(inv: PrincipalInvestigatorDetails) {   //(formid: string){

        let formid = inv.RecId;
        this.PrincipalInvestigatorNameToDownload = inv.Name;
        this.downloadUrl = "";
        this.service.generateComplianceForm(formid)
            .subscribe((item: any) => {
                this.downloadUrl = this.configService.getApiHost() +  item;
                console.log("item:" + item);
                 console.log("this.downloadUrl:" + this.downloadUrl);
            },
            error => {
                this.ComplianceFormGenerationError = "Error: Compliance Form could not be generated."
            });
    }

    handleUpload(data: any): void {
        this.Loading = true;
        this.zone.run(() => {
            //this.response = data.response;
            if (data.response == null) {

            }
            else {
                this.Loading = false;
                this.modal.close();
                this.LoadPrincipalInvestigators();
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

    getBackgroundColor(color: number) {
        let retColor: string;

        switch (color) {
            case 0:
                retColor = "grey"; //Grey
                break;
            case 1:
                retColor = "#00b300";
                break;
            case 2:
                retColor = "#00ff00";
                break;
            case 3:
                retColor = "#b30000";
                break;
            case 4:
                retColor = "#ff0000";
                break;
            default: retColor = "grey";
        }
        return retColor;

    }

    get diagnostic() { return JSON.stringify(this.response); }
}