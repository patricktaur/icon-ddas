using DDAS.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel.Channels;

//Not used:
namespace DDAS.API.Helpers
{
    public class CustomLogHandler : DelegatingHandler
    {
        private string _logFile = "";
        public CustomLogHandler(string logFile)
        {
            _logFile = logFile;
        }
        //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    return await base.SendAsync(request, cancellationToken);
        //}

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var logMetadata = BuildRequestMetadata(request);
            var response = await base.SendAsync(request, cancellationToken);
            logMetadata = BuildResponseMetadata(logMetadata, response);
            await SendToLog(logMetadata);
            return response;
        }

        private LogMetaData BuildRequestMetadata(HttpRequestMessage request)
        {

            var x = request.GetUserPrincipal();
            var y = request.GetRequestContext();
            
            var owin = request.GetOwinContext();
            
            var z = y.Principal;
            var name = z.Identity.Name;

            LogMetaData log = new LogMetaData
            {

                RequestMethod = request.Method.Method,
                RequestTimestamp = DateTime.Now,
                RequestUri = request.RequestUri.ToString()
                
                
            };
            return log;
        }
        private LogMetaData BuildResponseMetadata(LogMetaData logMetadata, HttpResponseMessage response)
        {
            logMetadata.ResponseStatusCode = response.StatusCode;
            logMetadata.ResponseTimestamp = DateTime.Now;
            logMetadata.ResponseContentType = response.Content.Headers.ContentType.MediaType;
            return logMetadata;
        }
        private async Task<bool> SendToLog(LogMetaData logMetadata)
        {
            //var fileName = @"c:\\temp\\test.csv";
            //var logText = ConvertObjectToString(logMetadata);
           
            var method = @logMetadata.RequestUri.Substring(logMetadata.RequestUri.LastIndexOf('/') + 1);

            var paramPos = method.LastIndexOf('?');

            var param = "";
            if (paramPos> 0)
            {
                param = method.Substring(method.LastIndexOf('?') + 1);
            }
         
           
            int index = method.IndexOf("?");
            method = (index > 0 ? method.Substring(0, index) : method);

            var startTime = logMetadata.RequestTimestamp;
            var processTime = (logMetadata.ResponseTimestamp.Value - logMetadata.RequestTimestamp.Value).Milliseconds;
            var logText = string.Format("{0}, {1}, {2}, {3}\r\n", startTime, method, param, processTime);

            await FileReadWriteAsync.WriteTextAsync(_logFile, logText);
            return true;
           
        }

        
    }
}