export enum SiteEnum {
    FDADebarPage,
    ClinicalInvestigatorInspectionPage,
    FDAWarningLettersPage,
    ERRProposalToDebarPage,
    AdequateAssuranceListPage,
    ClinicalInvestigatorDisqualificationPage,
    CBERClinicalInvestigatorInspectionPage,
    PHSAdministrativeActionListingPage,
    ExclusionDatabaseSearchPage,
    CorporateIntegrityAgreementsListPage,
    SystemForAwardManagementPage,
    SpeciallyDesignedNationalsListPage
}

export class DownloadDataFilesViewModel{
    FileName: string;
    FullPath: string;
    SiteName: string;
    DownloadedOn: Date;
    FileSize: number;
    FileType: string;
}