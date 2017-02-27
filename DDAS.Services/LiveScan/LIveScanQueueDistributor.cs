using System;
using System.Collections.Generic;
using System.Linq;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System.Diagnostics;
using DDAS.Services.Search;

namespace DDAS.Services.LiveScan
{
    public class LiveScanQueueDistributor
    {
        private IUnitOfWork _UOW;
        private Boolean _continue;
 
        private ILog _Log;
 
        private int _numberOfQueues;
        private int _currentQueue = 1;
        private ISearchService _CompFormService;

        public LiveScanQueueDistributor(ISearchService compFormService, IUnitOfWork uow,  ILog log, int queues)
        {
            _CompFormService = compFormService;
            _UOW = uow;
            _Log = log;
            _continue = true;
            _numberOfQueues = queues;

        }

        public void Start()
        {
            _Log.LogStart();

            DistributeQueue();
        }

        public void Stop()
        {
            _Log.LogEnd();
            _continue = false;
        }


        private void DistributeQueue()
        {
            do
            {

                var compFormsToScan = GetComplianceFormsToScan();

                if (compFormsToScan.Count > 0)
                {
                    compFormsToScan.ForEach(formToScan => {
                        formToScan.ExtractionQueue = _currentQueue;
                        _CompFormService.UpdateComplianceFormNIgnoreIfNotFound(formToScan);
                        _currentQueue += 1;
                        if (_currentQueue > _numberOfQueues)
                        {
                            _currentQueue = 1;
                        }
                    });
                }
                else
                {
                    System.Threading.Thread.Sleep(10000); //10 seconds

                }
            } while (_continue == true);
        }

        private List<ComplianceForm> GetComplianceFormsToScan()
        {
            List<ComplianceForm> forms = _UOW.ComplianceFormRepository.GetAll();

            var formForLiveScans = forms.Where(f => (f.ExtractionQueue < 1 || f.ExtractionQueue > _numberOfQueues) && f.InvestigatorDetails.Any(
              i => i.SitesSearched.Any
              (s => s.ExtractionMode == "Live"
              && s.ExtractedOn == null
              && s.StatusEnum != ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified
              && s.StatusEnum != ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified
              )
              ))
              .ToList().OrderBy(o => o.SearchStartedOn).ToList();


            //var formForLiveScans = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(s => s.ExtractionMode == "Live" && s.ExtractedOn == null))).ToList().OrderBy(o => o.SearchStartedOn).ToList();
            return formForLiveScans;
        }


        private int getScanPendingSiteCount(ComplianceForm frm)
        {
            int siteCount = 0;
            foreach (InvestigatorSearched inv in frm.InvestigatorDetails)
            {
                foreach (SiteSearchStatus s in inv.SitesSearched)
                {
                    if (s.ExtractionPending == true)
                    {
                        siteCount += 1;
                    }
                }
            }
            return siteCount;
        }
    }
}
