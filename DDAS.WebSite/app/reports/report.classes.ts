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
    ReportPeriodEnum: number;
}

export class InvestigationsReport {
    ReportByUsers: ReportByUser[];
}

export class ReportByUser {
    UserName: string;
    ReportItems: ReportItem[];
}

export class ReportItem {
    ReportPeriod: string;
    Value: number;
}

