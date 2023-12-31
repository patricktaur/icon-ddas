import { Component, OnInit, OnDestroy, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource, ComplianceFormStatusEnum, Finding, InstituteFindingsSummaryViewModel } from '../../../search/search.classes';
import { SearchService } from '../../../search/search-service';


@Component({
    selector: '[comp-form-institute-edit]',
    moduleId: module.id,
    templateUrl: 'institute-edit.component.html',
})
export class ComplianceFormInstituteEditComponent implements OnInit, OnChanges {
    @Input() CompForm: ComplianceFormA;
    @Input() RootPath: string;
    @Output() ValueChanged = new EventEmitter();

    public InstituteSearchSummary: InstituteFindingsSummaryViewModel[] = [];
    private pageChanged: boolean = false;

    constructor(
        private service: SearchService,
        private route: ActivatedRoute,
        private router: Router,
        private _location: Location,
        private sanitizer: DomSanitizer
    ) { }

    ngOnInit() {
        this.LoadInstituteSiteSummary();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (this.ComplianceFormId) {
            this.LoadInstituteSiteSummary();
        }
    }

    get ComplianceFormId() {
        return this.CompForm.RecId;
    }

    LoadInstituteSiteSummary() {
        if (this.ComplianceFormId != null && this.ComplianceFormId.length > 0) {  //ComplianceFormId is null when the form is created manually
            //this.formLoading = true;
            this.service.getInstituteFindingsSummary(this.ComplianceFormId)
                .subscribe((item: any) => {
                    this.InstituteSearchSummary = item;
                    //this.formLoading = false;
                },
                (error: any) => {
                    //this.formLoading = false;
                });
        }
    }

    private Todate = new Date();

    isUrl(url: string) {
        if (url == null) {
            return false;
        }
        else {
            if (url.toLowerCase().startsWith("http")) {
                return true;
            } else {
                return false;
            }
        }
    }

    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }

    formValueChanged() {
        this.pageChanged = true;
        this.ValueChanged.emit(true) ;
    }

    gotoSiteDetails(SiteSourceId: number) {
        this.router.navigate(['institute-findings', this.ComplianceFormId, SiteSourceId, { rootPath: this.RootPath }],
            { relativeTo: this.route.parent });
    }

}