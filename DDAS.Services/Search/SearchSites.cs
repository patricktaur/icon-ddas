using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System.Collections.Generic;

namespace DDAS.Services.Search
{
    public static class SearchSites
    {
        //public static SearchQuery GetNewSearchQuery()
        //{
        //    return new SearchQuery
        //    {
        //        NameToSearch = "Anthony, James Michael",
        //        SearchSites = new List<SearchQuerySite>
        //        {
        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="FDA Debarment List", SiteShortName="FDA Debarment List", SiteEnum = SiteEnum.FDADebarPage, SiteUrl="http://www.fda.gov/ora/compliance_ref/debar/default.htm" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="Clinical Investigator Inspection List (CLIL)(CDER", SiteShortName="Clinical Investigator Insp...", SiteEnum = SiteEnum.ClinicalInvestigatorInspectionPage, SiteUrl="http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Live", SiteName="FDA Warning Letters and Responses", SiteShortName="FDA Warning Letters ...", SiteEnum = SiteEnum.FDAWarningLettersPage, SiteUrl="http://www.fda.gov/ICECI/EnforcementActions/WarningLetters/default.htm" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="Notice of Opportunity for Hearing (NOOH) – Proposal to Debar", SiteShortName="NOOH – Proposal to Debar", SiteEnum = SiteEnum.ERRProposalToDebarPage, SiteUrl="http://www.fda.gov/RegulatoryInformation/FOI/ElectronicReadingRoom/ucm143240.htm" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="Adequate Assurances List for Clinical Investigators", SiteShortName="Adequate Assurances List ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl="http://www.fda.gov/ora/compliance_ref/bimo/asurlist.htm" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Live", SiteName="Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName="Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl="http://www.accessdata.fda.gov/scripts/SDA/sdNavigation.cfm?sd=clinicalinvestigatorsdisqualificationproceedings&previewMode=true&displayAll=true" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="Clinical Investigator Inspection List (CBER)", SiteShortName="CBER Clinical Investigator ...", SiteEnum = SiteEnum.CBERClinicalInvestigatorInspectionPage, SiteUrl="http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195364.htm" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="PHS Administrative Actions Listing ", SiteShortName="PHS Administrative Actions", SiteEnum = SiteEnum.PHSAdministrativeActionListingPage, SiteUrl="https://ori.hhs.gov/ORI_PHS_alert.html?d=update" },

        //            new SearchQuerySite{ Mandatory = true,  ExtractionMode = "DB", SiteName="HHS/OIG/ EXCLUSIONS DATABASE SEARCH/ FRAUD", SiteShortName="HHS/OIG/ EXCLUSIONS ...", SiteEnum = SiteEnum.ExclusionDatabaseSearchPage, SiteUrl="https://oig.hhs.gov/exclusions/exclusions_list.asp" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="HHS/OIG Corporate Integrity Agreements/Watch List", SiteShortName="HHS/OIG Corporate Integrity", SiteEnum = SiteEnum.CorporateIntegrityAgreementsListPage, SiteUrl="http://oig.hhs.gov/compliance/corporate-integrity-agreements/cia-documents.asp" },

        //            //new SearchQuerySite { Mandatory = true,  ExtractionMode = "Live", SiteName="SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName="SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.SystemForAwardManagementPage, SiteUrl="https://www.sam.gov/portal/public/SAM" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="LIST OF SPECIALLY DESIGNATED NATIONALS", SiteShortName="SPECIALLY DESIGNATED ...", SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage, SiteUrl="http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx" },

