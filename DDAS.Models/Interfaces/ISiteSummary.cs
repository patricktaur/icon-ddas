using DDAS.Models.Entities.Domain;
using System;

namespace DDAS.Models.Interfaces
{
    public interface ISiteSummary
    {
        SearchSummary GetSearchSummaryStatus(
            string NameToSearch, Guid? ComplianceFormId);
    }
}
