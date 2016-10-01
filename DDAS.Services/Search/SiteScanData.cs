using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                ScanData.Add(scanData);
            }
            return ScanData;
        }

        public SiteScan GetSiteScanData(SiteEnum Enum)
        {
            switch(Enum)
            {
                case 0 : return GetScanDetails();
            }

            return null;
        }

        public SiteScan GetScanDetails()
        {
            var SiteData = _UOW.FDADebarPageRepository.GetAll().OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            SiteScan scan = new SiteScan();
           
            scan.DataExtractedOn = SiteData.CreatedOn;
            scan.SiteLastUpdatedOn = SiteData.SiteLastUpdatedOn;
            scan.DataId = SiteData.RecId;

            return scan;
        }
    }
}