        //            //Manual sites
        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="AUSTRALIAN HEALTH PRACTITIONER REGULATION AGENCY", SiteShortName="HEALTH PRACTITIONER ...", SiteEnum = SiteEnum.AustralianHealthPratitionerRegulationPage, SiteUrl="http://www.ahpra.gov.au/Registration/Registers-of-Practitioners.aspx" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="Belgium1 - ZOEK EEN ARTS", SiteShortName="ZOEK EEN ARTS", SiteEnum = SiteEnum.ZoekEenArtsPage, SiteUrl="https://ordomedic.be/nl/zoek-een-arts/" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="Belgium2 - RIZIV - Zoeken", SiteShortName="RIZIV - Zoeken", SiteEnum = SiteEnum.RizivZoekenPage, SiteUrl="https://www.riziv.fgov.be/webprd/appl/psilverpages/nl" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="Brazil - CONSELHOS DE MEDICINA", SiteShortName="CONSELHOS DE MEDICINA", SiteEnum = SiteEnum.ConselhosDeMedicinaPage, SiteUrl="http://portal.cfm.org.br/index.php?option=com_medicos&Itemid=59" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="Colombia - EL TRIBUNAL NACIONAL DE ÉTICA MÉDICA", SiteShortName="EL TRIBUNAL NACIONAL...", SiteEnum = SiteEnum.TribunalNationalDeEticaMedicaPage, SiteUrl="http://www.tribunalnacionaldeeticamedica.org/site/biblioteca_documental" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="Finland - VALVIRA", SiteShortName="VALVIRA", SiteEnum = SiteEnum.ValviraPage, SiteUrl="https://julkiterhikki.valvira.fi/" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="France - CONSEIL NATIONAL DE L'ORDRE DES MEDECINS", SiteShortName="CONSEIL NATIONAL DE L'ORDRE...", SiteEnum = SiteEnum.ConseilNationalDeMedecinsPage, SiteUrl="http://www.conseil-national.medecin.fr/annuaire" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="MEDICAL COUNCIL OF INDIA", SiteShortName="MEDICAL COUNSIL OF INDIA", SiteEnum = SiteEnum.MedicalCouncilOfIndiaPage, SiteUrl="http://online.mciindia.org/online//Index.aspx?qstr_level=01" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="Israel - MINISTRY OF HEALTH ISRAEL", SiteShortName="MINISTRY OF HEALTH ISRAEL", SiteEnum = SiteEnum.MinistryOfHealthIsraelPage, SiteUrl="http://www.health.gov.il/UnitsOffice/HR/professions/postponements/Pages/default.aspx" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="New Zeland - LIST OF REGISTERED DOCTORS", SiteShortName="LIST OF REGISTERED DOCTORS", SiteEnum = SiteEnum.ListOfRegisteredDoctorsPage, SiteUrl="https://www.mcnz.org.nz/support-for-doctors/list-of-registered-doctors/" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="Poland - NACZELNA IZBA LEKARSKA", SiteShortName="NACZELNA IZBA LEKARSKA", SiteEnum = SiteEnum.NaczelnaIzbaLekarskaPage, SiteUrl="http://rejestr.nil.org.pl/xml/nil/rejlek/hurtd" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="Portugal - PORTAL OFICIAL DA ORDEM DOS MEDICOS", SiteShortName="PORTAL OFICIAL DA ORDEM...", SiteEnum = SiteEnum.PortalOficialDaOrdemDosMedicosPage, SiteUrl="https://www.ordemdosmedicos.pt/" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="Spain - ORGANIZACION MEDICA COLEGIAL DE ESPANA", SiteShortName="ORGANIZACION MEDICA COLEGIAL...", SiteEnum = SiteEnum.OrganizacionMedicaColegialDeEspanaPage, SiteUrl="http://www.cgcom.es/consultapublicacolegiados" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="SINGAPORE MEDICAL COUNCIL", SiteShortName="SINGAPORE MEDICAL COUNCIL...", SiteEnum = SiteEnum.SingaporeMedicalCouncilPage, SiteUrl="http://www.healthprofessionals.gov.sg/content/hprof/smc/en.html" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="SRI LANKA MEDICAL COUNCIL", SiteShortName="SRI LANKA MEDICAL COUNCIL...", SiteEnum = SiteEnum.SriLankaMedicalCouncilPage, SiteUrl="http://www.srilankamedicalcouncil.org/registry.php" },

        //            new SearchQuerySite { Mandatory = true,  ExtractionMode = "Manual", SiteName="HEALTH GUIDE USA", SiteShortName="HEALTH GUIDE USA", SiteEnum = SiteEnum.HealthGuideUSAPage, SiteUrl="http://www.healthguideusa.org/medical_license_lookup.htm" }
        //        }
        //    };
        //}

