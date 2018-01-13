import { Component, OnInit, OnDestroy, NgZone, ViewChild, OnChanges, SimpleChanges } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';

import {
    ComplianceFormA,
    InvestigatorSearched,
    SiteSourceToSearch,
    SiteSource,
    ComplianceFormStatusEnum,
    Finding,
    InstituteFindingsSummaryViewModel,
    ReviewerRoleEnum,
    ReviewStatusEnum,
    Comment
}
    from '../../search/search.classes';
import { SearchService } from '../../search/search-service';
import { Location } from '@angular/common';
import { ModalComponent } from '../../shared/utils/ng2-bs3-modal/ng2-bs3-modal';
import { ConfigService } from '../../shared/utils/config.service';
import { AuthService } from '../../auth/auth.service';
//import {SiteSourceViewModel} from '../../admin/appAdmin.classes';
import {DefaultSite, SiteSourceViewModel} from '../../admin/appAdmin.classes';
import { Response } from '@angular/http/src/static_response';
//import {XXX} from '../../admin/appAdmin.classes';



// C:\Development\p926-ddas\DDAS.WebSite\app\app-admin\app-admin.classes.ts

@Component({
    moduleId: module.id,

    templateUrl: 'comp-form-edit.component.html',
    styles: [`
    body {
  padding : 10px ;
  
}

#exTab1 .tab-content {
  color : white;
  background-color: #428bca;
  padding : 5px 15px;
}

#exTab2 h3 {
  color : white;
  background-color: #428bca;
  padding : 5px 15px;
}

/* remove border radius for the tab */

#exTab1 .nav-pills > li > a {
  border-radius: 0;
}

/* change border radius for the tab , apply corners on top*/

#exTab3 .nav-pills > li > a {
  border-radius: 4px 4px 0 0 ;
}

#exTab3 .tab-content {
  color : white;
  background-color: #428bca;
  padding : 5px 15px;
}

    `]
})
export class CompFormEditComponent implements OnInit {
    compFormForm: FormGroup;
    public formLoading: boolean;

    public CompForm: ComplianceFormA = new ComplianceFormA;
    private ComplianceFormId: string;
    public SitesAvailable: SiteSource[] = [];
    public searchInProgress: boolean = false;
    public Selected: boolean = false;
    public InvestigatorToRemove: InvestigatorSearched = new InvestigatorSearched;
    public siteToRemove: SiteSourceToSearch = new SiteSourceToSearch;
    private pageChanged: boolean = false;

    public InstituteSearchSummary: InstituteFindingsSummaryViewModel[] = [];

    private rootPath: string;
    private page: number;
    private currentTab: string;
    public defaultTab: boolean = true;
    public invTab: boolean = false;
    public invTabInActive: string;
    public defaultTabInActive: string = " in active";

    public selectedFinding: Finding = new Finding;
    public fileUploaded: string;
    public SiteSources: any[];
    //public SourceSite: DefaultSite = new DefaultSite;
    //public SourceSite: DefaultSite = new DefaultSite;
    public SiteSource: SiteSource = new SiteSource;
    public isQCVerifier: boolean;
    public reviewStatus: string;

    public qcAssignedTo: string;

    @ViewChild('FindingsAddModal') FindingsAddModal: ModalComponent;

