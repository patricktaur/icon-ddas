import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { PrincipalInvestigatorDetails } from '../search//search.classes';
import { ReportService } from './report-service';
import { DialogService } from '../shared/utils/dialog.service';


@Component({
    moduleId: module.id,

    templateUrl: 'report.component1.html',

})

export class ReportComponent1 {

    private PrincipalInvestigators: PrincipalInvestigatorDetails[];
    public ComplianceFormIdToDelete: string;
    public InvestigatorNameToDelete: string;

    public filterStatus: number = -1;
    public filterInvestigatorName: string = "";


    constructor(private service: ReportService, private route: ActivatedRoute, private dialog: DialogService) {

    }
    ngOnInit() {
        this.LoadPrincipalInvestigators();
    }

    // LoadReportResults(){
    //   this.service.getNamesFromClosedComplianceForms()
    //           .subscribe((item: any) => {
    //             this.CompForms = item;  
    //           },
    //           error => {

    //           });
    // }

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
            filter1 = this.PrincipalInvestigators.filter((a) =>
                a.Name.toLowerCase().includes(this.filterInvestigatorName.toLowerCase())
            )
        }

        let filter2: PrincipalInvestigatorDetails[];
        filter2 = filter1;
        if (this.filterStatus >= 0) {
            filter2 = filter1.filter((a) =>
                a.StatusEnum == this.filterStatus
            )
        }
        return filter2;
    }

    setComplianceFormIdToDelete(inv: PrincipalInvestigatorDetails) {
        this.ComplianceFormIdToDelete = inv.RecId;
        this.InvestigatorNameToDelete = inv.Name;
    }
    DeleteComplianceForm() {
        //CompFormId to be set by the delete button

        this.service.getDeleteComplianceForm(this.ComplianceFormIdToDelete)
            .subscribe((item: any) => {
                this.LoadPrincipalInvestigators();
            },
            error => {

            });
    }
    
    get diagnostic() { return JSON.stringify(null); }

}