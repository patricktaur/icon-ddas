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

namespace WebScraping.Selenium.SearchEngine
{
    public class SearchEngine : ISearchEngine, IDisposable
    {
        private IWebDriver _Driver;
        private string _DownloadFolder;

        public SearchEngine(string downloadFolder)
        {
            _DownloadFolder = downloadFolder;
        }
        
        public SearchResult SearchByName(SearchQuery searchQuery)
        {
            Stopwatch stopwatch = new Stopwatch();

            string NameToSearch = searchQuery.NameToSearch.Replace(",","");

            SearchResult searchResult = new SearchResult();
            searchResult.NameToSearch = searchQuery.NameToSearch;
            searchResult.SearchedBy = "Pradeep";
            searchResult.SearchedOn = DateTime.Now.ToString();
            foreach (SearhQuerySite site in searchQuery.SearchSites)
            {
               // if (site.Selected == true)
               // {
                    ResultAtSite resultAtSite = new ResultAtSite();
                    stopwatch.Start();
                    resultAtSite = SearchByName(NameToSearch, site.SiteEnum);
                    stopwatch.Stop();
                    resultAtSite.TimeTakenInMs = stopwatch.ElapsedMilliseconds.ToString();

                    searchResult.resultAtSites.Add(resultAtSite);
                    stopwatch.Reset();
               // }

            }
            return searchResult;
        }

        
        public ResultAtSite SearchByName(string NameToSearch, SiteEnum siteEnum)
        {

            //try
            //{
                NameToSearch = NameToSearch.Replace(",", "");
                var PageObject = GetSearchPage(siteEnum);

                PageObject.LoadContent(NameToSearch);

                var result = PageObject.GetResultAtSite(NameToSearch);
                result.SiteEnum = siteEnum;
                result.HasErrors = false;
                return result;
            //}
            //catch (Exception Ex)
            //{

            //    return new ResultAtSite { HasErrors = true, ErrorDescription = "Error while reading the site" + Ex.Message };
            //}

        }

        /*
    public SearchResult SearchByName(string NameToSearch, List<SiteEnum> siteEnums)
    {
        Stopwatch stopwatch = new Stopwatch();

        List<ResultAtSite> Results = new List<ResultAtSite>();

        ResultAtSite resultAtSite = new ResultAtSite();

        SearchResult searchResult = new SearchResult();

        searchResult.NameToSearch = NameToSearch;
        searchResult.SearchedBy = "Pradeep";
        searchResult.SearchedOn = DateTime.Now.ToString();

        foreach (SiteEnum site in siteEnums)
        {
            stopwatch.Start();
            resultAtSite = SearchByName(NameToSearch, site);
            stopwatch.Stop();
            resultAtSite.TimeTakenInMs = stopwatch.ElapsedMilliseconds.ToString();
            Results.Add(resultAtSite);
            stopwatch.Reset();
        }

        searchResult.resultAtSites = Results;
        return searchResult;
    }
    */

        /*
    public SearchResult SearchByName(string NameToSearch)
    {
        List<SiteEnum> siteEnums = new List<SiteEnum>();

        siteEnums.Add(SiteEnum.FDADebarPage);

        SearchResult results = new SearchResult();

        NameToSearch.Trim();
        results = SearchByName(NameToSearch, siteEnums);

        return results;
    }
    */

        private ISearchPage GetSearchPage(SiteEnum siteEnum)
        {
            switch (siteEnum)
            {
                case SiteEnum.FDADebarPage:
                    return new FDADebarPage(Driver);
                case SiteEnum.AdequateAssuranceListPage:
                    return new AdequateAssuranceListPage(Driver);
                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return new ClinicalInvestigatorDisqualificationPage(Driver);
                case SiteEnum.ERRProposalToDebarPage:
                    return new ERRProposalToDebarPage(Driver);
                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return new ClinicalInvestigatorInspectionPage(Driver);
                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return new CBERClinicalInvestigatorInspectionPage(Driver);
                case SiteEnum.ExclusionDatabaseSearchPage:
                    return new ExclusionDatabaseSearchPage(Driver);
                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return new SpeciallyDesignatedNationalsListPage(_DownloadFolder);
                case SiteEnum.FDAWarningLettersPage:
                    return new FDAWarningLettersPage(Driver);
                case SiteEnum.PHSAdministrativeActionListingPage:
                    return new PHSAdministrativeActionListingPage(Driver);
                case SiteEnum.CorporateIntegrityAgreementsListPage:
                    return new CorporateIntegrityAgreementsListPage(Driver);
                case SiteEnum.SystemForAwardManagementPage:
                    return new SystemForAwardManagementPage(Driver);
                        
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
                    //_Driver = new EdgeDriver();
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
                SearchSites = new List<SearhQuerySite>
                {
                    new SearhQuerySite {Selected = true, SiteName="FDA Debarment List", SiteShortName="FDA Debarment List", SiteEnum = SiteEnum.FDADebarPage, SiteUrl="XXX" },


                    new SearhQuerySite {Selected = true, SiteName="Clinical Investigator Inspection List (CLIL)(CDER", SiteShortName="Clinical Investigator Insp...", SiteEnum = SiteEnum.ClinicalInvestigatorInspectionPage, SiteUrl="XXX" },
                    
                    new SearhQuerySite {Selected = true, SiteName="FDA Warning Letters and Responses", SiteShortName="FDA Warning Letters ...", SiteEnum = SiteEnum.FDAWarningLettersPage, SiteUrl="XXX" },
                    
                    new SearhQuerySite {Selected = true, SiteName="Notice of Opportunity for Hearing (NOOH) – Proposal to Debar", SiteShortName="NOOH – Proposal to Debar", SiteEnum = SiteEnum.ERRProposalToDebarPage, SiteUrl="XXX" },
                    
                    new SearhQuerySite {Selected = true, SiteName="Adequate Assurances List for Clinical Investigators", SiteShortName="Adequate Assurances List ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl="XXX" },

                    new SearhQuerySite {Selected = true, SiteName="Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName="Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl="XXX" },
                    
                    new SearhQuerySite {Selected = true, SiteName="Clinical Investigator Inspection List (CBER)", SiteShortName="Inspection List", SiteEnum = SiteEnum.CBERClinicalInvestigatorInspectionPage, SiteUrl="XXX" },

                    new SearhQuerySite {Selected = true, SiteName="PHS Administrative Actions Listing ", SiteShortName="PHS Administrative Actions", SiteEnum = SiteEnum.PHSAdministrativeActionListingPage, SiteUrl="XXX" },


                    new SearhQuerySite {Selected = true, SiteName="HHS/OIG/ EXCLUSIONS DATABASE SEARCH/ FRAUD", SiteShortName="HHS/OIG/ EXCLUSIONS ...", SiteEnum = SiteEnum.ExclusionDatabaseSearchPage, SiteUrl="XXX" },
                    

                    new SearhQuerySite {Selected = true, SiteName="HHS/OIG Corporate Integrity Agreements/Watch List", SiteShortName="HHS/OIG Corporate Integrity", SiteEnum = SiteEnum.CorporateIntegrityAgreementsListPage, SiteUrl="XXX" },

                    
                    //new SearhQuerySite {Selected = true, SiteName="SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName="SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.SystemForAwardManagementPage, SiteUrl="XXX" },

                    //new SearhQuerySite {Selected = true, SiteName="LIST OF SPECIALLY DESIGNATED NATIONALS", SiteShortName="SPECIALLY DESIGNATED ...", SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage, SiteUrl="XXX" },
                    
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
