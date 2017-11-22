export class complianceForms {
    NameToSearch: string = "";
    SearchStartedOn: string = "";
    TotalIssuesFound: number = 0;
    RecId: string = "";
}

export class ReportFilters {
    AssignedTo: string;
    FromDate: Date;
    ToDate: Date;
    ReportPeriodEnum: ReportPeriodEnum;
}

export class InvestigationsReport {
    DatesAdjustedTo: string;
    ReportByUsers: ReportByUser[] = [];
}

export class ReportByUser {
    UserName: string;
    ReportItems: ReportItem[] = [];
}

export class ReportItem {
    ReportPeriod: string;
    Value: number;
}

export enum ReportPeriodEnum{
    Day,
    Week,
    Month,
    Quarter,
    Year
}

export class AdminDashboardViewModel{
    UserName: string;
    OpeningBalance: number;
    InvestigatorUploaded: number;
    InvestigatorReviewCompleted: number;
    ClosingBalance: number;
}

export class AssignmentHistoryViewModel{
    PrincipalInvestigator: string;
    ProjectNumber: string;
    ProjetNumber2: string;
    AssignedBy: string;
    AssingedOn: Date;
    AssignedTo: string;
    RemovedOn: Date;
}

export class InvestigatorFindingViewModel{
    ProjectNumber: string;
    ProjetNumber2: string;
    InvestigatorName: string;
    Role: string;
    ReviewCompletedBy: string;
    ReviewCompletedOn: Date;
    SiteName: string;
    FindingObservation: string;    
}
