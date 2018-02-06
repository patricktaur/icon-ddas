using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace DDAS.API.App_Start
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        /// This core method should implement custom error handling, if any.
        /// It determines how an exception will be serialized for client-side processing.
        

        public override void Handle(ExceptionHandlerContext context)
        {
            var requestContext = context.RequestContext;
            var config = requestContext.Configuration;

            //context.Result = new ErrorResult(
              //context.Exception,
              //requestContext == null ? false : requestContext.IncludeErrorDetail,
              //config.Services.GetContentNegotiator(),
              //context.Request,
              //config.Formatters);
        }

        /// An implementation of IHttpActionResult interface.
        private class ErrorResult : ExceptionResult
        {
            public ErrorResult(
              Exception exception,
              bool includeErrorDetail,
              IContentNegotiator negotiator,
              HttpRequestMessage request,
              IEnumerable<MediaTypeFormatter> formatters) :
              base(exception, includeErrorDetail, negotiator, request, formatters)
            {
            }

            /// Creates an HttpResponseMessage instance asynchronously.
            /// This method determines how a HttpResponseMessage content will look like.
            public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var content = new HttpError(Exception, IncludeErrorDetail);

                // define an additional content field with name "ErrorID"
                content.Add("ErrorID", Exception.Data["NesterovskyBros:id"] as long?);

                var result =
                  ContentNegotiator.Negotiate(typeof(HttpError), Request, Formatters);

                var message = new HttpResponseMessage
                {
                    RequestMessage = Request,
                    StatusCode = result == null ?
                    HttpStatusCode.NotAcceptable : HttpStatusCode.InternalServerError
                };

                if (result != null)
                {
                    try
                    {
                        // serializes the HttpError instance either to JSON or to XML
                        // depend on requested by the client MIME type.
                        message.Content = new ObjectContent<HttpError>(
                          content,
                          result.Formatter,
                          result.MediaType);
                    }
                    catch
                    {
                        message.Dispose();

                        throw;
                    }
                }

                return Task.FromResult(message);
            }
        }
    }
}