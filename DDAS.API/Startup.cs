using DDAS.API.App_Start;
using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using DDAS.API.Helpers;
using DDAS.Data.Mongo.Maps;
using System.Web;
[assembly: OwinStartupAttribute(typeof(DDAS.API.Startup))]
[assembly: OwinStartup(typeof(DDAS.API.Startup))]
namespace DDAS.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var config = new HttpConfiguration();
            //To generate day-wise log file : PerfLog$$DateTime - $$DateTime replaced by current Date
            var logFile = HttpRuntime.AppDomainAppPath + "Logs\\PerfLog$$DateTime.csv";
            config.Filters.Add(new ExecutionTimeFilterAttribute(logFile));
            //config.Routes.MapHttpRoute("DefaultAPI",
            //    "api/{controller}/{id}",
            //    new { id = RouteParameter.Optional });

            SimpleInjectorWebApiInitializer.Initialize(config);

            WebApiConfig.Register(config);
            
            //Enable CORS
            var cors = new EnableCorsAttribute("*", "*", "*");
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ConfigureAuth(app);

           
            
            app.UseWebApi(config);
            MongoMaps.Initialize();
            
        }
    }
}
