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

namespace WebScraping.Selenium.SearchEngine
{
    public class SearchEngine  : ISearchEngine, IDisposable
    {
        private IWebDriver _Driver;
        private string _DownloadFolder;
        private ILog _log;
        private IUnitOfWork _uow;

        public SearchEngine( ILog log, IUnitOfWork uow)
        {
             //_DownloadFolder = downloadFolder;
            _log = log;
            _uow = uow;
        }
        
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
               // if (site.Selected == true)
               // {
                    //ResultAtSite resultAtSite = new ResultAtSite();
                    stopwatch.Start();
                    var LoadContent = SearchByName(NameToSearch, site.SiteEnum);
                    stopwatch.Stop();
                    //resultAtSite.TimeTakenInMs = stopwatch.ElapsedMilliseconds.ToString();

                    //searchResult.resultAtSites.Add(resultAtSite);
                    stopwatch.Reset();
               // }
            }
            return searchResult;
        }

        public bool SearchByName(string NameToSearch, SiteEnum siteEnum)
        {
            //try
            //{
                NameToSearch = NameToSearch.Replace(",", "");
                var PageObject = GetSearchPage(siteEnum);
                PageObject.LoadContent(NameToSearch);
                PageObject.SaveData();
                return true;
            //var result = PageObject.GetResultAtSite(NameToSearch);
            //    result.SiteEnum = siteEnum;
            //    result.HasErrors = false;
            //    return result;
            //}

            //catch (Exception Ex)
            //{

            //    return new ResultAtSite { HasErrors = true, ErrorDescription = "Error while reading the site" + Ex.Message };
            //}

        }

        #region Load
        
        public void Load(string NameToSearch) //LoadAll
        {
            ISearchEngine SearchEngine = new SearchEngine(_log, _uow);
            SiteScanData ScanData = new SiteScanData(_uow, _log, SearchEngine);
            var query = ScanData.GetNewLiveSiteSearchQuery();

            //_log.WriteLog("Processing:" + query.SearchSites.Count + " sites");
            foreach (SearchQuerySite site in query.SearchSites)
            {
                Load(site.SiteEnum, NameToSearch);
            }
        }

        public void Load(SearchQuery query)  //Load some
        {
            _log.WriteLog("Processing:" + query.SearchSites.Count + " sites");
            foreach (SearchQuerySite site in query.SearchSites)
            {
                Load(site.SiteEnum, query.NameToSearch);
            }
        }

        public void Load(SiteEnum siteEnum, string NameToSearch)  //Load one
        {
            //_log.WriteLog(DateTime.Now.ToString(), "Start extracting from:" + siteEnum);

            var page = GetSearchPage(siteEnum);
            page.LoadContent(NameToSearch);

            //_log.WriteLog(DateTime.Now.ToString(), "End extracting from:" + siteEnum);

            page.SaveData();
            //_log.WriteLog( "Data Saved" );

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
                    return new SpeciallyDesignatedNationalsListPage(_DownloadFolder, _uow, Driver);
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
                    
                    //PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
                    //service.IgnoreSslErrors = true;
                    //service.SslProtocol = "any";

                    _Driver = new PhantomJSDriver();

                    //_Driver = new ChromeDriver(@"C:\Development\p926-ddas\Libraries\ChromeDriver");

                    _Driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

                    return _Driver;
                }
                else
                    return _Driver;
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

    
    }
}
