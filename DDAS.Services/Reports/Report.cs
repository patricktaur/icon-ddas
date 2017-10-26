using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using DDAS.Models.Enums;
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

        #region Closed Investigations
        public InvestigationsReport GetInvestigationsReport(
            ReportFilters Filters)
        {
            var InvestigationsReport = new InvestigationsReport();

            var AdjustedStartDate = AdjustStartDate(
                Filters.FromDate, Filters.ReportPeriodEnum);

            var AdjustedEndDate = AdjustEndDate(
                Filters.ToDate, Filters.ReportPeriodEnum);

            InvestigationsReport.DatesAdjustedTo =
                "Review Completed On From and To dates are adjusted to " +
                AdjustedStartDate.Date.ToString("dd MMM yyyy") +
                " - " +
                AdjustedEndDate.Date.ToString("dd MMM yyyy");

            int EndPeriod = GetEndPeriod(AdjustedStartDate, AdjustedEndDate,
                Filters.ReportPeriodEnum);

            InvestigationsReport.ReportByUsers = FillUpUserNames();

            var ComplianceForms = _UOW.ComplianceFormRepository.GetAll();

            if (ComplianceForms.Count == 0)
                return null;

            foreach (ReportByUser Report in InvestigationsReport.ReportByUsers)
            {
                for (int IncrementPeriodBy = 0; IncrementPeriodBy < EndPeriod; IncrementPeriodBy++)
                {
                    var reportItem = new ReportItem();

                    var CurrentStartDate = GetCurrentStartDate(
                        AdjustedStartDate, Filters.ReportPeriodEnum,
                        IncrementPeriodBy);

                    var CurrentEndDate = GetCurrentEndDate(
                        AdjustedStartDate, AdjustedEndDate,
                        Filters.ReportPeriodEnum, IncrementPeriodBy);

                    reportItem.Value = GetInvestigationsCompletedCount(
                        CurrentStartDate, CurrentEndDate,
                        ComplianceForms, Report.UserName);

                    reportItem.ReportPeriod = GetCurrentPeriod(
                        CurrentStartDate, CurrentEndDate,
                        Filters.ReportPeriodEnum);

                    Report.ReportItems.Add(reportItem);
                }
            }
            return InvestigationsReport;
        }

        private DateTime GetCurrentStartDate(DateTime StartDate, ReportPeriodEnum Enum,
            int IncrementBy)
        {
            var tempStartDate = StartDate.Date;
            int Count = 0;
            switch (Enum)
            {
                case ReportPeriodEnum.Day:
                    Count = IncrementBy;
                    tempStartDate = tempStartDate.AddDays(Count);
                    break;
                case ReportPeriodEnum.Week:
                    Count = IncrementBy * 7;
                    tempStartDate = tempStartDate.AddDays(Count);

                    //tempStartDate = DateTimeExtensions.StartOfWeek(tempStartDate, DayOfWeek.Monday);
                    break;
                case ReportPeriodEnum.Month:
                    Count = IncrementBy;
                    tempStartDate = tempStartDate.AddMonths(Count);
                    //tempStartDate = DateTimeExtensions.FirstDayOfMonth(tempStartDate);
                    break;
                case ReportPeriodEnum.Quarter:
                    Count = IncrementBy * 3;
                    tempStartDate = tempStartDate.AddMonths(Count);
                    //tempStartDate = DateTimeExtensions.FirstDayOfQuarter(tempStartDate);
                    break;
                case ReportPeriodEnum.Year:
                    Count = IncrementBy;
                    tempStartDate = StartDate.AddYears(Count);
                    //tempStartDate = DateTimeExtensions.FirstDayOfYear(tempStartDate);
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return tempStartDate.Date;
        }

        private DateTime GetCurrentEndDate(DateTime StartDate, DateTime EndDate,
            ReportPeriodEnum Enum, int IncrementBy)
        {
            var tempEndDate = StartDate.Date;
            //var tempEndDate = EndDate.Date;
            int Count = 0;
            switch (Enum)
            {
                case ReportPeriodEnum.Day:
                    Count = IncrementBy + 1;
                    tempEndDate = tempEndDate.AddDays(Count);
                    break;
                case ReportPeriodEnum.Week:
                    Count = (IncrementBy + 1) * 7;
                    //tempEndDate = DateTimeExtensions.StartOfWeek(tempEndDate, DayOfWeek.Monday);
                    tempEndDate = tempEndDate.Date.AddDays(Count).AddSeconds(-1);
                    break;
                case ReportPeriodEnum.Month:
                    Count = IncrementBy;
                    tempEndDate = tempEndDate.AddMonths(Count);
                    tempEndDate = DateTimeExtensions.LastDayOfMonth(tempEndDate);
                    break;
                case ReportPeriodEnum.Quarter:
                    Count = IncrementBy * 3;
                    //tempEndDate = DateTimeExtensions.FirstDayOfQuarter(tempEndDate);
                    tempEndDate = DateTimeExtensions.LastDayOfQuarter(tempEndDate);
                    tempEndDate = tempEndDate.AddMonths(Count);
                    break;
                case ReportPeriodEnum.Year:
                    Count = IncrementBy;
                    tempEndDate = tempEndDate.AddYears(Count);
                    tempEndDate = DateTimeExtensions.LastDayOfYear(tempEndDate);
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return tempEndDate.Date;
        }

        private DateTime AdjustStartDate(DateTime StartDate, ReportPeriodEnum Enum)
        {
            var tempStartDate = new DateTime();
            switch (Enum)
            {
                case ReportPeriodEnum.Day:
                    tempStartDate = StartDate;
                    break;
                case ReportPeriodEnum.Week:
                    tempStartDate = DateTimeExtensions.StartOfWeek(StartDate.Date, DayOfWeek.Monday);
                    break;
                case ReportPeriodEnum.Month:
                    tempStartDate = DateTimeExtensions.FirstDayOfMonth(StartDate.Date);
                    break;
                case ReportPeriodEnum.Quarter:
                    tempStartDate = DateTimeExtensions.FirstDayOfQuarter(StartDate.Date);
                    break;
                case ReportPeriodEnum.Year:
                    tempStartDate = DateTimeExtensions.FirstDayOfYear(StartDate.Date);
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return tempStartDate.Date;
        }

        private DateTime AdjustEndDate(DateTime EndDate, ReportPeriodEnum Enum)
        {
            var tempEndDate = new DateTime();
            switch (Enum)
            {
                case ReportPeriodEnum.Day:
                    tempEndDate = EndDate;
                    break;
                case ReportPeriodEnum.Week:
                    tempEndDate = DateTimeExtensions.StartOfWeek(EndDate.Date, DayOfWeek.Monday);
                    tempEndDate = tempEndDate.Date.AddDays(7);
                    break;
                case ReportPeriodEnum.Month:
                    tempEndDate = DateTimeExtensions.LastDayOfMonth(EndDate.Date);
                    break;
                case ReportPeriodEnum.Quarter:
                    tempEndDate = DateTimeExtensions.FirstDayOfQuarter(EndDate.Date);
                    tempEndDate = DateTimeExtensions.LastDayOfQuarter(tempEndDate);
                    break;
                case ReportPeriodEnum.Year:
                    tempEndDate = DateTimeExtensions.LastDayOfYear(EndDate.Date);
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return tempEndDate.Date;
        }

        private string GetCurrentPeriod(DateTime StartDate, DateTime EndDate,
            ReportPeriodEnum Enum)
        {
            var Period = "";
            switch (Enum)
            {
                case ReportPeriodEnum.Day:
                    Period = StartDate.Day.ToString();
                    break;
                case ReportPeriodEnum.Week:
                    Period = StartDate.Day.ToString() + " " + StartDate.ToString("MMM") +
                        " - " +
                        EndDate.Day.ToString() + " " + EndDate.ToString("MMM");
                    break;
                case ReportPeriodEnum.Month:
                    Period = StartDate.ToString("MMM yy");
                    break;
                case ReportPeriodEnum.Quarter:
                    Period = StartDate.ToString("MMM yy")
                        + " - "
                        + EndDate.ToString("MMM yy");
                    break;
                case ReportPeriodEnum.Year:
                    Period = StartDate.Year.ToString();
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return Period;
        }

        private int GetEndPeriod(DateTime StartDate, DateTime EndDate,
            ReportPeriodEnum Enum)
        {
            int EndPeriod = 0;
            switch (Enum)
            {
                case ReportPeriodEnum.Day:
                    EndPeriod = (int)(EndDate.Date - StartDate.Date).TotalDays;
                    EndPeriod += 1; //instead of adding 1 day to the end date
                    break;
                case ReportPeriodEnum.Week:
                    EndPeriod = (int)(EndDate.Date - StartDate.Date).TotalDays / 7;
                    break;
                case ReportPeriodEnum.Month:
                    EndPeriod = (EndDate.Year * 12 + EndDate.Month) -
                        (StartDate.Year * 12 + StartDate.Month);

                    EndPeriod += 1; //because month is zero indexed                    
                    break;
                case ReportPeriodEnum.Quarter:
                    EndPeriod = DateTimeExtensions.QuarterDifference(StartDate, EndDate);
                    break;
                case ReportPeriodEnum.Year:
                    EndPeriod = EndDate.Year - StartDate.Year;
                    EndPeriod += 1;
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return EndPeriod;
        }

        private int GetInvestigationsCompletedCount(DateTime StartDate, DateTime EndDate,
            List<ComplianceForm> ComplianceForms, string UserName)
        {
            var Count =
                 ComplianceForms.Where(x =>
                 x.AssignedTo == UserName)
                 .SelectMany(Inv => Inv.InvestigatorDetails)
                 .Where(s =>
                 s.ReviewCompletedOn != null &&
                 s.ReviewCompletedOn >= StartDate &&
                 s.ReviewCompletedOn <= EndDate).Count();
            return Count;
        }

        private List<ReportByUser> FillUpUserNames()
        {
            var Users = _UOW.UserRepository.GetAll();

            if (Users.Count == 0)
                throw new Exception("No users found in database");

            var ReportByUsers = new List<ReportByUser>();

            foreach (User user in Users)
            {
                var reportByUser = new ReportByUser();
                reportByUser.UserName = user.UserName;
                ReportByUsers.Add(reportByUser);
            }

            return ReportByUsers;
        }
        #endregion

        #region Open Investigations

        public List<OpenInvestigationsViewModel> GetOpenInvestigations()
        {
            var ComplianceForms = _UOW.ComplianceFormRepository.GetAll();

            if (ComplianceForms.Count == 0)
                return null;

            var Users = _UOW.UserRepository.GetAllUsers();

            var OpenInvestigations = new List<OpenInvestigationsViewModel>();

            foreach(User user in Users)
            {
                var OpenInvestigation = new OpenInvestigationsViewModel();

                var OpenInvestigators = ComplianceForms.Where(x =>
                x.AssignedTo.ToLower() == user.UserName.ToLower())
                .SelectMany(Inv => Inv.InvestigatorDetails)
                .Where(s => s.ReviewCompletedOn == null)
                .ToList();

                if (OpenInvestigators.Count == 0)
                    continue;

                OpenInvestigation.AssignedTo = user.UserName;
                OpenInvestigation.Count = OpenInvestigators.Count;

                OpenInvestigation.Earliest =
                OpenInvestigators.OrderBy(x => x.AddedOn)
                .First()
                .AddedOn;

                OpenInvestigation.Latest =
                OpenInvestigators.OrderByDescending(x => x.AddedOn)
                .First()
                .AddedOn;
                OpenInvestigations.Add(OpenInvestigation);
            }
            return OpenInvestigations;
        }
        #endregion
    }
}

