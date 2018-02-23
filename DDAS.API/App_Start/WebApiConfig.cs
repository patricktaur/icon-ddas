using DDAS.API.App_Start;
using DDAS.API.Helpers.Formatters;
using DDAS.API.Setup;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;

namespace DDAS.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //config.Services.Replace(typeof(IExceptionLogger), new UnhandledExceptionLogger());
            
            // Web API routes
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.MapHttpAttributeRoutes();

            //var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);

            config.Filters.Add(new OfflineActionFilter());
            //GlobalFilters.Filters.Add(new OfflineActionFilter());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Start Ref: http://www.nesterovsky-bros.com/weblog/2014/03/10/CustomErrorHandlingWithWebAPI.aspx
            //Two related files: class GlobalExceptionLogger and class GlobalExceptionHandler
            // register the exception logger and handler
            config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLogger());
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            // set error detail policy according with value from Web.config
            var customErrors =
              (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");

            if (customErrors != null)
            {
                switch (customErrors.Mode)
                {
                    case CustomErrorsMode.RemoteOnly:
                        {
                            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

                            break;
                        }
                    case CustomErrorsMode.On:
                        {
                            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;

                            break;
                        }
                    case CustomErrorsMode.Off:
                        {
                            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

                            break;
                        }
                    default:
                        {
                            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Default;

                            break;
                        }
                }
            }
            //End Ref: http://www.nesterovsky-bros ...

            //config.Formatters.Add(new csvOutputFormatter());

        }
    }
}
