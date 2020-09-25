using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.CompilerServices;

using System.Diagnostics;
namespace DDAS.API.Helpers
{
    public class TimeMeasurementBlock : IDisposable
    {
        public Stopwatch _stopWatch { get; private set; }
        private NLog.Logger _Logger;
        private string _logGUID;
        private string _currentUser;
        private string _callerName;
        public TimeMeasurementBlock(NLog.Logger Logger, string CurrentUser, string CallerName)
        {
            _Logger = Logger;
            _stopWatch = new Stopwatch();
            _callerName = CallerName;
            _currentUser = CurrentUser;
            LogStart();
        }

        public void Dispose()
        {
            if (_stopWatch != null)
                _stopWatch.Stop();
            LogEnd();

        }

        private void LogStart()
        {
            _stopWatch.Start();
            _logGUID = shortGUID();
            _Logger.Info(String.Format("{0} | {1} | Start | {2}  ", _callerName, _logGUID, _currentUser));

        }

        private void LogEnd()
        {
            _stopWatch.Stop();
            _Logger.Info(String.Format("{0} | {1} | Stop | {2} | Elapsed ms: {3}", _callerName, _logGUID, _currentUser, _stopWatch.ElapsedMilliseconds));

        }


        private string GetCallerName([CallerMemberName] string caller = null)
        {
            return caller;
        }

        private string shortGUID()
        {
            var guid = Guid.NewGuid();
            var base64Guid = Convert.ToBase64String(guid.ToByteArray());

            // Replace URL unfriendly characters with better ones
            base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

            // Remove the trailing ==
            return base64Guid.Substring(0, base64Guid.Length - 2);
        }
    }
    
}

/* Usage:
        Stopwatch stopwatch = new Stopwatch();
        using (new TimeMeasurementBlock(stopwatch)) {
            var sut = new  VwStudyReportSummary_Query(views);
            var recs = sut.ReportOne();
        }
        var elapsedTime = stopwatch.ElapsedMilliseconds;
*/
