import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { 
    PrincipalInvestigatorDetails, 
    ComplianceFormManage, 
    CompFormFilter,
    ComplianceFormA,
    Review,
    ReviewerRoleEnum,
    ReviewStatusEnum
    } from './search.classes';
import { QualityCheck, AuditObservation } from '../qcworkflow/qc.classes';
import { SearchService } from './search-service';
import { ConfigService } from '../shared/utils/config.service';
import { ModalComponent } from '../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../auth/auth.service';
import { IMyDate, IMyDateModel } from '../shared/utils/my-date-picker/interfaces';
//import { Http, Response, Headers , RequestOptions } from '@angular/http';

@Component({
    moduleId: module.id,
    templateUrl: 'review-completed-icsf.component.html',
})
export class ReviewCompletedICSFComponent implements OnInit {
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

    public makeActiveCompFormId: string;
    public makeActivePrincipalInvestigatorName: string;

    public ComplianceFormFilter: CompFormFilter;

    public FromDate: IMyDateModel;
    public ToDate: IMyDateModel;

    public myDatePickerOptions = {

        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
    };

    public Users: any[];
    public SelectedInvestigatorName: string;
    public SelectedComplianceFormId: string;
    public audit: QualityCheck;
    public pageNumber: number;
    public compForm: ComplianceFormA;
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
            if (page != null) {
                this.pageNumber = page;
            }
        });
        this.ComplianceFormFilter = new CompFormFilter;
        this.audit = new QualityCheck;
        this.SetDefaultFilterValues();
        this.LoadPrincipalInvestigators();
        this.LoadUsers();
    }

    LoadUsers() {
        this.service.getAllUsers()
            .subscribe((item: any[]) => {
                this.Users = item;
            });
    }

    setSelectedRecordDetails(complainceFormId: string) {
        this.SelectedComplianceFormId = complainceFormId;
        console.log('comp form id: ', this.SelectedComplianceFormId);
    }

    SetDefaultFilterValues() {
        this.ComplianceFormFilter.InvestigatorName = null;
        this.ComplianceFormFilter.ProjectNumber = null;
        this.ComplianceFormFilter.SponsorProtocolNumber = null;
        this.ComplianceFormFilter.SearchedOnFrom = null;
        this.ComplianceFormFilter.SearchedOnTo = null;
        this.ComplianceFormFilter.Country = null;
        this.ComplianceFormFilter.AssignedTo = "-1";
        this.ComplianceFormFilter.Status = -1;

        var fromDay = new Date();
        fromDay.setDate(fromDay.getDate() - 10);

        this.FromDate = {
            date: {
                year: fromDay.getFullYear(), month: fromDay.getMonth() + 1, day: fromDay.getDate()
            },
            jsdate: '',
            formatted: '',
            epoc: null
        }
        var today = new Date();
        this.ToDate = {
            date: {
                year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate()
            },
            jsdate: '',
            formatted: '',
            epoc: null
        }
    }

    LoadPrincipalInvestigators() {

        if (this.FromDate != null) {
            //minus one month, plus one day is made so that the value is correctly converted on the server side.  
            //Otherwise incorrect values are produced when the property is read on API end point.
            this.ComplianceFormFilter.SearchedOnFrom = new Date(this.FromDate.date.year, this.FromDate.date.month - 1, this.FromDate.date.day + 1);
        }

        if (this.ToDate != null) {
            this.ComplianceFormFilter.SearchedOnTo = new Date(this.ToDate.date.year, this.ToDate.date.month - 1, this.ToDate.date.day + 1);
        }

        this.service.getClosedComplianceFormFilters(this.ComplianceFormFilter)
            .subscribe((item: any) => {
                this.PrincipalInvestigators = item;
            });

        // this.service.getMyReviewCompletedPrincipalInvestigators()
        //     .subscribe((item: any) => {
        //         this.PrincipalInvestigators = item;
        //     },
        //     error => {
        //     });
    }

    status(reviewStatus: number) {
        switch (reviewStatus) {
            case ReviewStatusEnum.SearchCompleted: return "Search Completed";
            case ReviewStatusEnum.ReviewInProgress: return "Review In Progress";
            case ReviewStatusEnum.ReviewCompleted: return "Review Completed";
            case ReviewStatusEnum.Completed: return "Completed";
            case ReviewStatusEnum.QCRequested: return "QC Requested";
            case ReviewStatusEnum.QCInProgress: return "QC In Progress";
            case ReviewStatusEnum.QCFailed: return "QC Failed";
            case ReviewStatusEnum.QCPassed: return "QC Passed";
            case ReviewStatusEnum.QCCorrectionInProgress: return "QC Correction In Progress";
            default: "";
        }
    }

    isQCRequested(currentStatus: number){
        if(currentStatus == ReviewStatusEnum.QCRequested ||
            currentStatus == ReviewStatusEnum.QCInProgress ||
            currentStatus == ReviewStatusEnum.QCFailed ||
            currentStatus == ReviewStatusEnum.QCPassed ||
            currentStatus == ReviewStatusEnum.QCCorrectionInProgress ||
            currentStatus == ReviewStatusEnum.Completed)
            return false;
        else if(currentStatus == ReviewStatusEnum.ReviewCompleted)
            return true;
    }

    get filteredRecords() {
        return this.PrincipalInvestigators;
    }

    setCompFormActiveValue(DataItem: PrincipalInvestigatorDetails) {

        this.makeActiveCompFormId = DataItem.RecId;
        this.makeActivePrincipalInvestigatorName = DataItem.Name;
    }
    makeCompFormActiveValue() {
        this.service.OpenComplianceForm(this.makeActiveCompFormId)
            .subscribe((item: any) => {
                this.LoadPrincipalInvestigators();
            },
            error => {
            });
    }

    OpenNew() {
        this.router.navigate(['complianceform', "", { rootPath: 'review-completed-icsf', page: this.pageNumber }], { relativeTo: this.route });
    }

    OpenForEdit(DataItem: PrincipalInvestigatorDetails) {
        //this.router.navigate(['complianceform', DataItem.RecId], { relativeTo: this.route });
        this.router.navigate(['comp-form-edit', DataItem.RecId, { rootPath: 'review-completed-icsf', page: this.pageNumber }], { relativeTo: this.route });

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
                this.downloadUrl = this.configService.getApiHost() + item;
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

    downloadComplianceForm(formId: string) {
        this.service.generateComplianceForm(formId)
            .subscribe((item: any) => {

            },
            error => {
            });
    }

    downloadComplianceFormPDF(formId: string) {
        this.service.generateComplianceFormPDF(formId)
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
                retColor = "green";
                //retColor = "#00b300";
                break;
            case 2:
                retColor = "lawngreen";
                //retColor = "#00ff00";
                break;
            case 3:
                retColor = "red";
                //retColor = "#b30000";
                break;
            case 4:
                retColor = "lightcoral";
                //retColor = "#ff0000";
                break;
            default: retColor = "grey";
        }
        return retColor;

    }

    requestAudit(qcVerifier: string, requestorComments:string) {
        if(qcVerifier == null || qcVerifier.length == 0){
            alert('please select an auditor');
            return;
        }        
        else if(this.authService.userName.toLowerCase() == qcVerifier.toLowerCase()){
            alert('cannot assign the audit to yourself');
            return;
        }
        
        this.service.getComplianceForm(this.SelectedComplianceFormId)
        .subscribe((item: ComplianceFormA) => {
            this.compForm = item;
        },
        error =>{

        });
        this.getComplianceForm(qcVerifier, requestorComments);
    }

    getComplianceForm(qcVerifier: string, requestorComments: string) {
        this.service.getComplianceForm(this.SelectedComplianceFormId)
        .subscribe((item: ComplianceFormA) => {
            this.compForm = item;
            this.postComplianceFormWithQC(qcVerifier, requestorComments);
        },
        error =>{

        });
    }

    postComplianceFormWithQC(qcVerifier: string, requestorComments: string){
        var review = new Review();
        review.RecId = null;
        review.AssigendTo = qcVerifier;
        review.AssignedBy = this.authService.userName;
        review.AssignedOn = new Date();
        review.Status = ReviewStatusEnum.QCRequested;
        review.ReviewerRole = ReviewerRoleEnum.QCVerifier;
        review.Comment = requestorComments;
        review.StartedOn = null;
        review.CompletedOn = null;
        this.compForm.Reviews.push(review);
        
        this.service.requestQC(this.compForm)
            .subscribe((item: boolean) => {
                this.LoadPrincipalInvestigators();
            },
            error => {

            });
    }

    get diagnostic() { return JSON.stringify(this.PrincipalInvestigators); }
}