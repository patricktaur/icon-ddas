using DDAS.Models.Entities.Domain;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IReport
    {
        List<ReportViewModel> 
            GetInvestigationsCompletedReport(ReportFilters Filters);

        List<ReportViewModel> GetInvestigationsCompletedReport(
            DateTime FromDate, DateTime ToDate);
    }
}
