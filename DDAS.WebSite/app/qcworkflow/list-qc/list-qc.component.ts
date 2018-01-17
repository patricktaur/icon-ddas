import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { QCListViewModel, ReviewStatusEnum } from '../qc.classes';
import { ConfigService } from '../../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { IMyDate, IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';
import { QCService } from '../qc-service';
import {CompFormLogicService} from "../../search/shared/services/comp-form-logic.service";
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
        private auditService: QCService,
        private compFormLogic: CompFormLogicService
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
        return this.compFormLogic.getReviewStatus(statusEnum);
    }

    // filterQCReviews(){
    //     this.qcList = this.qcList.filter(x => x.ComplianceFormId)
    // }

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

    isActionRequired(qcVerifier: string, requestor: string, Status: number) {
        if (this.authService.userName.toLowerCase() == qcVerifier.toLowerCase())
            return true;
        else if ((Status == ReviewStatusEnum.QCPassed || Status == ReviewStatusEnum.QCFailed) &&
            requestor.toLowerCase() == this.authService.userName.toLowerCase())
            return true;
        else
            return false;
    }

    editQC(complianceFormId: string, assignedTo: string) {
        //this.router.navigate(['edit-qc', complianceFormId, assignedTo, { relativeTo: this.route.parent }]);
        this.router.navigate(['edit-qc', complianceFormId, assignedTo, {rootPath:'qc', page:this.pageNumber}], { relativeTo: this.route });
    }
}