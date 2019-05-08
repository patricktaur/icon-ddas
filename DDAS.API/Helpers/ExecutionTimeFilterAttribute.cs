using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DDAS.API.Helpers
{
    public class ExecutionTimeFilterAttribute :ActionFilterAttribute
    {
        private string _logFile = "";
        public ExecutionTimeFilterAttribute(string logFile)
        {
            _logFile = logFile;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            actionContext.Request.Properties[actionContext.ActionDescriptor.ActionName] = Stopwatch.StartNew();

        }
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            //return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
            Stopwatch stopWatch = (Stopwatch)actionExecutedContext.Request.Properties[actionExecutedContext.ActionContext.ActionDescriptor.ActionName];
            var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            var requestedUri = actionExecutedContext.Request.RequestUri;
            var elapsedTime = stopWatch.Elapsed.Milliseconds;
            string userName = "";
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                userName = HttpContext.Current.User.Identity.Name;
            }
            var logText =  string.Format("{0}, {1}, {2}, {3}, {4}\r\n", DateTime.Now, actionName, userName, requestedUri, elapsedTime);
            //with hr and min: String.Format("{0:yyyyMMddHHmm}"
            
            var logFile1 = _logFile.Replace("$$DateTime", String.Format("{0:yyyyMMdd}", DateTime.Now));
            var logFile = logFile1.Replace("$$UserName", userName);


            await FileReadWriteAsync.WriteTextAsync(logFile, logText);
        }

        
    }
}