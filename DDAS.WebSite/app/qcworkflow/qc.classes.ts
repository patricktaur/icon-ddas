export class QCListViewModel {
        RecId: string;
        ComplianceFormId: string;
        Requestor: string;
        RequestedOn: Date;
        QCVerifier: string;
        CompletedOn: Date;
        Status: ReviewStatusEnum;
        PrincipalInvestigator: string;
        ProjectNumber: string;
        ProjectNumber2: string;
}

export enum ReviewStatusEnum {
        SearchCompleted,
        ReviewInProgress,
        ReviewCompleted,
        Completed,
        QCRequested,
        QCInProgress,
        QCFailed,
        QCPassed
}

export class QualityCheck {
        ComplianceFormId: string;
        RequestedBy: string;
        RequestedOn: Date;
        QCVerifier: string;
        CompletedOn: Date;
        IsSubmitted: boolean;
        QCVerifierComment: string;
        RequestorComment: string;
        QCStatus: string;
        Observations: AuditObservation[] = [];
        Auditor: string;
        AuditorComments: string;
        RequestorComments: string;
        AuditStatus: string;
}

export class AuditObservation {
        SiteId: number;
        SiteShortName: string;
        Comments: string;
        Status: string;
}