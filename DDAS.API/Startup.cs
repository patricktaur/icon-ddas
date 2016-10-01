using DDAS.API.App_Start;
using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using DDAS.Data.Mongo.Maps;

[assembly: OwinStartup(typeof(DDAS.API.Startup))]

namespace DDAS.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var config = new HttpConfiguration();

            //config.Routes.MapHttpRoute("DefaultAPI",
            //    "api/{controller}/{id}",
            //    new { id = RouteParameter.Optional });

           
            SimpleInjectorWebApiInitializer.Initialize(config);

            WebApiConfig.Register(config);

            //Enable CORS
            
            var cors = new EnableCorsAttribute("*", "*", "*");
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(config);
            MongoMaps.Initialize();
           
        }
    }
}
