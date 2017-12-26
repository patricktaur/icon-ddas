export class AuditListViewModel{
        RecId: string;
        ComplianceFormId: string;
        RequestedBy: string;
        RequestedOn: Date;
        Auditor: string;
        CompletedOn: Date;
        AuditStatus: string;
        PrincipalInvestigator: string;
        ProjectNumber: string;
        ProjectNumber2: string;
}

export class Audit{
        ComplianceFormId: string;
        RequestedBy: string;
        RequestedOn: Date;
        Auditor: string;
        CompletedOn: Date;
        IsSubmitted: boolean;
        AuditorComments: string;
        RequestorComments: string;
        AuditStatus: string;
        Observations: AuditObservation[] = [];
}

export class AuditObservation{
        SiteId: number;
        SiteShortName: string;
        Comments: string;
        Status: string;
   }