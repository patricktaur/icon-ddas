﻿using System;

using System.Linq;

using System.Web.Http;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using DDAS.Models;

using AutoMapper;
using DDAS.Models.Interfaces;
using WebScraping.Selenium.SearchEngine;

using Utilities;
using DDAS.Data.Mongo;
using DDAS.Services.Search;
using DDAS.Services.UserService;
using Utilities.EMail;
using DDAS.Models.Entities.Domain;
using DDAS.Services.AppAdminService;
using System.Web;
using DDAS.Services.Reports;
using DDAS.Services.AuditService;

namespace DDAS.API.App_Start
{
    public static class SimpleInjectorWebApiInitializer
    {
        //Initialize the container and register it as Web API Dependency Resolver
        //This will be initialized in OWIN startup

        public static void Initialize(HttpConfiguration config)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            InitializeContainer(container);

            container.RegisterWebApiControllers(config);
            //container.Verify();

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static void InitializeContainer(Container container)
        {
            //container.RegisterWebApiRequest<IUnitOfWork>(() => new UnitOfWork("DefaultConnection"));
            var ConnectionString = 
                System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var DBName =
                System.Configuration.ConfigurationManager.AppSettings["DBName"];

            container.Register<IUnitOfWork>(() => new UnitOfWork(ConnectionString, DBName));

            //For Automapper <<<< 
            //Get all my Profiles from the assembly (in my case was the webapi)
            var profiles = from t in typeof(SimpleInjectorWebApiInitializer).Assembly.GetTypes()
                           where typeof(Profile).IsAssignableFrom(t)
                           select (Profile)Activator.CreateInstance(t);

            //add all profiles found to the MapperConfiguration
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            container.Register(() => config.CreateMapper(container.GetInstance));
           
            //Patrick: 23Sept2016:  http context is not available when running on self host mode:
            //get from config file:
            // string downloadFolder = HttpContext.Current.Request.PhysicalApplicationPath + @"Downloads";
            //string downloadFolder = @"C:\Development\p926-ddas\DDAS.API\Downloads\"; //HttpContext.Current.Request.PhysicalApplicationPath + @"Downloads";

            string logFile = @"C:\Development\p926-ddas\DDAS.API\Logs\DataExtraction.log";
            //var log = new LogText(logFile);

            //container.RegisterWebApiRequest<ISearchEngine, SearchEngine>();
            container.Register<ISearchEngine, SearchEngine>();
            container.Register<IDataExtractorService, DataExtractorService>();
            
            //container.RegisterWebApiRequest<ILog>(() => new LogText(logFile, true));

            container.Register<ILog>(() => new LogText(logFile, true));

            //container.RegisterWebApiRequest<ISearchService, ComplianceFormService>();
            container.Register<ISearchService, ComplianceFormService>();
            container.Register<IComplianceFormArchiveService, ComplianceFormArchiveService>();

            var RootPath = HttpRuntime.AppDomainAppPath;

            container.Register<IConfig>(() =>
            new Config() {
               AppDataDownloadsFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["AppDataDownloadsFolder"],
               DataExtractionLogFile = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["DataExtractionLogFile"],
               ErrorScreenCaptureFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["ErrorScreenCaptureFolder"],
               AttachmentsFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["AttachmentsFolder"],
               ComplianceFormFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["ComplianceFormFolder"],
               ExcelTempateFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["ExcelTemplateFolder"],
               UploadsFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["UploadsFolder"],
               WordTemplateFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["WordTemplateFolder"],
               CIILFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["CIILFolder"],
               OutputFileFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["OutputFileFolder"],
               ExclusionDatabaseFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["ExclusionDatabaseFolder"],
               FDAWarningLettersFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["FDAWarningLettersFolder"],
               SAMFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["SAMFolder"],
               SAMApiKeyFile = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["SAMApiKeyFile"],
                SDNFolder = RootPath +
               System.Configuration.ConfigurationManager.AppSettings["SDNFolder"]
            });
            
            //container.RegisterWebApiRequest<IUserService, UserService>();
            container.Register<IUserService, UserService>();

            var cred = new EMailServerCredentialsModel();
            cred.EMailHost = System.Configuration.ConfigurationManager.AppSettings["EMailHost"];
            string port = System.Configuration.ConfigurationManager.AppSettings["EMailPort"];
            cred.EMailPort = Int32.Parse(port); 
            cred.FromEMailId = System.Configuration.ConfigurationManager.AppSettings["FromEMailId"];
            cred.FromEMailPassword = System.Configuration.ConfigurationManager.AppSettings["FromEMailPassword"];
            cred.SSLEnabled = Boolean.Parse( System.Configuration.ConfigurationManager.AppSettings["EMailSSLEnabled"]);
            container.Register<IEMailService>(() => new EMailService(cred));

            container.Register<IAppAdminService, AppAdminService>();

            container.Register<IReport, Report>();

            container.Register<IAudit, AuditService>();
        }
    }
}