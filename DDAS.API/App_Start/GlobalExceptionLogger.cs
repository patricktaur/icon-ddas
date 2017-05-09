using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

//ref: http://www.nesterovsky-bros.com/weblog/2014/03/10/CustomErrorHandlingWithWebAPI.aspx
namespace DDAS.API.App_Start
{
    public class GlobalExceptionLogger:ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            //context.RequestContext.Principal.Identity.Name
            //context.RequestContext.ClientCertificate.
            //context.ExceptionContext.ControllerContext.ControllerDescriptor.ControllerName
            //context.Request.Content.
            
            var log = context.Exception.ToString();


            try
            {
                var request = context.Request;
                var exception = context.Exception;

                var id = LogError(
                  request.RequestUri.ToString(),
                  context.RequestContext == null ?
                    null : context.RequestContext.Principal.Identity.Name,
                  request.ToString(),
                  exception.Message,
                  exception.StackTrace);

                // associates retrieved error ID with the current exception
                exception.Data["NesterovskyBros:id"] = id;
            }
            catch
            {
                // logger shouldn't throw an exception!!!
            }
        }

        private long LogError(   string address,
               string userid,
               string request,
               string message,
               string stackTrace
            )
        {
            return 0;
  }
    }
}