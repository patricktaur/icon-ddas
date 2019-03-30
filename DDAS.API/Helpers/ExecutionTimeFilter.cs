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
            var param = "";
            var elapsedTime = stopWatch.Elapsed.Milliseconds;
            string userName = "";
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                userName = HttpContext.Current.User.Identity.Name;
            }
            //Trace.WriteLine(actionExecutedContext.ActionContext.ActionDescriptor.ActionName)
            var logText =  string.Format("{0}, {1}, {2}, {3}, {4}\r\n", DateTime.Now, actionName, userName, requestedUri, elapsedTime);

            await FileReadWriteAsync.WriteTextAsync(_logFile, logText);
        }

        
    }
}