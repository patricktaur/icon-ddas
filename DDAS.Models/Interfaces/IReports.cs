using DDAS.Models.Entities.Domain;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace DDAS.Models.Interfaces
{
    public interface IReport
    {
        InvestigationsReport GetInvestigationsReport(ReportFilters Filters);

        List<OpenInvestigationsViewModel> GetOpenInvestigations();

        List<AdminDashboardViewModel> GetAdminDashboard();

        List<AssignmentHistoryViewModel> GetAssignmentHistory();

        List<InvestigatorReviewCompletedTimeVM>
            GetInvestigatorsReviewCompletedTime(DateTime FromDate, DateTime ToDate);
    }
}
