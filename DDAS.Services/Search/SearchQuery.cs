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

        public SearchSummary GetSearchSummary(SearchQuery query)
        {
            SearchSummary searchSummary = new SearchSummary();

            var searchSummaryItem = new List<SearchSummaryItem>();

            string NameToSearch = query.NameToSearch.Replace(".,", "");

            SiteScanData ScanData = new SiteScanData(_SearchEngine, _UOW);

            List<SiteScan> SiteScanList = ScanData.GetSiteScanSummary();

            searchSummary.NameToSearch = NameToSearch;

            foreach (SiteScan Site in SiteScanList)
            {
                var SummaryItem = new SearchSummaryItem();

                SummaryItem.MatchStatus = GetMatchStatus(Site.SiteEnum, NameToSearch, Site.DataId);
                SummaryItem.SiteEnum = Site.SiteEnum;
                SummaryItem.SiteName = Site.SiteName;
                SummaryItem.SiteUrl = Site.SiteUrl;

                searchSummaryItem.Add(SummaryItem);
            }

            searchSummary.SearchSummaryItems = searchSummaryItem;

            return searchSummary;
        }

        public string GetMatchStatus(SiteEnum Enum, string NameToSearch, Guid? DataId)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchCount(NameToSearch, DataId);
            }
            return null;
        }

        public string GetFDADebarPageMatchCount(string NameToSearch, Guid? DataId)
        {
            FDADebarPageSiteData FDASearchResult = _UOW.FDADebarPageRepository.FindById(DataId);

            return GetFDADebarPageMatch(NameToSearch, FDASearchResult);

            //var DList = FDASearchResult.SelectMany(x => x.DebarredPersons).Where(
            //    y => y.NameOfPerson.Contains(NameToSearch));
        }

        public string GetFDADebarPageMatch(string NameToSearch, 
            FDADebarPageSiteData FDASearchResult)
        {
            string[] Name = NameToSearch.Split(' ');

            foreach (DebarredPerson debarredPerson in FDASearchResult.DebarredPersons)
            {
                int Count = 0;
                foreach (string SearchName in Name)
                {
                    if (debarredPerson.NameOfPerson.ToLower().Contains(SearchName.ToLower()))
                    {
                        Count += 1;
                    }
                }
                if (Count != 0)
                    debarredPerson.Matched = Count;
            }

            string MatchStatus = null;

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = FDASearchResult.DebarredPersons.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }


            //var Found = FDASearchResult[0].DebarredPersons.GroupBy(x => x.Matched).Select(
            //    g => new { g.Key = Matched, g.Value });

            //var Found = from FDASite in FDASearchResult[0].DebarredPersons
            //            group FDASite by FDASite.Matched into Matches
            //            select new
            //            {
            //                Matched = Matches.Key,
            //                Values = Matches
            //            };

            return MatchStatus;
        }

        public string GetPHSAdministrativeMatchCount(string NameToSearch)
        {
            List<PHSAdministrativeActionListingSiteData> PHSSiteData =
                _UOW.PHSAdministrativeActionListingRepository.GetAll();

            return null;
        }

        public string GetPHSAdministrativeSiteMatch(string NameToSearch, 
            List<PHSAdministrativeActionListingSiteData> PHSSiteData)
        {
            string[] Name = NameToSearch.Split(' ');

            foreach(PHSAdministrativeActionListingSiteData SiteData in PHSSiteData)
            {
                foreach(PHSAdministrativeAction PHSAction in SiteData.PHSAdministrativeSiteData)
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
            }

            return null;
        }
    }
}
