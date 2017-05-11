using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Services.Search;
using System;
using System.Configuration;
using Utilities;
using WebScraping.Selenium.SearchEngine;
using System.Diagnostics;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace DDAS.DataExtractor
{
    class Program
    {
        //public static string ConfigurationManager { get; private set; }

        //private static LogText _WriteLog;
        private static DBLog _WriteLog;

        private IWebDriver _Driver;

        static void Main(string[] args)
        {
            int? SiteNum = null;
            if (args.Length != 0)
            {
                SiteNum = int.Parse(args[0]);
            }
            ExtractData(SiteNum);
            return;
        }

        static void ExtractData(int? SiteNum = null)
        {
            MongoMaps.Initialize();
            //string appRootFolder = "";
            string configFile = ConfigurationManager.AppSettings["APIWebConfigFile"];

            IUnitOfWork uow = new UnitOfWork("DefaultConnection");

            if (configFile == null)
            {
                string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //_WriteLog = new LogText(exePath + @"\ERROR-DATA-EXTRACTION.log", true);
                _WriteLog = new DBLog(uow, "DDAS.Extractor");
                _WriteLog.LogStart();
                _WriteLog.WriteLog(DateTime.Now.ToString(), "Data Extractor: Entry in AppSettings: APIWebConfigFile not found");
                _WriteLog.LogEnd();
                return;
            }
            //appRootFolder = Path.GetDirectoryName(configFile);
            //string DataExtractionLogFile = appRootFolder + @"\" + GetWebConfigAppSetting(configFile, "DataExtractionLogFile");
            //_WriteLog = new LogText(DataExtractionLogFile, true);

            //DownloadFolder = appRootFolder + @"\" + GetWebConfigAppSetting(configFile, "AppDataDownloadFolder");

            //string DataExtractionLogFile = ConfigurationManager.AppSettings["DataExtractionLogFile"];

            IConfig _config = new Config();
            ISearchEngine searchEngine = new SearchEngine(uow, _config);

            var extractData = new ExtractData(searchEngine);

            //_WriteLog = new LogText(_config.DataExtractionLogFile, true);
            _WriteLog = new DBLog(uow, "DDAS.Extractor", true);
            _WriteLog.LogStart();
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Extract Data starts");

            try
            {
                if (SiteNum != null)
                {
                    SiteEnum siteEnum = (SiteEnum)SiteNum;

                    extractData.ExtractDataSingleSite(
                        siteEnum, _WriteLog);
                }
                else
                {
                    //var query = SearchSites.GetNewSearchQuery();
                    var query = uow.SiteSourceRepository.GetAll();

                    extractData.ExtractDataAllDBSites(
                        query, _WriteLog);
                }
                _WriteLog.WriteLog(DateTime.Now.ToString(), "Extract Data ends");
            }
            catch (Exception e)
            {
                _WriteLog.WriteLog("Unable to complete the data extract. Error Details: " + 
                    e.ToString());
            }
            finally
            {
                _WriteLog.WriteLog("=================================================================================");
                _WriteLog.LogEnd();

                Process currentProcess = Process.GetCurrentProcess();
                currentProcess.CloseMainWindow();
            }
        }

        static string GetWebConfigAppSetting(string configFile, string keyName)
        {
            string error;
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration config =
                ConfigurationManager.OpenMappedExeConfiguration
                (fileMap, ConfigurationUserLevel.None);
            if (config == null)
            {
                error = "Config file : " + configFile + " could not be loaded.";
                //_WriteLog.WriteLog(error);
                throw new Exception(error);
            }
            else
            {
                KeyValueConfigurationElement settings = config.AppSettings.Settings[keyName];
                if (settings != null)
                {
                    //_WriteLog.WriteLog("Key : " + keyName + ", Value: " + settings.Value);
                    return settings.Value;
                }
                else
                {
                    error = "Key : " + keyName + ", Value: " + settings.Value + " could not be read";
                    //_WriteLog.WriteLog(error);
                    throw new Exception(error);
                }
            }

        }

        static string GetWebConfigConnectionString(string configFile, string keyName)
        {
            string error;
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration config =
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

        public IWebDriver Driver
        {
            get
            {
                if (_Driver == null)
                {
                    PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
                    service.IgnoreSslErrors = true;
                    service.SslProtocol = "any";

                    _Driver = new PhantomJSDriver(service);
                    _Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);
                    _Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                    //Patrick 16Feb2017
                    //_Driver.Manage().Window.Maximize();
                    //_Driver.Manage().Window.Size = new Size(1124, 850);
                    //_Driver.Manage().Window.Size = new Size(800, 600);


                    //_Driver = new ChromeDriver(@"C:\Development\p926-ddas\Libraries\ChromeDriver");

                    return _Driver;
                }
                else
                    return _Driver;
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

        public string CIILFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["CIILZipFolder"];
            }

            set
            {

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

        public string ExclusionDatabaseFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["ExclusionDatabaseFolder"];
            }

            set
            {
                value = "";
            }
        }

        public string FDAWarningLettersFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["FDAWarningLettersFolder"];
            }

            set
            {
                value = "";
            }
        }

        public string OutputFileFolder
        {
            get
            {
                return null;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string SAMFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["SAMFolder"];
            }

            set
            {
                value = "";
            }
        }

        public string SDNFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["SDNFolder"];
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
