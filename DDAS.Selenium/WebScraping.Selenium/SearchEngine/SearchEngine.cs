using System;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DDAS.Models.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using WebScraping.Selenium.Pages;
using WebScraping.Selenium.BaseClasses;
using System.Collections.Generic;

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

            PageObject.LoadContent();

            var result = PageObject.Search(NameToSearch);

            return result;
        }

        public SearchResult SearchByName(string NameToSearch, List<SiteEnum> siteEnums)
        {
            List<ResultAtSite> Results = new List<ResultAtSite>();

            SearchResult searchResult = new SearchResult();

            searchResult.NameToSearch = NameToSearch;
            searchResult.SearchedBy = "Pradeep";
            searchResult.SearchedOn = DateTime.Now.ToString();
            
            foreach (SiteEnum site in siteEnums)
            {
                Results.Add(SearchName(NameToSearch, site));
            }

            searchResult.resultAtSites = Results;
            return searchResult;
        }

        public SearchResult SearchByName(string NameToSearch)
        {
            List<SiteEnum> siteEnums = new List<SiteEnum>();

            siteEnums.Add(SiteEnum.FDADebarPage);
            siteEnums.Add(SiteEnum.ERRProposalToDebarPage);
            siteEnums.Add(SiteEnum.AdequateAssuranceListPage);

            SearchResult results = new SearchResult();

            results = SearchByName(NameToSearch, siteEnums);

            return SearchByName(NameToSearch, siteEnums);
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
                //case SiteEnum.ClinicalInvestigatorDisqualificationPage:
                //    return new ClinicalInvestigatorDisqualificationPage(Driver);
                //case SiteEnum.CBERClinicalInvestigatorInspectionPage:
                //    return new CBERClinicalInvestigatorInspectionPage(Driver);
                //case SiteEnum.SpeciallyDesignedNationalsListPage:
                    //return new SpeciallyDesignatedNationalsListPage(Driver);

                default: return null;
            }
        }

        public void Dispose()
        {
            Driver.Quit();
        }
    }
}
