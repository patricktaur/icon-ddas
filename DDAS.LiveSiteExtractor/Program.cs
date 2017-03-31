using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using WebScraping.Selenium.SearchEngine;

namespace DDAS.LiveSiteExtractor
{
    class Program
    {
        private static LogText _WriteLog;
        static void Main(string[] args)
        {
           
            try
            {
                var proc = Process.GetCurrentProcess();
                var procId = proc.Id;

                int QueueNumber = 1;
                if (args.Length != 0)
                {
                    QueueNumber = int.Parse(args[0]);
                }
                
                MongoMaps.Initialize();
                Console.WriteLine("After Initialize");
                
                IUnitOfWork uow = new UnitOfWork("DefaultConnection");
                Console.WriteLine("After uow");

                var _config = new Config();
                ISearchEngine searchEngine = new SearchEngine(uow, _config);
                
                ILog log = new DBLog(uow, "Live Extractor Service", true);
                
                LiveScan liveScan = new LiveScan(uow, searchEngine, log, _config.ErrorScreenCaptureFolder, procId);
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
                var innerException = "None";
                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.Message;
                }
               

                _WriteLog.WriteLog(DateTime.Now.ToString(), ex.Message + "--Inner Exception:" + innerException);
                
                _WriteLog.LogEnd();
            }
        }
    }

    public class Config : IConfig
    {
        public string AppDataDownloadsFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["AppDataDownloadsFolder"];
            }
            set
            {
                value = "";
            }
        }

        public string AttachmentsFolder
        {
            get
            {
                return null;
            }
            set
            {
                value = "";
            }
        }

        public string ComplianceFormFolder
        {
            get
            {
                return null;
            }
            set
            {
                value = "";
            }
        }

        public string DataExtractionLogFile
        {
            get
            {
                return ConfigurationManager.AppSettings["DataExtractionLogFile"];
            }
            set
            {
                value = "";
            }
        }

        public string ErrorScreenCaptureFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["ErrorScreenCaptureFolder"];
            }
            set
            {
                value = "";
            }
        }

        public string ExcelTempateFolder
        {
            get
            {
                return null;
            }
            set
            {
                value = "";
            }
        }

        public string UploadsFolder
        {
            get
            {
                return null;
            }
            set
            {
                value = "";
            }
        }

        public string WordTemplateFolder
        {
            get
            {
                return null;
            }
            set
            {
                value = "";
            }
        }
    }

}