        public static List<SearchQuerySite> GetNewSearchQuery()
        {
            return new List<SearchQuerySite>
                {
                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="FDA Debarment List", SiteShortName="FDA Debarment List", SiteEnum = SiteEnum.FDADebarPage, SiteUrl="http://www.fda.gov/ora/compliance_ref/debar/default.htm" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="Clinical Investigator Inspection List (CLIL)(CDER", SiteShortName="Clinical Investigator Insp...", SiteEnum = SiteEnum.ClinicalInvestigatorInspectionPage, SiteUrl="http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "Live", SiteName="FDA Warning Letters and Responses", SiteShortName="FDA Warning Letters ...", SiteEnum = SiteEnum.FDAWarningLettersPage, SiteUrl="http://www.fda.gov/ICECI/EnforcementActions/WarningLetters/default.htm" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="Notice of Opportunity for Hearing (NOOH) – Proposal to Debar", SiteShortName="NOOH – Proposal to Debar", SiteEnum = SiteEnum.ERRProposalToDebarPage, SiteUrl="http://www.fda.gov/RegulatoryInformation/FOI/ElectronicReadingRoom/ucm143240.htm" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="Adequate Assurances List for Clinical Investigators", SiteShortName="Adequate Assurances List ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl="http://www.fda.gov/ora/compliance_ref/bimo/asurlist.htm" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "Live", SiteName="Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName="Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl="http://www.accessdata.fda.gov/scripts/SDA/sdNavigation.cfm?sd=clinicalinvestigatorsdisqualificationproceedings&previewMode=true&displayAll=true" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="Clinical Investigator Inspection List (CBER)", SiteShortName="CBER Clinical Investigator ...", SiteEnum = SiteEnum.CBERClinicalInvestigatorInspectionPage, SiteUrl="http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195364.htm" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="PHS Administrative Actions Listing ", SiteShortName="PHS Administrative Actions", SiteEnum = SiteEnum.PHSAdministrativeActionListingPage, SiteUrl="https://ori.hhs.gov/ORI_PHS_alert.html?d=update" },

                    new SearchQuerySite{ Mandatory = true,  ExtractionMode = "DB", SiteName="HHS/OIG/ EXCLUSIONS DATABASE SEARCH/ FRAUD", SiteShortName="HHS/OIG/ EXCLUSIONS ...", SiteEnum = SiteEnum.ExclusionDatabaseSearchPage, SiteUrl="https://oig.hhs.gov/exclusions/exclusions_list.asp" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="HHS/OIG Corporate Integrity Agreements/Watch List", SiteShortName="HHS/OIG Corporate Integrity", SiteEnum = SiteEnum.CorporateIntegrityAgreementsListPage, SiteUrl="http://oig.hhs.gov/compliance/corporate-integrity-agreements/cia-documents.asp" },

                    //new SearchQuerySite { Mandatory = true,  ExtractionMode = "Live", SiteName="SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName="SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.SystemForAwardManagementPage, SiteUrl="https://www.sam.gov/portal/public/SAM" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "DB", SiteName="LIST OF SPECIALLY DESIGNATED NATIONALS", SiteShortName="SPECIALLY DESIGNATED ...", SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage, SiteUrl="http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx" },

                    //Manual sites

                    new SearchQuerySite {Mandatory = false, ExtractionMode = "Manual", SiteName="Pfizer DMC Checks", SiteShortName="Pfizer DMC Checks", SiteEnum = SiteEnum.PfizerDMCChecksPage, SiteUrl=" http://ecf12.pfizer.com/sites/clinicaloversightcommittees/default.aspx" },

                    new SearchQuerySite {Mandatory = false, ExtractionMode = "Manual", SiteName="Pfizer Unavailable Checks", SiteShortName="Pfizer DMC Checks", SiteEnum = SiteEnum.PfizerUnavailableChecksPage, SiteUrl="http://ecf12.pfizer.com/" },

                    new SearchQuerySite {Mandatory = false, ExtractionMode = "Manual", SiteName="GSK Do Not Use Check", SiteShortName="GSK DNU Check", SiteEnum = SiteEnum.GSKDoNotUseCheckPage, SiteUrl="" },

                    new SearchQuerySite {Mandatory = false, ExtractionMode = "Manual", SiteName="Regeneron Usability Check", SiteShortName="Regeneron Usability Check", SiteEnum = SiteEnum.RegeneronUsabilityCheckPage, SiteUrl="" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="AUSTRALIAN HEALTH PRACTITIONER REGULATION AGENCY", SiteShortName="HEALTH PRACTITIONER ...", SiteEnum = SiteEnum.AustralianHealthPratitionerRegulationPage, SiteUrl="http://www.ahpra.gov.au/Registration/Registers-of-Practitioners.aspx" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="Belgium1 - ZOEK EEN ARTS", SiteShortName="ZOEK EEN ARTS", SiteEnum = SiteEnum.ZoekEenArtsPage, SiteUrl="https://ordomedic.be/nl/zoek-een-arts/" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="Belgium2 - RIZIV - Zoeken", SiteShortName="RIZIV - Zoeken", SiteEnum = SiteEnum.RizivZoekenPage, SiteUrl="https://www.riziv.fgov.be/webprd/appl/psilverpages/nl" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="Brazil - CONSELHOS DE MEDICINA", SiteShortName="CONSELHOS DE MEDICINA", SiteEnum = SiteEnum.ConselhosDeMedicinaPage, SiteUrl="http://portal.cfm.org.br/index.php?option=com_medicos&Itemid=59" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="Colombia - EL TRIBUNAL NACIONAL DE ÉTICA MÉDICA", SiteShortName="EL TRIBUNAL NACIONAL...", SiteEnum = SiteEnum.TribunalNationalDeEticaMedicaPage, SiteUrl="http://www.tribunalnacionaldeeticamedica.org/site/biblioteca_documental" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="Finland - VALVIRA", SiteShortName="VALVIRA", SiteEnum = SiteEnum.ValviraPage, SiteUrl="https://julkiterhikki.valvira.fi/" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="France - CONSEIL NATIONAL DE L'ORDRE DES MEDECINS", SiteShortName="CONSEIL NATIONAL DE L'ORDRE...", SiteEnum = SiteEnum.ConseilNationalDeMedecinsPage, SiteUrl="http://www.conseil-national.medecin.fr/annuaire" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="MEDICAL COUNCIL OF INDIA", SiteShortName="MEDICAL COUNSIL OF INDIA", SiteEnum = SiteEnum.MedicalCouncilOfIndiaPage, SiteUrl="http://online.mciindia.org/online//Index.aspx?qstr_level=01" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="Israel - MINISTRY OF HEALTH ISRAEL", SiteShortName="MINISTRY OF HEALTH ISRAEL", SiteEnum = SiteEnum.MinistryOfHealthIsraelPage, SiteUrl="http://www.health.gov.il/UnitsOffice/HR/professions/postponements/Pages/default.aspx" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="New Zeland - LIST OF REGISTERED DOCTORS", SiteShortName="LIST OF REGISTERED DOCTORS", SiteEnum = SiteEnum.ListOfRegisteredDoctorsPage, SiteUrl="https://www.mcnz.org.nz/support-for-doctors/list-of-registered-doctors/" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="Poland - NACZELNA IZBA LEKARSKA", SiteShortName="NACZELNA IZBA LEKARSKA", SiteEnum = SiteEnum.NaczelnaIzbaLekarskaPage, SiteUrl="http://rejestr.nil.org.pl/xml/nil/rejlek/hurtd" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="Portugal - PORTAL OFICIAL DA ORDEM DOS MEDICOS", SiteShortName="PORTAL OFICIAL DA ORDEM...", SiteEnum = SiteEnum.PortalOficialDaOrdemDosMedicosPage, SiteUrl="https://www.ordemdosmedicos.pt/" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="Spain - ORGANIZACION MEDICA COLEGIAL DE ESPANA", SiteShortName="ORGANIZACION MEDICA COLEGIAL...", SiteEnum = SiteEnum.OrganizacionMedicaColegialDeEspanaPage, SiteUrl="http://www.cgcom.es/consultapublicacolegiados" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="SINGAPORE MEDICAL COUNCIL", SiteShortName="SINGAPORE MEDICAL COUNCIL...", SiteEnum = SiteEnum.SingaporeMedicalCouncilPage, SiteUrl="http://www.healthprofessionals.gov.sg/content/hprof/smc/en.html" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="SRI LANKA MEDICAL COUNCIL", SiteShortName="SRI LANKA MEDICAL COUNCIL...", SiteEnum = SiteEnum.SriLankaMedicalCouncilPage, SiteUrl="http://www.srilankamedicalcouncil.org/registry.php" },

                    new SearchQuerySite { Mandatory = false,  ExtractionMode = "Manual", SiteName="HEALTH GUIDE USA", SiteShortName="HEALTH GUIDE USA", SiteEnum = SiteEnum.HealthGuideUSAPage, SiteUrl="http://www.healthguideusa.org/medical_license_lookup.htm" }
                };
        }


        public static SearchQuery GetNewLiveSiteSearchQuery()
        {
            return new SearchQuery
            {
                NameToSearch = "Anthony, James Michael",
                SearchSites = new List<SearchQuerySite>
                {
                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "Live", SiteName="FDA Warning Letters and Responses", SiteShortName="FDA Warning Letters ...", SiteEnum = SiteEnum.FDAWarningLettersPage, SiteUrl="http://www.fda.gov/ICECI/EnforcementActions/WarningLetters/default.htm" },

                    new SearchQuerySite { Mandatory = true,  ExtractionMode = "Live", SiteName="Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName="Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl="XXX" },

                    //new SearchQuerySite { SiteName="SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName="SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.SystemForAwardManagementPage, SiteUrl="XXX" }
                }
            };
        }
    }
}
