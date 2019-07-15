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
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.FDADebarPageRepository.GetLatestDocument();

            //    if(Data == null)
            //    {
            //        Data = new FDADebarPageSiteData()
            //        {
            //            DebarredPersons = new List<DebarredPerson>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (FDADebarPageSiteData)_cache.Get(cacheKey);
            return _UOW.FDADebarPageRepository.GetLatestDocument();
        }

        public ClinicalInvestigatorInspectionSiteData GetClinicalInvestigatorInspectionListLatestCache()
        {
            //string cacheKey = nameof(GetClinicalInvestigatorInspectionListLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.ClinicalInvestigatorInspectionListRepository.GetLatestDocument();

            //    if(Data == null)
            //    {
            //        Data = new ClinicalInvestigatorInspectionSiteData()
            //        {
            //            ClinicalInvestigatorInspectionList = new List<ClinicalInvestigator>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (ClinicalInvestigatorInspectionSiteData)_cache.Get(cacheKey);
            return _UOW.ClinicalInvestigatorInspectionListRepository.GetLatestDocument();
        }

        public List<ClinicalInvestigator> GetClinicalInvestigatorRecordsCache()
        {
            //string cacheKey = nameof(GetClinicalInvestigatorRecordsCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.ClinicalInvestigatorInspectionRepository.GetAll();

            //    if (Data == null)
            //    {
            //        Data = new List<ClinicalInvestigator>();
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (List<ClinicalInvestigator>)_cache.Get(cacheKey);
            return _UOW.ClinicalInvestigatorInspectionRepository.GetAll();
        }

        public FDAWarningLettersSiteData GetFDAWarningLettersLatestCache()
        {
            //string cacheKey = nameof(GetFDAWarningLettersLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.FDAWarningLettersRepository.GetLatestDocument();

            //    if(Data == null)
            //    {
            //        Data = new FDAWarningLettersSiteData();
            //        Data.FDAWarningLetterList = new List<FDAWarningLetter>();
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (FDAWarningLettersSiteData)_cache.Get(cacheKey);
            return _UOW.FDAWarningLettersRepository.GetLatestDocument();
        }

        public List<FDAWarningLetter> GetFDAWarningRecordsCache()
        {
            //string cacheKey = nameof(GetFDAWarningRecordsCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.FDAWarningRepository.GetAll();

            //    if(Data == null)
            //    {
            //        Data = new List<FDAWarningLetter>();
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (List<FDAWarningLetter>)_cache.Get(cacheKey);
            return _UOW.FDAWarningRepository.GetAll();
        }

        public ERRProposalToDebarPageSiteData GetProposalToDebarSiteScanDetailsLatestCache()
        {
            //string cacheKey = nameof(GetProposalToDebarSiteScanDetailsLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.ERRProposalToDebarRepository.GetLatestDocument();

            //    if (Data == null)
            //    {
            //        Data = new ERRProposalToDebarPageSiteData()
            //        {
            //            ProposalToDebar = new List<ProposalToDebar>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (ERRProposalToDebarPageSiteData)_cache.Get(cacheKey);
            return _UOW.ERRProposalToDebarRepository.GetLatestDocument();
        }

        public AdequateAssuranceListSiteData GetAdequateAssuranceListLatestCache()
        {
            //string cacheKey = nameof(GetAdequateAssuranceListLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.AdequateAssuranceListRepository.GetLatestDocument();

            //    if (Data == null)
            //    {
            //        Data = new AdequateAssuranceListSiteData()
            //        {
            //            AdequateAssurances = new List<AdequateAssuranceList>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (AdequateAssuranceListSiteData)_cache.Get(cacheKey);
            return _UOW.AdequateAssuranceListRepository.GetLatestDocument();
        }

        public ClinicalInvestigatorDisqualificationSiteData GetClinicalInvestigatorDisqualificationLatestCache()
        {
            //string cacheKey = nameof(GetClinicalInvestigatorDisqualificationLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.ClinicalInvestigatorDisqualificationRepository.GetLatestDocument();

            //    if (Data == null)
            //    {
            //        Data = new ClinicalInvestigatorDisqualificationSiteData()
            //        {
            //            DisqualifiedInvestigatorList = new List<DisqualifiedInvestigator>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (ClinicalInvestigatorDisqualificationSiteData)_cache.Get(cacheKey);
            return _UOW.ClinicalInvestigatorDisqualificationRepository.GetLatestDocument();
        }

        public CBERClinicalInvestigatorInspectionSiteData GetCBERClinicalInvestigatorLatestCache()
        {
            //string cacheKey = nameof(GetCBERClinicalInvestigatorLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.CBERClinicalInvestigatorRepository.GetLatestDocument();

            //    if (Data == null)
            //    {
            //        Data = new CBERClinicalInvestigatorInspectionSiteData()
            //        {
            //            ClinicalInvestigator = new List<CBERClinicalInvestigator>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (CBERClinicalInvestigatorInspectionSiteData)_cache.Get(cacheKey);
            return _UOW.CBERClinicalInvestigatorRepository.GetLatestDocument();
        }

        public PHSAdministrativeActionListingSiteData GetPHSAdministrativeActionListingLatestCache()
        {
            //string cacheKey = nameof(GetPHSAdministrativeActionListingLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.PHSAdministrativeActionListingRepository.GetLatestDocument();

            //    if (Data == null)
            //    {
            //        Data = new PHSAdministrativeActionListingSiteData()
            //        {
            //            PHSAdministrativeSiteData = new List<PHSAdministrativeAction>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (PHSAdministrativeActionListingSiteData)_cache.Get(cacheKey);
            return _UOW.PHSAdministrativeActionListingRepository.GetLatestDocument();
        }

        public ExclusionDatabaseSearchPageSiteData GetExclusionDatabaseSearchLatestCache()
        {
            //string cacheKey = nameof(GetExclusionDatabaseSearchLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.ExclusionDatabaseSearchRepository.GetLatestDocument();

            //    if (Data == null)
            //    {
            //        Data = new ExclusionDatabaseSearchPageSiteData()
            //        {
            //            ExclusionSearchList = new List<ExclusionDatabaseSearchList>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (ExclusionDatabaseSearchPageSiteData)_cache.Get(cacheKey);
            return _UOW.ExclusionDatabaseSearchRepository.GetLatestDocument();
        }

        public List<ExclusionDatabaseSearchList> GetExclusionDatabaseRecordsCache()
        {
            //string cacheKey = nameof(GetExclusionDatabaseRecordsCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.ExclusionDatabaseRepository.GetAll();

            //    if (Data == null)
            //    {
            //        Data = new List<ExclusionDatabaseSearchList>();
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (List<ExclusionDatabaseSearchList>)_cache.Get(cacheKey);
            return _UOW.ExclusionDatabaseRepository.GetAll();
        }

        public CorporateIntegrityAgreementListSiteData GetCorporateIntegrityAgreementLatestCache()
        {
            //string cacheKey = nameof(GetCorporateIntegrityAgreementLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.CorporateIntegrityAgreementRepository.GetLatestDocument();

            //    if (Data == null)
            //    {
            //        Data = new CorporateIntegrityAgreementListSiteData()
            //        {
            //            CIAListSiteData = new List<CIAList>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (CorporateIntegrityAgreementListSiteData)_cache.Get(cacheKey);
            return _UOW.CorporateIntegrityAgreementRepository.GetLatestDocument();
        }

        public SystemForAwardManagementPageSiteData GetSystemForAwardManagementLatestCache()
        {
            //string cacheKey = nameof(GetSystemForAwardManagementLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.SystemForAwardManagementRepository.GetLatestDocument();

            //    if (Data == null)
            //    {
            //        Data = new SystemForAwardManagementPageSiteData()
            //        {
            //            SAMSiteData = new List<SystemForAwardManagement>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (SystemForAwardManagementPageSiteData)_cache.Get(cacheKey);
            return _UOW.SystemForAwardManagementRepository.GetLatestDocument();
        }

        public List<SystemForAwardManagement> GetSystemForAwardManagementRecordsCache()
        {
            //string cacheKey = nameof(GetSystemForAwardManagementRecordsCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.SAMSiteDataRepository.GetAll();

            //    if (Data == null)
            //    {
            //        Data = new List<SystemForAwardManagement>();
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (List<SystemForAwardManagement>)_cache.Get(cacheKey);
            return _UOW.SAMSiteDataRepository.GetAll();
        }

        public SpeciallyDesignatedNationalsListSiteData GetSpeciallyDesignatedNationalsLatestCache()
        {
            //string cacheKey = nameof(GetSpeciallyDesignatedNationalsLatestCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.SpeciallyDesignatedNationalsRepository.GetLatestDocument();

            //    if (Data == null)
            //    {
            //        Data = new SpeciallyDesignatedNationalsListSiteData()
            //        {
            //            SDNListSiteData = new List<SDNList>()
            //        };
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (SpeciallyDesignatedNationalsListSiteData)_cache.Get(cacheKey);
            return _UOW.SpeciallyDesignatedNationalsRepository.GetLatestDocument();
        }

        public List<SDNList> GetSpeciallyDesignatedNationalsRecordsCache()
        {
            //string cacheKey = nameof(GetSpeciallyDesignatedNationalsRecordsCache);
            //if (!_cache.Contains(cacheKey))
            //{
            //    var Data =
            //   _UOW.SDNSiteDataRepository.GetAll();

            //    if (Data == null)
            //    {
            //        Data = new List<SDNList>();
            //    }

            //    AddToCache(cacheKey, Data);
            //}
            //return (List<SDNList>)_cache.Get(cacheKey);
            return _UOW.SDNSiteDataRepository.GetAll();
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
