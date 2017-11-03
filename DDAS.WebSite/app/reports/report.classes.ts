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
