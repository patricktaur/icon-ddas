using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Data.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain.SiteData;

namespace DDAS.Services.Search
{
    public class Search : ISearchSummary
    {
        private IUnitOfWork _UOW;
        private ISearchEngine _SearchEngine;

        public Search(IUnitOfWork uow, ISearchEngine SearchEngine)
        {
            _UOW = uow;
            _SearchEngine = SearchEngine;
        }

        public SearchSummary GetSearchSummary(NameToSearchQuery query)
        {
            SearchSummary searchSummary = new SearchSummary();

            var searchSummaryItems = new List<SearchSummaryItem>();

            string NameToSearch = query.NameToSearch.Replace(",", "");

            SiteScanData ScanData = new SiteScanData(_SearchEngine, _UOW);

            List<SiteScan> SiteScanList = ScanData.GetSiteScanSummary();

            searchSummary.NameToSearch = NameToSearch;

            foreach (SiteScan Site in SiteScanList)
            {
                var SummaryItem = new SearchSummaryItem();

                SummaryItem.MatchStatus = GetMatchStatus(
                    Site.SiteEnum, 
                    NameToSearch, 
                    Site.DataId);
                //???
                SummaryItem.SiteEnum = Site.SiteEnum;
                SummaryItem.SiteName = Site.SiteName;
                SummaryItem.SiteUrl = Site.SiteUrl;
                SummaryItem.RecId = Site.DataId;

                searchSummaryItems.Add(SummaryItem);
            }

            searchSummary.SearchSummaryItems = searchSummaryItems;

            return searchSummary;
        }

        public string GetMatchStatus(SiteEnum Enum, string NameToSearch, Guid DataId)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchCount(NameToSearch, DataId);
                //case SiteEnum.PHSAdministrativeActionListingPage:
                //    return GetPHSAdministrativeMatchCount(NameToSearch, DataId);
            }
            return null;
        }

        public string GetFDADebarPageMatchCount(string NameToSearch, Guid DataId)
        {
            var FDASearchResult = GetFDADebarPageMatch(NameToSearch, DataId);

            //List<ISiteDataItemBase> items = FDASearchResult.DebarredPersons;

            //var x = GetMatchStatus(FDASearchResult.DebarredPersons);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = FDASearchResult.DebarredPersons.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }

            return MatchStatus;
        }

        public string GetMatchStatus(List<SiteDataItemBase> items)
        {
            return null;
        }

        public FDADebarPageSiteData GetFDADebarPageMatch(string NameToSearch, 
            Guid? DataId)
        {
            string[] Name = NameToSearch.Split(' ');

            FDADebarPageSiteData FDASearchResult = 
                _UOW.FDADebarPageRepository.FindById(DataId);

            foreach (DebarredPerson debarredPerson in 
                FDASearchResult.DebarredPersons)
            {
                int Count = 0;
                foreach (string SearchName in Name)
                {
                    if (debarredPerson.NameOfPerson.ToLower().
                        Contains(SearchName.ToLower()))
                    {
                        Count += 1;
                    }
                }
                if (Count != 0)
                    debarredPerson.Matched = Count;
            }

            var DebarList = FDASearchResult.DebarredPersons.Where(
               debarredList => debarredList.Matched > 0).ToList();

            FDASearchResult.DebarredPersons = DebarList;

            return FDASearchResult;
        }

        /*
        public string GetPHSAdministrativeMatchCount(string NameToSearch, Guid? DataId)
        {
            PHSAdministrativeActionListingSiteData PHSSiteData =
                _UOW.PHSAdministrativeActionListingRepository.FindById(DataId);

            return GetPHSAdministrativeSiteMatch(NameToSearch, PHSSiteData);
        }

        public string GetPHSAdministrativeSiteMatch(string NameToSearch, 
            PHSAdministrativeActionListingSiteData PHSSiteData)
        {
            string[] Name = NameToSearch.Split(' ');

            foreach(PHSAdministrativeAction PHSAction in 
            PHSSiteData.PHSAdministrativeSiteData)
            {
                int Count = 0;
                string FullName = PHSAction.FirstName + " " +
                                    PHSAction.MiddleName + " " + 
                                    PHSAction.LastName;

                foreach (string SearchName in Name)
                {
                    if(FullName.ToLower().Contains(SearchName.ToLower()))
                    {
                        Count += 1;
                    }
                    if (Count != 0)
                        PHSAction.Matched = Count;
                }
            }

            string MatchStatus = null;

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = PHSSiteData.PHSAdministrativeSiteData.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }

            return MatchStatus;
        }
        */

        /*
        public void TestCall()
        {
            List<Test> x = new List<Services.Search.Test>();
            TestXYz(x);

        }

        public void TestXYz(List<SiteDataItemBase> items)
        {
            Console.Write(items.Count);
        }
        */
    }
}
