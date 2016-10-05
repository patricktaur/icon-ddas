using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface ISearchSummary
    {
        SearchSummary GetSearchSummary(NameToSearchQuery query);
        FDADebarPageSiteData GetFDADebarPageMatch(string NameToSearch, Guid? DataId);

    }
}
