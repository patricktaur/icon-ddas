using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
using System.Configuration;
using System.util;
using Utilities;
using WebScraping.Selenium.SearchEngine;

namespace DDAS.DataExtractor
{
    class Program
    {
        public static string ConfigurationManager { get; private set; }

        static void Main(string[] args)
        {
            ExtractData();
        }

        static void ExtractData()
        {
            //ILog log, IUnitOfWork uow
            MongoMaps.Initialize();

            string DataExtractionLogFile = System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"];
            
            ILog log = new LogText(DataExtractionLogFile, true);
            IUnitOfWork uow = new UnitOfWork("DefaultConnection");
            log.LogStart();
            log.WriteLog(System.DateTime.Now.ToString(), "Extract Data starts");
            ISearchEngine searchEngine = new SearchEngine(log, uow);

            var SiteScan = new SiteScanData(uow);
            var query = SiteScan.GetNewSearchQuery();
             
            searchEngine.Load(query);

            log.WriteLog(System.DateTime.Now.ToString(), "Extract Data ends");
            log.WriteLog("=================================================================================");
            log.LogEnd();
           
        }
    }
}
