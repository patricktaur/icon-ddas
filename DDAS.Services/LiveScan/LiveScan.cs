using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Services.Search
{
    public class LiveScan
    {
        private IUnitOfWork _UOW;
        private Boolean _continue;
        private ComplianceFormService _compFormService;
        private ILog _Log;
        private string _ErrorScreenCaptureFolder;
        private long _avgScanTimeInSecs;
        private long _totalScanTimeInSecs;
        private long _sitesScanned;
        private Stopwatch _stopWatch;

       

        public LiveScan(IUnitOfWork uow, ISearchEngine SearchEngine, ILog log, string ErrorScreenCaptureFolder)
        {
            _UOW = uow;
            _compFormService = new ComplianceFormService(uow, SearchEngine);
            _Log = log;
            _continue = true;
            _ErrorScreenCaptureFolder = ErrorScreenCaptureFolder;
            _avgScanTimeInSecs = 45;
             _stopWatch = new Stopwatch();
        }

        public void StartLiveScan()
        {

            _Log.WriteLog("Before ProcessSingleForm");
            ProcessSingleForm();
            _Log.WriteLog("After ProcessSingleForm");

            //var compForm = GetNextComplianceFormToScan();
            
        }

       
        public void StopLiveScan()
        {
            _Log.LogEnd();
            _continue = false;
        }

      
        private void ProcessSingleForm()
        {
            do
            {
                
                var compForm = GetNextComplianceFormToScan();
               // var compForm = new ComplianceForm();
                if (compForm != null)
                {
                    //Flag Extraction Start, so that other Extraction processes cannot retrieve this form.
                    // compForm.ExtractionQueStart = DateTime.Now;
                    //_UOW.ComplianceFormRepository.UpdateCollection(compForm);
                    _UOW.ComplianceFormRepository.UpdateExtractionQueStart(compForm.RecId.Value, DateTime.Now);
                    //Console.WriteLine("Extraction Start:" + compForm.ProjectNumber);
                    _Log.WriteLog("Scan Start:" + compForm.ProjectNumber);
                    //_compFormService.AddMatchingRecords(compForm, _Log, _ErrorScreenCaptureFolder, "live");

                     ScanNUpdate(compForm);
                    //compForm.ExtractionQueStart = null;
                    //_UOW.ComplianceFormRepository.UpdateCollection(compForm);
                    _UOW.ComplianceFormRepository.UpdateExtractionQueStart(compForm.RecId.Value, null);

                    _Log.WriteLog("Scan End:" + compForm.ProjectNumber);

                }
                else
                {
                    System.Threading.Thread.Sleep(10000); //10 seconds
                }
            } while (_continue == true);
        }

        private void Process()
        {
            do
            {
                var compFormsToScan = GetComplianceFormsToScan();
                
                if (compFormsToScan.Count > 0)
                {
                    UpdateQuePosition(compFormsToScan);
                    compFormsToScan.ForEach(f => {
                        //Forms can get deleted by other operations
                        //Therefore fetch again.
                        var formToScan = _UOW.ComplianceFormRepository.FindById(f.RecId);
                        if (formToScan != null)  
                        {
                            ScanNUpdate(f);
                        }
                      });
                 }
                else
                {
                    System.Threading.Thread.Sleep(10000); //10 seconds

                }
            } while (_continue == true);
        }

        private void UpdateQuePosition(List<ComplianceForm> forms)
        {
            int QuePosition = 1;
            int extractionPendingSites = 0;
            long estimatedCompletionSecs = 0;

            foreach (ComplianceForm frm in forms)
            {
                extractionPendingSites = getScanPendingSiteCount(frm);

                //frm.InvestigatorDetails.ForEach(inv => inv.SitesSearched.ForEach(s => (s.ExtractionPending == true){ }))

                estimatedCompletionSecs += extractionPendingSites * _avgScanTimeInSecs ;  //_avgScanTimeInSecs for each form of 3 live scans.
                var completionAt = DateTime.Now.AddSeconds(estimatedCompletionSecs * 1.30);  //extra 25%

                Guid id = frm.RecId.Value;
               
                _compFormService.UpdateExtractionQuePosition(id, QuePosition, DateTime.Now, completionAt);
                QuePosition += 1;
            }
        }
        private void ScanNUpdate(ComplianceForm frm)
        {
            string InvNameNProjNumber = "";
            try
            {
                var Inv = frm.InvestigatorDetails.FirstOrDefault().Name;
                var ProjNumher = frm.ProjectNumber;
                InvNameNProjNumber = Inv + "-" + ProjNumher;
                _Log.WriteLog("Live Scan started", InvNameNProjNumber);
                _sitesScanned += getScanPendingSiteCount(frm);
                _stopWatch.Restart();
                _compFormService.ScanUpdateComplianceForm(frm, _Log, _ErrorScreenCaptureFolder, "live");
                _stopWatch.Stop();
                
                _totalScanTimeInSecs += _stopWatch.ElapsedMilliseconds / 1000;
                _avgScanTimeInSecs = _totalScanTimeInSecs / _sitesScanned;

                //Clear if more than 100, average for last 100 only:
                if (_sitesScanned > 100000)
                {
                    _sitesScanned = 0;
                    _totalScanTimeInSecs = 0;
                    //retain previous _avgScanTimeInSecs value.
                }

                _Log.WriteLog("Live Scan completed", InvNameNProjNumber);
                
            }
            catch (Exception ex)
            {
                string innerException = "";
                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.Message;
                }
                _Log.WriteLog("Live Scan ERROR - " + InvNameNProjNumber, ex.Message + "- Inner Exception: " + innerException);
             }
        }


        private ComplianceForm GetNextComplianceFormToScan()
        {
            List<ComplianceForm> forms = _UOW.ComplianceFormRepository.GetAll();

            var count = forms.Where(f => (f.ExtractionQueStart == null) && f.InvestigatorDetails.Any(i => i.SitesSearched.Any(
                s => s.ExtractionMode == "Live"
                && s.ExtractedOn == null
                && !(s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                ))).ToList().OrderBy(o => o.SearchStartedOn).Count();
            _Log.WriteLog("Forms found to scan:" + count);

            var formForLiveScan = forms.Where(f => (f.ExtractionQueStart == null) && f.InvestigatorDetails.Any(i => i.SitesSearched.Any(
                s => s.ExtractionMode == "Live"
                && s.ExtractedOn == null
                && !(s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified || s.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
                ))).ToList().OrderBy(o => o.SearchStartedOn).FirstOrDefault();
           
            return formForLiveScan;
        }

        private List<ComplianceForm> GetComplianceFormsToScan()
        {
            List<ComplianceForm> forms = _UOW.ComplianceFormRepository.GetAll();
            var formForLiveScans = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(s => s.ExtractionMode == "Live" && s.ExtractedOn == null))).ToList().OrderBy(o => o.SearchStartedOn).ToList();
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
