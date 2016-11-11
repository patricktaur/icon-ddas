using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDAS.Services.Search
{
    public class SiteScanData
    {
        private IUnitOfWork _UOW;
        private ISearchEngine _SearchEngine;

        public SiteScanData( IUnitOfWork uow, ISearchEngine 
            SearchEngine)
        {
            _UOW = uow;
            _SearchEngine = SearchEngine;
        }

        public List<SiteScan> GetSiteScanSummary(string NameToSearch, ILog log)
        {
            //need to refactor
            List<SiteScan> ScanData = new List<SiteScan>();

            SearchQuery NewSearchQuery = SearchSites.GetNewSearchQuery();

            SearchQuery NewLiveSiteSearchQuery = SearchSites.GetNewLiveSiteSearchQuery();

            List<SearchQuerySite> Sites = new List<SearchQuerySite>();

            Sites.AddRange(NewSearchQuery.SearchSites);
            Sites.AddRange(NewLiveSiteSearchQuery.SearchSites);

            foreach (SearchQuerySite Site in Sites) //NewSearchQuery.SearchSites)
            {
                try
                {
                    var scanData = GetSiteScanData(Site.SiteEnum, NameToSearch, log);
                    scanData.SiteName = Site.SiteName;
                    scanData.SiteUrl = Site.SiteUrl;
                    scanData.SiteEnum = Site.SiteEnum;
                    ScanData.Add(scanData);
                }
                catch (Exception e)
                {
                    log.WriteLog("Error occured while processing the Site:" + Site.SiteEnum
                        + " Error Description:" + e.ToString());
                }
            }
            return ScanData;
        }

        public SiteScan GetSiteScanData(SiteEnum Enum, string NameToSearch, ILog log)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage : return GetFDADebarSiteScanDetails();

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    //return GetClinicalInevstigatorInspectionDetails();
                    return GetClinicalInvestigatorSiteScanDetails();

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLettersSiteScanDetails(NameToSearch, log);

                case SiteEnum.ERRProposalToDebarPage:
                    return GetProposalToDebarSiteScanDetails();

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssuranceSiteScanDetails();

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetClinicalInvestigatorDisqualificationSiteScanDetails();

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorSiteScanDetails();

                case SiteEnum.PHSAdministrativeActionListingPage :
                    return GetPHSSiteScanDetails();

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabaseSearchSiteScanDetails();

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCorporateIntegrityAgreementSiteScanDetails();

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSystemForAwardManagementSiteScanDetails();

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

        public SiteScan GetFDAWarningLettersSiteScanDetails(string NameToSearch, ILog log)
        {
            log.WriteLog(DateTime.Now.ToString(), "Extract Data starts");

            _SearchEngine.Load(NameToSearch);

            log.WriteLog(DateTime.Now.ToString(), "Extract Data ends");
            log.WriteLog("=================================================================================");

            var SiteData = _UOW.FDAWarningLettersRepository.GetAll().
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

        public SiteScan GetClinicalInvestigatorDisqualificationSiteScanDetails()
        {
            var SiteData = _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().
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

        public SiteScan GetSystemForAwardManagementSiteScanDetails()
        {
            var SiteData = _UOW.SystemForAwardManagementRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
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
    }
}
