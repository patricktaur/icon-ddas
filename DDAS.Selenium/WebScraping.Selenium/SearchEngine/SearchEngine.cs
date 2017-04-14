using System;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.Pages;
using System.Diagnostics;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Edge;
using DDAS.Models;
using DDAS.Services.Search;
using DDAS.Models.Entities.Domain.SiteData;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace WebScraping.Selenium.SearchEngine
{
    public class SearchEngine  : ISearchEngine, IDisposable
    {
        private IWebDriver _Driver;
        private IUnitOfWork _uow;
        private ISearchPage _searchPage;
        private IConfig _config;

        public SearchEngine(IUnitOfWork uow, IConfig Config)
        {
            _uow = uow;
            _config = Config;
            //_Driver = webDriver;
        }

        private ISearchPage GetSearchPage(SiteEnum siteEnum)
        {
            switch (siteEnum)
            {
                case SiteEnum.FDADebarPage:
                    return new FDADebarPage(Driver, _uow, _config);
                case SiteEnum.AdequateAssuranceListPage:
                    return new AdequateAssuranceListPage(Driver, _uow, _config);
                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return new ClinicalInvestigatorDisqualificationPage(Driver, _uow, _config);
                case SiteEnum.ERRProposalToDebarPage:
                    return new ERRProposalToDebarPage(Driver, _uow, _config);
                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return new ClinicalInvestigatorInspectionPage(Driver, _uow, _config);
                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return new CBERClinicalInvestigatorInspectionPage(Driver, _uow, _config);
                case SiteEnum.ExclusionDatabaseSearchPage:
                    return new ExclusionDatabaseSearchPage(Driver, _uow, _config);
                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return new SpeciallyDesignatedNationalsListPage(_uow, Driver, _config);
                case SiteEnum.FDAWarningLettersPage:
                    return new FDAWarningLettersPage(Driver, _uow, _config);
                case SiteEnum.PHSAdministrativeActionListingPage:
                    return new PHSAdministrativeActionListingPage(Driver, _uow, _config);
                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return new CorporateIntegrityAgreementsListPage(Driver, _uow, _config);
                case SiteEnum.SystemForAwardManagementPage:
                    return new SystemForAwardManagementPage(Driver, _uow, _config);
                        
                default: return null;
            }
        }

        #region Related to SiteLastUpdatedOn

        private DateTime? GetSiteLastUpdatedFromDatabase(SiteEnum siteEnum)
        {
            switch (siteEnum)
            {
                case SiteEnum.FDADebarPage:
                    var Collection = _uow.FDADebarPageRepository.GetAll();

                    if (Collection.Count == 0)
                        return null;

                    var SiteData = Collection.OrderByDescending(x => x.CreatedOn).First();
                    return SiteData.SiteLastUpdatedOn;

                case SiteEnum.AdequateAssuranceListPage:
                    var AdequateAssuranceCollection = 
                        _uow.AdequateAssuranceListRepository.GetAll();

                    if (AdequateAssuranceCollection.Count == 0)
                        return null;

                    var AdequateSiteData = 
                        AdequateAssuranceCollection.OrderByDescending(x => x.CreatedOn).First();

                    return AdequateSiteData.SiteLastUpdatedOn;

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    var DisqualificationSiteCollection = 
                        _uow.ClinicalInvestigatorDisqualificationRepository.GetAll();

                    if (DisqualificationSiteCollection.Count == 0)
                        return null;

                    var DisqualificationSiteData =
                        DisqualificationSiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return DisqualificationSiteData.SiteLastUpdatedOn;

                case SiteEnum.ERRProposalToDebarPage:
                    var ERRSiteCollection = 
                        _uow.ERRProposalToDebarRepository.GetAll();

                    if (ERRSiteCollection.Count == 0)
                        return null;

                    var ERRSiteData =
                        ERRSiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return ERRSiteData.SiteLastUpdatedOn;

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    var CIILSiteCollection = 
                        _uow.ClinicalInvestigatorInspectionListRepository.GetAll();

                    if (CIILSiteCollection.Count == 0)
                        return null;

                    var CIILSiteData =
                        CIILSiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return CIILSiteData.SiteLastUpdatedOn;

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    var CBERSiteCollection = 
                        _uow.CBERClinicalInvestigatorRepository.GetAll();

                    if (CBERSiteCollection.Count == 0)
                        return null;

                    var CBERSiteData =
                        CBERSiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return CBERSiteData.SiteLastUpdatedOn;

                case SiteEnum.ExclusionDatabaseSearchPage:
                    var ExclusionSiteCollection = 
                        _uow.ExclusionDatabaseSearchRepository.GetAll();

                    if (ExclusionSiteCollection.Count == 0)
                        return null;

                    var ExclusionSiteData =
                        ExclusionSiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return ExclusionSiteData.SiteLastUpdatedOn;

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    var SDNSiteCollection =
                        _uow.SpeciallyDesignatedNationalsRepository.GetAll();

                    if (SDNSiteCollection.Count == 0)
                        return null;

                    var SDNSiteData =
                        SDNSiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return SDNSiteData.SiteLastUpdatedOn;

                case SiteEnum.FDAWarningLettersPage:
                    var FDAWarningSiteCollection =
                        _uow.FDAWarningLettersRepository.GetAll();

                    if (FDAWarningSiteCollection.Count == 0)
                        return null;

                    var FDAWarningSiteData =
                        FDAWarningSiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return FDAWarningSiteData.SiteLastUpdatedOn;

                case SiteEnum.PHSAdministrativeActionListingPage:
                    var PHSSiteCollection =
                        _uow.PHSAdministrativeActionListingRepository.GetAll();

                    if (PHSSiteCollection.Count == 0)
                        return null;

                    var PHSSiteData =
                        PHSSiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return PHSSiteData.SiteLastUpdatedOn;

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    var CIASiteCollection =
                        _uow.CorporateIntegrityAgreementRepository.GetAll();

                    if (CIASiteCollection.Count == 0)
                        return null;

                    var CIASiteData =
                        CIASiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return CIASiteData.SiteLastUpdatedOn;

                case SiteEnum.SystemForAwardManagementPage:
                    var SAMSiteCollection =
                        _uow.SystemForAwardManagementRepository.GetAll();

                    if (SAMSiteCollection.Count == 0)
                        return null;

                    var SAMSiteData =
                        SAMSiteCollection.OrderByDescending(x => x.CreatedOn).First();

                    return SAMSiteData.SiteLastUpdatedOn;

                default: return null;
            }
        }

        //Pradeep 20Dec2016
        //Returning ReferenceId instead of RecId 
        private Guid? GetRecIdOfPreviousDocument(SiteEnum siteEnum)
        {
            switch (siteEnum)
            {
                case SiteEnum.FDADebarPage:
                    var FDASiteData =
                    _uow.FDADebarPageRepository.GetAll().OrderByDescending(
                        x => x.CreatedOn).First();

                    return FDASiteData.ReferenceId;

                case SiteEnum.AdequateAssuranceListPage:
                    var AdequateSiteData =
                    _uow.AdequateAssuranceListRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return AdequateSiteData.ReferenceId;

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    var DisqualificationSiteData =
                    _uow.ClinicalInvestigatorDisqualificationRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return DisqualificationSiteData.ReferenceId;

                case SiteEnum.ERRProposalToDebarPage:
                    var ERRSiteData =
                    _uow.ERRProposalToDebarRepository.GetAll().OrderByDescending(
                        x => x.CreatedOn).First();

                    return ERRSiteData.ReferenceId;

                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    var CIILSiteData =
                    _uow.ClinicalInvestigatorInspectionListRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return CIILSiteData.ReferenceId;

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    var CBERSiteData =
                    _uow.CBERClinicalInvestigatorRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return CBERSiteData.ReferenceId;

                case SiteEnum.ExclusionDatabaseSearchPage:
                    var ExclusionSiteData =
                    _uow.ExclusionDatabaseSearchRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return ExclusionSiteData.ReferenceId;

                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    var SDNSiteData =
                    _uow.SpeciallyDesignatedNationalsRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return SDNSiteData.ReferenceId;

                case SiteEnum.FDAWarningLettersPage:
                    var FDAWarningSiteData =
                    _uow.FDAWarningLettersRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return FDAWarningSiteData.ReferenceId;

                case SiteEnum.PHSAdministrativeActionListingPage:
                    var PHSSiteData =
                    _uow.PHSAdministrativeActionListingRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return PHSSiteData.ReferenceId;

                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    var CIASiteData =
                    _uow.CorporateIntegrityAgreementRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return CIASiteData.ReferenceId;

                case SiteEnum.SystemForAwardManagementPage:
                    var SAMSiteData =
                    _uow.SystemForAwardManagementRepository.GetAll().
                    OrderByDescending(x => x.CreatedOn).First();

                    return SAMSiteData.ReferenceId;

                default: return null;
            }
        }

        public bool IsDataExtractionRequired(SiteEnum siteEnum)
        {
            _searchPage = GetSearchPage(siteEnum);

            //Pradeep 21Dec2016 return true for live sites - Need to refactor
            if (siteEnum == SiteEnum.SystemForAwardManagementPage)
                //siteEnum == SiteEnum.FDAWarningLettersPage ||
                //siteEnum == SiteEnum.ClinicalInvestigatorDisqualificationPage)
                return true;

            var SiteUpdatedDateFromPage =
                _searchPage.SiteLastUpdatedDateFromPage;

            var SiteUpdatedDateFromDatabase =
                GetSiteLastUpdatedFromDatabase(siteEnum);

            if (SiteUpdatedDateFromDatabase == null ||
                SiteUpdatedDateFromPage > SiteUpdatedDateFromDatabase)
                return true;
            else
                return false;
        }
        
        #endregion

        //pending due to Repository<TEntity>
        private void SaveSiteData(SiteEnum siteEnum)
        {
            switch (siteEnum)
            {
                case SiteEnum.FDADebarPage:

                //_uow.FDADebarPageRepository.Add(_searchPage.baseSiteData);

                case SiteEnum.AdequateAssuranceListPage:

                case SiteEnum.ClinicalInvestigatorDisqualificationPage:

                case SiteEnum.ERRProposalToDebarPage:

                case SiteEnum.ClinicalInvestigatorInspectionPage:

                case SiteEnum.CBERClinicalInvestigatorInspectionPage:

                case SiteEnum.ExclusionDatabaseSearchPage:

                case SiteEnum.SpeciallyDesignedNationalsListPage:

                case SiteEnum.FDAWarningLettersPage:

                case SiteEnum.PHSAdministrativeActionListingPage:

                case SiteEnum.CorporateIntegrityAgreementsListPage:

                case SiteEnum.SystemForAwardManagementPage:
                    break;

            }
        }

        public IWebDriver Driver {
            get {
                if (_Driver == null)
                {
                    PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
                    service.IgnoreSslErrors = true;
                    service.SslProtocol = "any";
                    //service.LocalStoragePath = _config.AppDataDownloadsFolder;
                    _Driver = new PhantomJSDriver(service);

                    _Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);
                    _Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                    //Patrick 16Feb2017
                    //_Driver.Manage().Window.Maximize();
                    //_Driver.Manage().Window.Size = new Size(1124, 850);
                    //_Driver.Manage().Window.Size = new Size(800, 600);

                    //_Driver = new ChromeDriver(@"C:\Development\p926-ddas\Libraries\ChromeDriver");

                    return _Driver;
                }
                else
                    return _Driver;
            }
        }

        public IEnumerable<SiteDataItemBase> SiteData {
            get {
                return _searchPage.SiteData;
            }
        }

        public BaseSiteData baseSiteData {
            get {
                return _searchPage.baseSiteData;
            }
        }

        public enum DriverEnum
        {
            ChromeDriver,
            EdgeDriver,
            PhantomJSDriver
        };

        ~SearchEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            //foreach (var process in Process.GetProcessesByName("phantomjs"))
            //{
            //    process.Dispose();
            //}

            if (_Driver != null)
            {
                _Driver.Dispose();
            }
        }

        public void SaveData()
        {
            _searchPage.SaveData();
        }

        public void ExtractData(List<SitesToSearch> query, 
            ILog log)
        {
            var DBSites = query.Where(x => x.ExtractionMode.ToLower() == "db").ToList();

            log.WriteLog("Processing:" + DBSites.Count + " sites");
            foreach (SitesToSearch site in DBSites)
            {
                try
                {
                    ExtractData(site.SiteEnum, log);
                }
                catch (Exception e)
                {
                    log.WriteLog("Unable to extract data for: " + site.SiteEnum +
                        "Error Details: " + e.ToString());
                    continue;
                }
            }
        }

        public void ExtractData(SiteEnum siteEnum, ILog log)
        {
            var ExtractionRequired = IsDataExtractionRequired(siteEnum);

            var SiteData = _searchPage.baseSiteData;
            SiteData.SiteLastUpdatedOn = _searchPage.SiteLastUpdatedDateFromPage;

            if (ExtractionRequired)
            {
                log.WriteLog(DateTime.Now.ToString(), "Start extracting from: " + siteEnum);
                _searchPage.LoadContent();
                log.WriteLog(DateTime.Now.ToString(), "End extracting from: " + siteEnum);
            }
            else
            {
                SiteData.CreatedOn = DateTime.Now;
                SiteData.ReferenceId = GetRecIdOfPreviousDocument(siteEnum);
                log.WriteLog(DateTime.Now.ToString(), siteEnum + ": Source data has not been updated. Extraction is not required");
            }
            SaveData();
            log.WriteLog(DateTime.Now.ToString(), "Data Saved");
        }

        public void ExtractData(SiteEnum siteEnum, 
            string NameToSearch,
            int MatchCountLowerLimit,
            out DateTime? SiteLastUpdatedOn)
        {
            //var SiteData = _searchPage.baseSiteData;
            //SiteData.SiteLastUpdatedOn = _searchPage.SiteLastUpdatedDateFromPage;

            _searchPage = GetSearchPage(siteEnum);
            SiteLastUpdatedOn = _searchPage.SiteLastUpdatedDateFromPage;
            _searchPage.LoadContent(
                NameToSearch, MatchCountLowerLimit);
            _searchPage.SaveData();
        }
    }
}
