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
        ISearchEngine _SearchEngine;
        private IUnitOfWork _UOW;

        public SiteScanData(ISearchEngine SearchEngine, IUnitOfWork uow)
        {
            _SearchEngine = SearchEngine;
            _UOW = uow;
        }

        public List<SiteScan> GetSiteScanSummary()
        {
            List<SiteScan> ScanData = new List<SiteScan>();

            SearchQuery NewSearchQuery = _SearchEngine.GetNewSearchQuery();

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
                case SiteEnum.PHSAdministrativeActionListingPage :
                    return GetPHSSiteScanDetails();
                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInevstigatorInspectionDetails();
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
    }
}
