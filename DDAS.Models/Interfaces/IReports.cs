using DDAS.Models.Entities.Domain;
using DDAS.Models.ViewModels;
using System.Collections.Generic;

namespace DDAS.Models.Interfaces
{
    public interface IReport
    {
        InvestigationsReport GetInvestigationsReport(ReportFilters Filters);

        List<OpenInvestigationsViewModel> GetOpenInvestigations();
    }
}
