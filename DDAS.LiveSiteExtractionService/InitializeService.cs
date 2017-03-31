

using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
using System;
using Utilities;
using WebScraping.Selenium.SearchEngine;

namespace DDAS.LiveSiteExtractionService
{
    partial class Service1 
    {
        private static ILog _dbLog;

        void InitializeService()
        {
            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _dbLog = new LogText(exePath + @"\test.log", true);
            _dbLog.LogStart();

            _dbLog.WriteLog(DateTime.Now.ToShortTimeString(), "Started");
           
            
            try
            {


                int QueueNumber = 1;

                var ErrorScreenCaptureFolder =
                    System.Configuration.ConfigurationManager.AppSettings["ErrorScreenCaptureFolder"];

                MongoMaps.Initialize();

                _dbLog.WriteLog("After MongoMaps.Initialize");
                IUnitOfWork uow = new UnitOfWork("DefaultConnection");
                _dbLog.WriteLog("After uow");
                
                ISearchEngine searchEngine = new SearchEngine(uow);


                _LiveScan = new LiveScan(uow, searchEngine, _dbLog, ErrorScreenCaptureFolder, QueueNumber);
                _LiveScan.StartLiveScan();

                
                _dbLog.WriteLog(DateTime.Now.ToShortTimeString(), "Closing");
                _dbLog.LogEnd();

                //Console.ReadLine();
            }
            catch (Exception ex)
            {
               _dbLog.WriteLog("After uow");
                var innerException = "";
                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.Message;
                }
                _dbLog.WriteLog(DateTime.Now.ToString(), ex.Message + "--Inner Exception:" + innerException);
                _dbLog.LogEnd();
            }
        }
    }

    
}
