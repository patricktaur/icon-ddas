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
    UserFullName: string;
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

export enum AdminDashboardReportType{
    OpeningBalance,
    ComplianceFormsUploaded,
    ComplianceFormsCompleted,
    ClosingBalance
}

export class AdminDashboardViewModel{
    UserName: string;
    UserFullName: string;
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
    PreviouslyAssignedTo: string;
    InvestigatorCount: number;
    SearchStartedOn: Date;
}

export class InvestigatorFindingViewModel{
    ProjectNumber: string;
    ProjetNumber2: string;
    InvestigatorName: string;
    Role: string;
    ReviewCompletedBy: string;
    ReviewCompletedOn: Date;
    SiteShortName: string;
    FindingObservation: string;
}

export class ReportFilterViewModel{
    FromDate: Date;
    ToDate: Date;
    ProjectNumber: string;
    AssignedTo: string;
}