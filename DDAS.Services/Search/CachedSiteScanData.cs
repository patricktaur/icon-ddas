using System;
using DDAS.Models;
using System.Runtime.Caching;
using DDAS.Models.Entities.Domain.SiteData;
using System.Collections.Generic;

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

        public ClinicalInvestigatorInspectionSiteData GetClinicalInvestigatorInspectionListLatestCache()
        {
            string cacheKey = nameof(GetClinicalInvestigatorInspectionListLatestCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.ClinicalInvestigatorInspectionListRepository.GetLatestDocument();
                AddToCache(cacheKey, Data);
            }
            return (ClinicalInvestigatorInspectionSiteData)_cache.Get(cacheKey);
        }

        public List<ClinicalInvestigator> GetClinicalInvestigatorRecordsCache()
        {
            string cacheKey = nameof(GetClinicalInvestigatorRecordsCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.ClinicalInvestigatorInspectionRepository.GetAll();
                AddToCache(cacheKey, Data);
            }
            return (List<ClinicalInvestigator>)_cache.Get(cacheKey);
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

        public List<FDAWarningLetter> GetFDAWarningRecordsCache()
        {
            string cacheKey = nameof(GetFDAWarningRecordsCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.FDAWarningRepository.GetAll();
                AddToCache(cacheKey, Data);
            }
            return (List<FDAWarningLetter>)_cache.Get(cacheKey);
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

        public List<ExclusionDatabaseSearchList> GetExclusionDatabaseRecordsCache()
        {
            string cacheKey = nameof(GetExclusionDatabaseRecordsCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.ExclusionDatabaseRepository.GetAll();
                AddToCache(cacheKey, Data);
            }
            return (List<ExclusionDatabaseSearchList>)_cache.Get(cacheKey);
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

        public List<SystemForAwardManagement> GetSystemForAwardManagementRecordsCache()
        {
            string cacheKey = nameof(GetSystemForAwardManagementRecordsCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.SystemForAwardManagementRepository.GetAll();
                AddToCache(cacheKey, Data);
            }
            return (List<SystemForAwardManagement>)_cache.Get(cacheKey);
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

        public List<SDNList> GetSpeciallyDesignatedNationalsRecordsCache()
        {
            string cacheKey = nameof(GetSpeciallyDesignatedNationalsRecordsCache);
            if (!_cache.Contains(cacheKey))
            {
                var Data =
               _UOW.SDNSiteDataRepository.GetAll();
                AddToCache(cacheKey, Data);
            }
            return (List<SDNList>)_cache.Get(cacheKey);
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
