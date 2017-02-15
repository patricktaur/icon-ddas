using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using System;
using System.Collections.Generic;
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

        public LiveScan(IUnitOfWork uow, ISearchEngine SearchEngine, ILog log)
        {
            _UOW = uow;
            _compFormService = new ComplianceFormService(uow, SearchEngine);
            _Log = log;
            _continue = true;
        }

        public void StartLiveScan()
        {
            _Log.LogStart();

            Process1();
        }

        public void StopLiveScan()
        {
            _Log.LogEnd();
            _continue = false;
        }

        private void Process()
        {
            do
            {
                ComplianceForm formToScan = GetNextComplianceFormToScan();
                if (formToScan != null)
                {
                    ScanNUpdate(formToScan);
                }
                else
                {
                      System.Threading.Thread.Sleep(10000); //10 seconds
                    
                }
            } while (_continue == true);
        }

        private void Process1()
        {
            do
            {

                var compFormsToScan = GetComplianceFormsToScan();

                if (compFormsToScan.Count > 0)
                {
                    compFormsToScan.ForEach(f => {
                        ScanNUpdate(f);
                        //System.Threading.Thread.Sleep(30000);
                    });
                 }
                else
                {
                    System.Threading.Thread.Sleep(10000); //10 seconds

                }
            } while (_continue == true);
        }

        private void ScanNUpdate(ComplianceForm frm)
        {
            var Inv = frm.InvestigatorDetails.FirstOrDefault().Name;
            var ProjNumher = frm.ProjectNumber;
            var InvNameNProjNumber = Inv + "-" + ProjNumher;
            _Log.WriteLog( "Live Scan started", InvNameNProjNumber);
            _compFormService.ScanUpdateComplianceForm(frm, _Log, "live");
            _Log.WriteLog( "Live Scan completed", InvNameNProjNumber);
        }


        private ComplianceForm GetNextComplianceFormToScan()
        {
            List<ComplianceForm> forms = _UOW.ComplianceFormRepository.GetAll();
            var formForLiveScan = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(s => s.ExtractionMode == "Live" && s.ExtractedOn == null))).ToList().OrderBy(o => o.SearchStartedOn).FirstOrDefault();
            return formForLiveScan;
        }

        private List<ComplianceForm> GetComplianceFormsToScan()
        {
            List<ComplianceForm> forms = _UOW.ComplianceFormRepository.GetAll();
            var formForLiveScans = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(s => s.ExtractionMode == "Live" && s.ExtractedOn == null))).ToList().OrderBy(o => o.SearchStartedOn).ToList();
            return formForLiveScans;
        }

    }
}
