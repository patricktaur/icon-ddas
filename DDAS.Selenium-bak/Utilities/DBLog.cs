using DDAS.Models;
using DDAS.Models.Interfaces;
using DDAS.Models.Repository;
using System;

namespace DDAS.Data.Mongo
{
    public class DBLog : ILog
    {
        private IUnitOfWork _UOW;
        private string _LogStartedBy;
        private bool _showMessage;
        public DBLog(IUnitOfWork uow, string LogStartedBy, bool showMessage = false)
        {
            _UOW = uow;
            _LogStartedBy = LogStartedBy;
            _showMessage = showMessage;
        }

        public void LogEnd()
        {
            UpdateLog("Log End", "");
        }

        public void LogStart()
        {
            UpdateLog("Log Start", "");
        }

        public void WriteLog(string message)
        {
            UpdateLog("", message);
            if (_showMessage == true)
            {
                Console.WriteLine(message);
            }
        }

        public void WriteLog(Log log)
        {
            log.CreatedBy = _LogStartedBy;
            log.CreatedOn = DateTime.Now;
            _UOW.LogRepository.Add(log);
        }

        public void WriteLog(string caption, string message)
        {
            UpdateLog(caption, message);
            if (_showMessage == true)
            {
                Console.WriteLine(caption + " - " + message);
            }
        }

        private void UpdateLog(string caption, string message)
        {
            var log = new Log();
            log.CreatedBy = _LogStartedBy;
            log.CreatedOn = DateTime.Now;
            log.Caption = caption;
            log.Message = message;
            _UOW.LogRepository.Add(log);
        }
    }
}
