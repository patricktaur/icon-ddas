using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.Caching;
using DDAS.Models.Entities.Domain.SiteData;

namespace DDAS.Data.Mongo
{
    public class CachedData
    {
        private IUnitOfWork _UOW;
        private MemoryCache _cache;
        public CachedData()
        {
            _cache = MemoryCache.Default;
        }

        public AdequateAssuranceListSiteData AdequateAssuranceListSiteDataFromCache()
        {
            string cacheKey = "AdequateAssuranceListSiteDataCache";
            if (!_cache.Contains(cacheKey))
            {
                var AdequateAssuranceList =
               _UOW.AdequateAssuranceListRepository.GetLatestDocument();
                AddToCache(cacheKey, AdequateAssuranceList);
            }
            return (AdequateAssuranceListSiteData)_cache.Get(cacheKey);
        }

        public CBERClinicalInvestigatorInspectionSiteData CBERClinicalInvestigatorInspectionSiteDataCache()
        {
            string cacheKey = "CBERClinicalInvestigatorInspectionSiteDataCache";
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.AdequateAssuranceListRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (CBERClinicalInvestigatorInspectionSiteData)_cache.Get(cacheKey);
        }




        public FDADebarPageSiteData GetFDADebarPageRepositoryCache(Guid? SiteDataId)
        {
            string cacheKey = "FDADebarPageRepositoryCache";
            if (!_cache.Contains(cacheKey))
            {
                var FDASearchResult =
               _UOW.FDADebarPageRepository.FindById(SiteDataId);
                AddToCache(cacheKey, FDASearchResult);
            }
            return (FDADebarPageSiteData)_cache.Get(cacheKey);
        }



        private void AddToCache(string cacheKey, object data)
        {

            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            DateTime expiryDate = DateTime.Now.Date.AddHours(24);
            cacheItemPolicy.AbsoluteExpiration = expiryDate;
            _cache.Add(cacheKey, data, cacheItemPolicy);
        }

    }
}