    public myDatePickerOptions = {

        dateFormat: 'dd mmm yyyy',
        selectionTxtFontSize: 14
    };

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private _location: Location,
        private service: SearchService,
        private fb: FormBuilder,
        private sanitizer: DomSanitizer,
        private configService: ConfigService,
        private authService: AuthService
    ) { }

    
   
    ngOnInit() {
        this.formLoading = true;
        this.route.params.forEach((params: Params) => {
            this.ComplianceFormId = params['formId'];
            this.rootPath = params['rootPath'];
            this.page = +params['page'];
            this.currentTab = params['tab'];
            if (this.currentTab == "invTab") {

                this.setInvestigatorTab();
            }
            this.qcAssignedTo = params['qcAssignedTo'];  //discuss with Pradeep and remove this line.
            this.LoadOpenComplainceForm();
            this.LoadInstituteSiteSummary();
        });
    }

    get isFormValid() {
        let isValid: boolean;
        isValid = /\d{4}\/\d{4}/.test(this.CompForm.ProjectNumber);
        if (!isValid) {
            return false;
        } else {
            if (this.CompForm.ProjectNumber2){
                isValid = /\d{4}\/\d{4}/.test(this.CompForm.ProjectNumber2);
                if (!isValid) {
                    return false;
                } else {
                    //could not find reg exp for length:
                    if (this.CompForm.ProjectNumber.length == 9 && this.CompForm.ProjectNumber2.length == 9) {
                        return true;
                    } else {
                        return false;
                    }
                }
            }else{
                return true;
            }
        }
    }

    get status() {
        switch (this.CompForm.CurrentReviewStatus) {
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

    //for REactive Validation
    buildForm(): void {
        this.compFormForm = this.fb.group({
            'projNumber': [this.CompForm.ProjectNumber,
            [
                Validators.minLength(9),
                Validators.maxLength(9)
            ]
            ],

            'SponsorProtocolNumber': [this.CompForm.SponsorProtocolNumber],
            'SponsorProtocolNumber2': [this.CompForm.SponsorProtocolNumber2],
            'Institute': [this.CompForm.Institute],
            'Country': [this.CompForm.Country],
            'Address': [this.CompForm.Address],

            'Investigators': this.fb.array([])
        });

        this.LoadInvestigatorControls();
        // this.compFormForm.valueChanges
        // .subscribe(data => this.onValueChanged(data));
        //  this.onValueChanged(); // (re)set validation messages now
    }

    //===============Institute tab
    LoadInstituteSiteSummary() {

        if (this.ComplianceFormId != null && this.ComplianceFormId.length > 0) {  //ComplianceFormId is null when the form is created manually
            //console.log("*:" + this.ComplianceFormId + "*");
            this.formLoading = true;
            this.service.getInstituteFindingsSummary(this.ComplianceFormId)
                .subscribe((item: any) => {
                    this.InstituteSearchSummary = item;
                    this.formLoading = false;
                },
                error => {
                    this.formLoading = false;
                });
        }

    }

    gotoSiteDetails(SiteSourceId: number) {
        this.router.navigate(['institute-findings', this.ComplianceFormId, SiteSourceId, { rootPath: this.rootPath }],
            { relativeTo: this.route.parent });
    }
    //===============institute tab

    InitInvestigatorControls() {
        return this.fb.group({
            Name: ['', Validators.required],
            CanEdit: ['']
        });
    }

    LoadInvestigatorControls() {
        const control = <FormArray>this.compFormForm.controls['Investigators'];

        this.CompForm.InvestigatorDetails.forEach(inv =>
            control.push(this.getInvestigatorControl(inv))
        );
    }

    addInvestigatorControl() {
        const control = <FormArray>this.compFormForm.controls['Investigators'];
        let inv = new InvestigatorSearched;
        control.push(this.getInvestigatorControl(inv))
    }

    getInvestigatorControl(inv: InvestigatorSearched) {
        return this.fb.group({
            Name: [inv.Name, Validators.required],
            CanEdit: [inv.CanEdit]
        });
    }

    onValueChanged(data?: any) {
        if (!this.compFormForm) { return; }
        const form = this.compFormForm;
        for (const field in this.formErrors) {
            // clear previous error message (if any)
            this.formErrors[field] = '';
            const control = form.get(field);
            if (control && control.dirty && !control.valid) {
                const messages = this.validationMessages[field];
                for (const key in control.errors) {
                    this.formErrors[field] += messages[key] + ' ';
                }
            }
        }
    }

    formErrors = {
        'projNumber': '',
        'SponsorProtocolNumber': '',
        'Institute': '',
        'Country': '',
        'Address': '',

    };



    investigator = {

    }

    validationMessages = {
        'projNumber': {
            'required': 'Project Number is required.',
            'minlength': 'Project Number must be at 9 characters long.',
            'maxlength': 'Project Number must be at 9 characters long.'

        }
    };

    SaveReactiveForm() {


        (<FormGroup>this.compFormForm)
            .setValue(this.CompForm, { onlySelf: true });
        //this.CompForm = this.compFormForm.value;
        this.service.saveComplianceForm(this.CompForm)
            .subscribe((item: any) => {
                let newCompForm: ComplianceFormA = item;
                this.ComplianceFormId = newCompForm.RecId;
                this.LoadOpenComplainceForm();
            },
            error => {

            });
    }

    LoadOpenComplainceForm() {
        this.service.getComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                this.CompForm = item;
                //this.LoadSitesAvailable();
                this.loadSiteSources();
                this.Initialize();
                this.SetInvestigatorsSavedFlag();
                this.pageChanged = false;
                this.buildForm();
                this.setFileUploadFolderPath();
                this.formLoading = false;
                this.reviewStatus = this.status;
                this.isQCVerifier = this.isLoggedInUserQCVerifier;
                // this.addCommentCollection();
            },
            error => {
                this.formLoading = false;
            });
    }

    get isLoggedInUserQCVerifier() {
        if (this.CompForm.Reviews.length > 0) {
            var review = this.CompForm.Reviews.find(x =>
                x.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                x.ReviewerRole == ReviewerRoleEnum.QCVerifier);

            if (!review)
                return false;
            else
                return true;
        }
    }

    get isLoggedInUserReviewer(){
        if (this.CompForm.Reviews.length > 0) {
            var review = this.CompForm.Reviews.find(x =>
                x.AssigendTo.toLowerCase() == this.authService.userName.toLowerCase() &&
                x.ReviewerRole == ReviewerRoleEnum.Reviewer);
            if (!review)
                return false;
            else
                return true;
        }
    }

    get isFileUploaded(): boolean {
        if (this.CompForm.UploadedFileName == null)
            return false;
        else if (this.CompForm.UploadedFileName.trim().length == 0)
            return false;
        else
            return true;
    }

    setFileUploadFolderPath() {
        this.service.getUploadsFolderPath()
            .subscribe((item: any) => {
                this.fileUploaded = this.configService.getApiHost() + item;
            })
    }

    downloadUploadedFile(generatedFileName: string) {
        this.service.getUploadedFile(generatedFileName, this.CompForm.UploadedFileName)
            .subscribe((item: any) => {
                this.fileUploaded = this.configService.getApiHost() + item;
            })
    }

    Initialize() {

        let SearchPending = false;

        for (let inv of this.CompForm.InvestigatorDetails) {
            let SitesSearchedCount: number = 0;
            for (let searchStatus of inv.SitesSearched) {
                if (searchStatus.ExtractedOn == null) {
                    SearchPending = true;
                }
                else {
                    SitesSearchedCount += 1;
                }
                inv.CanEdit = true;
                inv.Saved = true;
                if (SitesSearchedCount > 0) {
                    inv.CanEdit = false;
                }
            }

            this.CompForm.SearchPending = SearchPending;
        }
    }

    formValueChanged(val: any) {
        this.pageChanged = val;
    }
    get PrincipalInvestigatorName() {
        let p = this.CompForm.InvestigatorDetails.find(x => x.Role == "PI");
        if (p != null) {
            return p.Name;
        }
        return null;
    }
    get CurrentComplianceForm() {
        if (this.CompForm == null) {
            return null;
        }
        return this.CompForm;
    }

    get statusEnum(): number {
        return this.CompForm.StatusEnum;
    }

    CloseForEdit() {
        this.service.CloseComplianceForm(this.ComplianceFormId)
            .subscribe((item: any) => {
                console.log("success");
                this.router.navigate(['search']);
            },
            error => {
            });
    }

    get EstimatedExtractionCompletionIn() {
        return "";
    }
    //Investigators:
    //================
    get Investigators() {
        if (this.CompForm == undefined) { return null; }
        return this.CompForm.InvestigatorDetails.filter(x => x.Deleted == false);
    }

    setInvestigatorTab() {

        this.defaultTab = false;
        this.defaultTabInActive = "";
        this.invTabInActive = " in active"
        this.invTab = true;
    }

    InvestigatorAdd() {
        this.setInvestigatorTab();
        let inv = new InvestigatorSearched;
        let lastInvestigatorNumber: number = this.LastInvestigatorNumber;
        inv.Id = lastInvestigatorNumber + 1;
        inv.Saved = false;
        inv.Help = "Save pending"

        this.CompForm.InvestigatorDetails.push(inv);
        this.SetInvestigatorRole();
        this.Initialize();
        this.pageChanged = true;
    }

    setInvestigatorRemove(inv: InvestigatorSearched) {
        this.InvestigatorToRemove = inv
        //  this.ComplianceFormIdToDelete = inv.RecId;
        //  this.InvestigatorNameToDelete = inv.PrincipalInvestigator;
    }

    InvestigatorRemove() {
        // item.Deleted = true;
        this.InvestigatorToRemove.Deleted = true;

        //Remove from Findings:
        this.CompForm.Findings = this.CompForm.Findings.filter(x => x.InvestigatorSearchedId != this.InvestigatorToRemove.Id)

        var index = this.CompForm.InvestigatorDetails.indexOf(this.InvestigatorToRemove);
        if (index > -1) {
            this.CompForm.InvestigatorDetails.splice(index, 1);
        }

        this.SetInvestigatorRole();
        this.Initialize();
        this.pageChanged = true;
    }

    //Better method?
    get LastInvestigatorNumber(): number {
        let lastNumber: number = 0
        for (let item of this.CompForm.InvestigatorDetails) {
            if (item.Id > lastNumber) {
                lastNumber = item.Id
            }
        }
        return lastNumber;
    }

    SetInvestigatorRole() {
        let index: number = 0
        for (let item of this.Investigators) {
            if (index == 0) {
                item.Role = "PI"
            }
            else {
                item.Role = "Sub I"
            }
            index += 1;
        }
    }

    SetInvestigatorsSavedFlag() {
        for (let item of this.Investigators) {
            item.Saved = true;
            item.Help = "Edit Investigator related findings"
        }
    }

    //Does not work if an item is markedAsDeleted
    move(idx: number, step: number) {

        var tmp = this.CompForm.InvestigatorDetails[idx];

        this.CompForm.InvestigatorDetails[idx] = this.CompForm.InvestigatorDetails[idx - step];
        this.CompForm.InvestigatorDetails[idx - step] = tmp;
        this.SetInvestigatorRole();

    }



    moveA(inv: InvestigatorSearched, step: number) {
        var tmp = inv;
    }



    //SitesParticpatingInSearch  
    //================================   

    // LoadSitesAvailable() {
    //     this.service.getSiteSources()
    //         .subscribe((item: any) => {
    //             this.SitesAvailable = item;
    //             this.InitializeSitesAvailableToAdd();
    //         },
    //         error => {
    //         });
    // }

    loadSiteSources() {
        this.service.getSiteSources()
            .subscribe((item: any[]) => {
                this.SiteSources = item;
            },
            error => {

            });
    }

    LoadMockSitesAvailable() {
        this.SitesAvailable = [];

        let s1 = new SiteSource;
        s1.SiteEnum = 13;
        s1.SiteName = "Site - 1";
        s1.Included = false;
        this.SitesAvailable.push(s1);

        let s2 = new SiteSource;
        s2.SiteEnum = 14;
        s2.SiteName = "Site - 2";
        s2.Included = false;
        this.SitesAvailable.push(s2);

        let s3 = new SiteSource;
        s3.SiteEnum = 15;
        s3.SiteName = "Site - 3";
        s2.Included = false;
        this.SitesAvailable.push(s3);

    }

    // InitializeSitesAvailableToAdd() {
    //     //Mark sites that are already included Comp Form:

    //     for (let site of this.SitesAvailable) {

    //         //let siteInCompForm = this.CompForm.SiteSources.find(x => x.SiteEnum == site.SiteEnum);
    //         let siteInCompForm = this.CompForm.SiteSources.find(x => x.SiteId == site.RecId);
    //         site.Selected = false;
    //         if (siteInCompForm == null) {
    //             site.Included = false;
    //         }
    //         else {
    //             site.Included = true;
    //         }
    //     }
    // }

    get SitesParticpatingInSearch() {
        return this.CompForm.SiteSources.filter(x => x.Deleted == false)
    }

    get MandatorySites() {
        return this.CompForm.SiteSources.filter(x => x.Deleted == false && x.IsMandatory == true)
    }

    get OptionalSites() {
        return this.CompForm.SiteSources.filter(x => x.Deleted == false && x.IsMandatory == false)
    }

    get SitesAvalaibleToInclude() {
        return this.SitesAvailable.filter(x => x.Included == false);
    }

    get AppliesToItems() {
        var items: { id: number, name: string }[] = [
            { "id": 0, "name": "PIs and SIs" },
            { "id": 1, "name": "PIs" },
            { "id": 2, "name": "Institute" }];
        return items;
    }

    onSiteSourceChange(value: any) {
        var site = this.SiteSources.find(x => x.RecId == value);

        this.SiteSource.RecId = site.RecId;
        this.SiteSource.SiteName = site.SiteName;
        this.SiteSource.SiteShortName = site.SiteShortName;
        this.SiteSource.SiteUrl = site.SiteUrl;
        this.SiteSource.ExtractionMode = site.ExtractionMode;
    }

    onSearchAppliesToChange(value: any) {
        this.SiteSource.SearchAppliesToText = value;
        console.log(value);
    }

    clearSelectedSite() {

        this.SiteSource = new SiteSource;
    }
    AddSelectedSite() {


        let siteToAdd = new SiteSourceToSearch;
        siteToAdd.SiteId = this.SiteSource.RecId;
        siteToAdd.SiteName = this.SiteSource.SiteName;
        //siteToAdd.SiteEnum = this.SitesAvailable[index].SiteEnum;
        siteToAdd.SiteUrl = this.SiteSource.SiteUrl;
        siteToAdd.SiteShortName = this.SiteSource.SiteShortName;
        siteToAdd.Id = this.LastSiteSourceId + 1;
        siteToAdd.IsMandatory = false;
        siteToAdd.SearchAppliesTo = this.SiteSource.SearchAppliesTo;
        siteToAdd.SearchAppliesToText = this.SiteSource.SearchAppliesToText;

        console.log("SearchAppliesTo : " + this.SiteSource.SearchAppliesTo);

        let extractionMode: string = "Manual";
        if (this.SiteSource.ExtractionMode != null) {
            extractionMode = this.SiteSource.ExtractionMode;
        }

        if (this.SiteSource.SearchAppliesTo == 2)  //Applies to Institute
        {
            extractionMode = "Manual"
        }
        siteToAdd.ExtractionMode = extractionMode;


        this.CompForm.SiteSources.push(siteToAdd);
        this.pageChanged = true;
        
        
        this.SetSiteDisplayPosition();
    }

    setSiteToRemove(site: SiteSourceToSearch) {
        this.siteToRemove = site;
    }


    RemoveSite(){
        
        let siteIdToRemove = this.siteToRemove.SiteId;



        //Remove Findings:
        var i: number;
        for (i = this.CompForm.Findings.length - 1; i >= 0; i -= 1) {
            if (this.CompForm.Findings[i].SiteSourceId == this.siteToRemove.Id) {
                console.log("Finding removed: " + this.CompForm.Findings[i].SiteId);
                this.CompForm.Findings.splice(i, 1);
            }
        }

        //remove siteSearchStatus for all Investigators.
        var inv: number;
        for (inv = this.CompForm.InvestigatorDetails.length - 1; inv >= 0; inv -= 1) {
            var s: number;
            for (s = this.CompForm.InvestigatorDetails[inv].SitesSearched.length - 1; s >= 0; s -= 1) {
                if (this.CompForm.InvestigatorDetails[inv].SitesSearched[s].SiteSourceId == this.siteToRemove.Id) {
                    var invName = this.CompForm.InvestigatorDetails[inv].SearchName;
                    // console.log("siteSearchStatus removed: inv: " + invName + "-" + this.CompForm.InvestigatorDetails[inv].SitesSearched[s].DisplayPosition);

                    this.CompForm.InvestigatorDetails[inv].SitesSearched.splice(s, 1);
                }

            }


        }

        var index = this.CompForm.SiteSources.indexOf(this.siteToRemove, 0);
        if (index > -1) {
            this.CompForm.SiteSources.splice(index, 1);
        }

        this.SetSiteDisplayPosition();
        this.SetSiteDisplayPositionInFindings();
        this.pageChanged = true;
    }
   
    
    SetSiteDisplayPosition() {
        let pos: number = 1
        for (let item of this.CompForm.SiteSources) {

            if (item.Deleted == false) {
                item.DisplayPosition = pos;
                pos += 1;
            }
            else {
                item.DisplayPosition = 0;
            }
        }
    }

    SetSiteDisplayPositionInFindings() {
        for (let finding of this.CompForm.Findings) {
            let pos: number = 0
            pos = this.CompForm.SiteSources.find(x => x.Id == finding.SiteSourceId).DisplayPosition;
            finding.SiteDisplayPosition = pos;
        }
    }

    //Better method?
    get LastSiteSourceId(): number {
        let lastNumber: number = 0
        for (let item of this.CompForm.SiteSources) {
            if (item.Id > lastNumber) {
                lastNumber = item.Id
            }
        }
        return lastNumber;
    }

    siteIsManual(value: string) {
        if (value == null) {
            value = "";
        }
        //    if (ExtractionMode.toLowerCase() == "manual"){
        //         retValue =  true;
        //   }
        if (value == "Manual") {
            return true;
        }
        else {
            return false;
        }
    }
    //Findgins
    get Findings() {
        if (this.CompForm == undefined) {
            return null;
        }
        //return this.CompForm.Findings.filter(x => x.Selected == true);
        return this.CompForm.Findings.filter(x => x.Selected == true).sort(
            function (a, b) {
                if (a.DisplayPosition < b.DisplayPosition) return -1;
                else if (a.DisplayPosition > b.DisplayPosition) return 1;
                else return 0;
            }
        );
    }

    AddFinding() {

        let finding = new Finding;
        finding.IsMatchedRecord = false;
        //finding.UISelected = true;
        finding.IsAnIssue = true;
        finding.InvestigatorSearchedId = null;
        finding.SiteSourceId = null;
        finding.SiteEnum = null;
        //finding.SourceNumber = 
        //finding.InvestigatorName =
        finding.DateOfInspection = new Date(Date.now());
        finding.Selected = true;
        this.CompForm.Findings.push(finding);
        this.EditFinding(finding);

    }

    EditFinding(finding: Finding) {
        this.selectedFinding = finding;
        this.FindingsAddModal.open();
    }

    DeleteFinding(finding: Finding) {
        if (window.confirm("Remove ?") == true) {
            var index = this.CompForm.Findings.indexOf(finding);
            this.CompForm.Findings.splice(index, 1);
        }
    }

    Save() {

        //this.service.saveComplianceForm(this.CompForm)
        if (this.BlankInvestigatorNamesFound()) {
            window.alert("The Name field is blank in one or more Investigator records.");
        }
        else {
            this.searchInProgress = true;
            this.service.saveCompFormGeneralNInvestigatorsNOptionalSites(this.CompForm)
                .subscribe((item: ComplianceFormA) => {
                    this.ComplianceFormId = item.RecId;
                    this.searchInProgress = false;
                    this.LoadOpenComplainceForm();
                },
                error => {
                });
        }
    }

    BlankInvestigatorNamesFound() {
        let retValue: boolean = false;
        this.CompForm.InvestigatorDetails.forEach(inv => {
            if (inv.Name == null || inv.Name.length == 0) {
                retValue = true;
            }
        }
        );
        return retValue;
    }

    ScanNSave() {
        this.searchInProgress = true;
        this.service.scanSaveComplianceForm(this.CompForm)
            .subscribe((item: any) => {
                this.ComplianceFormId = item.RecId;
                this.searchInProgress = false;
                this.LoadOpenComplainceForm();
            },
            error => {
                this.searchInProgress = false;
            });
    }
    selectionChange() {
        this.SitesAvalaibleToInclude.forEach(i => i.Selected = !this.Selected);
    }

    gotoInvestigatorSummaryResult(inv: InvestigatorSearched) {

        this.router.navigate(['investigator-summary', this.CompForm.RecId, inv.Id, { rootPath: this.rootPath }],
            { relativeTo: this.route.parent });

    }

    gotoInstituteSummaryFindings() {
        this.router.navigate(['institute-findings-summary', this.CompForm.RecId, { rootPath: this.rootPath }],
            { relativeTo: this.route.parent });
    }

    goBack() {
        //this.complianceFormId = params['complianceFormId'];
        //this.qcAssignedTo = params['qcAssignedTo'];
        if (this.rootPath == null) {
            this._location.back();
        }
        else {
            if (this.rootPath == 'edit-qc') {
                this.router.navigate([this.rootPath, this.ComplianceFormId, this.qcAssignedTo],  { relativeTo: this.route.parent });
            }else{
                this.router.navigate([this.rootPath, { id: this.ComplianceFormId, page: this.page }]);
            }
        }

    }

    Split = (RecordDetails: string) => {
        if (RecordDetails == undefined) {
            return null;
        }
        var middleNames: string[] = RecordDetails.split("~");

        return middleNames;
    }

    canDeactivate(): Promise<boolean> | boolean {

        if (this.pageChanged == false) {
            return true;
        }
        // Otherwise ask the user with the dialog service and return its
        // promise which resolves to true or false when the user decides
        //this.IgnoreChangesConfirmModal.open();
        //return this.canDeactivateValue;
        return window.confirm("Changes not saved. Ignore changes?");//this.dialogService.confirm('Discard changes?');
    }

    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(url);
    }


    dateChanged($event: any, dateValue: Date) {
        dateValue = $event.value;
        window.alert(JSON.stringify(dateValue));

        window.alert(JSON.stringify(dateValue));
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
    
     isComponentVisible(componentName: string){
        // return true;
        // console.log(this.CompForm.CurrentReviewStatus);
        switch(componentName){
            case "generalEdit":
                if((this.CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress))
                    return true;
                else
                    return false;
            // case "generalEditQC":
            //     if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress)
            //         return true;
            //     else
            //         return false;
            // case "generalEditResponseToQC":
            //     if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress &&
            //         this.isGeneralQCCommentAdded)
            //         return true;
            //     else
            //         return false;
            case "generalView":
                if((this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewCompleted ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCRequested ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCFailed ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.Completed))
                    return true;
                else
                    return false;
            case "instituteEdit":
                if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress)
                    return true;
                else
                    return false;
            case "instituteView":
                if((this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewCompleted ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCRequested ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCFailed ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.Completed))
                    return true;
                else
                    return false;
            case "investigatorEdit":
                if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress)
                    return true;
                else
                    return false;
            case "investigatorView":
                if((this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewCompleted ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCRequested ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCFailed ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.Completed))
                    return true;
                else
                    return false;
            case "mandatorySitesEdit":
                if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress)
                    return true;
                else
                    return false;
            case "mandatorySitesView":
                if((this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewCompleted ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCRequested ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCInProgress ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCFailed ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.QCCorrectionInProgress ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.Completed))
                    return true;
                else
                    return false;
            case "additionalSitesEdit":
                if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.SearchCompleted ||
                    this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress)
                    return true;
                else
                    return false;
            case "additionalSitesView":
                return true;
            case "findingsEdit":
                // if(this.CompForm.CurrentReviewStatus == ReviewStatusEnum.ReviewInProgress)
                    return true;
                // else
                //     return false;
            case "searchedByView": return true;
            case "summaryView": return true;

            default: return true;
        }
    }
    get diagnostic() { return JSON.stringify(this.rootPath); }


}

