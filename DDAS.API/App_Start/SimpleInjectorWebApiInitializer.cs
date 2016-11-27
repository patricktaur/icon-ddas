using System;

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
            container.RegisterWebApiRequest<IUnitOfWork>(() => new UnitOfWork("DefaultConnection"));


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

            container.RegisterWebApiRequest<ISearchEngine, SearchEngine>();

            container.RegisterWebApiRequest<ILog>(() => new LogText(logFile, true));

            container.RegisterWebApiRequest<ISearchSummary, SearchService>();
            container.RegisterWebApiRequest<ISiteSummary, SiteSummary>();
        }
    }
}