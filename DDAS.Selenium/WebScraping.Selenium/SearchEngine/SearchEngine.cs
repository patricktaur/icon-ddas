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

namespace WebScraping.Selenium.SearchEngine
{
    public class SearchEngine : ISearchEngine, IDisposable
    {
        private IWebDriver Driver;

        public SearchEngine()
        {
            Driver = new EdgeDriver();
        }

        public ResultAtSite SearchName(string NameToSearch, SiteEnum siteEnum)
        {
            var PageObject = GetSearchPage(siteEnum);

            PageObject.LoadContent(NameToSearch);

            var result = PageObject.Search(NameToSearch);

            return result;
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
                resultAtSite = SearchName(NameToSearch, site);
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

            //siteEnums.Add(SiteEnum.FDADebarPage);
            //siteEnums.Add(SiteEnum.ERRProposalToDebarPage);
            //siteEnums.Add(SiteEnum.AdequateAssuranceListPage);
            //siteEnums.Add(SiteEnum.ClinicalInvestigatorInspectionPage);
            //siteEnums.Add(SiteEnum.CBERClinicalInvestigatorInspectionPage);
            //siteEnums.Add(SiteEnum.ClinicalInvestigatorDisqualificationPage);
            //siteEnums.Add(SiteEnum.ExclusionDatabaseSearchPage);
            siteEnums.Add(SiteEnum.SpeciallyDesignedNationalsListPage);

            SearchResult results = new SearchResult();

            NameToSearch.Trim();
            results = SearchByName(NameToSearch, siteEnums);

            return results;
        }
        
        public BaseSearchPage GetSearchPage(SiteEnum siteEnum)
        {
            switch (siteEnum)
            {
                case SiteEnum.FDADebarPage: return new FDADebarPage(Driver);
                case SiteEnum.ERRProposalToDebarPage:
                    return new ERRProposalToDebarPage(Driver);
                case SiteEnum.AdequateAssuranceListPage:
                    return new AdequateAssuranceListPage(Driver);
                case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                    return new ClinicalInvestigatorDisqualificationPage(Driver);
                case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                    return new CBERClinicalInvestigatorInspectionPage(Driver);
                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return new SpeciallyDesignatedNationalsListPage(Driver);
                case SiteEnum.ExclusionDatabaseSearchPage:
                    return new ExclusionDatabaseSearchPage(Driver);
                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return new ClinicalInvestigatorInspectionPage(Driver);

                default: return null;
            }
        }

        public void Dispose()
        {
            Driver.Quit();
        }
    }
}
