import { Component } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { Location } from '@angular/common';
import { SearchService } from './search-service';
import { AuthService } from '../auth/auth.service';
import { CompFormLogicService } from './shared/services/comp-form-logic.service';
//import {SiteInfo,SearchSummaryItem,SearchSummary,NameSearch, MatchedRecordsPerSite} from './search.classes';

import {
    InvestigatorSearched, ComplianceFormA, SiteSource,
    SiteSearchStatus, ReviewerRoleEnum, ReviewStatusEnum
} from './search.classes';

@Component({
    moduleId: module.id,
    templateUrl: 'investigator-summary.component.html',
    styles: [
        `
  .selected {
    border: 2px solid blue;  
  }
  `
    ]
})
export class InvestigatorSummaryComponent {
    public InvestigatorId: number;
    private ComplianceFormId: string;
    public InvestigatorSummary: InvestigatorSearched = new InvestigatorSearched;

    public loading: boolean;
    public CompForm: ComplianceFormA = new ComplianceFormA;

    public retSiteEnum: number;
    public retSiteId: string;

    private rootPath: string;
    public HideReviewCompletedSites: boolean;
    private ShowMatchesFoundSites: boolean;
    public LoggedInUserIsAppAdmin: boolean;

    constructor(private service: SearchService,
        private route: ActivatedRoute,
        private _location: Location,
        private router: Router,
        private sanitizer: DomSanitizer,
        private authService: AuthService,
        private compFormLogic: CompFormLogicService
    ) { }

    ngOnInit() {
        this.HideReviewCompletedSites = true;

        this.route.params.forEach((params: Params) => {
            this.ComplianceFormId = params['formId'];
            this.InvestigatorId = +params['investigatorId'];
            // change to SiteId:
            this.retSiteEnum = +params['siteEnum'];
            this.retSiteId = params['siteId'];

            this.rootPath = params['rootPath'];

            // if (params['hideReviewCompleted'] != null){
            //     this.HideReviewCompletedSites = params['hideReviewCompleted'];
            // }

            this.LoadOpenComplainceForm();
            this.LoggedInUserIsAppAdmin = this.authService.isAppAdmin;

        });

    }

    LoadOpenComplainceForm() {
        this.loading = true;
        this.service.getComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.CompForm = item;
                //this.IntiliazeRecords();
                this.loading = false;

                if (this.CompForm &&
                    (this.CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                        this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress)) {
                    //...
                }
                else {
                    this.HideReviewCompletedSites = false;
                }

            },
            error => {
                this.loading = false;
            });
    }



    get InvestigatorSiteSummary() {

        let sitesSearched: SiteSearchStatus[];
        if (this.Investigator == undefined) {
            return sitesSearched;
        }
        else {
            return this.Investigator.SitesSearched.filter(x => x.Exclude == false);
        }

    }

    get FilteredInvestigatorSiteSummary() {

        if (this.HideReviewCompletedSites == true) {
            return this.InvestigatorSiteSummary.filter(x => x.ReviewCompleted == false && x.Exclude == false);
        }
        else {
            return this.InvestigatorSiteSummary.filter(x => x.Exclude == false);
        }

    }

    get Investigator(): InvestigatorSearched {

        let inv: InvestigatorSearched = new InvestigatorSearched;
        let inv1 = this.CompForm.InvestigatorDetails.find(x => x.Id == this.InvestigatorId);
        if (inv1 == undefined) {
            return inv;
        }
        else {
            return inv1;
        }
    }




    get ReviewPendingCount() {

        return (this.InvestigatorSiteSummary.length - this.Investigator.ReviewCompletedSiteCount);
    }

    get Summary() {
        if (this.CompForm == null) return null;

        let retSummary: string[];
        retSummary.push("Use 'Search' option to find matching records");
        retSummary.push("Use 'Search' option to find matching records aaaa");
        if (this.Investigator.ExtractionErrorSiteCount > 0) {
            retSummary.push("Extraction of matching records at" + this.Investigator.ExtractionErrorSiteCount + " site(s) was not successfull.");
            retSummary.push("Use 'Search' option on the Compliance Form re-extract matching records");

        }
        if (this.Investigator.ExtractionErrorSiteCount == 0) {
            if (this.Investigator.ExtractedOn == null) {
                retSummary.push("Use 'Search' option to find matching records");
            }
            else {
                retSummary.push("Matching records were extracted on: " + this.Investigator.ExtractedOn);
                if (this.Investigator.Sites_FullMatchCount > 0) {
                    retSummary.push(this.Investigator.Sites_FullMatchCount + " sites have full match records.");
                }
                else {
                    retSummary.push("No site found with Full Match records.");
                }
                if (this.Investigator.Sites_PartialMatchCount > 0) {
                    retSummary.push(this.Investigator.Sites_PartialMatchCount + " sites have partially matching records.");
                }
                else {
                    retSummary.push("No site found with partially matching records.");
                }
            }
            if (this.Investigator.ReviewCompletedSiteCount == this.Investigator.SitesSearched.length) {
                retSummary.push("Review completed for all sites");
            }
            else {
                retSummary.push("Review completed for " + this.Investigator.ReviewCompletedSiteCount + " of " + "");
            }
        }
        return retSummary;
    }

    unHideReviewCompletedSiteCheckBox() {
        this.compFormLogic.unHideReviewCompletedSiteCheckBox(this.CompForm);
    }

    gotoSiteDetails(siteSourceId: number) {
        this.router.navigate(['findings', this.ComplianceFormId, this.InvestigatorId, siteSourceId, { rootPath: this.rootPath, hideReviewCompleted: this.HideReviewCompletedSites }],
            { relativeTo: this.route.parent });
    }

    goBack() {
        //this._location.back();
        this.router.navigate(['comp-form-edit', this.ComplianceFormId, { rootPath: this.rootPath, tab: "invTab" }], { relativeTo: this.route.parent });
    }

    BoolYesNo(value: boolean): string {
        if (value == null) {
            return "";
        }
        if (value == true) {
            return "Yes"
        }
        else {
            return "No"
        }
    }

    dividerGeneration(indexVal: number) {
        if ((indexVal + 1) % 3 == 0) {
            return true;
        }
        else {
            return false;
        }
    }

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
    get diagnostic() { return JSON.stringify(this.Investigator); }
}