namespace DDAS.Models.Enums
{
    public enum SiteEnum
    {
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
        SpeciallyDesignedNationalsListPage,

        //16Dec2016 Pradeep SponsorSpecificSites
        PfizerDMCChecksPage,
        PfizerUnavailableChecksPage,
        GSKDoNotUseCheckPage,
        RegeneronUsabilityCheckPage,

        //Pradeep 7Dec2016
        //Manual sites
        AustralianHealthPratitionerRegulationPage,
        ZoekEenArtsPage,
        RizivZoekenPage,
        ConselhosDeMedicinaPage,
        TribunalNationalDeEticaMedicaPage,
        ValviraPage,
        ConseilNationalDeMedecinsPage,
        MedicalCouncilOfIndiaPage,
        MinistryOfHealthIsraelPage,
        ListOfRegisteredDoctorsPage,
        NaczelnaIzbaLekarskaPage,
        PortalOficialDaOrdemDosMedicosPage,
        OrganizacionMedicaColegialDeEspanaPage,
        SingaporeMedicalCouncilPage,
        SriLankaMedicalCouncilPage,
        HealthGuideUSAPage,
        WorldCheckPage, //27Jan2017 Pradeep
        IconInternalFlagCheck
    };

    public enum ComplianceFormStatusEnum
    {
        NotScanned,
        ReviewCompletedIssuesIdentified,
        ReviewCompletedIssuesNotIdentified,
        FullMatchFoundReviewPending,
        PartialMatchFoundReviewPending,
        NoMatchFoundReviewPending,
        ManualSearchSiteReviewPending,
        IssuesIdentifiedReviewPending,
        HasExtractionErrors,
        SingleMatchFoundReviewPending
    }

    public enum SearchAppliesToEnum
    {
        PIs_SIs,
        PIs,
        Institute
    }

    public enum ReportPeriodEnum
    {
        Day,
        Week,
        Month,
        Quarter,
        Year
    }

    public enum ReviewStatusEnum
    {
        SearchCompleted,
        ReviewInProgress,
        ReviewCompleted,
        Completed,
        QCRequested,
        QCInProgress,
        QCFailed,
        QCPassed,
        QCCorrectionInProgress,
        ExportedToiSprint,
        QCCompleted
    }

    public enum ReviewerRoleEnum
    {
        Reviewer,
        QCVerifier
    }

    public enum CommentCategoryEnum
    {
        Minor,
        Major,
        Critical,
        Suggestion,
        Others,
        CorrectionPending,
        CorrectionCompleted,
        Accepted,
        NotApplicable,
        ExcludeFinding,
        NotAccepted
    }

    public enum AdminDashboardReportType
    {
        OpeningBalance,
        ComplianceFormsUploaded,
        ComplianceFormsCompleted,
        ClosingBalance
    }

    public enum UndoEnum
    {
        UndoQCRequest,
        UndoQCSubmit,
        UndoQCResponse,
        UndoCompleted
    }

    public enum QCCompletedStatusEnum
    {
        NotApplicable,
        InProgress,
        NoIssues,
        IssuesNoted
    }
}
