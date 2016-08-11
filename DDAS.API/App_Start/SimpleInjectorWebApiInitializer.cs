using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using DDAS.Models;
using DDAS.EF;
using AutoMapper;

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
            container.Verify();

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

            container.Register<IMapper>(() => config.CreateMapper(container.GetInstance));

            // >>>>>
        }
    }
}