using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
using System;
using System.Collections.Generic;
using System.Configuration;
using Utilities;
using WebScraping.Selenium.SearchEngine;
using System.Diagnostics;
using System.Threading;

namespace DDAS.DataExtractor
{
    class Program
    {
        //public static string ConfigurationManager { get; private set; }
        private static LogText _WriteLog;

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

            string configFile = System.Configuration.ConfigurationManager.AppSettings["APIWebConfigFile"];
            if (configFile == null)
            {
                string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                _WriteLog = new LogText(exePath + @"\ERROR-DATA-EXTRACTION.log", true);
                _WriteLog.LogStart();
                _WriteLog.WriteLog(DateTime.Now.ToString(), "Data Extractor: Entry in AppSettings: APIWebConfigFile not found");
                _WriteLog.LogEnd();

            }
       
            string DataExtractionLogFile =
            System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"];

            string DownloadFolder =
            System.Configuration.ConfigurationManager.AppSettings["DownloadFolder"];

            //ILog log = new LogText(DataExtractionLogFile,  true);
            _WriteLog = new LogText(DataExtractionLogFile, true);
            IUnitOfWork uow = new UnitOfWork("DefaultConnection");
            _WriteLog.LogStart();
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Extract Data starts");
            ISearchEngine searchEngine = new SearchEngine(uow);

            var SiteScan = new SiteScanData(uow, searchEngine);

            try

            {
                if (SiteNum != null)
                {
                    SiteEnum siteEnum = (SiteEnum)SiteNum;
                    _WriteLog.WriteLog(DateTime.Now.ToString(), "Extract Data for:" + siteEnum.ToString());

                    searchEngine.Load(siteEnum, "", DownloadFolder);
                    _WriteLog.WriteLog(DateTime.Now.ToString(), "Extract completed");
                    searchEngine.SaveData();
                    _WriteLog.WriteLog(DateTime.Now.ToString(), "Data Saved");
                }
                else
                {
                    var query = SearchSites.GetNewSearchQuery();
                    //searchEngine.Load(query, DownloadFolder, _WriteLog);
                }
                _WriteLog.WriteLog(DateTime.Now.ToString(), "Extract Data ends");

            }
            catch(Exception e)
            {
                _WriteLog.WriteLog("Unable to complete the data extract. Error Details: " + 
                    e.ToString());
            }

            finally
            {
                _WriteLog.WriteLog("=================================================================================");
                _WriteLog.LogEnd();
                ForcedCleanUp();
                Environment.Exit(0);
            }
            

        }

        static string GetWebConfigAppSetting(string configFile, string keyName)
        {
            string error;
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            System.Configuration.Configuration config =
                ConfigurationManager.OpenMappedExeConfiguration
                (fileMap, ConfigurationUserLevel.None);
            if (config == null)
            {
                error = "Config file : " + configFile + " could not be loaded.";
                _WriteLog.WriteLog(error);
                throw new Exception(error);
            }
            else
            {
                System.Configuration.KeyValueConfigurationElement settings = config.AppSettings.Settings[keyName];
                if (settings != null)
                {
                    _WriteLog.WriteLog("Key : " + keyName + ", Value: " + settings.Value);
                    return settings.Value;
                }
                else
                {
                    error = "Key : " + keyName + ", Value: " + settings.Value + " could not be read";
                    _WriteLog.WriteLog(error);
                    throw new Exception(error);
                }
            }

        }
        static string GetWebConfigConnectionString(string configFile, string keyName)
        {
            string error;
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            System.Configuration.Configuration config =
                ConfigurationManager.OpenMappedExeConfiguration
                (fileMap, ConfigurationUserLevel.None);
            if (config == null)
            {
                error = "Config file : " + configFile + " could not be loaded.";
                _WriteLog.WriteLog(error);
                throw new Exception(error);
            }
            else
            {
                string connStr = config.ConnectionStrings.ConnectionStrings[keyName].ConnectionString;
                if (connStr != null)
                {
                    _WriteLog.WriteLog("Connection String: " + connStr);
                    return connStr;
                }
                else
                {
                    error = "ConnectionString could not be read";
                    _WriteLog.WriteLog(error);
                    throw new Exception(error);
                }
            }

        }

        static void ForcedCleanUp()
        {
   
            ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;
            foreach (ProcessThread thread in currentThreads)
            {
                thread.Dispose();
            }

            //foreach (Thread thread in currentThreads)
            //{
            //    //thread.Interupt(); // If thread is waiting, stop waiting
            //    //thread.Interrupt();

            //    // or

            //    thread.Abort(); // Terminate thread immediately 

            //    // or
            //    //thread.IsBackground = true;
                
            //}
        }

    }



}
