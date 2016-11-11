using DDAS.Models;
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
            SearchSummary searchSummary = new SearchSummary();
            var searchSummaryItems = new List<SearchSummaryItem>();

            searchSummary.ComplianceFormId = ComplianceFormId;

            searchSummary.SearchSummaryItems = GetSiteMatchStatus(ComplianceFormId);

            return searchSummary;
        }

        public List<SearchSummaryItem> GetSiteMatchStatus(Guid? ComplianceFormId)
        {
            var ComplianceForm = _UOW.ComplianceFormRepository.FindById(ComplianceFormId);

            if (ComplianceForm == null)
                return null;

            var SiteSummaryItems = new List<SearchSummaryItem>();

            foreach(SitesIncludedInSearch Site in ComplianceForm.SiteDetails)
            {
                var SummaryItem = new SearchSummaryItem();

                SummaryItem.FullMatch = Site.FullMatchCount;
                SummaryItem.PartialMatch = Site.PartialMatchCount;
                SummaryItem.SiteEnum = Site.SiteEnum;
                SummaryItem.SiteUrl = Site.SiteUrl;
                SummaryItem.SiteName = Site.SiteName;

                SiteSummaryItems.Add(SummaryItem);
            }

            return SiteSummaryItems;
        }
    }
}
