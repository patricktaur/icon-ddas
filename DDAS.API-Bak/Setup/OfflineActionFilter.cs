using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
//source: https://www.simple-talk.com/dotnet/asp-net/how-to-take-an-asp-net-mvc-web-site-down-for-maintenance/
namespace DDAS.API.Setup
{
    public class OfflineActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var ipAddress = HttpContext.Current.Request.UserHostAddress;

            //var offlineHelper = new OfflineHelper(ipAddress,
            //     filterContext.HttpContext.Server.MapPath);
            //if (offlineHelper.ThisUserShouldBeOffline)
            //{
            //    //Yes, we are "down for maintenance" for this user
            //    if (filterContext.IsChildAction)
            //    {
            //        filterContext.Result = new ContentResult { Content = string.Empty };
            //        return;
            //    }

            //    filterContext.Result = new ViewResult
            //    {
            //        ViewName = "Offline"
            //    };
            //    var response = filterContext.HttpContext.Response;
            //    response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            //    response.TrySkipIisCustomErrors = true;

            //    return;
            }

            //otherwise we let this through as normal
            //base.OnActionExecuting(filterContext);
        }
    }
