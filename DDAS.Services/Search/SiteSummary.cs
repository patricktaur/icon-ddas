﻿using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Services.Search
{
    public class SiteSummary : ISiteSummary
    {
        private IUnitOfWork _UOW;
        public SiteSummary(IUnitOfWork uow)
        {
            _UOW = uow;
        }

        public SearchSummary GetSearchSummaryStatus(Guid? ComplianceFormId)
        {
            var SiteSearchSummary = GetSiteMatchStatus(ComplianceFormId);
            return SiteSearchSummary;
        }

        public SearchSummary GetSiteMatchStatus(Guid? ComplianceFormId)
        {
            var ComplianceForm = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (ComplianceForm == null)
                return null;

            SearchSummary searchSummary = new SearchSummary();
            var searchSummaryItems = new List<SearchSummaryItem>();

            searchSummary.ComplianceFormId = ComplianceFormId;

            searchSummary.Sites_FullMatchCount = 
                ComplianceForm.Sites_FullMatchCount;
            searchSummary.Sites_PartialMatchCount = 
                ComplianceForm.Sites_PartialMatchCount;
            searchSummary.TotalIssuesFound = 
                ComplianceForm.TotalIssuesFound;

            foreach (SitesIncludedInSearch Site in ComplianceForm.SiteDetails)
            {
                var SummaryItem = new SearchSummaryItem();
                
                SummaryItem.FullMatch = Site.FullMatchCount;
                SummaryItem.PartialMatch = Site.PartialMatchCount;
                SummaryItem.DataExtractedOn = Site.DataExtractedOn;
                SummaryItem.SiteLastUpdatedOn = Site.SiteLastUpdatedOn;
                SummaryItem.SiteEnum = Site.SiteEnum;
                SummaryItem.SiteUrl = Site.SiteUrl;
                SummaryItem.SiteName = Site.SiteName;
                SummaryItem.MatchStatus = Site.MatchStatus;
                SummaryItem.IssuesFound = Site.IssuesFound;
                SummaryItem.IssuesFoundStatus = Site.IssuesFoundStatus;

                searchSummaryItems.Add(SummaryItem);
                searchSummaryItems = searchSummaryItems.OrderBy(Item => Item.SiteEnum).ToList();
            }
            searchSummary.SearchSummaryItems = searchSummaryItems;
            return searchSummary;
        }
    }
}
