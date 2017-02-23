using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
using WebScraping.Selenium.SearchEngine;
using Utilities;
using System;

namespace ConsoleApplication1
{
    class Program
    {
        private static LogText _WriteLog;

        static void Main(string[] args)
        {
            try
            {

                var ErrorScreenCaptureFolder =
                    System.Configuration.ConfigurationManager.AppSettings["ErrorScreenCaptureFolder"];

                MongoMaps.Initialize();
                Console.WriteLine("After Initialize");
                IUnitOfWork uow = new UnitOfWork("DefaultConnection");
                Console.WriteLine("After uow");
                ISearchEngine searchEngine = new SearchEngine(uow);

                ILog log = new DBLog(uow, "Live Extractor Service");

                LiveScan liveScan = new LiveScan(uow, searchEngine, log, ErrorScreenCaptureFolder);
                Console.WriteLine("Before StartLiveScan");
                liveScan.StartLiveScan();
                Console.WriteLine("After StartLiveScan");
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                _WriteLog = new LogText(exePath + @"\ERROR-LIVE-SCAN.log", true);
                _WriteLog.LogStart();
                var innerException = "";
                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.Message;
                }
                _WriteLog.WriteLog(DateTime.Now.ToString(), ex.Message + "--Inner Exception:" + innerException);
                _WriteLog.LogEnd();
            } 
     
        }
        

      
    }
}
