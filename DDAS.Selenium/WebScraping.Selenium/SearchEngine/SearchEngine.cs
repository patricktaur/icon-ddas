using System;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using WebScraping.Selenium.Pages;
using WebScraping.Selenium.BaseClasses;
using System.Collections.Generic;
using System.Diagnostics;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Chrome;

namespace WebScraping.Selenium.SearchEngine
{
    public class SearchEngine : ISearchEngine, IDisposable
    {
        private IWebDriver _Driver;
        private string _DownloadFolder;
        public SearchEngine( string downloadFolder)
        {
            _Driver = new ChromeDriver(@"C:\Development\p926-ddas\Libraries\ChromeDriver");
            //_Driver =  new FirefoxDriver();
            _DownloadFolder = downloadFolder;
        }

        public ResultAtSite SearchByName(string NameToSearch, SiteEnum siteEnum)
        {
           
            var PageObject = GetSearchPage(siteEnum);
            
            PageObject.LoadContent(NameToSearch);

            var result = PageObject.GetResultAtSite(NameToSearch);
            result.SiteEnum = siteEnum;            
            return result;
        }
        
        //refactor: remove other related functins.
        public SearchResult SearchByName(SearchQuery searchQuery)
        {
            Stopwatch stopwatch = new Stopwatch();
           
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
                    resultAtSite = SearchByName(searchQuery.NameToSearch, site.SiteEnum);
                    stopwatch.Stop();
                    resultAtSite.TimeTakenInMs = stopwatch.ElapsedMilliseconds.ToString();

                    searchResult.resultAtSites.Add(resultAtSite);
                    stopwatch.Reset();
               // }

            }
            return searchResult;
        }
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

        public SearchResult SearchByName(string NameToSearch)
        {
            List<SiteEnum> siteEnums = new List<SiteEnum>();

            siteEnums.Add(SiteEnum.FDADebarPage);
            //siteEnums.Add(SiteEnum.ERRProposalToDebarPage);
            //siteEnums.Add(SiteEnum.AdequateAssuranceListPage);
            //siteEnums.Add(SiteEnum.ClinicalInvestigatorInspectionPage);
            //siteEnums.Add(SiteEnum.CBERClinicalInvestigatorInspectionPage);
            //siteEnums.Add(SiteEnum.ClinicalInvestigatorDisqualificationPage);
            //siteEnums.Add(SiteEnum.ExclusionDatabaseSearchPage);
            //siteEnums.Add(SiteEnum.SpeciallyDesignedNationalsListPage);

            SearchResult results = new SearchResult();

            NameToSearch.Trim();
            results = SearchByName(NameToSearch, siteEnums);

            return results;
        }
        
        //public BaseSearchPage GetSearchPage(SiteEnum siteEnum)
            public ISearchPage GetSearchPage(SiteEnum siteEnum)
        {
            switch (siteEnum)
            {
                case SiteEnum.FDADebarPage:
                    return new FDADebarPage(_Driver);
                case SiteEnum.AdequateAssuranceListPage:
                    return new AdequateAssuranceListPage(_Driver);
                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return new ClinicalInvestigatorDisqualificationPage(_Driver);
                case SiteEnum.ERRProposalToDebarPage:
                    return new ERRProposalToDebarPage(_Driver);
                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return new ClinicalInvestigatorInspectionPage(_Driver);
                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return new CBERClinicalInvestigatorInspectionPage(_Driver);
                case SiteEnum.ExclusionDatabaseSearchPage:
                    return new ExclusionDatabaseSearchPage(_Driver);
                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return new SpeciallyDesignatedNationalsListPage(_DownloadFolder);
                default: return null;
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
                    
                    //not ready:
                    //new SearhQuerySite {Selected = true, SiteName="FDA Warning Letters and Responses", SiteShortName="FDA Warning Letters ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl="XXX" },

                    
                    new SearhQuerySite {Selected = false, SiteName="Notice of Opportunity for Hearing (NOOH) – Proposal to Debar", SiteShortName="NOOH – Proposal to Debar", SiteEnum = SiteEnum.ERRProposalToDebarPage, SiteUrl="XXX" },
                    
                    new SearhQuerySite {Selected = true, SiteName="Adequate Assurances List for Clinical Investigators", SiteShortName="Adequate Assurances List ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl="XXX" },
                    new SearhQuerySite {Selected = true, SiteName="Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName="Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl="XXX" },
                    
                    new SearhQuerySite {Selected = true, SiteName="Clinical Investigator Inspection List (CBER)", SiteShortName="Inspection List", SiteEnum = SiteEnum.CBERClinicalInvestigatorInspectionPage, SiteUrl="XXX" },

                    //Page not found!
                    //new SearhQuerySite {Selected = true, SiteName="PHS Administrative Actions Listing ", SiteShortName="PHS Administrative Actions", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl="XXX" },

                    new SearhQuerySite {Selected = true, SiteName="HHS/OIG/ EXCLUSIONS DATABASE SEARCH/ FRAUD", SiteShortName="HHS/OIG/ EXCLUSIONS ...", SiteEnum = SiteEnum.ExclusionDatabaseSearchPage, SiteUrl="XXX" },
                    
                    //not ready:
                    //new SearhQuerySite {Selected = true, SiteName="HHS/OIG Corporate Integrity Agreements/Watch List", SiteShortName="HHS/OIG Corporate Integrity", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl="XXX" },
                    
                    //new SearhQuerySite {Selected = true, SiteName="SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName="SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.XXX, SiteUrl="XXX" },

                    //new SearhQuerySite {Selected = true, SiteName="LIST OF SPECIALLY DESIGNATED NATIONALS", SiteShortName="SPECIALLY DESIGNATED ...", SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage, SiteUrl="XXX" },
                    
         }

            };
           
        }

        public void Dispose()
        {
            _Driver.Quit();
        }
    }
}
