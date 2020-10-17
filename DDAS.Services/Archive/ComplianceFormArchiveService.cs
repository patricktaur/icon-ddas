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
        private ISearchEngine _SearchEngine;
        private IConfig _config;
        //private CachedSiteScanData _cachedData;

        private const int _MatchCountLowerLimit = 2;
        private int _NumberOfRunningExtractionProcesses = 4;

        public ComplianceFormArchiveService(IUnitOfWork uow,
            ISearchEngine SearchEngine,
            IConfig Config)
        {
            _UOW = uow;
            _SearchEngine = SearchEngine;
            _config = Config;
            //_cachedData = new CachedSiteScanData(_UOW);
        }

         //Patrick: 7Jan2018 For use in ExtractDataService
        public IConfig Config
        {
            get
            {
                return _config;
            }
        }

        

        public string GetUserFullName(string UserName)
        {
            var User = _UOW.UserRepository.FindByUserName(UserName);

            return (User == null ? null : User.UserFullName);
        }

        

        public string ArchiveComplianceFormsWithSearchDaysGreaterThan(int days)
        {
            var compFormsToArchive = _UOW.ComplianceFormRepository.FindComplianceForms(days, 1);
          


            foreach (var comp in compFormsToArchive)
            {
                var compFormArchv = new ComplianceFormArchive();
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
                
            }

            return "";
        }

        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        
    }
}
