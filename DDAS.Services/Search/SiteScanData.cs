﻿using DDAS.Models;
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
        private CachedSiteScanData _cachedData;

        public SiteScanData( IUnitOfWork uow)
        {
            _UOW = uow;
            _cachedData = new CachedSiteScanData(_UOW);
        }
        
        public SiteScan GetSiteScanData(SiteEnum Enum)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage : return GetFDADebarSiteScanDetails();

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInvestigatorSiteScanDetails();

                case SiteEnum.FDAWarningLettersPage:
                    return GetFDAWarningLettersSiteScanDetails();

                case SiteEnum.ERRProposalToDebarPage:
                    return GetProposalToDebarSiteScanDetails();

                case SiteEnum.AdequateAssuranceListPage:
                    return GetAdequateAssuranceSiteScanDetails();

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return GetClinicalInvestigatorDisqualificationSiteScanDetails();

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return GetCBERClinicalInvestigatorSiteScanDetails();

                case SiteEnum.PHSAdministrativeActionListingPage:
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

        //Pradeep 17Dec2016 Changing FindById(SiteData.RecId) to
        //FindById(SiteData.ReferenceId)

        private SiteScan GetFDADebarSiteScanDetails()
        {
            //var SiteData = _UOW.FDADebarPageRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            var SiteData = _cachedData.GetFDADebarPageLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                //Patrick 02Dec2016
                return null;
            //throw new Exception("Failed to extract data for the site: FDADebarPage");

            //var ExtractedSiteData = 
            //    _UOW.FDADebarPageRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();
            
            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetClinicalInvestigatorSiteScanDetails()
        {
            //var SiteData = _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.ClinicalInvestigatorInspectionListRepository.GetLatestDocument();

            var SiteData = _cachedData.GetClinicalInvestigatorInspectionListLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                //Patrick 02Dec2016
                return null;
            //throw new Exception("Failed to extract data for the site: FDADebarPage");

            //var ExtractedSiteData =
            //    _UOW.ClinicalInvestigatorInspectionListRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetFDAWarningLettersSiteScanDetails()
        {
            //var SiteData = _UOW.FDAWarningLettersRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.FDAWarningLettersRepository.GetLatestDocument();

            var SiteData = _cachedData.GetFDAWarningLettersLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                return null;

            //var ExtractedSiteData =
            //    _UOW.FDAWarningLettersRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetProposalToDebarSiteScanDetails()
        {
            //var SiteData = _UOW.ERRProposalToDebarRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.ERRProposalToDebarRepository.GetLatestDocument();

            var SiteData = _cachedData.GetProposalToDebarSiteScanDetailsLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                //Patrick 02Dec2016
                return null;
            //throw new Exception("Failed to extract data for the site: FDADebarPage");

            //var ExtractedSiteData =
            //    _UOW.ERRProposalToDebarRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetAdequateAssuranceSiteScanDetails()
        {
            //var SiteData = _UOW.AdequateAssuranceListRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.AdequateAssuranceListRepository.GetLatestDocument();
            //Test:
            var SiteData = _cachedData.GetAdequateAssuranceListLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                return null;

            //var ExtractedSiteData =
            //    _UOW.AdequateAssuranceListRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetClinicalInvestigatorDisqualificationSiteScanDetails()
        {
            //var SiteData = _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.ClinicalInvestigatorDisqualificationRepository.GetLatestDocument();

            var SiteData = _cachedData.GetClinicalInvestigatorDisqualificationLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                return null;

            //var ExtractedSiteData =
            //    _UOW.ClinicalInvestigatorDisqualificationRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetCBERClinicalInvestigatorSiteScanDetails()
        {
            //var SiteData = _UOW.CBERClinicalInvestigatorRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.CBERClinicalInvestigatorRepository.GetLatestDocument();

            var SiteData = _cachedData.GetCBERClinicalInvestigatorLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                //Patrick 02Dec2016
                return null;
            //throw new Exception("Failed to extract data for the site: FDADebarPage");

            //var ExtractedSiteData =
            //    _UOW.CBERClinicalInvestigatorRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetPHSSiteScanDetails()
        {
            //var SiteData = _UOW.PHSAdministrativeActionListingRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.PHSAdministrativeActionListingRepository.GetLatestDocument();

            var SiteData = _cachedData.GetPHSAdministrativeActionListingLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                //Patrick 02Dec2016
                return null;
            //throw new Exception("Failed to extract data for the site: FDADebarPage");

            //var ExtractedSiteData =
            //    _UOW.PHSAdministrativeActionListingRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetExclusionDatabaseSearchSiteScanDetails()
        {
            //var SiteData = _UOW.ExclusionDatabaseSearchRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.ExclusionDatabaseSearchRepository.GetLatestDocument();

            var SiteData = _cachedData.GetExclusionDatabaseSearchLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                //Patrick 02Dec2016
                return null;
            //throw new Exception("Failed to extract data for the site: FDADebarPage");

            //var ExtractedSiteData =
            //    _UOW.ExclusionDatabaseSearchRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetCorporateIntegrityAgreementSiteScanDetails()
        {
            //var SiteData = _UOW.CorporateIntegrityAgreementRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.CorporateIntegrityAgreementRepository.GetLatestDocument();

            var SiteData = _cachedData.GetCorporateIntegrityAgreementLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                //Patrick 02Dec2016
                return null;
            //throw new Exception("Failed to extract data for the site: FDADebarPage");

            //var ExtractedSiteData =
            //    _UOW.CorporateIntegrityAgreementRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetSystemForAwardManagementSiteScanDetails()
        {
            //var SiteData = _UOW.SystemForAwardManagementRepository.GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.SystemForAwardManagementRepository.GetLatestDocument();

            var SiteData = _cachedData.GetSystemForAwardManagementLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                return null;

            //var ExtractedSiteData =
            //    _UOW.SystemForAwardManagementRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }

        private SiteScan GetSpeciallyDesignatedNationsDetails()
        {
            //var SiteData = _UOW.SpeciallyDesignatedNationalsRepository.
            //    GetAll().
            //    OrderByDescending(t => t.CreatedOn).FirstOrDefault();

            //var SiteData = _UOW.SpeciallyDesignatedNationalsRepository.GetLatestDocument();

            var SiteData = _cachedData.GetSpeciallyDesignatedNationalsLatestCache();

            if (SiteData == null || SiteData.ReferenceId == null)
                //Patrick 02Dec2016
                return null;
            //throw new Exception("Failed to extract data for the site: FDADebarPage");

            //var ExtractedSiteData =
            //    _UOW.SpeciallyDesignatedNationalsRepository.FindById(SiteData.ReferenceId);

            var ExtractedSiteData = SiteData;

            SiteScan scan = new SiteScan();

            scan.DataExtractedOn = ExtractedSiteData.CreatedOn;
            scan.SiteLastUpdatedOn = ExtractedSiteData.SiteLastUpdatedOn;
            scan.DataId = ExtractedSiteData.ReferenceId;

            return scan;
        }
    }
}
