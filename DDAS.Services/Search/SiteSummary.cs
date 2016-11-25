using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDAS.Services.Search
{
    public class SiteSummary : ISiteSummary
    {
        private IUnitOfWork _UOW;
        public SiteSummary(IUnitOfWork uow)
        {
            _UOW = uow;
        }

        public SearchSummary GetSearchSummaryStatus(
            string NameToSearch, Guid? ComplianceFormId)
        {
            var SiteSearchSummary = 
                GetSiteMatchStatus(NameToSearch, ComplianceFormId);

            return SiteSearchSummary;
        }

        public SearchSummary GetSiteMatchStatus(string NameToSearch, Guid? ComplianceFormId)
        {
            var ComplianceForm = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (ComplianceForm == null)
                return null;
            
            var Investigator = ComplianceForm.InvestigatorDetails.Where(x =>
            x.Name.ToLower() == NameToSearch.ToLower()).FirstOrDefault();

            SearchSummary searchSummary = new SearchSummary();
            var searchSummaryItems = new List<SearchSummaryItem>();

            searchSummary.ComplianceFormId = ComplianceFormId;

            searchSummary.Sites_FullMatchCount =
                Investigator.Sites_FullMatchCount;
            searchSummary.Sites_PartialMatchCount =
                Investigator.Sites_PartialMatchCount;
            searchSummary.TotalIssuesFound =
                Investigator.TotalIssuesFound;

            foreach (SitesIncludedInSearch Site in Investigator.SiteDetails)
            {
                var SummaryItem = new SearchSummaryItem();
                
                SummaryItem.FullMatch = Site.FullMatchCount;
                SummaryItem.PartialMatch = Site.PartialMatchCount;
                SummaryItem.DataExtractedOn = Site.DataExtractedOn;
                SummaryItem.SiteLastUpdatedOn = Site.UpdatedOn;
                SummaryItem.SiteEnum = Site.SiteEnum;
                SummaryItem.SiteUrl = Site.SiteUrl;
                SummaryItem.SiteName = Site.SiteName;
                SummaryItem.MatchStatus = Site.MatchStatus;
                SummaryItem.IssuesFound = Site.IssuesFound;
                SummaryItem.IssuesFoundStatus = Site.IssuesFoundStatus;

                searchSummaryItems.Add(SummaryItem);
                searchSummaryItems = 
                    searchSummaryItems.OrderBy(Item => Item.SiteEnum).ToList();
            }
            searchSummary.SearchSummaryItems = searchSummaryItems;
            return searchSummary;
        }
    }
}
