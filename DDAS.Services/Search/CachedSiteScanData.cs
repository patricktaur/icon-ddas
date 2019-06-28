using System;
using DDAS.Models;
using System.Runtime.Caching;
using DDAS.Models.Entities.Domain.SiteData;

namespace DDAS.Services.Search
{
    public class CachedSiteScanData
    {
        
        private IUnitOfWork _UOW;
        private MemoryCache _cache;
        public CachedSiteScanData(IUnitOfWork uow)
        {
            _cache = MemoryCache.Default;
            _UOW = uow;
        }

        public FDADebarPageSiteData GetFDADebarPageLatestCache()
        {
            string cacheKey = nameof(GetFDADebarPageLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.FDADebarPageRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (FDADebarPageSiteData)_cache.Get(cacheKey);
        }

        public ClinicalInvestigatorDisqualificationSiteData GetClinicalInvestigatorInspectionListLatestCache()
        {
            string cacheKey = nameof(GetClinicalInvestigatorInspectionListLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.ClinicalInvestigatorDisqualificationRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (ClinicalInvestigatorDisqualificationSiteData)_cache.Get(cacheKey);
        }

        public FDAWarningLettersSiteData GetFDAWarningLettersLatestCache()
        {
            string cacheKey = nameof(GetFDAWarningLettersLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.FDAWarningLettersRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (FDAWarningLettersSiteData)_cache.Get(cacheKey);
        }

        public ERRProposalToDebarPageSiteData GetProposalToDebarSiteScanDetailsLatestCache()
        {
            string cacheKey = nameof(GetProposalToDebarSiteScanDetailsLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.ERRProposalToDebarRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (ERRProposalToDebarPageSiteData)_cache.Get(cacheKey);
        }

    

        public AdequateAssuranceListSiteData GetAdequateAssuranceListLatestCache()
        {
            string cacheKey = nameof(GetAdequateAssuranceListLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.AdequateAssuranceListRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (AdequateAssuranceListSiteData)_cache.Get(cacheKey);
        }

        public ClinicalInvestigatorDisqualificationSiteData GetClinicalInvestigatorDisqualificationLatestCache()
        {
            string cacheKey = nameof(GetClinicalInvestigatorDisqualificationLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.ClinicalInvestigatorDisqualificationRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (ClinicalInvestigatorDisqualificationSiteData)_cache.Get(cacheKey);
        }

        public CBERClinicalInvestigatorInspectionSiteData GetCBERClinicalInvestigatorLatestCache()
        {
            string cacheKey = nameof(GetCBERClinicalInvestigatorLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.CBERClinicalInvestigatorRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (CBERClinicalInvestigatorInspectionSiteData)_cache.Get(cacheKey);
        }

        public PHSAdministrativeActionListingSiteData GetPHSAdministrativeActionListingLatestCache()
        {
            string cacheKey = nameof(GetPHSAdministrativeActionListingLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.PHSAdministrativeActionListingRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (PHSAdministrativeActionListingSiteData)_cache.Get(cacheKey);
        }

        public ExclusionDatabaseSearchPageSiteData GetExclusionDatabaseSearchLatestCache()
        {
            string cacheKey = nameof(GetExclusionDatabaseSearchLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.ExclusionDatabaseSearchRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (ExclusionDatabaseSearchPageSiteData)_cache.Get(cacheKey);
        }


        public CorporateIntegrityAgreementListSiteData GetCorporateIntegrityAgreementLatestCache()
        {
            string cacheKey = nameof(GetCorporateIntegrityAgreementLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.CorporateIntegrityAgreementRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (CorporateIntegrityAgreementListSiteData)_cache.Get(cacheKey);
        }

        public SystemForAwardManagementPageSiteData GetSystemForAwardManagementLatestCache()
        {
            string cacheKey = nameof(GetSystemForAwardManagementLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.SystemForAwardManagementRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (SystemForAwardManagementPageSiteData)_cache.Get(cacheKey);
        }

        public SpeciallyDesignatedNationalsListSiteData GetSpeciallyDesignatedNationalsLatestCache()
        {
            string cacheKey = nameof(GetSpeciallyDesignatedNationalsLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.SpeciallyDesignatedNationalsRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (SpeciallyDesignatedNationalsListSiteData)_cache.Get(cacheKey);
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
