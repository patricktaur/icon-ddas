import { ReviewStatusEnum } from '../search/search.classes';

export class QCListViewModel {
        RecId: string;
        ComplianceFormId: string;
        Requester: string;
        RequesterFullName: string;
        RequestedOn: Date;
        QCVerifier: string;
        QCVerifierFullName: string;
        CompletedOn: Date;
        Status: ReviewStatusEnum;
        PrincipalInvestigator: string;
        InvestigatorCount: number;
        ProjectNumber: string;
        ProjectNumber2: string;
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