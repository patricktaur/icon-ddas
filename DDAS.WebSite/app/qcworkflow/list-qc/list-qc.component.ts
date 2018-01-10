import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { QCListViewModel, ReviewStatusEnum } from '../qc.classes';
import { ConfigService } from '../../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { IMyDate, IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';
import { QCService } from '../qc-service';
//import { Http, Response, Headers , RequestOptions } from '@angular/http';

@Component({
    moduleId: module.id,
    templateUrl: 'list-qc.component.html',
})
export class ListQCComponent implements OnInit {
    public Loading: boolean = false;
    private error: any;

    public FromDate: IMyDateModel;
    public ToDate: IMyDateModel;

    public myDatePickerOptions = {

        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
    };

    public Users: any[];
    public SelectedComplianceFormId: string;
    public qcList: QCListViewModel[];
    public pageNumber: number = 1;
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private configService: ConfigService,
        private authService: AuthService,
        private auditService: QCService
    ) { }

    ngOnInit() {
        this.auditService.listAudits()
            .subscribe((item: QCListViewModel[]) => {
                this.qcList = item;
            },
            error => {

            });
    }

    status(statusEnum: number){
        switch(statusEnum){
            case ReviewStatusEnum.SearchCompleted: return "Search Completed";
            case ReviewStatusEnum.ReviewInProgress: return "Review in progress";
            case ReviewStatusEnum.ReviewCompleted: return "Review completed";
            case ReviewStatusEnum.Completed: return "Completed";
            case ReviewStatusEnum.QCRequested: return "QC Requested";
            case ReviewStatusEnum.QCInProgress: return "QC in progress";
            case ReviewStatusEnum.QCFailed: return "QC Failed";
            case ReviewStatusEnum.QCPassed: return "QC Passed";
            default: "";
        }
    }

    get filterQCByUserName() {
        if(this.authService.isAdmin){
            return this.qcList;
        }
        else if (this.qcList != undefined || this.qcList != null) {
            return this.qcList.filter(x =>
                x.Requestor.toLowerCase() == this.authService.userName.toLowerCase() ||
                x.QCVerifier.toLowerCase() == this.authService.userName.toLowerCase());
        }
        else
            return null;
    }

    isQCVerifierOrIsQCCompleted(qcVerifier: string, Status: number) {
        if (this.authService.userName.toLowerCase() == qcVerifier.toLowerCase())
            return true;
        else if (Status == ReviewStatusEnum.QCPassed || Status == ReviewStatusEnum.QCFailed)
            return true;
        else
            return false;
    }

    editQC(complianceFormId: string, assignedTo: string) {
        //this.router.navigate(['edit-qc', complianceFormId, assignedTo, { relativeTo: this.route.parent }]);
        this.router.navigate(['edit-qc', complianceFormId, assignedTo, {rootPath:'qc', page:this.pageNumber}], { relativeTo: this.route });
    }
}