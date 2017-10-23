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

        public InvestigationsReport GetInvestigationsReport(ReportFilters Filters)
        {
            var InvestigationsReport = new InvestigationsReport();

            //var AdjustedStartDate = DateTimeExtensions.FirstDayOfMonth(Filters.FromDate.Date);
            //var AdjustedEndDate = DateTimeExtensions.LastDayOfMonth(Filters.ToDate.Date);

            var AdjustedStartDate = AdjustStartDate(Filters.FromDate.Date,
                Filters.ReportPeriodEnum, 0);

            var AdjustedEndDate = AdjustEndDate(Filters.FromDate.Date, 
                Filters.ToDate.Date, 
                Filters.ReportPeriodEnum, 0);

            //var AdjustedStartOfWeek = 
            //    DateTimeExtensions.StartOfWeek(Filters.FromDate.Date, DayOfWeek.Monday);

            //var AdjustedEndOfWeek = 
            //    DateTimeExtensions.StartOfWeek(Filters.ToDate.Date, DayOfWeek.Monday);
            //AdjustedEndOfWeek = AdjustedEndOfWeek.Date.AddDays(7);

            int EndPeriod = GetEndPeriod(AdjustedStartDate, AdjustedEndDate,
                Filters.ReportPeriodEnum);

            InvestigationsReport.ReportByUsers = FillUpUserNames();

            var ComplianceForms = _UOW.ComplianceFormRepository.GetAll();

            if (ComplianceForms.Count == 0)
                return null;
            
            foreach(ReportByUser Report in InvestigationsReport.ReportByUsers)
            {
                for(int IncrementPeriodBy = 0; IncrementPeriodBy < EndPeriod; IncrementPeriodBy++)
                {
                    var reportItem = new ReportItem();

                    var CurrentStartDate = AdjustStartDate(
                        Filters.FromDate, Filters.ReportPeriodEnum, 
                        IncrementPeriodBy);

                    var CurrentEndDate = AdjustEndDate(
                        Filters.FromDate, Filters.ToDate, 
                        Filters.ReportPeriodEnum, IncrementPeriodBy);

                    reportItem.Value = GetInvestigationsReport(
                        CurrentStartDate, CurrentEndDate,
                        ComplianceForms, Report.UserName);

                    reportItem.ReportPeriod = GetCurrentPeriod(
                        AdjustedStartDate, AdjustedEndDate, 
                        Filters.ReportPeriodEnum);

                    Report.ReportItems.Add(reportItem);
                }
            }
            return InvestigationsReport;
        }

        private DateTime AdjustStartDate(DateTime StartDate, ReportPeriodEnum Enum, 
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
                    
                    tempStartDate = DateTimeExtensions.StartOfWeek(tempStartDate, DayOfWeek.Monday);
                    break;
                case ReportPeriodEnum.Month:
                    Count = IncrementBy;
                    tempStartDate = tempStartDate.AddMonths(Count);
                    tempStartDate = DateTimeExtensions.FirstDayOfMonth(tempStartDate);
                    break;
                case ReportPeriodEnum.Quarter:
                    tempStartDate = DateTimeExtensions.FirstDayOfMonth(tempStartDate);
                    break;
                case ReportPeriodEnum.Year:
                    Count = IncrementBy;
                    tempStartDate = DateTimeExtensions.FirstDayOfMonth(StartDate);
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return tempStartDate.Date;
        }

        private DateTime AdjustEndDate(DateTime StartDate, DateTime EndDate, 
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
                    tempEndDate = DateTimeExtensions.StartOfWeek(tempEndDate, DayOfWeek.Monday);
                    tempEndDate = tempEndDate.Date.AddDays(Count).AddSeconds(-1);
                    break;
                case ReportPeriodEnum.Month:
                    Count = IncrementBy;
                    tempEndDate = tempEndDate.AddMonths(Count);
                    tempEndDate = DateTimeExtensions.LastDayOfMonth(tempEndDate);
                    break;
                case ReportPeriodEnum.Quarter:
                    tempEndDate = tempEndDate.AddMonths(3);
                    tempEndDate = DateTimeExtensions.LastDayOfMonth(tempEndDate);
                    break;
                case ReportPeriodEnum.Year:
                    tempEndDate = DateTimeExtensions.LastDayOfMonth(tempEndDate);
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return tempEndDate.Date;
        }

        private string GetCurrentPeriod(DateTime StartDate, DateTime EndDate,
            ReportPeriodEnum Enum)
        {
            var Period = "";
            switch(Enum)
            {
                case ReportPeriodEnum.Day:
                    Period = StartDate.Day.ToString();
                    break;
                case ReportPeriodEnum.Week:
                    Period = StartDate.Day.ToString() + " " + StartDate.ToString("MMM") +
                        "-" + 
                        EndDate.Day.ToString() + " " + EndDate.ToString("MMM");
                    break;
                case ReportPeriodEnum.Month:
                    Period = StartDate.ToString("MMM");
                    break;
                case ReportPeriodEnum.Quarter:
                    Period = StartDate.ToString("MMM");
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
                    break;
                case ReportPeriodEnum.Year:
                    EndPeriod = EndDate.Year - StartDate.Year;
                    EndPeriod += 1;
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return EndPeriod;
        }

        private int GetInvestigationsReport(DateTime StartDate, DateTime EndDate,
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

            foreach(User user in Users)
            {
                var reportByUser = new ReportByUser();
                reportByUser.UserName = user.UserName;
                ReportByUsers.Add(reportByUser);
            }

            return ReportByUsers;
        }

    }
}
