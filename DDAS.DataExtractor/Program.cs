using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
using System;
using Utilities;
using WebScraping.Selenium.SearchEngine;

namespace DDAS.DataExtractor
{
    class Program
    {
        public static string ConfigurationManager { get; private set; }

        static void Main(string[] args)
        {
            int? SiteNum = null;
            if (args.Length != 0)
            {

                SiteNum = int.Parse(args[0]);

            }
            ExtractData(SiteNum);
        }

        static void ExtractData(int? SiteNum = null)
        {
            //ILog log, IUnitOfWork uow
            MongoMaps.Initialize();

            string DataExtractionLogFile =
            System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"];

            string DownloadFolder =
            System.Configuration.ConfigurationManager.AppSettings["DownloadFolder"];

            ILog log = new LogText(DataExtractionLogFile,  true);
            IUnitOfWork uow = new UnitOfWork("DefaultConnection");
            log.LogStart();
            log.WriteLog(DateTime.Now.ToString(), "Extract Data starts");
            ISearchEngine searchEngine = new SearchEngine(uow);

            var SiteScan = new SiteScanData(uow, searchEngine);

            try

            {
                if (SiteNum != null)
                {
                    SiteEnum siteEnum = (SiteEnum)SiteNum;
                    log.WriteLog(DateTime.Now.ToString(), "Extract Data for:" + siteEnum.ToString());
                    searchEngine.Load(siteEnum, "", DownloadFolder, log);
                }
                else
                {
                    var query = SearchSites.GetNewSearchQuery();
                    searchEngine.Load(query, DownloadFolder, log);
                }
                log.WriteLog(DateTime.Now.ToString(), "Extract Data ends");

            }
            catch(Exception e)
            {
                log.WriteLog("Unable to complete the data extract. Error Details: " + 
                    e.ToString());
            }

            finally
            {
                log.WriteLog("=================================================================================");
                log.LogEnd();
                Environment.Exit(0);
            }
            

        }
    }
}
