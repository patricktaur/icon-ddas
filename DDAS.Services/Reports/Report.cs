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

            var AdjustedStartDate = DateTimeExtensions.FirstDayOfMonth(Filters.FromDate.Date);
            var AdjustedEndDate = DateTimeExtensions.LastDayOfMonth(Filters.ToDate.Date);

            var AdjustedStartOfWeek = 
                DateTimeExtensions.StartOfWeek(Filters.FromDate.Date, DayOfWeek.Monday);

            var AdjustedEndOfWeek = 
                DateTimeExtensions.StartOfWeek(Filters.ToDate.Date, DayOfWeek.Monday);
            AdjustedEndOfWeek = AdjustedEndOfWeek.Date.AddDays(7);

            int EndPeriod = 0;

            if (Filters.ReportPeriodEnum == ReportPeriodEnum.Day)
                EndPeriod = (int)(Filters.ToDate.Date - Filters.FromDate.Date).TotalDays;
            else if (Filters.ReportPeriodEnum == ReportPeriodEnum.Week)
            {
                EndPeriod = (int)(AdjustedEndOfWeek.Date - AdjustedStartOfWeek.Date).TotalDays / 7;
            }
            else if (Filters.ReportPeriodEnum == ReportPeriodEnum.Month)
            {
                EndPeriod = (AdjustedEndDate.Year * 12 + AdjustedEndDate.Month) -
                    (AdjustedStartDate.Year * 12 + AdjustedStartDate.Month);
            }
            else if (Filters.ReportPeriodEnum == ReportPeriodEnum.Quarter)
            {

            }
            else if (Filters.ReportPeriodEnum == ReportPeriodEnum.Year)
            {
                EndPeriod = AdjustedEndDate.Year - AdjustedStartDate.Year;
            }

            InvestigationsReport.ReportByUsers = FillUpUserNames();

            var ComplianceForms = _UOW.ComplianceFormRepository.GetAll();

            if (ComplianceForms.Count == 0)
                return null;
            
            foreach(ReportByUser Report in InvestigationsReport.ReportByUsers)
            {
                for(int IncrementBy = 0; IncrementBy < EndPeriod; IncrementBy++)
                {
                    string Period = "";
                    var reportItem = new ReportItem();

                    AdjustedStartDate = AdjustStartDate(
                        Filters.FromDate, Filters.ReportPeriodEnum, 
                        IncrementBy, out Period);

                     AdjustedEndDate = AdjustEndDate(
                        Filters.FromDate, Filters.ToDate, 
                        Filters.ReportPeriodEnum, IncrementBy);

                    reportItem.Value = GetInvestigationsReport(
                        AdjustedStartDate, AdjustedEndDate,
                        ComplianceForms, Report.UserName);

                    reportItem.ReportPeriod = Period;

                    Report.ReportItems.Add(reportItem);
                }
            }
            return InvestigationsReport;
        }

        private DateTime AdjustStartDate(DateTime StartDate, ReportPeriodEnum Enum, 
            int IncrementBy, out string Period)
        {
            var tempStartDate = StartDate.Date;
            Period = "";
            int Count = 0;
            switch (Enum)
            {
                case ReportPeriodEnum.Day:
                    Count = IncrementBy;
                    tempStartDate = tempStartDate.AddDays(Count);
                    Period = tempStartDate.Day.ToString();
                    break;
                case ReportPeriodEnum.Week:
                    Count = IncrementBy * 7;
                    tempStartDate = tempStartDate.AddDays(Count);
                    
                    tempStartDate = DateTimeExtensions.StartOfWeek(tempStartDate, DayOfWeek.Monday);
                    Period = "week " + (IncrementBy + 1).ToString();
                    break;
                case ReportPeriodEnum.Month:
                    Count = IncrementBy;
                    tempStartDate = tempStartDate.AddMonths(Count);
                    tempStartDate = DateTimeExtensions.FirstDayOfMonth(tempStartDate);
                    Period = tempStartDate.Month.ToString("MMM");
                    break;
                case ReportPeriodEnum.Quarter:
                    tempStartDate = DateTimeExtensions.FirstDayOfMonth(tempStartDate);
                    break;
                case ReportPeriodEnum.Year:
                    Count = IncrementBy;
                    tempStartDate = DateTimeExtensions.FirstDayOfMonth(StartDate);
                    Period = tempStartDate.Year.ToString();
                    break;
                default: throw new Exception("Invalid ReportPeriodEnum");
            }
            return tempStartDate.Date;
        }

        private DateTime AdjustEndDate(DateTime StartDate, DateTime EndDate, 
            ReportPeriodEnum Enum, int IncrementBy)
        {
            var tempEndDate = EndDate.Date;
            int Count = 0;
            switch (Enum)
            {
                case ReportPeriodEnum.Day:
                    Count = IncrementBy + 1;
                    tempEndDate = StartDate.AddDays(Count).Date;
                    break;
                case ReportPeriodEnum.Week:
                    Count = (IncrementBy + 1) * 7;
                    tempEndDate = DateTimeExtensions.StartOfWeek(StartDate.Date, DayOfWeek.Monday);
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
