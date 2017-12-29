import { Component, OnInit, OnDestroy, Input, OnChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
//import { ComplianceFormA, SiteSource, Finding } from '../../../search/search.classes';
import { ComplianceFormA, InvestigatorSearched, SiteSourceToSearch, SiteSource, ComplianceFormStatusEnum, Finding, InstituteFindingsSummaryViewModel } from '../../../search/search.classes';

@Component({
    selector: '[comp-form-investigator-edit]',
    moduleId: module.id,
    templateUrl: 'investigator-edit.component.html',
})
export class ComplianceFormInvestigatorEditComponent implements OnInit, OnChanges {
    @Input() CompForm: ComplianceFormA;

    compFormForm: FormGroup;
    public formLoading: boolean;

    //public CompForm: ComplianceFormA = new ComplianceFormA;
    private ComplianceFormId: string;
    public SitesAvailable: SiteSource[] = [];
    public searchInProgress: boolean = false;
    public Selected: boolean = false;
    public InvestigatorToRemove: InvestigatorSearched = new InvestigatorSearched;
    public siteToRemove: SiteSourceToSearch = new SiteSourceToSearch;
    private pageChanged: boolean = false;

    public InstituteSearchSummary : InstituteFindingsSummaryViewModel[] = [];

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

    constructor(
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private _location: Location,
    ) { }

    ngOnInit() {
        
    }


    ngOnChanges(){
      
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
    
    get Investigators() {
        if (this.CompForm == undefined) { return null; }
        return this.CompForm.InvestigatorDetails.filter(x => x.Deleted == false);
    }

    setInvestigatorTab()
    {
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

    // InitInvestigatorControls() {
    //     return this.fb.group({
    //         Name: ['', Validators.required],
    //         CanEdit: ['']
    //     });
    // }

    // LoadInvestigatorControls() {
    //     const control = <FormArray>this.compFormForm.controls['Investigators'];

    //     this.CompForm.InvestigatorDetails.forEach(inv =>
    //         control.push(this.getInvestigatorControl(inv))
    //     );
    // }

    // addInvestigatorControl() {
    //     const control = <FormArray>this.compFormForm.controls['Investigators'];
    //     let inv = new InvestigatorSearched;
    //     control.push(this.getInvestigatorControl(inv))
    // }

    // getInvestigatorControl(inv: InvestigatorSearched) {
    //     return this.fb.group({
    //         Name: [inv.Name, Validators.required],
    //         CanEdit: [inv.CanEdit]
    //     });
    // }

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

    move(idx: number, step: number) {
        
                var tmp = this.CompForm.InvestigatorDetails[idx];
        
        
                this.CompForm.InvestigatorDetails[idx] = this.CompForm.InvestigatorDetails[idx - step];
                this.CompForm.InvestigatorDetails[idx - step] = tmp;
                this.SetInvestigatorRole();
        
            }

gotoInvestigatorSummaryResult(inv: InvestigatorSearched) {
    
            this.router.navigate(['investigator-summary', this.CompForm.RecId, inv.Id, { rootPath: this.rootPath }],
                { relativeTo: this.route.parent });
    
        }

    get diagnostic() { return JSON.stringify(this.CompForm); }    
}