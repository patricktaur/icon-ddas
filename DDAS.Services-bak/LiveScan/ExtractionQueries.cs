using DDAS.Models;
using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Services.LiveScan
{
    public class ExtractionQueries
    {
        private IUnitOfWork _UOW;
        private int _ExtractionProcesses;
        private int _AvgSiteScanTimeSecs;
        public ExtractionQueries(IUnitOfWork uow, int ExtractionProcesses)
        {
            _UOW = uow;
            _ExtractionProcesses = ExtractionProcesses;
            _AvgSiteScanTimeSecs = 15;
        }
        public DateTime getNextExtractionCompletion()
        {
            List<ComplianceForm> forms = _UOW.ComplianceFormRepository.GetAll();
            var sitesToScan = forms.Where(f => f.InvestigatorDetails.Any(i => i.SitesSearched.Any(s => s.ExtractionMode == "Live" && s.ExtractedOn == null))).ToList().Count;
            var TotSecsRqd = (sitesToScan * _AvgSiteScanTimeSecs) / _ExtractionProcesses;
            return DateTime.Now.AddSeconds(TotSecsRqd * 1.35);
        }
        public int AverageExtractionTimeInSecs { get {
                return _AvgSiteScanTimeSecs;
            } }
    }
}
