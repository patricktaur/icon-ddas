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
        WorldCheckPage //27Jan2017 Pradeep
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

}
