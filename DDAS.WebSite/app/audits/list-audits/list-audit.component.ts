import { Component, OnInit, OnDestroy, NgZone, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { AuditListViewModel } from '../audit.classes';
import { ConfigService } from '../../shared/utils/config.service';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { AuthService } from '../../auth/auth.service';
import { IMyDate, IMyDateModel } from '../../shared/utils/my-date-picker/interfaces';
import { AuditService } from '../audit-service';
//import { Http, Response, Headers , RequestOptions } from '@angular/http';

@Component({
    moduleId: module.id,
    templateUrl: 'list-audit.component.html',
})
export class ListAuditsComponent implements OnInit {
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
    public audits: AuditListViewModel[];
    public pageNumber: number;
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private configService: ConfigService,
        private authService: AuthService,
        private auditService: AuditService
    ) { }

    ngOnInit() {
        this.auditService.listAudits()
        .subscribe((item: AuditListViewModel[]) => {
            this.audits = item;
        },
        error => {

        });
    }

    isAuditorOrIsAuditPending(auditor: string, auditStatus: string){
        if(this.authService.userName.toLowerCase() == auditor)
            return true;
        else if(auditStatus.toLowerCase() != "pending")
            return true;
        else
            return false;
    }

    editAudit(auditId: string){
        this.router.navigate(['edit-audit', auditId], { relativeTo: this.route.parent});
    }
}