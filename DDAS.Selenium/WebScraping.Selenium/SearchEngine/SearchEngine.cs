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

namespace WebScraping.Selenium.SearchEngine
{
    public class SearchEngine  : ISearchEngine, IDisposable
    {
        private IWebDriver _Driver;
        private IUnitOfWork _uow;
        private ISearchPage _searchPage;
        public SearchEngine(IUnitOfWork uow)
        {
            _uow = uow;
        }

        #region To be deleted
        public SearchResult SearchByName(SearchQuery searchQuery)
        {
            Stopwatch stopwatch = new Stopwatch();

            string NameToSearch = searchQuery.NameToSearch.Replace(",","");

            SearchResult searchResult = new SearchResult();
            searchResult.NameToSearch = searchQuery.NameToSearch;
            searchResult.SearchedBy = "Pradeep";
            searchResult.SearchedOn = DateTime.Now.ToString();
            foreach (SearchQuerySite site in searchQuery.SearchSites)
            {
                stopwatch.Start();
                var LoadContent = SearchByName(NameToSearch, site.SiteEnum);
                stopwatch.Stop();
                stopwatch.Reset();
            }
            return searchResult;
        }

        public bool SearchByName(string NameToSearch, SiteEnum siteEnum)
        {
                NameToSearch = NameToSearch.Replace(",", "");
                var PageObject = GetSearchPage(siteEnum);
                PageObject.LoadContent(NameToSearch, "");
                PageObject.SaveData();
                return true;
        }
        #endregion

        #region Load

        //public void Load(string NameToSearch, string DownloadFolder, ILog log) //LoadAll
        //{
        //    var query = SearchSites.GetNewLiveSiteSearchQuery();

        //    log.WriteLog("Processing:" + query.SearchSites.Count + " sites");
        //    foreach (SearchQuerySite site in query.SearchSites)
        //    {
        //        Load(site.SiteEnum, NameToSearch, DownloadFolder);                
        //    }
        //}

        public void Load(List<SearchQuerySite> query, string DownloadFolder, ILog log)  //Load some
        {
            log.WriteLog("Processing:" + query.Count + " sites");
            foreach (SearchQuerySite site in query)
            {
                try
                {
                    log.WriteLog(DateTime.Now.ToString(), "Start extracting from:" + site.SiteEnum);
                    Load(site.SiteEnum, "", DownloadFolder);
                    log.WriteLog(DateTime.Now.ToString(), "End extracting from:" + site.SiteEnum);
                    SaveData();
                    log.WriteLog("Data Saved");
                }
                catch (WebDriverTimeoutException e)
                {
                    throw new Exception(e.ToString());
                }
            }
        }

        public void Load(SiteEnum siteEnum, string NameToSearch, 
            string DownloadFolder)  //Load one
        {
            

            //var page = GetSearchPage(siteEnum);

            //page.LoadContent(NameToSearch, DownloadFolder);

            _searchPage = GetSearchPage(siteEnum);
            _searchPage.LoadContent(NameToSearch, DownloadFolder);

           

            //page.SaveData();
            //log.WriteLog( "Data Saved" );
        }
        #endregion

        private ISearchPage GetSearchPage(SiteEnum siteEnum)
        {
            switch (siteEnum)
            {
                case SiteEnum.FDADebarPage:
                    return new FDADebarPage(Driver, _uow);
                case SiteEnum.AdequateAssuranceListPage:
                    return new AdequateAssuranceListPage(Driver, _uow);
                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return new ClinicalInvestigatorDisqualificationPage(Driver, _uow);
                case SiteEnum.ERRProposalToDebarPage:
                    return new ERRProposalToDebarPage(Driver, _uow);
                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return new ClinicalInvestigatorInspectionPage(Driver, _uow);
                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return new CBERClinicalInvestigatorInspectionPage(Driver, _uow);
                case SiteEnum.ExclusionDatabaseSearchPage:
                    return new ExclusionDatabaseSearchPage(Driver, _uow);
                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return new SpeciallyDesignatedNationalsListPage(_uow, Driver);
                case SiteEnum.FDAWarningLettersPage:
                    return new FDAWarningLettersPage(Driver, _uow);
                case SiteEnum.PHSAdministrativeActionListingPage:
                    return new PHSAdministrativeActionListingPage(Driver, _uow);
                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return new CorporateIntegrityAgreementsListPage(Driver, _uow);
                case SiteEnum.SystemForAwardManagementPage:
                    return new SystemForAwardManagementPage(Driver, _uow);
                        
                default: return null;
            }
        }

         public IWebDriver Driver {
            get { 
                if (_Driver == null)
                {

                    PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
                    service.IgnoreSslErrors = true;
                    service.SslProtocol = "any";

                    _Driver = new PhantomJSDriver(service);

                    //_Driver = new ChromeDriver(@"C:\Development\p926-ddas\Libraries\ChromeDriver");

                    _Driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

                    return _Driver;
                }
                else
                    return _Driver;
            }
        }

        public IEnumerable<SiteDataItemBase> SiteData
        {
            get
            {
                return _searchPage.SiteData;
            }
        }

        public enum DriverEnum
        {
            ChromeDriver,
            EdgeDriver,
            PhantomJSDriver
        };

        public void Dispose()
        {
            if (_Driver != null)
            {
                _Driver.Quit();
            }
            
        }

        public void SaveData()
        {
            _searchPage.SaveData();
        }
    }
}
