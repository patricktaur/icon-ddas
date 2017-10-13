using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Services.Reports
{
    public class Report : IReport
    {
        private IUnitOfWork _UOW;
        public Report(IUnitOfWork UOW)
        {
            _UOW = UOW;
        }

        public List<ReportViewModel> GetInvestigationsCompletedReport(
            ReportFilters Filters)
        {
            var AllCompForms = _UOW.ComplianceFormRepository.GetAll();

            if (AllCompForms.Count == 0)
                throw new Exception("No compliance forms found!");

            var Investigators = new List<InvestigatorSearched>();

            var ReportList = new List<ReportViewModel>();

            if (Filters.AssignedTo != null &&
                Filters.AssignedTo.Length > 0 &&
                Filters.AssignedTo.ToLower() != "all")
            {
                var Report = new ReportViewModel();
                Report.AssignedTo = Filters.AssignedTo;

                var Filter1 = GetInvestigationsCompletedReport(
                    AllCompForms, Filters.AssignedTo);

                var ReviewCompletedInvestigators = Filter1.SelectMany(x =>
                    x.InvestigatorDetails).Where(s =>
                    s.ReviewCompletedOn != null &&
                    s.AddedOn >= Filters.FromDate &&
                    s.AddedOn <= Filters.ToDate)
                    .ToList();

                if (ReviewCompletedInvestigators.Count > 0)
                {
                    Report.Count = ReviewCompletedInvestigators.Count;
                    ReportList.Add(Report);
                }
                return ReportList;
            }

            if (Filters.AssignedTo != null &&
                Filters.AssignedTo.Length > 0 &&
                Filters.AssignedTo.ToLower() == "all")
            {

            }
            return ReportList;
        }

        public List<ReportViewModel> GetInvestigationsCompletedReport(
            DateTime FromDate, DateTime ToDate)
        {
            var AllComplianceForms = _UOW.ComplianceFormRepository.GetAll();

            if (AllComplianceForms.Count == 0)
                throw new Exception("No compliance forms found!");

            var Reports = new List<ReportViewModel>();
            var Report = new ReportViewModel();

            Report.Count = AllComplianceForms.SelectMany(s =>
            s.InvestigatorDetails).Where(x =>
            x.ReviewCompletedOn != null &&
            x.AddedOn >= FromDate &&
            x.AddedOn <= ToDate).Count();

            Reports.Add(Report);

            return Reports;
        }

        public List<ReportViewModel> GetInvestigationsCompletedReport(
            DateTime FromDate, DateTime ToDate, string AssignedTo)
        {
            var AllComplianceForms = _UOW.ComplianceFormRepository.GetAll();

            var Filter1 = GetInvestigationsCompletedReport(AllComplianceForms, AssignedTo);

            

            return null;
        }

        private List<ComplianceForm> GetInvestigationsCompletedReport(
            List<ComplianceForm> AllComplianceForms, string UserName)
        {
            return
                AllComplianceForms
                .Where(x => x.AssignedTo == UserName).ToList();
        }

        private int GetInvestigationsCompletedReport(
            List<ComplianceForm> ComplianceForms,
            int Day, int Month, int Year)
        {
            return
            ComplianceForms.SelectMany(s =>
            s.InvestigatorDetails)
            .Where(x =>
            x.AddedOn.Value.Month >= Month &&
            x.AddedOn.Value.Year >= Year &&
            x.AddedOn.Value.Month <= Month &&
            x.AddedOn.Value.Year <= Year &&
            x.ReviewCompletedOn != null).Count();
        }

        private void GetInvestigations(List<ComplianceForm> ComplianceForms)
        {
            var Users = _UOW.UserRepository.GetAll();

            foreach(User user in Users)
            {
                ComplianceForms.FindAll(x =>
                x.AssignedTo == user.UserName);
            }
        }
    }
}
