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

            Sites = Sites.OrderBy(Site => Site.SiteEnum).ToList();

            foreach (SearchQuerySite Site in Sites) //NewSearchQuery.SearchSites)
            {
                var scanData = new SiteScan();
                try
                {
                    scanData = GetSiteScanData(Site.SiteEnum, NameToSearch, log);
                    scanData.SiteName = Site.SiteName;
                    scanData.SiteUrl = Site.SiteUrl;
                    scanData.SiteEnum = Site.SiteEnum;
                    ScanData.Add(scanData);
                }
                catch (Exception e)
                {
                    log.WriteLog("Error occured while processing the Site:" + Site.SiteEnum
                        + " Error Description:" + e.ToString());
                    //scanData.HasErrors = true;
                    //scanData.ErrorDescription = "";
                }
                finally
                {
                    //ScanData.Add(scanData);
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
                    return GetFDAWarningLettersSiteScanDetails(NameToSearch, log, Enum);

                case SiteEnum.ERRProposalToDebarPage:
                    return GetProposalToDebarSiteScanDetails();

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssuranceSiteScanDetails();

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetClinicalInvestigatorDisqualificationSiteScanDetails(
                        NameToSearch, log, Enum);

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorSiteScanDetails();

                case SiteEnum.PHSAdministrativeActionListingPage :
                    return GetPHSSiteScanDetails();

                case SiteEnum.ExclusionDatabaseSearchPage:
                    return GetExclusionDatabaseSearchSiteScanDetails();

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return GetCorporateIntegrityAgreementSiteScanDetails();

                case SiteEnum.SystemForAwardManagementPage:
                    return GetSystemForAwardManagementSiteScanDetails(
                        NameToSearch, log, Enum);

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

            if (SiteData == null)
                throw new Exception("no extracts found for the site: FDADebarPage");
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

            if (SiteData == null)
                throw new Exception("no extracts found for the site: ClinicalInvestigatorInspectionPage");

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetFDAWarningLettersSiteScanDetails(
            string NameToSearch, ILog log, SiteEnum Enum)
        {
            log.WriteLog(DateTime.Now.ToString(), "Extract Data starts");

            _SearchEngine.Load(Enum, NameToSearch, "", log);

            log.WriteLog(DateTime.Now.ToString(), "Extract Data ends");
            //log.WriteLog("=================================================================================");

            var SiteData = _UOW.FDAWarningLettersRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            if (SiteData == null)
                throw new Exception("no extracts found for the site: FDAWarningLettersPage");

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

            if (SiteData == null)
                throw new Exception("no extracts found for the site: ERRProposalToDebarPage");

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

            if (SiteData == null)
                throw new Exception("no extracts found for the site: AdequateAssuranceListPage");

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetClinicalInvestigatorDisqualificationSiteScanDetails(
            string NameToSearch, ILog log, SiteEnum Enum)
        {
            log.WriteLog(DateTime.Now.ToString(), "Extract Data starts");

            _SearchEngine.Load(Enum, NameToSearch, "", log);

            log.WriteLog(DateTime.Now.ToString(), "Extract Data ends");
            //log.WriteLog("=================================================================================");

            var SiteData = _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            if (SiteData == null)
                throw new Exception("no extracts found for the site: DisqualificationProceedings");

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

            if (SiteData == null)
                throw new Exception("no extracts found for the site: CBERClinicalInvestigatorPage");

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

            if (SiteData == null)
                throw new Exception("no extracts found for the site: PHSSitePage");

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

            if (SiteData == null)
                throw new Exception("no extracts found for the site: ExclusionDatabaseSearchPage");

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

            if (SiteData == null)
                throw new Exception("no extracts found for the site: CorporateIntegrityAgreementListPage");

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }

        public SiteScan GetSystemForAwardManagementSiteScanDetails(
            string NameToSearch, ILog log, SiteEnum Enum)
        {
            log.WriteLog(DateTime.Now.ToString(), "Extract Data starts");

            _SearchEngine.Load(Enum, NameToSearch, "", log);

            log.WriteLog(DateTime.Now.ToString(), "Extract Data ends");
            //log.WriteLog("=================================================================================");

            var SiteData = _UOW.SystemForAwardManagementRepository.GetAll().
                OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            if (SiteData == null)
                throw new Exception("no extracts found for the site: SystemForAwardManagementSite");

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

            if (SiteData == null)
                throw new Exception("no extracts found for the site: SpeciallyDesignatedNationalsPage");

            SiteScan siteScan = new SiteScan();

            siteScan.DataExtractedOn = SiteData.CreatedOn;
            siteScan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            siteScan.DataId = SiteData.RecId;

            return siteScan;
        }
    }
}
