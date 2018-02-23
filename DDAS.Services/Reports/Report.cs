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
                Filters.FromDate, Filters.ToDate, Filters.ReportPeriodEnum);

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

        private DateTime AdjustEndDate(DateTime StartDate, DateTime EndDate, ReportPeriodEnum Enum)
        {
            var maxResults = 15;
            var tempEndDate = new DateTime();
            switch (Enum)
            {
                case ReportPeriodEnum.Day:
                    var days = (EndDate - StartDate).Days;
                    if (days > maxResults)
                    {
                        tempEndDate = StartDate.Date.AddDays(maxResults);
                    }
                    else
                    {
                        tempEndDate = EndDate;
                    }
                    
                    break;
                case ReportPeriodEnum.Week:
                    var weeks = (EndDate - StartDate).Days / 7;
                    if (weeks > maxResults)
                    {
                        tempEndDate = DateTimeExtensions.StartOfWeek(StartDate.AddDays(7 * maxResults), DayOfWeek.Monday);
                        tempEndDate = tempEndDate.Date.AddDays(6);
                    }
                    else
                    {
                        tempEndDate = DateTimeExtensions.StartOfWeek(EndDate.Date, DayOfWeek.Monday);
                        tempEndDate = tempEndDate.Date.AddDays(6);
                    }
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
                reportByUser.UserFullName = user.UserFullName;
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

            foreach (User user in Users)
            {
                var OpenInvestigation = new OpenInvestigationsViewModel();

                var OpenInvestigators = ComplianceForms.Where(x =>
                (x.AssignedTo != null || x.AssignedTo != "") &&
                x.AssignedTo.ToLower() == user.UserName.ToLower())
                .SelectMany(Inv => Inv.InvestigatorDetails)
                .Where(s => s.ReviewCompletedOn == null)
                .ToList();

                if (OpenInvestigators.Count == 0)
                    continue;

                OpenInvestigation.AssignedTo = user.UserFullName;
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


        #region Admin Dashboard

        public List<AdminDashboardViewModel> GetAdminDashboard()
        {
            var Users = _UOW.UserRepository.GetAll();

            if (Users.Count == 0)
                throw new Exception("No Users found in the database");

            var Forms = _UOW.ComplianceFormRepository.GetAll();

            if (Forms.Count == 0)
                throw new Exception("No compliance forms found");

            var AdminDashboardList = new List<AdminDashboardViewModel>();

            foreach (User user in Users)
            {
                var AdminDashboard = new AdminDashboardViewModel();
                AdminDashboard.UserName = user.UserName;
                AdminDashboard.UserFullName = user.UserFullName;
                AdminDashboard.OpeningBalance =
                    OpeningBalance(user.UserName, Forms);
                AdminDashboard.InvestigatorUploaded =
                    ComplianceFormsUploadedToday(user.UserName, Forms);
                AdminDashboard.InvestigatorReviewCompleted =
                    ComplianceFormsReviewCompletedToday(user.UserName, Forms);
                AdminDashboard.ClosingBalance = ClosingBalance(user.UserName, Forms);
                AdminDashboardList.Add(AdminDashboard);
            }
            return AdminDashboardList;
        }

        public List<AdminDashboardDrillDownViewModel> 
            GetAdminDashboardDrillDownDetails(
            string AssignedTo, AdminDashboardReportType ReportType)
        {
            var Forms = _UOW.ComplianceFormRepository.GetAll();

            if (Forms.Count == 0)
                throw new Exception("No compliance forms found");

            switch (ReportType)
            {
                case AdminDashboardReportType.OpeningBalance:
                    return OpeningBalanceList(AssignedTo, Forms);
                case AdminDashboardReportType.ComplianceFormsUploaded:
                    return ComplianceFormsUploadedList(AssignedTo, Forms);
                case AdminDashboardReportType.ComplianceFormsCompleted:
                    return ComplianceFormsReviewCompletedList(AssignedTo, Forms);
                case AdminDashboardReportType.ClosingBalance:
                    return ClosingBalanceList(AssignedTo, Forms);
                default:
                    throw new Exception("invalid enum");
            }
        }

        private int OpeningBalance(string UserName, List<ComplianceForm> Forms)
        {
            return OpeningBalanceList(UserName, Forms).Count();
        }

        private List<AdminDashboardDrillDownViewModel> OpeningBalanceList(string UserName, List<ComplianceForm> Forms)
        {
            Forms = Forms.Where(x =>
            (x.AssignedTo != null || x.AssignedTo != "") &&
            x.AssignedTo.ToLower() == UserName.ToLower() &&
            (x.ReviewCompletedOn == null ||
            x.ReviewCompletedOn.Value.Date >= DateTime.Now.Date) &&
            x.SearchStartedOn.Date < DateTime.Now.Date)
            .ToList();

            var DrillDownList = new List<AdminDashboardDrillDownViewModel>();

            foreach (ComplianceForm Form in Forms)
            {
                var DrillDownRecord = new AdminDashboardDrillDownViewModel();
                DrillDownRecord.UserFullName = GetUserFullName(UserName);
                DrillDownRecord.PrincipalInvestigator = 
                    Form.InvestigatorDetails.FirstOrDefault().Name;
                DrillDownRecord.InvestigatorCount = Form.InvestigatorDetails.Count();
                DrillDownRecord.ProjectNumber = Form.ProjectNumber;
                DrillDownRecord.ProjectNumber2 = Form.ProjectNumber2;
                DrillDownRecord.SponsorProtocolNumber = Form.SponsorProtocolNumber;
                DrillDownRecord.SponsorProtocolNumber2 = Form.SponsorProtocolNumber2;
                DrillDownRecord.ComplianceFormStatus = Form.Status;

                DrillDownList.Add(DrillDownRecord);
            }
            return DrillDownList;
        }



        private int ComplianceFormsUploadedToday(string UserName, List<ComplianceForm> Forms)
        {
            return ComplianceFormsUploadedList(UserName, Forms).Count();
        }

        private List<AdminDashboardDrillDownViewModel> ComplianceFormsUploadedList(string UserName, List<ComplianceForm> Forms)
        {
            Forms = Forms.Where(x => 
            x.SearchStartedOn.Date >= DateTime.Now.Date)
            .ToList();

            var DrillDownList = new List<AdminDashboardDrillDownViewModel>();

            foreach (ComplianceForm Form in Forms)
            {
                var DrillDownRecord = new AdminDashboardDrillDownViewModel();
                DrillDownRecord.UserFullName = GetUserFullName(UserName);
                DrillDownRecord.PrincipalInvestigator =
                    Form.InvestigatorDetails.FirstOrDefault().Name;
                DrillDownRecord.InvestigatorCount = Form.InvestigatorDetails.Count();
                DrillDownRecord.ProjectNumber = Form.ProjectNumber;
                DrillDownRecord.ProjectNumber2 = Form.ProjectNumber2;
                DrillDownRecord.SponsorProtocolNumber = Form.SponsorProtocolNumber;
                DrillDownRecord.SponsorProtocolNumber2 = Form.SponsorProtocolNumber2;
                DrillDownRecord.ComplianceFormStatus = Form.Status;

                DrillDownList.Add(DrillDownRecord);
            }
            return DrillDownList;
        }

        private int ComplianceFormsReviewCompletedToday(string UserName, List<ComplianceForm> Forms)
        {
            return ComplianceFormsReviewCompletedList(UserName, Forms).Count();
        }

        private List<AdminDashboardDrillDownViewModel> ComplianceFormsReviewCompletedList(string UserName, List<ComplianceForm> Forms)
        {
            Forms = Forms.Where(x =>
            (x.AssignedTo != null || x.AssignedTo != "") &&
            x.AssignedTo.ToLower() == UserName.ToLower() &&
            x.ReviewCompletedOn != null &&
            x.ReviewCompletedOn.Value.Date >= DateTime.Now.Date)
            .ToList();

            var DrillDownList = new List<AdminDashboardDrillDownViewModel>();

            foreach (ComplianceForm Form in Forms)
            {
                var DrillDownRecord = new AdminDashboardDrillDownViewModel();
                DrillDownRecord.UserFullName = GetUserFullName(UserName);
                DrillDownRecord.PrincipalInvestigator =
                    Form.InvestigatorDetails.FirstOrDefault().Name;
                DrillDownRecord.InvestigatorCount = Form.InvestigatorDetails.Count();
                DrillDownRecord.ProjectNumber = Form.ProjectNumber;
                DrillDownRecord.ProjectNumber2 = Form.ProjectNumber2;
                DrillDownRecord.SponsorProtocolNumber = Form.SponsorProtocolNumber;
                DrillDownRecord.SponsorProtocolNumber2 = Form.SponsorProtocolNumber2;
                DrillDownRecord.ComplianceFormStatus = Form.Status;

                DrillDownList.Add(DrillDownRecord);
            }
            return DrillDownList;
        }

        private int ClosingBalance(string UserName, List<ComplianceForm> Forms)
        {
            return ClosingBalanceList(UserName, Forms).Count();
        }

        private List<AdminDashboardDrillDownViewModel> ClosingBalanceList(string UserName, List<ComplianceForm> Forms)
        {
            Forms = Forms.Where(x =>
            (x.AssignedTo != null || x.AssignedTo != "") &&
            x.AssignedTo.ToLower() == UserName.ToLower() &&
            x.IsReviewCompleted == false)
            .ToList();

            var DrillDownList = new List<AdminDashboardDrillDownViewModel>();

            foreach (ComplianceForm Form in Forms)
            {
                var DrillDownRecord = new AdminDashboardDrillDownViewModel();
                DrillDownRecord.UserFullName = GetUserFullName(UserName);
                DrillDownRecord.InvestigatorCount = Form.InvestigatorDetails.Count();

                if (Form.InvestigatorDetails.Count > 0)
                    DrillDownRecord.PrincipalInvestigator =
                        Form.InvestigatorDetails.FirstOrDefault().Name;

                DrillDownRecord.ProjectNumber = Form.ProjectNumber;
                DrillDownRecord.ProjectNumber2 = Form.ProjectNumber2;
                DrillDownRecord.SponsorProtocolNumber = Form.SponsorProtocolNumber;
                DrillDownRecord.SponsorProtocolNumber2 = Form.SponsorProtocolNumber2;
                DrillDownRecord.ComplianceFormStatus = Form.Status;

                DrillDownList.Add(DrillDownRecord);
            }
            return DrillDownList;
        }
        #endregion

        public List<AssignmentHistoryViewModel> GetAssignmentHistory(
            ReportFilterViewModel ReportFilter)
        {
            var AssignmentHistoryList =
                _UOW.AssignmentHistoryRepository.GetAll();

            if (AssignmentHistoryList.Count == 0)
                return null;

            AssignmentHistoryList = AssignmentHistoryList.Where(x =>
            x.AssignedOn >= ReportFilter.FromDate.Date &&
            x.AssignedOn <= ReportFilter.ToDate.Date)
            .ToList();

            var Assignments = new List<AssignmentHistoryViewModel>();

            foreach (AssignmentHistory assignmentHistory in AssignmentHistoryList)
            {
                var assignmentHistoryViewModel = new AssignmentHistoryViewModel();

                var ComplianceForm = _UOW.ComplianceFormRepository
                    .FindById(assignmentHistory.ComplianceFormId);

                if (ComplianceForm == null)
                    continue;

                if(ComplianceForm.InvestigatorDetails.Count > 0)
                {
                    assignmentHistoryViewModel.PrincipalInvestigator =
                        ComplianceForm.InvestigatorDetails.FirstOrDefault().Name;
                    assignmentHistoryViewModel.InvestigatorCount =
                        ComplianceForm.InvestigatorDetails.Count - 1;
                }

                assignmentHistoryViewModel.ProjectNumber =
                    ComplianceForm.ProjectNumber;
                assignmentHistoryViewModel.ProjectNumber2 =
                    ComplianceForm.ProjectNumber2;
                assignmentHistoryViewModel.SearchStartedOn =
                    ComplianceForm.SearchStartedOn;

                assignmentHistoryViewModel.AssignedBy =
                    GetUserFullName(assignmentHistory.AssignedBy);
                assignmentHistoryViewModel.AssignedOn =
                    assignmentHistory.AssignedOn;
                assignmentHistoryViewModel.AssignedTo =
                    GetUserFullName(assignmentHistory.AssignedTo);
                assignmentHistoryViewModel.PreviouslyAssignedTo =
                    GetUserFullName(assignmentHistory.PreviouslyAssignedTo);

                Assignments.Add(assignmentHistoryViewModel);
            }

            return Assignments.OrderByDescending(x => x.AssignedOn).ToList();
        }

        public List<InvestigatorReviewCompletedTimeVM>
            GetInvestigatorsReviewCompletedTime(ReportFilterViewModel ReportFilter)
        {
            var ComplianceForms = _UOW.ComplianceFormRepository.GetAll();

            if (ComplianceForms.Count == 0)
                return null;

            var ReviewCompletedInvestigatorsVM =
                new List<InvestigatorReviewCompletedTimeVM>();

            if (ReportFilter.AssignedTo != null && ReportFilter.AssignedTo.ToLower() != "all")
                ComplianceForms = ComplianceForms.Where(x =>
                x.AssignedTo.ToLower() == ReportFilter.AssignedTo.ToLower())
                .ToList();

            var ReviewCompletedInvestigators = ComplianceForms
                .SelectMany(x => x.InvestigatorDetails,
                (ComplianceForm, InvestigatorSearched) =>
                new {
                    ComplianceForm,
                    InvestigatorSearched
                })
                .Where(s => s.InvestigatorSearched.ReviewCompletedOn != null &&
                s.InvestigatorSearched.ReviewCompletedOn >= ReportFilter.FromDate.Date &&
                s.InvestigatorSearched.ReviewCompletedOn <= ReportFilter.ToDate.Date)
                .Select(s =>
                new
                {
                    ProjectNumber = s.ComplianceForm.ProjectNumber,
                    ProjectNumber2 = s.ComplianceForm.ProjectNumber2,
                    Name = s.InvestigatorSearched.Name,
                    Role = s.InvestigatorSearched.Role,
                    SearchStartedOn = s.ComplianceForm.SearchStartedOn,
                    ReviewCompletedOn = s.InvestigatorSearched.ReviewCompletedOn.Value,
                    AssignedTo = s.ComplianceForm.AssignedTo,
                    FullMatchCount =
                    s.InvestigatorSearched.SitesSearched.Sum(x => x.FullMatchCount),
                    PartialMatchCount =
                    s.InvestigatorSearched.SitesSearched.Sum(x => x.PartialMatchCount),
                    SingleMatchCount =
                    s.InvestigatorSearched.SitesSearched.Sum(x => x.SingleMatchCount),
                    IssuesIdentified = s.InvestigatorSearched.TotalIssuesFound
                })
                .ToList();

            ReviewCompletedInvestigators.ForEach(Investigator =>
            {
                var VM = new InvestigatorReviewCompletedTimeVM();
                VM.InvestigatorName = Investigator.Name;
                VM.Role = Investigator.Role;
                VM.ProjectNumber = Investigator.ProjectNumber;
                VM.ProjectNumber2 = Investigator.ProjectNumber2;
                VM.SearchStartedOn = Investigator.SearchStartedOn;
                VM.ReviewCompletedOn = Investigator.ReviewCompletedOn;

                VM.AssignedTo =
                    GetUserFullName(Investigator.AssignedTo);
                VM.FullMatchCount = Investigator.FullMatchCount;
                VM.PartialMatchCount = Investigator.PartialMatchCount;
                VM.SingleMatchCount = Investigator.SingleMatchCount;

                VM.IssuesIdentifiedStatus = Investigator.IssuesIdentified == 0
                    ? "No Issues Identified" : "Issues Identified";

                ReviewCompletedInvestigatorsVM.Add(VM);
            });
            return ReviewCompletedInvestigatorsVM
                .OrderByDescending(
                x => x.ReviewCompletedOn)
                .ToList();
        }

        public List<StudySpecificInvestigatorVM>
            GetStudySpecificInvestigators(ReportFilterViewModel ReportFilter)
        {
            var ComplianceForms = _UOW.ComplianceFormRepository.GetAll();

            if (ComplianceForms.Count == 0)
                return null;

            var StudySpecificInvestigators =
                ComplianceForms.Where(form => form.ProjectNumber ==
                ReportFilter.ProjectNumber ||
                form.ProjectNumber2 == ReportFilter.ProjectNumber)
                .SelectMany(form =>
                form.InvestigatorDetails, (form, Investigator) =>
                new { form, Investigator })
                .Where(s => s.Investigator.ReviewCompletedOn != null &&
                s.Investigator.ReviewCompletedOn >= ReportFilter.FromDate.Date &&
                s.Investigator.ReviewCompletedOn <= ReportFilter.ToDate.Date)
                .Select(s =>
                new
                {
                    ProjectNumber = s.form.ProjectNumber,
                    ProjectNumber2 = s.form.ProjectNumber2,
                    InvestigatorName = s.Investigator.Name,
                    ReviewCompletedOn = s.Investigator.ReviewCompletedOn.Value,
                    FindingStatus = s.Investigator.IssuesFoundSiteCount,
                    AssigendTo = s.form.AssignedTo,
                    Institute = s.form.Institute,
                    Country = s.form.Country,
                    SponsorProtocolNumber = s.form.SponsorProtocolNumber,
                    SponsorProtocolNumber2 = s.form.SponsorProtocolNumber2,
                    MedicalLicenseNumber = s.Investigator.MedicalLiceseNumber,
                    Role = s.Investigator.Role
                })
                .ToList();

            var Limit = StudySpecificInvestigators.Count();

            var StudySpecificInvestigatorVMList =
                new List<StudySpecificInvestigatorVM>();

            for (int Index = 0; Index < Limit; Index++)
            {
                var VM = new StudySpecificInvestigatorVM();

                VM.ProjectNumber = StudySpecificInvestigators[Index].ProjectNumber;
                VM.ProjectNumber2 = StudySpecificInvestigators[Index].ProjectNumber2;
                VM.InvestigatorName = StudySpecificInvestigators[Index].InvestigatorName;
                VM.ReviewCompletedOn = StudySpecificInvestigators[Index].ReviewCompletedOn;

                VM.FindingStatus = StudySpecificInvestigators[Index].FindingStatus == 0
                    ? "No Issues Identified" : "Issues Identified";

                VM.AssignedTo =
                    GetUserFullName(StudySpecificInvestigators[Index].AssigendTo);

                VM.Role = StudySpecificInvestigators[Index].Role;
                VM.MedicalLicenseNumber = StudySpecificInvestigators[Index].MedicalLicenseNumber;
                VM.Institute = StudySpecificInvestigators[Index].Institute;
                VM.SponsorProtocolNumber = StudySpecificInvestigators[Index].SponsorProtocolNumber;
                VM.SponsorProtocolNumber2 = StudySpecificInvestigators[Index].SponsorProtocolNumber2;
                VM.Country = StudySpecificInvestigators[Index].Country;

                StudySpecificInvestigatorVMList.Add(VM);
            }
            return StudySpecificInvestigatorVMList
                .OrderByDescending(x => x.ReviewCompletedOn).ToList();
        }

        public List<InvestigatorFindingViewModel> GetInvestigatorByFinding(
            ReportFilterViewModel ReportFilter)
        {
            var ComplianceForms = _UOW.ComplianceFormRepository.GetAll();

            if (ComplianceForms.Count == 0)
                return null;

            if (ReportFilter.AssignedTo != null && ReportFilter.AssignedTo.ToLower() != "all")
                ComplianceForms = ComplianceForms.Where(x =>
                x.AssignedTo.ToLower() == ReportFilter.AssignedTo.ToLower())
                .ToList();

            var ReviewCompletedInvestigators = ComplianceForms
                .SelectMany(Form => Form.InvestigatorDetails,
                (Form, Investigator) =>
                new { Form, Investigator })
                .Where(s =>
                s.Investigator.ReviewCompletedOn != null &&
                s.Investigator.ReviewCompletedOn >= ReportFilter.FromDate.Date &&
                s.Investigator.ReviewCompletedOn <= ReportFilter.ToDate.Date)
                .Select(s =>
                new
                {
                    ProjectNumber = s.Form.ProjectNumber,
                    ProjectNumber2 = (s.Form.ProjectNumber2 == null ? "" : s.Form.ProjectNumber2),
                    AssignedTo = s.Form.AssignedTo,
                    InvestigatorId = s.Investigator.Id,
                    InvestigatorName = s.Investigator.Name,
                    Role = s.Investigator.Role,
                    ReviewCompletedOn = s.Investigator.ReviewCompletedOn.Value,

                    Findings = s.Form.Findings
                        .Where(finding => finding.InvestigatorSearchedId ==
                        s.Investigator.Id &&
                        finding.InvestigatorName.ToLower() ==
                        s.Investigator.Name.ToLower()).ToList(),
                })
                .ToList();

            var InvestigatorFindingVMList =
                new List<InvestigatorFindingViewModel>();

            var Limit = ReviewCompletedInvestigators.Count();

            for (int Index = 0; Index < Limit; Index++)
            {
                var VM = new InvestigatorFindingViewModel();

                VM.ProjectNumber = ReviewCompletedInvestigators[Index].ProjectNumber;
                VM.ProjectNumber2 = ReviewCompletedInvestigators[Index].ProjectNumber2;
                VM.InvestigatorName = ReviewCompletedInvestigators[Index].InvestigatorName;
                VM.Role = ReviewCompletedInvestigators[Index].Role;

                VM.ReviewCompletedBy =
                    GetUserFullName(ReviewCompletedInvestigators[Index].AssignedTo);

                VM.ReviewCompletedOn = ReviewCompletedInvestigators[Index].ReviewCompletedOn;

                var Findings = ReviewCompletedInvestigators[Index].Findings;

                if (Findings.Count() > 0 &&
                    Findings.Where(x => x.IsAnIssue == true).Count() > 0)
                {
                    Findings
                        .Where(x => x.IsAnIssue == true)
                        .ToList()
                        .ForEach(finding =>
                    {
                        var tempVM = VM;
                        if (finding.SiteEnum != null)
                            tempVM.SiteShortName =
                            _UOW.SiteSourceRepository.GetAll()
                            .Find(x => x.SiteEnum == finding.SiteEnum).SiteShortName;
                        else
                            tempVM.SiteShortName = finding.SiteEnum.ToString();
                        tempVM.FindingObservation = finding.Observation;
                        InvestigatorFindingVMList.Add(tempVM);
                    });
                }
                else
                    InvestigatorFindingVMList.Add(VM);
            }

            var filter1 = InvestigatorFindingVMList;

            if (ReportFilter.ProjectNumber != null &&
                ReportFilter.ProjectNumber != "")
            {
                filter1 = InvestigatorFindingVMList.Where(x =>
                x.ProjectNumber == ReportFilter.ProjectNumber ||
                x.ProjectNumber2 == ReportFilter.ProjectNumber)
                .ToList();
            }
            return filter1
                .OrderByDescending(x => x.ReviewCompletedOn)
                .ToList();
        }

        private string GetUserFullName(string AssignedTo)
        {
            if (AssignedTo == null || AssignedTo == "")
                return "";

            var User = _UOW.UserRepository.GetAll()
                .Find(x => x.UserName.ToLower() == AssignedTo.ToLower());

            return
                User != null ? User.UserFullName : "";
        }
    }
}
