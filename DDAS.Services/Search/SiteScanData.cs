using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDAS.Services.Search
{
    public class SiteScanData
    {
        private IUnitOfWork _UOW;

        public SiteScanData( IUnitOfWork uow)
        {
            _UOW = uow;
        }

        public List<SiteScan> GetSiteScanSummary()
        {
            List<SiteScan> ScanData = new List<SiteScan>();

            SearchQuery NewSearchQuery = GetNewSearchQuery();

            foreach (SearchQuerySite Site in NewSearchQuery.SearchSites)
            {
                var scanData = GetSiteScanData(Site.SiteEnum);
                scanData.SiteName = Site.SiteName;
                scanData.SiteUrl = Site.SiteUrl;
                scanData.SiteEnum = Site.SiteEnum;
                ScanData.Add(scanData);
            }
            return ScanData;
        }

        public SiteScan GetSiteScanData(SiteEnum Enum)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage : return GetFDADebarSiteScanDetails();

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInevstigatorInspectionDetails();

                case SiteEnum.ERRProposalToDebarPage:
                    return GetProposalToDebarSiteScanDetails();

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssuranceSiteScanDetails();

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorSiteScanDetails();

                case SiteEnum.PHSAdministrativeActionListingPage :
                    return GetPHSSiteScanDetails();

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabaseSearchSiteScanDetails();

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCorporateIntegrityAgreementSiteScanDetails();

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSpeciallyDesignatedNationsDetails();

                default:
                    throw new Exception("Invalid Enum");
            }
        }

        public SiteScan GetFDADebarSiteScanDetails()
        {
            var SiteData = _UOW.FDADebarPageRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan scan = new SiteScan();
            
            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetClinicalInvestigatorSiteScanDetails()
        {

            var SiteData = _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetProposalToDebarSiteScanDetails()
        {
            var SiteData = _UOW.ERRProposalToDebarRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetAdequateAssuranceSiteScanDetails()
        {
            var SiteData = _UOW.AdequateAssuranceListRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetCBERClinicalInvestigatorSiteScanDetails()
        {
            var SiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetPHSSiteScanDetails()
        {
            var SiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan siteScan = new SiteScan();

            siteScan.DataExtractedOn = SiteData.CreatedOn;
            siteScan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            siteScan.DataId = SiteData.RecId;

            return siteScan;
        }

        public SiteScan GetExclusionDatabaseSearchSiteScanDetails()
        {
            var SiteData = _UOW.ExclusionDatabaseSearchRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetCorporateIntegrityAgreementSiteScanDetails()
        {
            var SiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetClinicalInevstigatorInspectionDetails()
        {
            var SiteData = _UOW.ClinicalInvestigatorInspectionListRepository.
                GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan siteScan = new SiteScan();

            siteScan.DataExtractedOn = SiteData.CreatedOn;
            siteScan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            siteScan.DataId = SiteData.RecId;

            return siteScan;
        }

        public SiteScan GetSpeciallyDesignatedNationsDetails()
        {
            var SiteData = _UOW.SpeciallyDesignatedNationalsRepository.
                GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan siteScan = new SiteScan();

            siteScan.DataExtractedOn = SiteData.CreatedOn;
            siteScan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            siteScan.DataId = SiteData.RecId;

            return siteScan;
        }

        public SearchQuery GetNewSearchQuery()
        {
            return new SearchQuery
            {
                NameToSearch = "Anthony, James Michael",
                SearchSites = new List<SearchQuerySite>
                {
                    new SearchQuerySite {Selected = true, SiteName="FDA Debarment List", SiteShortName="FDA Debarment List", SiteEnum = SiteEnum.FDADebarPage, SiteUrl="http://www.fda.gov/ora/compliance_ref/debar/default.htm" },

                    new SearchQuerySite {Selected = true, SiteName="Clinical Investigator Inspection List (CLIL)(CDER", SiteShortName="Clinical Investigator Insp...", SiteEnum = SiteEnum.ClinicalInvestigatorInspectionPage, SiteUrl="http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm" },
                   
                    //new SearchQuerySite {Selected = true, SiteName="FDA Warning Letters and Responses", SiteShortName="FDA Warning Letters ...", SiteEnum = SiteEnum.FDAWarningLettersPage, SiteUrl="XXX" },
                    
                    new SearchQuerySite {Selected = true, SiteName="Notice of Opportunity for Hearing (NOOH) – Proposal to Debar", SiteShortName="NOOH – Proposal to Debar", SiteEnum = SiteEnum.ERRProposalToDebarPage, SiteUrl="XXX" },
                    
                    new SearchQuerySite {Selected = true, SiteName="Adequate Assurances List for Clinical Investigators", SiteShortName="Adequate Assurances List ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl="XXX" },

                    //new SearchQuerySite {Selected = true, SiteName="Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName="Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl="XXX" },
                    
                    new SearchQuerySite {Selected = true, SiteName="Clinical Investigator Inspection List (CBER)", SiteShortName="Inspection List", SiteEnum = SiteEnum.CBERClinicalInvestigatorInspectionPage, SiteUrl="XXX" },

                    new SearchQuerySite {Selected = true, SiteName="PHS Administrative Actions Listing ", SiteShortName="PHS Administrative Actions", SiteEnum = SiteEnum.PHSAdministrativeActionListingPage, SiteUrl="http://ori.hhs.gov/misconduct/AdminBulletinBoard.shtml" },

                    new SearchQuerySite{Selected = true, SiteName="HHS/OIG/ EXCLUSIONS DATABASE SEARCH/ FRAUD", SiteShortName="HHS/OIG/ EXCLUSIONS ...", SiteEnum = SiteEnum.ExclusionDatabaseSearchPage, SiteUrl="https://oig.hhs.gov/exclusions/exclusions_list.asp" },
                    
                    new SearchQuerySite {Selected = true, SiteName="HHS/OIG Corporate Integrity Agreements/Watch List", SiteShortName="HHS/OIG Corporate Integrity", SiteEnum = SiteEnum.CorporateIntegrityAgreementsListPage, SiteUrl="XXX" },

                    //new SearchQuerySite {Selected = true, SiteName="SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName="SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.SystemForAwardManagementPage, SiteUrl="XXX" },

                    new SearchQuerySite {Selected = true, SiteName="LIST OF SPECIALLY DESIGNATED NATIONALS", SiteShortName="SPECIALLY DESIGNATED ...", SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage, SiteUrl="http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx" },
                }

            };

        }

    }
}
