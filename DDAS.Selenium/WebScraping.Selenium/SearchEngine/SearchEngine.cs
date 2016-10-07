using System;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using WebScraping.Selenium.Pages;
using System.Collections.Generic;
using System.Diagnostics;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using DDAS.Models.Repository;
//using DDAS.Data.Mongo.Repositories;
using DDAS.Models;

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
                PageObject.LoadContent();
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
       
        public void Load() //LoadAll
        {
            var query = GetNewSearchQuery();
            _log.WriteLog("Processing:" + query.SearchSites.Count + " sites");
            foreach (SearchQuerySite site in query.SearchSites)
            {
                Load(site.SiteEnum);
            }
        }

        public void Load(SearchQuery query)  //Load some
        {
            _log.WriteLog("Processing:" + query.SearchSites.Count + " sites");
            foreach (SearchQuerySite site in query.SearchSites)
            {
                Load(site.SiteEnum);
            }
        }

        public void Load(SiteEnum siteEnum)  //Load one
        {
            _log.WriteLog(DateTime.Now.ToString(), "Start extracting from:" + siteEnum);

            var page = GetSearchPage(siteEnum);
            page.LoadContent();

            _log.WriteLog(DateTime.Now.ToString(), "End extracting from:" + siteEnum);

            page.SaveData();
            _log.WriteLog( "Data Saved" );

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
                    /*
                    PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
                    service.IgnoreSslErrors = true;
                    service.SslProtocol = "any";

                    _Driver = new PhantomJSDriver(service);
                    */

                    _Driver = new ChromeDriver(@"C:\Development\p926-ddas\Libraries\ChromeDriver");

                    _Driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

                    return _Driver;
                }
                else
                {
                    return _Driver;
                }
            }
        }

        public SearchQuery GetNewSearchQuery()
        {
            return new SearchQuery
            {
                NameToSearch = "Anthony, James Michael",
                SearchSites = new List<SearchQuerySite>
                {
                    new SearchQuerySite {Selected = true, SiteName="FDA Debarment List", SiteShortName="FDA Debarment List", SiteEnum = SiteEnum.FDADebarPage, SiteUrl="XXX" },

                    //new SearchQuerySite {Selected = true, SiteName="Clinical Investigator Inspection List (CLIL)(CDER", SiteShortName="Clinical Investigator Insp...", SiteEnum = SiteEnum.ClinicalInvestigatorInspectionPage, SiteUrl="XXX" },
                   
                    //new SearhQuerySite {Selected = true, SiteName="FDA Warning Letters and Responses", SiteShortName="FDA Warning Letters ...", SiteEnum = SiteEnum.FDAWarningLettersPage, SiteUrl="XXX" },
                    
                    //new SearchQuerySite {Selected = true, SiteName="Notice of Opportunity for Hearing (NOOH) – Proposal to Debar", SiteShortName="NOOH – Proposal to Debar", SiteEnum = SiteEnum.ERRProposalToDebarPage, SiteUrl="XXX" },
                    
                    //new SearchQuerySite {Selected = true, SiteName="Adequate Assurances List for Clinical Investigators", SiteShortName="Adequate Assurances List ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl="XXX" },

                    //new SearhQuerySite {Selected = true, SiteName="Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName="Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl="XXX" },
                    
                    //new SearhQuerySite {Selected = true, SiteName="Clinical Investigator Inspection List (CBER)", SiteShortName="Inspection List", SiteEnum = SiteEnum.CBERClinicalInvestigatorInspectionPage, SiteUrl="XXX" },

                    //new SearchQuerySite {Selected = true, SiteName="PHS Administrative Actions Listing ", SiteShortName="PHS Administrative Actions", SiteEnum = SiteEnum.PHSAdministrativeActionListingPage, SiteUrl="XXX" },

                    //new SearhQuerySite {Selected = true, SiteName="HHS/OIG/ EXCLUSIONS DATABASE SEARCH/ FRAUD", SiteShortName="HHS/OIG/ EXCLUSIONS ...", SiteEnum = SiteEnum.ExclusionDatabaseSearchPage, SiteUrl="XXX" },
                    
                    //new SearhQuerySite {Selected = true, SiteName="HHS/OIG Corporate Integrity Agreements/Watch List", SiteShortName="HHS/OIG Corporate Integrity", SiteEnum = SiteEnum.CorporateIntegrityAgreementsListPage, SiteUrl="XXX" },

                    //new SearhQuerySite {Selected = true, SiteName="SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName="SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.SystemForAwardManagementPage, SiteUrl="XXX" },

                    //new SearchQuerySite {Selected = true, SiteName="LIST OF SPECIALLY DESIGNATED NATIONALS", SiteShortName="SPECIALLY DESIGNATED ...", SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage, SiteUrl="XXX" },
                             
                }

            };
           
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
