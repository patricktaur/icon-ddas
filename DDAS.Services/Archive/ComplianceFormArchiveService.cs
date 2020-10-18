using DDAS.Models;
using DDAS.Models.Entities;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Utilities;
using Utilities.WordTemplate;
using static DDAS.Models.ViewModels.RequestPayloadforDDAS;
using static DDAS.Models.ViewModels.RequestPayloadforiSprint;

namespace DDAS.Services.Search
{
    public class ComplianceFormArchiveService : IComplianceFormArchiveService, IDisposable
    {
        private IUnitOfWork _UOW;
        //private ISearchEngine _SearchEngine;
        private IConfig _config;
        //private CachedSiteScanData _cachedData;

        private const int _MatchCountLowerLimit = 2;
        //private int _NumberOfRunningExtractionProcesses = 4;

        public ComplianceFormArchiveService(IUnitOfWork uow,
            //ISearchEngine SearchEngine,
            
            IConfig Config)
        {
            _UOW = uow;
            //_SearchEngine = SearchEngine;
            _config = Config;
            //_cachedData = new CachedSiteScanData(_UOW);
        }

        // //Patrick: 7Jan2018 For use in ExtractDataService
        //public IConfig Config
        //{
        //    get
        //    {
        //        return _config;
        //    }
        //}
        public List<PrincipalInvestigatorArchive> GetComplianceFormsFromFiltersWithReviewDates(
                    ComplianceFormArchiveFilter CompFormFilter)
        {

            var compForms = GetComplianceFormsFromFilters(CompFormFilter);
            //ReviewCompletedOnFrom is a computed value
            var compForms1 = compForms;
            if (CompFormFilter.ReviewCompletedOnFrom != null)
            {
                DateTime startDate;
                startDate = CompFormFilter.ReviewCompletedOnFrom.Value.Date;
                compForms1 = compForms.Where(x => x.ReviewCompletedOn >= startDate).ToList();
            }
            var compForms2 = compForms1;
            if (CompFormFilter.ReviewCompletedOnTo != null)
            {
                DateTime endDate;
                endDate = CompFormFilter.ReviewCompletedOnTo.Value.Date.AddDays(1);
                compForms2 = compForms1.Where(x => x.ReviewCompletedOn < endDate).ToList();
            }
            return compForms2;


        }
        public List<PrincipalInvestigatorArchive> GetComplianceFormsFromFilters(
                    ComplianceFormArchiveFilter CompFormFilter)
        {
            if (CompFormFilter == null)
            {
                throw new Exception("Invalid CompFormFilter");
            }

            var compForms = _UOW.ComplianceFormArchiveRepository.FindComplianceForms(CompFormFilter);

            if ((int)CompFormFilter.Status == -1)
            {
                //Commented on 16May2018: Pradeep
                //compForms = compForms.FindAll(x => x.StatusEnum ==
                //ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
                //x.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                //.ToList();
            }
            else if (CompFormFilter.Status == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified)
            {
                compForms = compForms.Where(x =>
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                .ToList();
            }
            else if (CompFormFilter.Status == ComplianceFormStatusEnum.FullMatchFoundReviewPending)
            {
                compForms = compForms.Where(x =>
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.FullMatchFoundReviewPending ||
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.PartialMatchFoundReviewPending ||
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.NoMatchFoundReviewPending ||
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.IssuesIdentifiedReviewPending ||
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.SingleMatchFoundReviewPending ||
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.ManualSearchSiteReviewPending ||
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.HasExtractionErrors ||
                x.ComplianceForm.StatusEnum == ComplianceFormStatusEnum.NotScanned)
                .ToList();
            }

            return getPrincipalInvestigators(compForms).OrderByDescending(x => x.SearchStartedOn).ToList();
        }

        public List<PrincipalInvestigatorArchive> getPrincipalInvestigators(List<ComplianceFormArchive> Forms)
        {
            var PIList = new List<PrincipalInvestigatorArchive>();

            foreach (ComplianceFormArchive Form in Forms)
            {
                var PI = getPrincipalInvestigators(Form);
                PIList.Add(PI);
            }
            return PIList;
        }

        private PrincipalInvestigatorArchive getPrincipalInvestigators(ComplianceFormArchive compForm)
        {
            var item = new PrincipalInvestigatorArchive();
            item.ArchivedOn = compForm.ArchivedOn;
            item.InputSource = compForm.ComplianceForm.InputSource;
            item.Address = compForm.ComplianceForm.Address;
            item.Country = compForm.ComplianceForm.Country;
            item.ProjectNumber = compForm.ProjectNumber;
            item.ProjectNumber2 = compForm.ProjectNumber2;
            item.Institute = compForm.ComplianceForm.Institute;
            item.SponsorProtocolNumber = compForm.SponsorProtocolNumber;
            item.SponsorProtocolNumber2 = compForm.SponsorProtocolNumber2;
            item.RecId = compForm.ComplianceForm.RecId;
            item.Active = compForm.ComplianceForm.Active;
            item.SearchStartedOn = compForm.SearchStartedOn;
            item.CurrentReviewStatus = compForm.ComplianceForm.CurrentReviewStatus;
            item.Reviewer = compForm.Reviewer;
            item.QCVerifier = compForm.ComplianceForm.QCVerifier;
            item.ExportedToiSprintOn = compForm.ComplianceForm.ExportedToiSprintOn;
            item.ReviewCompletedOn = compForm.ReviewCompletedOn;
            if (compForm.ComplianceForm.InvestigatorDetails.Count > 0)
            {
                item.Name = compForm.ComplianceForm.InvestigatorDetails.FirstOrDefault().Name;
            }
            item.AssignedTo = compForm.ComplianceForm.AssignedTo;
            item.AssignedToFullName = GetUserFullName(compForm.ComplianceForm.AssignedTo);
            item.Status = compForm.Status;
            item.StatusEnum = compForm.ComplianceForm.StatusEnum;
            item.ExtractionErrorInvestigatorCount = compForm.ComplianceForm.ExtractionErrorInvestigatorCount;
            item.ExtractionPendingInvestigatorCount = compForm.ComplianceForm.ExtractionPendingInvestigatorCount;
            item.EstimatedExtractionCompletionWithin = compForm.ComplianceForm.EstimatedExtractionCompletionWithin;

            foreach (InvestigatorSearched Investigator in compForm.ComplianceForm.InvestigatorDetails)
            {
                if (Investigator.Role.ToLower() == "sub i")
                {
                    var SubInv = new SubInvestigator();
                    SubInv.Name = Investigator.Name;
                    SubInv.Status = Investigator.Status;
                    SubInv.StatusEnum = Investigator.StatusEnum;
                    item.SubInvestigators.Add(SubInv);
                }
            }
            //CanUndoQC(item, compForm);
            return item;
        }


        public int ArchiveComplianceFormsRecordCount(int days,  string type)
        {
            List<ComplianceForm> compFormsToArchive = new List<ComplianceForm>();
            var limit = 100000; //no limit
            int recordCount = 0;
            switch (type)
            {
                case "search":
                    recordCount =  _UOW.ComplianceFormRepository.FindComplianceFormsBySearchedOn(days, limit).Count();
                        
                    break;
                case "review":
                    var dt = DateTime.Now.AddDays(-days);
                    //ReviewCompletedOn is a computed value, cannot be accessed through mongodb
                    //limit the records
                    //by quering records searched before 'days'
                    //further filter only review completed records before 'days'
                    compFormsToArchive = _UOW.ComplianceFormRepository.FindComplianceFormsBySearchedOn(days, limit);
                    recordCount = compFormsToArchive.Where(x => x.ReviewCompletedOn < dt).Count();
                    break;
                default:
                    
                    break;
            }
            return recordCount;
        }


        public string ArchiveComplianceFormsWithSearchDaysGreaterThan(int days, int limit, string type)
        {
            List<ComplianceForm> compFormsToArchive = new List<ComplianceForm>();

            switch (type)
            {
                case "search":
                    compFormsToArchive = _UOW.ComplianceFormRepository.FindComplianceFormsBySearchedOn(days, limit)
                        .OrderBy(x => x.SearchStartedOn).ToList();
                    break;
                case  "review":
                    var dt = DateTime.Now.AddDays(-days);
                    //ReviewCompletedOn is a computed value, cannot be accessed through mongodb
                    //limit the records
                    //by quering records searched before 'days'
                    //further filter only review completed records before 'days'
                    compFormsToArchive = _UOW.ComplianceFormRepository.FindComplianceFormsBySearchedOn(days, limit);
                    compFormsToArchive = compFormsToArchive.Where(x => x.ReviewCompletedOn < dt)
                        .OrderBy(x => x.ReviewCompletedOn).ToList();
                    break;
                default:
                    break;
            }

            var archivedCount = 0;
            foreach (var comp in compFormsToArchive)
            {
                var compFormArchv = new ComplianceFormArchive();
                compFormArchv.RecId = comp.RecId;
                compFormArchv.ArchivedOn = DateTime.Now;
                compFormArchv.SponsorProtocolNumber = comp.SponsorProtocolNumber;
                compFormArchv.SponsorProtocolNumber2 = comp.SponsorProtocolNumber2;
                compFormArchv.ProjectNumber = comp.ProjectNumber;
                compFormArchv.ProjectNumber2 = comp.ProjectNumber2;
                compFormArchv.SearchStartedOn = comp.SearchStartedOn;
                compFormArchv.Status = comp.Status;
                compFormArchv.AssignedToFullName = comp.AssignedTo;
                compFormArchv.Reviewer = comp.Reviewer;
                compFormArchv.ReviewCompleted = comp.IsReviewCompleted;
                compFormArchv.ReviewCompletedOn = comp.ReviewCompletedOn;
                compFormArchv.ComplianceForm = comp;

                _UOW.ComplianceFormArchiveRepository.Add(compFormArchv);
                _UOW.ComplianceFormRepository.DropComplianceForm(comp.RecId);
                archivedCount += 1;
            }
            var msg = String.Format("Archived: {0}", archivedCount);
            return msg;
        }

        public string UndoArchive(string RecId)
        {
            var msg = "";
            var compFormArchv = _UOW.ComplianceFormArchiveRepository.FindByComplianceFormId(RecId);
            var compForm = compFormArchv.ComplianceForm;
            _UOW.ComplianceFormRepository.Add(compForm);
            _UOW.ComplianceFormArchiveRepository.DropComplianceForm(compForm.RecId);
            msg = String.Format("Compliance Form Id: {0}, Study No: {1} restored from archive", compForm.RecId, compForm.ProjectNumber);
            return msg;
        }

        public string GetUserFullName(string UserName)
        {
            var User = _UOW.UserRepository.FindByUserName(UserName);

            return (User == null ? null : User.UserFullName);
        }

        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        
    }
}
