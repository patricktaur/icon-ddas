using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DDAS.Models.Entities.Domain.SiteData;

namespace DDAS.Services.Search
{
    public class SearchService : ISearchSummary
    {
        private IUnitOfWork _UOW;
        private ISearchEngine _SearchEngine;

        public SearchService(IUnitOfWork uow, ISearchEngine SearchEngine)
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

        public string GetMatchStatus(SiteEnum Enum, string NameToSearch, Guid? DataId)
        {
            switch(Enum)
            {
                case SiteEnum.FDADebarPage:
                    return GetFDADebarPageMatchCount(NameToSearch, DataId);
                case SiteEnum.PHSAdministrativeActionListingPage:
                    return GetPHSAdministrativeMatchCount(NameToSearch, DataId);
                case SiteEnum.ClinicalInvestigatorInspectionPage:
                    return GetClinicalInvestigatorMatchCount(NameToSearch, DataId);
            }
            return null;
        }

        #region FDADebarSite
        public string GetFDADebarPageMatchCount(string NameToSearch, Guid? DataId)
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
                        Count += 1;
                }
                if (Count != 0)
                    debarredPerson.Matched = Count;
            }

            var DebarList = FDASearchResult.DebarredPersons.Where(
               debarredList => debarredList.Matched > 0).ToList();

            FDASearchResult.DebarredPersons = DebarList;

            return FDASearchResult;
        }
        #endregion

        #region PHSSite
        public string GetPHSAdministrativeMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var PHSData = GetPHSAdministrativeSiteMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = PHSData.PHSAdministrativeSiteData.Where(
                    x => x.Matched == counter).Count();
                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }

            return MatchStatus;
        }

        public PHSAdministrativeActionListingSiteData GetPHSAdministrativeSiteMatch(
            string NameToSearch, 
            Guid? DataId)
        {
            PHSAdministrativeActionListingSiteData PHSSiteData =
                _UOW.PHSAdministrativeActionListingRepository.FindById(DataId);

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

            var PHSData = PHSSiteData.PHSAdministrativeSiteData.Where(
                PHS => PHS.Matched > 0).ToList();

            PHSSiteData.PHSAdministrativeSiteData = PHSData;

            return PHSSiteData;
        }
        #endregion

        #region ClinicalInvestigatorInspectionSite
        
        public string GetClinicalInvestigatorMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var ClinicalSiteData = GetClinicalInvestigatorSiteMatch(NameToSearch,
                DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound = 
                    ClinicalSiteData.ClinicalInvestigatorInspectionList
                    .Where(x => x.Matched == counter).Count();

                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public ClinicalInvestigatorInspectionSiteData 
            GetClinicalInvestigatorSiteMatch(string NameToSearch, Guid? DataId)
        {
            ClinicalInvestigatorInspectionSiteData ClinicalSiteData =
                _UOW.ClinicalInvestigatorInspectionListRepository.FindById(DataId);

            string[] Name = NameToSearch.Split(' ');

            foreach (ClinicalInvestigator clinicalInvestigator in
                ClinicalSiteData.ClinicalInvestigatorInspectionList)
            {
                int Count = 0;

                foreach(string SearchName in Name)
                {
                    if (clinicalInvestigator.Name.ToLower().
                        Contains(SearchName.ToLower()))
                        Count += 1;
                }
                if (Count != 0)
                    clinicalInvestigator.Matched = Count;
            }

            var CIISiteData = ClinicalSiteData.ClinicalInvestigatorInspectionList.
                Where(CIIData => CIIData.Matched > 0).ToList();

            ClinicalSiteData.ClinicalInvestigatorInspectionList = CIISiteData;

            return ClinicalSiteData;
        }

        #endregion

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

        #region Save and Update Approved/Rejected records
        public bool SaveRecordStatus(SaveSearchResult Result)
        {
            Result.CreatedOn = DateTime.Now;
            _UOW.SaveSearchResultRepository.Add(Result);
            return true;
        }
        
        public FDADebarPageSiteData GetStatusOfFDASiteDataRecords(
            FDADebarPageSiteData FDASiteData)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                OrderByDescending(searchResult => searchResult.CreatedOn).
                FirstOrDefault();

            foreach(DebarredPerson debarredPerson in FDASiteData.DebarredPersons)
            {
                foreach(SaveSearchDetails SearchDetails in 
                    SavedSearchResult.saveSearchDetails)
                {
                    if (debarredPerson.RowNumber == SearchDetails.RowNumber)
                        debarredPerson.Status = SearchDetails.Status;
                }
            }
            return FDASiteData;
        }
        #endregion
    }
}
