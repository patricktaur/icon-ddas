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

        public SearchService(IUnitOfWork uow)
        {
            _UOW = uow;
        }

        public SearchSummary GetSearchSummary(NameToSearchQuery query)
        {
            SearchSummary searchSummary = new SearchSummary();
            var searchSummaryItems = new List<SearchSummaryItem>();

            string NameToSearch = query.NameToSearch.Replace(",", "");

            SiteScanData ScanData = new SiteScanData(_UOW);
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
                case SiteEnum.SpeciallyDesignedNationalsListPage:
                    return GetSpeciallyDesignatedNationalsMatchCount(NameToSearch, 
                        DataId);
                default : throw new Exception("Invalid Enum");
            }
        }

        #region FDADebarSite
        public string GetFDADebarPageMatchCount(string NameToSearch, Guid? DataId)
        {
            var FDASearchResult = GetFDADebarPageMatch(NameToSearch, DataId);

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

            ////Patrick-4       
            UpdateMatchStatus(FDASearchResult.DebarredPersons, NameToSearch);

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

            UpdateMatchStatus(PHSSiteData.PHSAdministrativeSiteData, NameToSearch);

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
            var ClinicalSiteData =
                _UOW.ClinicalInvestigatorInspectionListRepository.FindById(DataId);

            string[] Name = NameToSearch.Split(' ');

            UpdateMatchStatus(ClinicalSiteData.ClinicalInvestigatorInspectionList,
                NameToSearch);

            var CIISiteData = ClinicalSiteData.ClinicalInvestigatorInspectionList.
                Where(CIIData => CIIData.Matched > 0).ToList();

            ClinicalSiteData.ClinicalInvestigatorInspectionList = CIISiteData;

            return ClinicalSiteData;
        }

        #endregion

        #region SpeciallyDesignatedNations

        public string GetSpeciallyDesignatedNationalsMatchCount(string NameToSearch,
            Guid? DataId)
        {
            var SDNSiteData = GetSpeciallyDesignatedNationsMatch(NameToSearch, DataId);

            string MatchStatus = null;

            string[] Name = NameToSearch.Split(' ');

            for (int counter = 1; counter <= Name.Length; counter++)
            {
                int MatchesFound =
                    SDNSiteData.SDNListSiteData.
                    Where(x => x.Matched == counter).Count();

                if (MatchesFound != 0 && MatchStatus != null)
                    MatchStatus = MatchStatus + ", " + MatchesFound + ":" + counter;
                else if (MatchesFound != 0)
                    MatchStatus = MatchesFound + ":" + counter;
            }
            return MatchStatus;
        }

        public SpeciallyDesignatedNationalsListSiteData GetSpeciallyDesignatedNationsMatch(
            string NameToSearch, Guid? DataId)
        {
            var SDNSiteData = _UOW.SpeciallyDesignatedNationalsRepository.FindById(DataId);

            string[] Name = NameToSearch.Split(' ');

            UpdateMatchStatus(SDNSiteData.SDNListSiteData,
                NameToSearch);

            var CIISiteData = SDNSiteData.SDNListSiteData.
                Where(CIIData => CIIData.Matched > 0).ToList();

            SDNSiteData.SDNListSiteData = CIISiteData;

            return SDNSiteData;
        }
        #endregion

        void UpdateMatchStatus(IEnumerable<SiteDataItemBase> items, string NameToSearch)
        {
            string[] namesInNameToSearch = NameToSearch.Split(' ');
            foreach (SiteDataItemBase item in items)
            {
                int Count = 0;
                foreach (string name in namesInNameToSearch)
                {
                    if (item.FullName.ToLower().Contains(name.ToLower()))
                    {
                        Count += 1;
                    }
                }
                item.Matched = Count;
            }
        }
        
        #region Save and Update Approved/Rejected records
        public bool SaveRecordStatus(SaveSearchResult Result)
        {
            Result.CreatedOn = DateTime.Now;
            _UOW.SaveSearchResultRepository.Add(Result);
            return true;
        }
        
        public FDADebarPageSiteData GetStatusOfFDASiteRecords(
            FDADebarPageSiteData FDASiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.FDADebarPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null) 
                return FDASiteData;

            foreach (DebarredPerson debarredPerson in FDASiteData.DebarredPersons)
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

        public ClinicalInvestigatorInspectionSiteData GetStatusOfClinicalSiteRecords(
            ClinicalInvestigatorInspectionSiteData ClinicalSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.ClinicalInvestigatorInspectionPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return ClinicalSiteData;

            foreach (ClinicalInvestigator Investigator in 
                ClinicalSiteData.ClinicalInvestigatorInspectionList)
            {
                foreach(SaveSearchDetails SearchDetails in 
                    SavedSearchResult.saveSearchDetails)
                {
                    if (Investigator.RowNumber == SearchDetails.RowNumber)
                        Investigator.Status = SearchDetails.Status;
                }
            }
            return ClinicalSiteData;
        }

        public PHSAdministrativeActionListingSiteData GetStatusOfPHSSiteRecords(
            PHSAdministrativeActionListingSiteData PHSSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.PHSAdministrativeActionListingPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return PHSSiteData;

            foreach (PHSAdministrativeAction PHSData in 
                PHSSiteData.PHSAdministrativeSiteData)
            {
                foreach(SaveSearchDetails SearchDetails in 
                    SavedSearchResult.saveSearchDetails)
                {
                    if (PHSData.RowNumber == SearchDetails.RowNumber)
                        PHSData.Status = SearchDetails.Status;
                }
            }
            return PHSSiteData;
        }

        public SpeciallyDesignatedNationalsListSiteData GetStatusOfSDNSiteRecords(
            SpeciallyDesignatedNationalsListSiteData SDNSiteData, string NameToSearch)
        {
            var SavedSearchResult = _UOW.SaveSearchResultRepository.GetAll().
                Where(x => x.siteEnum == SiteEnum.SpeciallyDesignedNationalsListPage
                && x.NameToSearch.ToLower() == NameToSearch.ToLower()).
                OrderByDescending(y => y.CreatedOn).
                FirstOrDefault();

            if (SavedSearchResult == null)
                return SDNSiteData;

            foreach(SDNList SDNListData in SDNSiteData.SDNListSiteData)
            {
                foreach (SaveSearchDetails SearchDetails in
                    SavedSearchResult.saveSearchDetails)
                {
                    if (SDNListData.RowNumber == SearchDetails.RowNumber)
                        SDNListData.Status = SearchDetails.Status;
                }
            }
            return SDNSiteData;
        }
        #endregion
    }
}
