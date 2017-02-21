﻿using DDAS.Models;
using DDAS.Models.Interfaces;
using DDAS.Models.Repository;
using System;

namespace DDAS.Data.Mongo
{
    public class DBLog : ILog
    {
        private IUnitOfWork _UOW;
        private string _LogStartedBy;
        public DBLog(IUnitOfWork uow, string LogStartedBy)
        {
            _UOW = uow;
            _LogStartedBy = LogStartedBy;
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
        }

        public void WriteLog(string caption, string message)
        {
            UpdateLog(caption, message);
        }

        private void UpdateLog(string caption, string message)
        {
            var log = new Log();
           
            log.CreatedBy = _LogStartedBy;
            log.Caption = caption;
            log.Message = message;
            _UOW.LogRepository.Add(log);
        }
    }
}